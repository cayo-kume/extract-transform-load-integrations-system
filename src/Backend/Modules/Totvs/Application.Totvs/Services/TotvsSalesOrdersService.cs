using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Extensions;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Dtos.Reads.Base;
using Domain.Totvs.Enums;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Application.Totvs.Services
{
    /// <summary>
    /// pedidos de venda da Totvs. O método faz as próprias chamadas à API; o comum (horário do lote, parâmetros, lojas e token)
    /// vem do ITotvsService
    /// </summary>
    public class TotvsSalesOrdersService : ITotvsSalesOrdersService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsSalesOrdersRepository _totvsSalesOrdersRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder> _orderValidator;

        private const string OrdersKey = "Totvs:SalesOrders";

        // máximo da rota (swagger da API Sales Order); 252 pedidos com o expand completo levaram 6 s (testado)
        private const int OrdersPageSize = 500;

        // a carga inicial (5 meses) é lida em fatias, e cada fatia é gravada ao terminar (como nos produtos e pessoas)
        private const int OrdersWindowDays = 30;

        // expand da documentação; o expand do código antigo já batia com a documentação
        private const string Expand = "items,invoices,shippingAddress,observations,invoiceObservations,representativeObservations,classifications,discounts,commissioneds,counts";

        public TotvsSalesOrdersService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsSalesOrdersRepository totvsSalesOrdersRepository,
            IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder> orderValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsSalesOrdersRepository, _orderValidator) =
            (apiClient, logger, cache, totvsService, totvsSalesOrdersRepository, orderValidator);

        public async Task<bool?> PostSearchSalesOrders()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchSalesOrders);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // branchCodeList é obrigatório (swagger): todas as lojas da holding
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    var endDate = _totvsService.GetBatchTime();

                    // as datas da Totvs são do horário local, sem fuso (o "Z" é ignorado pela API, testado). Banco vazio: desde 01/01/2025
                    var lastChange = await _totvsSalesOrdersRepository.GetSalesOrdersLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2025, 1, 1);

                    for (var windowStart = since; windowStart < endDate; windowStart = windowStart.AddDays(OrdersWindowDays))
                    {
                        var windowEnd = windowStart.AddDays(OrdersWindowDays) < endDate ? windowStart.AddDays(OrdersWindowDays) : endDate;

                        var orders = new List<Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder>();
                        var ordersJson = new Dictionary<(int, int), string>();
                        var page = 1;
                        var totalPages = 1;
                        var failed = false;

                        while (page <= totalPages)
                        {
                            var requestBody = JsonConvert.SerializeObject(new
                            {
                                filter = new
                                {
                                    change = new
                                    {
                                        startDate = $"{windowStart:yyyy-MM-ddTHH:mm:ss}",
                                        endDate = $"{windowEnd:yyyy-MM-ddTHH:mm:ss}"
                                    },
                                    branchCodeList = branchCodes
                                },
                                order = "maxChangeFilterDate",
                                page,
                                pageSize = OrdersPageSize,
                                expand = Expand
                            });

                            // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                var token = await _totvsService.GetToken(company);

                                if (token is null)
                                    break;   // a mensagem do login já foi para o log

                                using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                                response = await _apiClient.SendAsync(
                                    HttpMethod.Post,
                                    "sales-order/v2/orders/search",
                                    content,
                                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } }
                                );

                                if (attempt == 1 && response.StatusCode == 401)
                                {
                                    _totvsService.RemoveToken(company);
                                    continue;
                                }

                                if (attempt == 1 && response.StatusCode == 429)
                                {
                                    var seconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var value) ? value : 5;
                                    await Task.Delay(TimeSpan.FromSeconds(Math.Clamp(seconds, 1, 60)));
                                    continue;
                                }

                                if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                    continue;
                                }

                                break;
                            }

                            if (response is null || !response.IsSuccess)
                            {
                                if (response is not null)
                                {
                                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                    _logger.AddMessage($"Erro da API Totvs em sales-order/v2/orders/search (empresa {company.doc_company}, janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}, página {page}): {response.StatusCode} {body}");
                                }

                                failed = true;
                                break;
                            }

                            Page<Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder>? result = null;

                            try
                            {
                                result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder>>(response.Content ?? String.Empty);

                                foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                                {
                                    var key = (ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["branchCode"]), ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["orderCode"]));

                                    if (key.Item1 != 0 && key.Item2 != 0)
                                        ordersJson[key] = item.ToString(Formatting.None);
                                }
                            }
                            catch (JsonException)
                            {
                                // result continua null: tratado abaixo como resposta inválida
                            }

                            if (result?.items is null)
                            {
                                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                _logger.AddMessage($"Resposta inválida da API Totvs em sales-order/v2/orders/search (empresa {company.doc_company}, página {page}): {body}");

                                failed = true;
                                break;
                            }

                            orders.AddRange(result.items);

                            totalPages = result.totalPages;
                            page++;
                        }

                        // fatia incompleta não é gravada, e as seguintes também não: a próxima execução continua do MAX(maxChangeFilterDate)
                        if (failed)
                            break;

                        if (orders.Count == 0)
                            continue;

                        // cache de estado: (empresa da loja, número) -> pedido gravado (com o maxChangeFilterDate), por empresa
                        var ordersCache = await _cache.GetOrCreateAsync($"{OrdersKey}:{company.doc_company}", async entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                            var list = await _totvsSalesOrdersRepository.GetSalesOrdersByKeys(
                                company.doc_company!,
                                orders.Select(Key).Where(x => x.branchCode != 0 && x.orderCode != 0).Distinct().ToList()
                            );

                            return list
                                .Where(x => x != null)
                                .GroupBy(x => (x!.branchCode, x.orderCode))
                                .ToDictionary(g => g.Key, g => g.First()!);
                        });

                        // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                        orders.RemoveAll(x =>
                            ordersCache!.TryGetValue(Key(x), out var y) &&
                            NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                        );

                        // um horário de lote por fatia: um pedido que volta noutra fatia precisa de horário maior para a exclusão das filhas
                        totalInseridos += await SaveSalesOrders(company.doc_company!, _totvsService.GetBatchTime(), orders, ordersJson, ordersCache);
                    }
                }

                _logger.AddMessage(
                    $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
                );

                return true;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// busca um pedido de venda por loja + número, grava nas tabelas de pedidos (sempre, sem consultar o cache)
        /// e registra a chamada em totvs.TotvsRequests
        /// </summary>
        public async Task<bool?> GetSalesOrderByKey(string user_name, string doc_company, int branchCode, int orderCode)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSalesOrderByKey);

            // rota registrada no log de requisições (sem o token); o filtro vai no corpo
            const string route = "sales-order/v2/orders/search";
            var key = $"{branchCode}|{orderCode}";
            bool? result;

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em TotvsRequests (igual ao Movidesk)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em totvs.TotvsParameters.");
                    return null;
                }

                // o change desde 01/01/2025 (a carga inicial do job) é o que faz o maxChangeFilterDate vir preenchido (testado):
                // sem ele o pedido seria regravado na próxima execução do job. A rota de pedidos aceita janela maior que 6 meses
                var requestBody = JsonConvert.SerializeObject(new
                {
                    filter = new
                    {
                        branchCodeList = new[] { branchCode },
                        orderCodeList = new[] { orderCode },
                        change = new
                        {
                            startDate = "2025-01-01T00:00:00",
                            endDate = $"{_totvsService.GetBatchTime():yyyy-MM-ddTHH:mm:ss}"
                        }
                    },
                    page = 1,
                    pageSize = 1,
                    expand = Expand
                });

                // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                ApiResponse? response = null;

                try
                {
                    for (var attempt = 1; attempt <= 2; attempt++)
                    {
                        var token = await _totvsService.GetToken(company);

                        if (token is null)
                            break;   // a mensagem do login já foi para o log

                        using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                        response = await _apiClient.SendAsync(
                            HttpMethod.Post,
                            route,
                            content,
                            headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } }
                        );

                        if (attempt == 1 && response.StatusCode == 401)
                        {
                            _totvsService.RemoveToken(company);
                            continue;
                        }

                        if (attempt == 1 && response.StatusCode == 429)
                        {
                            var seconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var value) ? value : 5;
                            await Task.Delay(TimeSpan.FromSeconds(Math.Clamp(seconds, 1, 60)));
                            continue;
                        }

                        if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(10));
                            continue;
                        }

                        break;
                    }
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _totvsService.SaveRequest(EnumJob.TotvsGetSalesOrderByKey, user_name, "POST", route, doc_company, key, $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                    throw;
                }

                if (response is null)
                {
                    // login falhou: nenhuma chamada à busca
                    _totvsService.SaveRequest(EnumJob.TotvsGetSalesOrderByKey, user_name, "POST", route, doc_company, key, null, false, requestBody);
                    return null;
                }

                if (!response.IsSuccess)
                {
                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                    _logger.AddMessage($"Erro da API Totvs em {route} (empresa {doc_company}, pedido de venda {key}): {response.StatusCode} {body}");

                    _totvsService.SaveRequest(EnumJob.TotvsGetSalesOrderByKey, user_name, "POST", route, doc_company, key, response.Content, false, requestBody);
                    return null;
                }

                Page<Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder>? page = null;
                string? json = null;

                try
                {
                    page = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder>>(response.Content ?? String.Empty);
                    json = ((JObject.Parse(response.Content!)["items"] as JArray) ?? []).FirstOrDefault()?.ToString(Formatting.None);
                }
                catch (JsonException)
                {
                    // page continua null: tratado abaixo como resposta inválida
                }

                var order = page?.items?.FirstOrDefault(x => Key(x) == (branchCode, orderCode));

                if (page?.items is null)
                {
                    _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {doc_company}, pedido de venda {key}): {response.Content}");
                    result = null;
                }
                else if (order is null || json is null)
                {
                    // a busca responde 200 com items vazio quando o pedido não existe
                    _logger.AddMessage($"Pedido de venda {key} não encontrado na Totvs (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    // atualiza o cache do PostSearchSalesOrders, se ele estiver carregado, para a próxima execução não regravar o pedido
                    _cache.TryGetValue($"{OrdersKey}:{doc_company}", out Dictionary<(int branchCode, int orderCode), Domain.Totvs.Entities.Reads.SalesOrders.SaleOrder>? ordersCache);

                    await SaveSalesOrders(doc_company, _totvsService.GetBatchTime(), [order], new Dictionary<(int, int), string> { { (branchCode, orderCode), json } }, ordersCache);

                    _logger.AddMessage($"Concluída com sucesso: pedido de venda {key} inserido ou atualizado!");
                    result = true;
                }

                _totvsService.SaveRequest(EnumJob.TotvsGetSalesOrderByKey, user_name, "POST", route, doc_company, key, response.Content, result == true, requestBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// chave do pedido: o número é único por loja (branchCode), não na holding
        /// </summary>
        private static (int branchCode, int orderCode) Key(Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder x) =>
            (ConverterValidatorsExtensions.ConvertToInt32Validation(x.branchCode), ConverterValidatorsExtensions.ConvertToInt32Validation(x.orderCode));

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de pedidos de venda gravados.
        /// </summary>
        private async Task<int> SaveSalesOrders(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder> orders,
            IReadOnlyDictionary<(int, int), string> ordersJson,
            Dictionary<(int branchCode, int orderCode), Domain.Totvs.Entities.Reads.SalesOrders.SaleOrder>? ordersCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.SalesOrders.SaleOrder>();

            // o mesmo pedido pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var order in orders.Where(x => Key(x).branchCode != 0 && Key(x).orderCode != 0).GroupBy(Key).Select(g => g.Last()))
            {
                var validations = _orderValidator.Validate(order);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"pedido de venda {order.branchCode}/{order.orderCode}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.SalesOrders.SaleOrder(doc_company, batchTime, order));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsSalesOrdersRepository.BulkInsertIntoTotvsSaleOrders(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (ordersJson.TryGetValue((record.branchCode, record.orderCode), out var json))
                    _logger.AddRecord($"{record.branchCode}|{record.orderCode}", json);

                if (ordersCache is not null)
                    ordersCache[(record.branchCode, record.orderCode)] = record;
            }

            return _listSomenteNovos.Count;
        }
    }
}

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
    /// pedidos de compra da Totvs. O método faz as próprias chamadas à API; o comum (horário do lote, parâmetros, lojas e token)
    /// vem do ITotvsService
    /// </summary>
    public class TotvsPurchaseOrdersService : ITotvsPurchaseOrdersService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsPurchaseOrdersRepository _totvsPurchaseOrdersRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.PurchaseOrders.PurchaseOrder> _orderValidator;

        private const string OrdersKey = "Totvs:PurchaseOrders";

        // máximo da rota (swagger da API Purchase Order)
        private const int OrdersPageSize = 100;

        // a carga inicial (5 meses) é lida em fatias, e cada fatia é gravada ao terminar (como nos produtos e pessoas)
        private const int OrdersWindowDays = 30;

        // expand da documentação; requests (solicitações de compra) não vinha no código antigo
        private const string Expand = "items,requests";

        public TotvsPurchaseOrdersService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsPurchaseOrdersRepository totvsPurchaseOrdersRepository,
            IValidator<Domain.Totvs.Dtos.Reads.PurchaseOrders.PurchaseOrder> orderValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsPurchaseOrdersRepository, _orderValidator) =
            (apiClient, logger, cache, totvsService, totvsPurchaseOrdersRepository, orderValidator);

        public async Task<bool?> PostSearchPurchaseOrders()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchPurchaseOrders);

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

                    // as datas da Totvs são do horário local, sem fuso (o "Z" é ignorado pela API, testado). Banco vazio: 5 meses
                    var lastChange = await _totvsPurchaseOrdersRepository.GetPurchaseOrdersLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : endDate.AddMonths(-5);

                    for (var windowStart = since; windowStart < endDate; windowStart = windowStart.AddDays(OrdersWindowDays))
                    {
                        var windowEnd = windowStart.AddDays(OrdersWindowDays) < endDate ? windowStart.AddDays(OrdersWindowDays) : endDate;

                        var orders = new List<Domain.Totvs.Dtos.Reads.PurchaseOrders.PurchaseOrder>();
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
                                    "purchase-order/v2/search",
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
                                    _logger.AddMessage($"Erro da API Totvs em purchase-order/v2/search (empresa {company.doc_company}, janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}, página {page}): {response.StatusCode} {body}");
                                }

                                failed = true;
                                break;
                            }

                            Page<Domain.Totvs.Dtos.Reads.PurchaseOrders.PurchaseOrder>? result = null;

                            try
                            {
                                result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.PurchaseOrders.PurchaseOrder>>(response.Content ?? String.Empty);

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
                                _logger.AddMessage($"Resposta inválida da API Totvs em purchase-order/v2/search (empresa {company.doc_company}, página {page}): {body}");

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

                            var list = await _totvsPurchaseOrdersRepository.GetPurchaseOrdersByKeys(
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
                        totalInseridos += await SavePurchaseOrders(company.doc_company!, _totvsService.GetBatchTime(), orders, ordersJson, ordersCache);
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
        /// chave do pedido: o número é único por loja (branchCode), não na holding
        /// </summary>
        private static (int branchCode, int orderCode) Key(Domain.Totvs.Dtos.Reads.PurchaseOrders.PurchaseOrder x) =>
            (ConverterValidatorsExtensions.ConvertToInt32Validation(x.branchCode), ConverterValidatorsExtensions.ConvertToInt32Validation(x.orderCode));

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de pedidos de compra gravados.
        /// </summary>
        private async Task<int> SavePurchaseOrders(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.PurchaseOrders.PurchaseOrder> orders,
            IReadOnlyDictionary<(int, int), string> ordersJson,
            Dictionary<(int branchCode, int orderCode), Domain.Totvs.Entities.Reads.PurchaseOrders.PurchaseOrder>? ordersCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.PurchaseOrders.PurchaseOrder>();

            // o mesmo pedido pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var order in orders.Where(x => Key(x).branchCode != 0 && Key(x).orderCode != 0).GroupBy(Key).Select(g => g.Last()))
            {
                var validations = _orderValidator.Validate(order);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"pedido de compra {order.branchCode}/{order.orderCode}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.PurchaseOrders.PurchaseOrder(doc_company, batchTime, order));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsPurchaseOrdersRepository.BulkInsertIntoTotvsPurchaseOrders(_listSomenteNovos, _logger.GetExecutionGuid());

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

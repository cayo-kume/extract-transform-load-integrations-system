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

        // grupo das escritas desta API na configuração (Totvs:Writes:PurchaseOrders:Enabled, T4)
        private const string WritesGroup = "PurchaseOrders";

        // situações do pedido de compra (StatusOrderType), pelo número gravado em totvs.TotvsPurchaseOrders.status
        private static readonly Dictionary<string, string> StatusNames = new()
        {
            ["1"] = "em andamento", ["2"] = "liberado para baixa", ["3"] = "parcialmente recebido", ["4"] = "atendido",
            ["5"] = "bloqueado", ["6"] = "cancelado", ["7"] = "distribuído", ["8"] = "pré-pedido"
        };

        private static string StatusName(string status) => StatusNames.TryGetValue(status, out var name) ? $"{name} ({status})" : status;

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

        /// <summary>
        /// busca um pedido de compra por loja + número, grava nas tabelas de pedidos (sempre, sem consultar o cache)
        /// e registra a chamada em totvs.TotvsRequests
        /// </summary>
        public async Task<bool?> GetPurchaseOrderByKey(string user_name, string doc_company, int branchCode, int orderCode)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetPurchaseOrderByKey);

            // rota registrada no log de requisições (sem o token); o filtro vai no corpo
            const string route = "purchase-order/v2/search";
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

                // o change é o que faz o maxChangeFilterDate vir preenchido (sem ele vem nulo, testado), e a busca de pedidos de compra aceita a
                // janela desde 2000: sem ele o pedido seria regravado na próxima execução do job (fase PO-1, DP4)
                var requestBody = JsonConvert.SerializeObject(new
                {
                    filter = new
                    {
                        branchCodeList = new[] { branchCode },
                        orderCodeList = new[] { orderCode },
                        change = new
                        {
                            startDate = "2000-01-01T00:00:00",
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
                    _totvsService.SaveRequest(EnumJob.TotvsGetPurchaseOrderByKey, user_name, "POST", route, doc_company, key, $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                    throw;
                }

                if (response is null)
                {
                    // login falhou: nenhuma chamada à busca
                    _totvsService.SaveRequest(EnumJob.TotvsGetPurchaseOrderByKey, user_name, "POST", route, doc_company, key, null, false, requestBody);
                    return null;
                }

                if (!response.IsSuccess)
                {
                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                    _logger.AddMessage($"Erro da API Totvs em {route} (empresa {doc_company}, pedido de compra {key}): {response.StatusCode} {body}");

                    _totvsService.SaveRequest(EnumJob.TotvsGetPurchaseOrderByKey, user_name, "POST", route, doc_company, key, response.Content, false, requestBody);
                    return null;
                }

                Page<Domain.Totvs.Dtos.Reads.PurchaseOrders.PurchaseOrder>? page = null;
                string? json = null;

                try
                {
                    page = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.PurchaseOrders.PurchaseOrder>>(response.Content ?? String.Empty);
                    json = ((JObject.Parse(response.Content!)["items"] as JArray) ?? []).FirstOrDefault()?.ToString(Formatting.None);
                }
                catch (JsonException)
                {
                    // page continua null: tratado abaixo como resposta inválida
                }

                var order = page?.items?.FirstOrDefault(x => Key(x) == (branchCode, orderCode));

                if (page?.items is null)
                {
                    _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {doc_company}, pedido de compra {key}): {response.Content}");
                    result = null;
                }
                else if (order is null || json is null)
                {
                    // a busca responde 200 com items vazio, ou com outro pedido, quando o número não existe (testado): vale só a chave igual
                    _logger.AddMessage($"Pedido de venda {key} não encontrado na Totvs (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    // atualiza o cache do PostSearchPurchaseOrders, se ele estiver carregado, para a próxima execução não regravar o pedido
                    _cache.TryGetValue($"{OrdersKey}:{doc_company}", out Dictionary<(int branchCode, int orderCode), Domain.Totvs.Entities.Reads.PurchaseOrders.PurchaseOrder>? ordersCache);

                    await SavePurchaseOrders(doc_company, _totvsService.GetBatchTime(), [order], new Dictionary<(int, int), string> { { (branchCode, orderCode), json } }, ordersCache);

                    _logger.AddMessage($"Concluída com sucesso: pedido de compra {key} inserido ou atualizado!");
                    result = true;
                }

                _totvsService.SaveRequest(EnumJob.TotvsGetPurchaseOrderByKey, user_name, "POST", route, doc_company, key, response.Content, result == true, requestBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrdersChangeStatus(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.UpdateOrderStatus body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostPurchaseOrdersChangeStatus);

            var key = $"{body.branchCode}|{body.orderCode}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // passo 3 da §1.2: a loja do pedido precisa ser da empresa
                if (!(await _totvsService.GetBranchCodes(company)).Contains(body.branchCode!.Value))
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCode.Value.ToString()]);

                // conferência local (DP7), só com o pedido no banco (até 3 min atrasado): a troca só vale entre em andamento e bloqueado. A busca
                // devolve a situação como número (StatusOrderType), gravado assim em status
                var current = await _totvsPurchaseOrdersRepository.GetPurchaseOrderStatus(doc_company, body.branchCode.Value, body.orderCode!.Value);
                var to = ((int)body.status!.Value).ToString();
                var from = body.status == Domain.Totvs.Dtos.Writes.PurchaseOrders.StatusOrderChange.Blocked ? "1" : "5";

                if (!String.IsNullOrEmpty(current) && current != from)
                {
                    var refused = current == to
                        ? $"O pedido de compra {key} já está {StatusName(to)} em totvs.TotvsPurchaseOrders."
                        : $"O pedido de compra {key} está {StatusName(current)} em totvs.TotvsPurchaseOrders: a troca só vale de {StatusName(from)} para {StatusName(to)}.";
                    _logger.AddMessage(refused);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = refused };
                }

                // valor absoluto: repetir não muda nada, sem trava
                return await _totvsService.Write(company, EnumJob.TotvsPostPurchaseOrdersChangeStatus, user_name, key, HttpMethod.Post, "purchase-order/v2/orders/change-status", body,
                    "Confira a situação do pedido na Totvs antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrders(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.InsertPurchaseOrder body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostPurchaseOrders);

            var key = $"{body.branchCode}|{(object?)body.supplierCode ?? body.supplierCpfCnpj}|{body.supplierOrderCode}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (!(await _totvsService.GetBranchCodes(company)).Contains(body.branchCode!.Value))
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCode.Value.ToString()]);

                // DP2: um pedido do mesmo fornecedor com o mesmo supplierOrderCode já gravado é a repetição de uma inclusão (depois de um 502, por
                // exemplo); a trava T5 cobre os 10 minutos, o banco cobre depois que o job de pedidos trouxe o pedido
                var existing = await _totvsPurchaseOrdersRepository.GetPurchaseOrderBySupplierOrderCode(doc_company, body.branchCode.Value, body.supplierCode, body.supplierCpfCnpj, body.supplierOrderCode!);

                if (existing is not null)
                {
                    var refused = $"Já existe o pedido de compra {body.branchCode}|{existing} do fornecedor com o pedido {body.supplierOrderCode} em totvs.TotvsPurchaseOrders.";
                    _logger.AddMessage(refused);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = refused };
                }

                return await _totvsService.Write(company, EnumJob.TotvsPostPurchaseOrders, user_name, key, HttpMethod.Post, "purchase-order/v2/orders", body,
                    "Confira o pedido pelo GET PurchaseOrders/{docCompany}/{branchCode}/{orderCode} ou pelo supplierOrderCode antes de repetir.", repeatLock: true);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrdersQuantityItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.RequestedQuantity body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostPurchaseOrdersQuantityItems);

            var key = $"{body.branchCode}|{body.orderCode}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (!(await _totvsService.GetBranchCodes(company)).Contains(body.branchCode!.Value))
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCode.Value.ToString()]);

                // conferência local (DP7), só com o pedido no banco: atendido ou cancelado não muda; o produto precisa estar no pedido e a quantidade
                // nova não fica abaixo da já recebida
                var current = await _totvsPurchaseOrdersRepository.GetPurchaseOrderStatus(doc_company, body.branchCode.Value, body.orderCode!.Value);
                string? refused = null;

                if (current is "4" or "6")
                    refused = $"O pedido de compra {key} está {StatusName(current)} em totvs.TotvsPurchaseOrders: a quantidade não muda.";
                else if (!String.IsNullOrEmpty(current))
                {
                    var products = (await _totvsPurchaseOrdersRepository.GetPurchaseOrderProducts(doc_company, body.branchCode.Value, body.orderCode.Value))
                        .Where(x => x is not null)
                        .ToDictionary(x => x!.productCode, x => x!.settledQuantity ?? 0);
                    var missing = body.items!.Where(x => !products.ContainsKey(x.productCode!.Value)).Select(x => x.productCode).ToList();
                    var below = body.items!.Where(x => products.TryGetValue(x.productCode!.Value, out var settled) && x.quantity < settled)
                        .Select(x => $"{x.productCode} (recebido {products[x.productCode!.Value]:0.###})").ToList();

                    if (missing.Count > 0)
                        refused = $"Produto(s) {String.Join(", ", missing)} fora do pedido de compra {key} em totvs.TotvsPurchaseOrdersProducts.";
                    else if (below.Count > 0)
                        refused = $"Quantidade abaixo da já recebida no pedido de compra {key}: {String.Join(", ", below)}.";
                }

                if (refused is not null)
                {
                    _logger.AddMessage(refused);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = refused };
                }

                // quantidade absoluta: repetir não muda nada, sem trava
                return await _totvsService.Write(company, EnumJob.TotvsPostPurchaseOrdersQuantityItems, user_name, key, HttpMethod.Post, "purchase-order/v2/quantity-items", body,
                    "Confira as quantidades do pedido na Totvs antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrderBatchLink(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.BatchLink body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostPurchaseOrderBatchLink);

            var key = $"{body.branchCode}|{(body.batchItemBarCode ?? $"{body.batchCode}|{body.sequence}")}|{body.orderCode}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // a loja do corpo é a do lote; a do pedido a Totvs procura (§6.5)
                if (!(await _totvsService.GetBranchCodes(company)).Contains(body.branchCode!.Value))
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCode.Value.ToString()]);

                // cria ou troca o vínculo: repetir dá no mesmo, sem trava. 404 e 409 da Totvs voltam como 404 e 409 (DP5)
                return await _totvsService.Write(company, EnumJob.TotvsPostPurchaseOrderBatchLink, user_name, key, HttpMethod.Post, "purchase-order/v2/purchase-order/batch-link", body,
                    "Confira o vínculo do item de lote antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostPurchaseOrdersCancel(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.PurchaseOrders.CancelPurchaseOrder body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostPurchaseOrdersCancel);

            var key = $"{body.branchCode}|{body.orderCode}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (!(await _totvsService.GetBranchCodes(company)).Contains(body.branchCode!.Value))
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCode.Value.ToString()]);

                // DP3 e DP7, só com o pedido no banco: o fornecedor precisa ser o esperado (um número errado cancelaria o pedido de outro fornecedor),
                // e cancelado, atendido, parcialmente recebido ou distribuído não cancela
                var state = await _totvsPurchaseOrdersRepository.GetPurchaseOrderState(doc_company, body.branchCode.Value, body.orderCode!.Value);
                string? refused = null;

                if (state is not null)
                {
                    var supplierMatches = body.supplierCode is not null ? state.supplierCode == body.supplierCode : state.supplierCpfCnpj == body.supplierCpfCnpj;

                    if (!supplierMatches)
                        refused = $"O pedido de compra {key} é de outro fornecedor em totvs.TotvsPurchaseOrders: nada foi cancelado.";
                    else if (state.status is "6")
                        refused = $"O pedido de compra {key} já está {StatusName(state.status)} em totvs.TotvsPurchaseOrders.";
                    else if (state.status is "3" or "4" or "7")
                        refused = $"O pedido de compra {key} está {StatusName(state.status)} em totvs.TotvsPurchaseOrders: já há recebimento.";
                }

                if (refused is not null)
                {
                    _logger.AddMessage(refused);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = refused };
                }

                // à Totvs vão só loja e pedido (CancelOrderCommand); o fornecedor esperado é só nosso. Repetir dá 400 (já cancelado): sem trava
                return await _totvsService.Write(company, EnumJob.TotvsPostPurchaseOrdersCancel, user_name, key, HttpMethod.Post, "purchase-order/v2/orders/cancel",
                    new { body.branchCode, body.orderCode }, "Confira a situação do pedido na Totvs antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }
    }
}

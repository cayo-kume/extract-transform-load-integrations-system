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
using Application.Shared.Attributes;

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
        private readonly IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs.SaleDiscountType> _saleDiscountTypeValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs.SaleClassification> _saleClassificationValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs.SaleOrderBatch> _saleOrderBatchValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.BillingSuggestions.BillingSuggestion> _billingSuggestionValidator;

        private const string OrdersKey = "Totvs:SalesOrders";
        private const string SaleDiscountTypeKey = "Totvs:SaleDiscountTypes";
        private const string SaleClassificationKey = "Totvs:SaleClassifications";
        private const string SaleOrderBatchKey = "Totvs:SaleOrderBatches";
        private const string BillingSuggestionsKey = "Totvs:BillingSuggestions";

        // máximo das rotas de catálogo, lote de pedidos e sugestão de faturamento (swagger da API Sales Order)
        private const int CatalogsPageSize = 1000;

        // máximo da rota (swagger da API Sales Order); 252 pedidos com o expand completo levaram 6 s (testado)
        private const int OrdersPageSize = 500;

        // a carga inicial (5 meses) é lida em fatias, e cada fatia é gravada ao terminar (como nos produtos e pessoas)
        private const int OrdersWindowDays = 30;

        // expand da documentação; o expand do código antigo já batia com a documentação
        private const string Expand = "items,invoices,shippingAddress,observations,invoiceObservations,representativeObservations,classifications,discounts,commissioneds,counts";

        // grupo das escritas desta API na configuração (Totvs:Writes:SalesOrders:Enabled, T4)
        private const string WritesGroup = "SalesOrders";

        public TotvsSalesOrdersService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsSalesOrdersRepository totvsSalesOrdersRepository,
            IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.SaleOrder> orderValidator,
            IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs.SaleDiscountType> saleDiscountTypeValidator,
            IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs.SaleClassification> saleClassificationValidator,
            IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs.SaleOrderBatch> saleOrderBatchValidator,
            IValidator<Domain.Totvs.Dtos.Reads.SalesOrders.BillingSuggestions.BillingSuggestion> billingSuggestionValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsSalesOrdersRepository, _orderValidator, _saleDiscountTypeValidator, _saleClassificationValidator, _saleOrderBatchValidator, _billingSuggestionValidator) =
            (apiClient, logger, cache, totvsService, totvsSalesOrdersRepository, orderValidator, saleDiscountTypeValidator, saleClassificationValidator, saleOrderBatchValidator, billingSuggestionValidator);

        [JobVersion("1.0.0")]
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

        [JobVersion("1.0.0")]
        public async Task<bool?> GetDiscountType()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSalesOrderDiscountType);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsSalesOrdersRepository.GetSaleDiscountTypesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs.SaleDiscountType>(company, "sales-order/v2/discount-type", page => new Dictionary<string, string>
                    {
                        { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                        { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                        { "Page", page.ToString() },
                        { "PageSize", CatalogsPageSize.ToString() }
                    });

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{SaleDiscountTypeKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsSalesOrdersRepository.GetSaleDiscountTypes(company.doc_company!);

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.code)
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    items.RemoveAll(x =>
                        itemsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code), out var y) &&
                        NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa. Sem a chave, o registro vai
                    // para o log e não é gravado. Devolve a quantidade de registros gravados.
                    var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleDiscountType record, string json)>();

                    foreach (var item in items)
                    {
                        var validations = _saleDiscountTypeValidator.Validate(item.dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"tipo de desconto {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code)}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        if (ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code) != 0)
                            _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleDiscountType(company.doc_company!, batchTime, item.dto), item.json.ToString(Formatting.None)));
                    }

                    // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    var records = _listSomenteNovos.GroupBy(x => x.record.code).Select(g => g.Last()).ToList();

                    if (records.Count == 0)
                        continue;

                    await _totvsSalesOrdersRepository.BulkInsertIntoTotvsSaleDiscountTypes(records.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

                    foreach (var (record, json) in records)
                    {
                        var key = record.code;

                        _logger.AddRecord(key.ToString(), json);

                        if (itemsCache is not null)
                            itemsCache[key] = record;
                    }

                    totalInseridos += records.Count;
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

        [JobVersion("1.0.0")]
        public async Task<bool?> GetClassifications()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSalesOrderClassifications);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsSalesOrdersRepository.GetSaleClassificationsLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs.SaleClassification>(company, "sales-order/v2/classifications", page => new Dictionary<string, string>
                    {
                        { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                        { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                        { "Page", page.ToString() },
                        { "PageSize", CatalogsPageSize.ToString() }
                    });

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{SaleClassificationKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsSalesOrdersRepository.GetSaleClassifications(company.doc_company!);

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => $"{x!.typeCode}|{x!.code.ToUpperInvariant()}")
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    items.RemoveAll(x =>
                        itemsCache!.TryGetValue($"{ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.typeCode)}|{(x.dto.code ?? String.Empty).ToUpperInvariant()}", out var y) &&
                        NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa. Sem a chave, o registro vai
                    // para o log e não é gravado. Devolve a quantidade de registros gravados.
                    var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleClassification record, string json)>();

                    foreach (var item in items)
                    {
                        var validations = _saleClassificationValidator.Validate(item.dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"classificação {$"{ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.typeCode)}|{(item.dto.code ?? String.Empty).ToUpperInvariant()}"}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        if (ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.typeCode) != 0 && !String.IsNullOrWhiteSpace(item.dto.code))
                            _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleClassification(company.doc_company!, batchTime, item.dto), item.json.ToString(Formatting.None)));
                    }

                    // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    var records = _listSomenteNovos.GroupBy(x => $"{x.record.typeCode}|{x.record.code.ToUpperInvariant()}").Select(g => g.Last()).ToList();

                    if (records.Count == 0)
                        continue;

                    await _totvsSalesOrdersRepository.BulkInsertIntoTotvsSaleClassifications(records.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

                    foreach (var (record, json) in records)
                    {
                        var key = $"{record.typeCode}|{record.code.ToUpperInvariant()}";

                        _logger.AddRecord(key.ToString(), json);

                        if (itemsCache is not null)
                            itemsCache[key] = record;
                    }

                    totalInseridos += records.Count;
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

        [JobVersion("1.0.0")]
        public async Task<bool?> GetBatchItems()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSalesOrderBatchItems);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsSalesOrdersRepository.GetSaleOrderBatchesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs.SaleOrderBatch>(company, "sales-order/v2/batch-items", page => new Dictionary<string, string>
                    {
                        { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                        { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                        { "Page", page.ToString() },
                        { "PageSize", CatalogsPageSize.ToString() }
                    });

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{SaleOrderBatchKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsSalesOrdersRepository.GetSaleOrderBatches(company.doc_company!);

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.code.ToUpperInvariant())
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    items.RemoveAll(x =>
                        itemsCache!.TryGetValue((x.dto.code ?? String.Empty).ToUpperInvariant(), out var y) &&
                        NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa. Sem a chave, o registro vai
                    // para o log e não é gravado. Devolve a quantidade de registros gravados.
                    var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleOrderBatch record, string json)>();

                    foreach (var item in items)
                    {
                        var validations = _saleOrderBatchValidator.Validate(item.dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"lote de pedidos {(item.dto.code ?? String.Empty).ToUpperInvariant()}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        if (!String.IsNullOrWhiteSpace(item.dto.code))
                            _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleOrderBatch(company.doc_company!, batchTime, item.dto), item.json.ToString(Formatting.None)));
                    }

                    // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    var records = _listSomenteNovos.GroupBy(x => x.record.code.ToUpperInvariant()).Select(g => g.Last()).ToList();

                    if (records.Count == 0)
                        continue;

                    await _totvsSalesOrdersRepository.BulkInsertIntoTotvsSaleOrderBatches(records.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

                    foreach (var (record, json) in records)
                    {
                        var key = record.code.ToUpperInvariant();

                        _logger.AddRecord(key.ToString(), json);

                        if (itemsCache is not null)
                            itemsCache[key] = record;
                    }

                    totalInseridos += records.Count;
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

        [JobVersion("1.0.0")]
        public async Task<bool?> GetBillingSuggestions()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSalesOrderBillingSuggestions);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // a rota não filtra por lista de lojas (BranchCode só junto com SuggestionCode): a sugestão de loja fora de
                    // TotvsBranches da empresa vai para o log e é descartada
                    var branchCodes = (await _totvsService.GetBranchCodes(company)).ToHashSet();

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    var endDate = _totvsService.GetBatchTime();

                    // banco vazio: desde 01/01/2025 (a carga inicial dos pedidos), em fatias de 30 dias; depois, MAX(maxChangeFilterDate) − 1 h
                    var lastChange = await _totvsSalesOrdersRepository.GetBillingSuggestionsLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2025, 1, 1);

                    for (var windowStart = since; windowStart < endDate; windowStart = windowStart.AddDays(OrdersWindowDays))
                    {
                        var windowEnd = windowStart.AddDays(OrdersWindowDays) < endDate ? windowStart.AddDays(OrdersWindowDays) : endDate;

                        var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.SalesOrders.BillingSuggestions.BillingSuggestion>(company, "sales-order/v2/billing-suggestions", page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{windowStart:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{windowEnd:yyyy-MM-ddTHH:mm:ss}" },
                            { "Expand", "items" },
                            { "Order", "branchCode,suggestionCode" },
                            { "Page", page.ToString() },
                            { "PageSize", CatalogsPageSize.ToString() }
                        });

                        // fatia incompleta para a leitura: a próxima execução continua do MAX(maxChangeFilterDate)
                        if (items is null)
                            break;

                        foreach (var outside in items.Where(x => !branchCodes.Contains(ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.branchCode))).Select(x => x.dto.branchCode).Distinct())
                            _logger.AddMessage($"Sugestões de faturamento da loja {outside} descartadas: a loja não está em totvs.TotvsBranches da empresa {company.doc_company}.");

                        items.RemoveAll(x => !branchCodes.Contains(ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.branchCode)));

                        if (items.Count == 0)
                            continue;

                        static (int, int) Key(Domain.Totvs.Dtos.Reads.SalesOrders.BillingSuggestions.BillingSuggestion x) =>
                            (ConverterValidatorsExtensions.ConvertToInt32Validation(x.branchCode), ConverterValidatorsExtensions.ConvertToInt32Validation(x.suggestionCode));

                        // cache de estado: (loja, sugestão) -> maxChangeFilterDate gravado, por empresa
                        var itemsCache = await _cache.GetOrCreateAsync($"{BillingSuggestionsKey}:{company.doc_company}", async entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                            var list = await _totvsSalesOrdersRepository.GetBillingSuggestionsByKeys(company.doc_company!, items.Select(x => Key(x.dto)).Where(x => x.Item1 != 0 && x.Item2 != 0).Distinct().ToList());

                            return list
                                .Where(x => x != null)
                                .GroupBy(x => (x!.branchCode, x.suggestionCode))
                                .ToDictionary(g => g.Key, g => g.First()!.maxChangeFilterDate);
                        });

                        items.RemoveAll(x =>
                            itemsCache!.TryGetValue(Key(x.dto), out var y) &&
                            NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y
                        );

                        // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
                        // Devolve a quantidade de sugestões gravadas.
                        var batchTime = _totvsService.GetBatchTime();

                        var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestion record, string json)>();

                        foreach (var item in items)
                        {
                            var validations = _billingSuggestionValidator.Validate(item.dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"sugestão de faturamento {item.dto.branchCode}/{item.dto.suggestionCode}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            var record = new Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestion(company.doc_company!, batchTime, item.dto);

                            if (record.branchCode != 0 && record.suggestionCode != 0)
                                _listSomenteNovos.Add((record, item.json.ToString(Formatting.None)));
                        }

                        // a mesma sugestão pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                        var records = _listSomenteNovos.GroupBy(x => (x.record.branchCode, x.record.suggestionCode)).Select(g => g.Last()).ToList();

                        if (records.Count == 0)
                            continue;

                        await _totvsSalesOrdersRepository.BulkInsertIntoTotvsBillingSuggestions(records.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

                        foreach (var (record, json) in records)
                        {
                            _logger.AddRecord($"{record.branchCode}|{record.suggestionCode}", json);

                            if (itemsCache is not null)
                                itemsCache[(record.branchCode, record.suggestionCode)] = record.maxChangeFilterDate;
                        }

                        totalInseridos += records.Count;
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

        public async Task<Domain.Totvs.Dtos.Writes.Result> GetInvoices(string user_name, string doc_company, int branchCode, int orderCode, List<string>? invoiceStatus)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSalesOrderInvoices);

            var key = $"{branchCode}|{orderCode}";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // as notas já chegam pelo orders/search (TotvsSaleOrdersInvoices): gravar a filha sem o pai quebraria o merge (§7.3). Pedido
                // inexistente: 400 KeyNotFound (404 aqui)
                return await _totvsService.ReadOnDemand(company, EnumJob.TotvsGetSalesOrderInvoices, user_name, key, HttpMethod.Get, TotvsQuery.Route("sales-order/v2/invoices", ("BranchCode", branchCode), ("OrderCode", orderCode), ("InvoiceStatusType", invoiceStatus)), null, "invoices");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> GetPendingItems(string user_name, string doc_company, int branchCode, int orderCode)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSalesOrderPendingItems);

            var key = $"{branchCode}|{orderCode}";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // pedido sem pendência responde 200 com items vazio, que aqui também é 200 (é a resposta: nada pendente)
                return await _totvsService.ReadOnDemand(company, EnumJob.TotvsGetSalesOrderPendingItems, user_name, key, HttpMethod.Get, TotvsQuery.Route("sales-order/v2/pending-items", ("BranchCode", branchCode), ("OrderCode", orderCode)), null, null);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> GetChangedBatchItems(string user_name, string doc_company, int branchCode, int orderCode, int? batchCode, DateTime? startDate, DateTime? endDate)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSalesOrderChangedBatchItems);

            var key = $"{branchCode}|{orderCode}";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // a rota exige loja, pedido e o período (400 EmptyField, testado): não serve de leitura recorrente (veio da SO-1); sem período, desde 2000
                return await _totvsService.ReadOnDemand(company, EnumJob.TotvsGetSalesOrderChangedBatchItems, user_name, key, HttpMethod.Get, TotvsQuery.Route("sales-order/v2/changed-batch-items", ("BranchCode", branchCode), ("OrderCode", orderCode), ("BatchCode", batchCode), ("StartDate", $"{startDate ?? new DateTime(2000, 1, 1):yyyy-MM-ddTHH:mm:ss}"), ("EndDate", $"{endDate ?? _totvsService.GetBatchTime():yyyy-MM-ddTHH:mm:ss}")), null, "items");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostAdditionalOrder(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderAdditional body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSalesOrderAdditionalOrder);

            var key = OrderKeyValue(body);

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (await OrderBranchNotFound(company, doc_company, key, body) is { } branchNotFound)
                    return branchNotFound;

                // valores absolutos: repetir não muda nada, sem trava
                return await _totvsService.Write(company, EnumJob.TotvsPostSalesOrderAdditionalOrder, user_name, key, HttpMethod.Post, "sales-order/v2/additional-order", body,
                    "Confira os dados adicionais do pedido na Totvs antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostShippingOrder(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderShipping body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSalesOrderShippingOrder);

            var key = OrderKeyValue(body);

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (await OrderBranchNotFound(company, doc_company, key, body) is { } branchNotFound)
                    return branchNotFound;

                // valores absolutos (o frete muda o total do pedido, mas repetir não soma): sem trava
                return await _totvsService.Write(company, EnumJob.TotvsPostSalesOrderShippingOrder, user_name, key, HttpMethod.Post, "sales-order/v2/shipping-order", body,
                    "Confira o transporte do pedido na Totvs antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostObservationsOrder(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderObservations body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSalesOrderObservationsOrder);

            var key = OrderKeyValue(body);

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (await OrderBranchNotFound(company, doc_company, key, body) is { } branchNotFound)
                    return branchNotFound;

                // inclui as linhas: repetir duplica, daí a trava (T5)
                return await _totvsService.Write(company, EnumJob.TotvsPostSalesOrderObservationsOrder, user_name, key, HttpMethod.Post, "sales-order/v2/observations-order", body,
                    "Confira as observações do pedido na Totvs antes de repetir.", repeatLock: true);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostAdditionalItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsAdditional body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSalesOrderAdditionalItems);

            var key = OrderKeyValue(body);

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (await OrderBranchNotFound(company, doc_company, key, body) is { } branchNotFound)
                    return branchNotFound;

                // valores absolutos: repetir não muda nada, sem trava
                return await _totvsService.Write(company, EnumJob.TotvsPostSalesOrderAdditionalItems, user_name, key, HttpMethod.Post, "sales-order/v2/additional-items", body,
                    "Confira os itens do pedido na Totvs antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        // situação valor absoluto: repetir não muda nada, sem trava. BillingReleased libera o pedido para faturar
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostOrdersChangeStatus(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderStatusChange body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderOrdersChangeStatus, HttpMethod.Post, "sales-order/v2/orders/change-status", body.branchCode,
                $"{OrderKeyValue(body)}|{body.statusOrder}", body, "Confira a situação do pedido antes de repetir.");

        // SO7: operação e tabela de preço só mudam com o pedido em andamento ou bloqueado, conferido quando o pedido está no banco
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostHeader(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderHeader body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderHeader, HttpMethod.Post, "sales-order/v2/header", body.branchCode, OrderKeyValue(body), body,
                "Confira a capa do pedido antes de repetir.",
                check: async () =>
                {
                    if ((body.operationCode is null && body.priceTableCode is null) || body.branchCode is null)
                        return null;

                    var status = await _totvsSalesOrdersRepository.GetSaleOrderStatus(doc_company, body.branchCode.Value, body.orderCode!.Value);

                    return String.IsNullOrEmpty(status) || status is "InProgress" or "Blocked"
                        ? null
                        : $"O pedido de venda {OrderKeyValue(body)} está {status} em totvs.TotvsSaleOrders: operação e tabela de preço só mudam em andamento ou bloqueado.";
                });

        // SO7: a quantidade nova precisa ser maior que a atual (reduzir é pelo cancelamento), conferida para os itens que estão no banco
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostQuantityItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsQuantity body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderQuantityItems, HttpMethod.Post, "sales-order/v2/quantity-items", body.branchCode, OrderKeyValue(body), body,
                "Confira as quantidades do pedido antes de repetir.",
                check: async () =>
                {
                    if (body.branchCode is null)
                        return null;

                    var stored = (await _totvsSalesOrdersRepository.GetSaleOrderProducts(doc_company, body.branchCode.Value, body.orderCode!.Value)).Where(x => x is not null).ToList();
                    var notAbove = body.items!
                        .Select(i => (item: i, current: stored.FirstOrDefault(s => i.productCode is not null ? s!.productCode == i.productCode : s!.productSku == i.productSku)))
                        .Where(x => x.current?.quantity is decimal current && x.item.quantity <= current)
                        .Select(x => $"{(object?)x.item.productCode ?? x.item.productSku} (atual {x.current!.quantity:0.###})")
                        .ToList();

                    return notAbove.Count == 0 ? null : $"A quantidade nova precisa ser maior que a atual (para reduzir, cancele itens): {String.Join(", ", notAbove)}.";
                });

        // inclui itens: repetir somaria ou daria erro, daí a trava (T5)
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsAdd body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderItems, HttpMethod.Post, "sales-order/v2/items", body.branchCode, OrderKeyValue(body), body,
                "Confira os itens do pedido antes de repetir.", repeatLock: true);

        // sem corpo: os campos vão na query (a rota gravada em TotvsRequests os leva); LastChangeUser e ValidationResult.* não vão (§7.4.2)
        public async Task<Domain.Totvs.Dtos.Writes.Result> DeleteItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemDelete parameters) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsDeleteSalesOrderItems, HttpMethod.Delete,
                TotvsQuery.Route("sales-order/v2/items", ("BranchCode", parameters.branchCode), ("OrderCode", parameters.orderCode), ("OrderId", parameters.orderId),
                    ("ProductCode", parameters.productCode), ("ProductSku", parameters.productSku)),
                parameters.branchCode, $"{OrderKeyValue(parameters)}|{(object?)parameters.productCode ?? parameters.productSku}", null, "Confira os itens do pedido antes de repetir.");

        // vínculo de contagens: repetir pode duplicar, daí a trava (T5)
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostRelationshipCounts(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderCounts body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderRelationshipCounts, HttpMethod.Post, "sales-order/v2/relationship-counts", body.branchCode,
                $"{body.branchCode}|{body.orderCode}", body, "Confira as contagens do pedido antes de repetir.", repeatLock: true);

        // item de lote já ligado dá erro e isUpdateQuantity muda a quantidade do pedido: trava (T5)
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostRelationshipBatchItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItems body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderRelationshipBatchItems, HttpMethod.Post, "sales-order/v2/relationship-batch-items", body.branchCode,
                $"{body.branchCode}|{body.orderCode}", body, "Confira os itens de lote do pedido antes de repetir.", repeatLock: true,
                note: body.items!.Any(x => x.isUpdateQuantity == true) ? $"Vínculo de itens de lote no pedido {body.branchCode}|{body.orderCode} com isUpdateQuantity: a quantidade do pedido pode mudar." : null);

        public async Task<Domain.Totvs.Dtos.Writes.Result> DeleteBatchItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItemDelete parameters) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsDeleteSalesOrderBatchItems, HttpMethod.Delete,
                TotvsQuery.Route("sales-order/v2/batch-items", ("BranchCode", parameters.branchCode), ("OrderCode", parameters.orderCode), ("OrderId", parameters.orderId),
                    ("ProductCode", parameters.productCode), ("ProductSku", parameters.productSku), ("BatchSequence", parameters.batchSequence)),
                parameters.branchCode, $"{OrderKeyValue(parameters)}|{(object?)parameters.productCode ?? parameters.productSku}|{parameters.batchSequence}", null,
                "Confira os itens de lote do pedido antes de repetir.");

        // o ERP escolhe e grava os itens de lote: trava (T5)
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostSuggestBatchItemsNuance(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItemsNuance body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderSuggestBatchItemsNuance, HttpMethod.Post, "sales-order/v2/suggest-batch-items-nuance", body.branchCode,
                OrderKeyValue(body), body, "Confira os itens de lote do pedido antes de repetir.", repeatLock: true);

        // preço novo dos itens: valor absoluto, sem trava; sem conferência local de preço (o corpo fica em TotvsRequests para auditoria)
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostPriceItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsPrice body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderPriceItems, HttpMethod.Post, "sales-order/v2/price-items", body.branchCode, OrderKeyValue(body), body,
                "Confira os preços dos itens do pedido antes de repetir.");

        // cancelQuantity é delta: repetir cancela de novo, daí a trava (T5). SO7: com o item no banco, não passa da quantidade pendente
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostCancelItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsCancel body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderCancelItems, HttpMethod.Post, "sales-order/v2/cancel-items", body.branchCode, OrderKeyValue(body), body,
                "Confira as quantidades canceladas do pedido antes de repetir.", repeatLock: true,
                check: async () =>
                {
                    if (body.branchCode is null)
                        return null;

                    var stored = (await _totvsSalesOrdersRepository.GetSaleOrderProducts(doc_company, body.branchCode.Value, body.orderCode!.Value)).Where(x => x is not null).ToList();
                    var above = body.items!
                        .Select(i => (item: i, current: stored.FirstOrDefault(s => i.productCode is not null ? s!.productCode == i.productCode : s!.productSku == i.productSku)))
                        .Where(x => x.current?.pendingQuantity is decimal pending && x.item.cancelQuantity > pending)
                        .Select(x => $"{(object?)x.item.productCode ?? x.item.productSku} (pendente {x.current!.pendingQuantity:0.###})")
                        .ToList();

                    return above.Count == 0 ? null : $"Quantidade a cancelar acima da pendente: {String.Join(", ", above)}.";
                });

        // cancela o pedido inteiro, sem volta pela API. SO7: com o pedido no banco, cancelado ou atendido não cancela. O segundo cancelamento a
        // Totvs recusa: sem trava
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostOrdersCancel(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderCancel body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderOrdersCancel, HttpMethod.Post, "sales-order/v2/orders/cancel", body.branchCode, OrderKeyValue(body), body,
                "Confira a situação do pedido antes de repetir.",
                check: async () =>
                {
                    if (body.branchCode is null)
                        return null;

                    var status = await _totvsSalesOrdersRepository.GetSaleOrderStatus(doc_company, body.branchCode.Value, body.orderCode!.Value);

                    return status switch
                    {
                        "Canceled" => $"O pedido de venda {OrderKeyValue(body)} já está cancelado em totvs.TotvsSaleOrders.",
                        "Attended" => $"O pedido de venda {OrderKeyValue(body)} está atendido em totvs.TotvsSaleOrders: não cancela.",
                        _ => null
                    };
                });

        // liga o pedido a uma venda do PDV (passa a constar como faturado pelo PDV): trava (T5), a repetição com o mesmo transactionId é incerta
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostPdvTransactions(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.OrderPdvTransaction body) =>
            await WriteOrder(user_name, doc_company, EnumJob.TotvsPostSalesOrderPdvTransactions, HttpMethod.Post, "sales-order/v2/pdv-transactions", body.branchCode,
                $"{OrderKeyValue(body)}|{body.transactionId}", body, "Confira a transação do pedido antes de repetir.", repeatLock: true);

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostB2COrders(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders.B2COrder body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSalesOrderB2COrders);

            var key = $"{body.branchCode}|{body.orderId}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (!(await _totvsService.GetBranchCodes(company)).Contains(body.branchCode!.Value))
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCode.Value.ToString()]);

                // SO5: o orderId é o único controle de duplicidade e não há nova tentativa em 5xx, então a busca vem antes, sempre. Achou: 422 com a
                // chave do pedido existente. Não deu para conferir (5xx, 429, login): não inclui. A busca fica em TotvsRequests
                var search = await _totvsService.ReadOnDemand(company, EnumJob.TotvsPostSalesOrderB2COrders, user_name, key, HttpMethod.Post, "sales-order/v2/orders/search",
                    JsonConvert.SerializeObject(new { filter = new { branchCodeList = new[] { body.branchCode }, orderIdList = new[] { body.orderId } }, page = 1, pageSize = 1 }), "items");

                if (search.success)
                {
                    var existing = search.data is { ValueKind: System.Text.Json.JsonValueKind.Object } data && data.TryGetProperty("items", out var items) && items.GetArrayLength() > 0
                        ? $"{items[0].GetProperty("branchCode")}|{items[0].GetProperty("orderCode")}" : "?";
                    var refused = $"O orderId {body.orderId} já existe na Totvs (pedido {existing}): nada foi incluído.";
                    _logger.AddMessage(refused);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = refused };
                }

                if (search.statusCode != 404)
                {
                    var notChecked = $"Não foi possível conferir se o orderId {body.orderId} já existe ({search.message}): nada foi incluído.";
                    _logger.AddMessage(notChecked);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = search.statusCode, apiCalled = search.apiCalled, retryAfterSeconds = search.retryAfterSeconds, message = notChecked };
                }

                return await _totvsService.Write(company, EnumJob.TotvsPostSalesOrderB2COrders, user_name, key, HttpMethod.Post, "sales-order/v2/b2c-orders", body,
                    "Confira o pedido pelo orderId antes de repetir (uma nova chamada já confere).", repeatLock: true);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        // ------------------------------------------------------------------------------------------------------

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

        /// <summary>
        /// lê todas as páginas de um GET das leituras de catálogo e de sugestão. Devolve cada item com o seu JSON, ou null se alguma
        /// página falhou (a mensagem vai para o log). 404 (declarado por discount-type, batch-items e billing-suggestions) é lista vazia
        /// </summary>
        private async Task<List<(T dto, JObject json)>?> GetCatalogPages<T>(Domain.Totvs.Entities.Parameters.Company company, string route, Func<int, Dictionary<string, string>> query)
        {
            var items = new List<(T dto, JObject json)>();
            var page = 1;
            var totalPages = 1;

            while (page <= totalPages)
            {
                // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                ApiResponse? response = null;

                for (var attempt = 1; attempt <= 2; attempt++)
                {
                    var token = await _totvsService.GetToken(company);

                    if (token is null)
                        break;   // a mensagem do login já foi para o log

                    response = await _apiClient.SendAsync(
                        HttpMethod.Get,
                        route,
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } },
                        queryParams: query(page)
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

                if (response is not null && response.StatusCode == 404)
                    return items;

                if (response is null || !response.IsSuccess)
                {
                    if (response is not null)
                    {
                        var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                        _logger.AddMessage($"Erro da API Totvs em {route} (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
                    }

                    return null;
                }

                Page<T>? result = null;
                JArray json = [];

                try
                {
                    result = JsonConvert.DeserializeObject<Page<T>>(response.Content ?? String.Empty);

                    // DateParseHandling.None: o JSON gravado fica com as datas como a API mandou
                    using var reader = new JsonTextReader(new StringReader(response.Content!)) { DateParseHandling = DateParseHandling.None };
                    json = (JObject.Load(reader)["items"] as JArray) ?? [];
                }
                catch (JsonException)
                {
                    // result continua null: tratado abaixo como resposta inválida
                }

                if (result?.items is null || json.Count != result.items.Count)
                {
                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                    _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {company.doc_company}, página {page}): {body}");

                    return null;
                }

                for (var i = 0; i < json.Count; i++)
                    items.Add((result.items[i], (JObject)json[i]));

                totalPages = result.totalPages;
                page++;
            }

            return items;
        }

        // chave das escritas do pedido em TotvsRequests: loja|pedido ou o orderId
        private static string OrderKeyValue(Domain.Totvs.Dtos.Writes.SalesOrders.OrderKey body) =>
            String.IsNullOrEmpty(body.orderId) ? $"{body.branchCode}|{body.orderCode}" : body.orderId;

        // passo 3 da §1.2: com loja + número, a loja precisa ser da empresa; com orderId não há loja a conferir (a Totvs acha o pedido)
        private async Task<Domain.Totvs.Dtos.Writes.Result?> OrderBranchNotFound(Domain.Totvs.Entities.Parameters.Company company, string doc_company, string key, Domain.Totvs.Dtos.Writes.SalesOrders.OrderKey body)
        {
            if (body.branchCode is not int branchCode || (await _totvsService.GetBranchCodes(company)).Contains(branchCode))
                return null;

            return _totvsService.BranchNotFound(doc_company, key, [branchCode.ToString()]);
        }

        // escritas do pedido de venda da onda 4 (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §7.4) pela base comum: liga/desliga, empresa, loja do pedido
        // (quando vem por loja + número), a conferência local, se houver (SO7, só com o pedido no banco), e a chamada
        private async Task<Domain.Totvs.Dtos.Writes.Result> WriteOrder(string user_name, string doc_company, EnumJob job, HttpMethod method, string route, int? branchCode, string key, object? body,
            string checkHint, bool repeatLock = false, Func<Task<string?>>? check = null, string? note = null)
        {
            _logger
                .Clear()
                .AddLog(job);

            if (note is not null)
                _logger.AddMessage(note);

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (branchCode is int branch && !(await _totvsService.GetBranchCodes(company)).Contains(branch))
                    return _totvsService.BranchNotFound(doc_company, key, [branch.ToString()]);

                if (check is not null && await check() is { } refused)
                {
                    _logger.AddMessage(refused);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = refused };
                }

                return await _totvsService.Write(company, job, user_name, key, method, route, body, checkHint, repeatLock);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }
    }
}

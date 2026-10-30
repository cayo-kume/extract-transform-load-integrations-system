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
using System.Text.RegularExpressions;

namespace Application.Totvs.Services
{
    /// <summary>
    /// preços e custos dos produtos (fase PR-2, docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9, PR12): todos os códigos de preço e de custo
    /// do cadastro, uma chamada por loja de cada marca; e os itens das tabelas de preço e das promoções (§5.2). Cada método faz as
    /// próprias chamadas à API; o comum vem do ITotvsService
    /// </summary>
    public class TotvsProductPricesService : ITotvsProductPricesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsProductPricesRepository _totvsProductPricesRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Values.ProductPrice> _productPriceValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Values.ProductCost> _productCostValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.PriceTables.PriceTableProduct> _priceTableProductValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Promotions.Promotion> _promotionValidator;

        private const string PricesKey = "Totvs:Prices";
        private const string CostsKey = "Totvs:Costs";
        private const string PriceCodesKey = "Totvs:PriceCodes";
        private const string CostCodesKey = "Totvs:CostCodes";
        private const string PriceTableItemsKey = "Totvs:PriceTableItems";

        // sem tabela de preço na base, o tempo não foi medido: a página segue a dos preços
        private const int PriceTablesPageSize = 100;

        // o swagger diz 500, mas a rota recusa mais de 100 (400 FieldValueGreaterThan, testado)
        private const int PromotionsPageSize = 100;

        // uma loja com os 6 preços: 100 produtos por página em ~2 s; com os 8 custos, 300 em ~14 s (testado)
        private const int PricesPageSize = 100;
        private const int CostsPageSize = 100;

        // os códigos de preço, custo e saldo do cadastro (PRDFL003 e PRDFL002) vão de 1 a 999
        private const int MaxValueCode = 999;

        // startProductCode exige endProductCode (testado): o último código possível (9 dígitos)
        private const int LastProductCode = 999999999;

        // o filter.change aceita no máximo 1 ano (400 GeneralGreaterThanOneYear, testado): a janela vai em fatias
        private const int WindowDays = 30;

        public TotvsProductPricesService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsProductPricesRepository totvsProductPricesRepository,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Values.ProductPrice> productPriceValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Values.ProductCost> productCostValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.PriceTables.PriceTableProduct> priceTableProductValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Promotions.Promotion> promotionValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsProductPricesRepository, _productPriceValidator, _productCostValidator, _priceTableProductValidator, _promotionValidator) =
            (apiClient, logger, cache, totvsService, totvsProductPricesRepository, productPriceValidator, productCostValidator, priceTableProductValidator, promotionValidator);

        public async Task<bool?> PostSearchPrices()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchPrices);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var codes = await GetPriceCodes(company);

                    if (codes is null)
                        continue;   // a mensagem já foi para o log

                    foreach (var (baseBranchCode, brandClassificationCode) in TotvsProductBrands.BaseBranchCodes)
                    {
                        var brandBranchCodes = await _totvsService.GetBrandBranchCodes(company, baseBranchCode);

                        if (brandBranchCodes.Count == 0)
                        {
                            _logger.AddMessage($"Empresa {company.doc_company}: matriz {baseBranchCode} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                            continue;
                        }

                        // uma chamada por loja: o tempo de resposta cresce com produtos x lojas x códigos (15 lojas numa chamada deram 502, testado)
                        foreach (var branchCode in brandBranchCodes)
                            totalInseridos += await SyncPrices(company, brandClassificationCode, branchCode, codes);
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

            // preços de uma loja. Sem maxChangeFilterDate gravado, a loja está na carga inicial: o filter.change aceita no máximo 1 ano
            // (testado), então ela é lida inteira, sem change, em ordem de produto a partir do maior gravado, e cada página é gravada ao chegar
            // (uma falha para a loja, e a próxima execução continua de onde parou). Depois, a janela vai do maior maxChangeFilterDate − 1 h
            // (ou do começo da carga inicial − 1 h) até agora, em fatias de {WindowDays} dias. Devolve a quantidade de produtos gravados
            async Task<int> SyncPrices(Domain.Totvs.Entities.Parameters.Company company, string brandClassificationCode, int branchCode, List<int> codes)
            {
                var batchTime = _totvsService.GetBatchTime();
                var state = await _totvsProductPricesRepository.GetPricesState(company.doc_company!, branchCode);
                var total = 0;

                if (state?.lastChange is null)
                {
                    var startProductCode = (state?.maxProductCode ?? 0) + 1;
                    var (saved, ok) = await ReadPrices(company, branchCode, batchTime, false, page => PricesBody(brandClassificationCode, branchCode, codes, page, startProductCode, null, null), $"carga inicial a partir do produto {startProductCode}");

                    total += saved;

                    if (!ok)
                        return total;
                }

                var since = (state?.lastChange ?? state?.firstIntegrated ?? batchTime).AddHours(-1);

                for (var windowStart = since; windowStart < batchTime; windowStart = windowStart.AddDays(WindowDays))
                {
                    var windowEnd = windowStart.AddDays(WindowDays) < batchTime ? windowStart.AddDays(WindowDays) : batchTime;
                    var (saved, ok) = await ReadPrices(company, branchCode, batchTime, true, page => PricesBody(brandClassificationCode, branchCode, codes, page, null, windowStart, windowEnd), $"janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}");

                    total += saved;

                    // fatia incompleta para a loja: a próxima execução relê desde o MAX(maxChangeFilterDate)
                    if (!ok)
                        break;
                }

                return total;

                // corpo do product/v2/prices/search de uma loja: na carga inicial, a faixa de produto (startProductCode exige endProductCode, testado);
                // na janela, o filter.change. Sempre a marca (classificação 6000) e a ordem por produto
                static string PricesBody(string brandClassificationCode, int branchCode, List<int> codes, int page, int? startProductCode, DateTime? windowStart, DateTime? windowEnd) =>
                    JsonConvert.SerializeObject(new
                    {
                        filter = new
                        {
                            classifications = new[] { new { type = TotvsProductBrands.ClassificationType, codeList = new[] { brandClassificationCode } } },
                            startProductCode,
                            endProductCode = startProductCode is null ? (int?)null : LastProductCode,
                            change = windowStart is null ? null : new
                            {
                                startDate = $"{windowStart:yyyy-MM-ddTHH:mm:ss}",
                                endDate = $"{windowEnd:yyyy-MM-ddTHH:mm:ss}",
                                inPrice = true,
                                // inPrice não pega a mudança de promoção: vai também o inPromotionalPrice (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9)
                                inPromotionalPrice = true,
                                branchPriceCodeList = new[] { branchCode },
                                priceCodeList = codes
                            }
                        },
                        option = new
                        {
                            prices = new[]
                            {
                                new
                                {
                                    branchCode,
                                    priceCodeList = codes,
                                    // o preço programado (PRDFM232) não tem campo próprio na resposta: fica de fora
                                    isPromotionalPrice = true,
                                    isScheduledPrice = false
                                }
                            }
                        },
                        order = "productCode",
                        expand = "promotionalInformation,informationOtherPromotions",
                        page,
                        pageSize = PricesPageSize
                    }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                // lê as páginas de uma consulta da loja e grava cada uma ao chegar. Na janela, descarta o produto cujo maxChangeFilterDate não é
                // mais recente que o gravado (cache de estado por loja). Devolve os produtos gravados e se a leitura chegou ao fim
                async Task<(int saved, bool ok)> ReadPrices(Domain.Totvs.Entities.Parameters.Company company, int branchCode, DateTime batchTime, bool useCache, Func<int, string> body, string context)
                {
                    var saved = 0;
                    var page = 1;
                    var totalPages = 1;

                    while (page <= totalPages)
                    {
                        var result = await GetValuePage<Domain.Totvs.Dtos.Reads.Products.Values.ProductPrice>(company, "product/v2/prices/search", body(page), $"loja {branchCode}, {context}, página {page}");

                        if (result is null)
                            return (saved, false);

                        var items = result.Value.items;
                        Dictionary<int, DateTime?>? itemsCache = null;

                        if (useCache && items.Count > 0)
                        {
                            // cache de estado: produto -> maior maxChangeFilterDate gravado, por empresa e loja
                            itemsCache = await _cache.GetOrCreateAsync($"{PricesKey}:{company.doc_company}:{branchCode}", async entry =>
                            {
                                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                                var list = await _totvsProductPricesRepository.GetPricesChanges(
                                    company.doc_company!,
                                    branchCode,
                                    items.Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode)).Where(x => x != 0).Distinct().ToList()
                                );

                                return list
                                    .Where(x => x != null)
                                    .GroupBy(x => x!.productCode)
                                    .ToDictionary(g => g.Key, g => g.First()!.maxChangeFilterDate);
                            });

                            // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                            items.RemoveAll(x =>
                                itemsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode), out var y) &&
                                NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y
                            );
                        }

                        // valida, monta uma linha por loja e código (com o mesmo horário de lote), grava e atualiza o cache da loja.
                        // Devolve a quantidade de produtos gravados.
                        var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.Products.Values.Price>();
                        var records = new List<(int productCode, DateTime? maxChangeFilterDate, string json)>();

                        // o mesmo produto pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                        foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode) != 0).GroupBy(x => x.dto.productCode).Select(g => g.Last()))
                        {
                            var productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.productCode);
                            var validations = _productPriceValidator.Validate(item.dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"preços do produto {productCode} na loja {branchCode}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            var rows = (item.dto.prices ?? [])
                                .Select(x => new Domain.Totvs.Entities.Reads.Products.Values.Price(company.doc_company!, batchTime, item.dto, x))
                                .Where(x => x.branchCode != 0 && x.priceCode != 0)
                                .GroupBy(x => (x.branchCode, x.priceCode))
                                .Select(g => g.Last())
                                .ToList();

                            _listSomenteNovos.AddRange(rows);
                            records.Add((productCode, NullableConverters.ToNullableDateTime(item.dto.maxChangeFilterDate), item.json.ToString(Formatting.None)));
                        }

                        if (_listSomenteNovos.Count > 0)
                        {
                            await _totvsProductPricesRepository.BulkInsertIntoTotvsPrices(_listSomenteNovos, _logger.GetExecutionGuid());

                            foreach (var (productCode, maxChangeFilterDate, json) in records)
                            {
                                _logger.AddRecord($"{productCode}|{branchCode}", json);

                                if (itemsCache is not null)
                                    itemsCache[productCode] = maxChangeFilterDate;
                            }

                            saved += records.Count;
                        }

                        totalPages = result.Value.totalPages;
                        page++;
                    }

                    return (saved, true);
                }
            }

            // todos os códigos de preço do cadastro (PRDFL003), descobertos pela própria API e guardados em cache (2 h / 30 min): os códigos de 1 a 999 vão numa chamada, e a API
            // responde 400 com "Price N not found" para cada um que não existe (testado: 1 a 6); o resto é a lista
            async Task<List<int>?> GetPriceCodes(Domain.Totvs.Entities.Parameters.Company company)
            {
                var key = $"{PriceCodesKey}:{company.doc_company}";

                if (_cache.TryGetValue(key, out List<int>? cached) && cached is not null)
                    return cached;

                var (baseBranchCode, brandClassificationCode) = TotvsProductBrands.BaseBranchCodes.First();
                var allCodes = Enumerable.Range(1, MaxValueCode).ToList();

                var requestBody = JsonConvert.SerializeObject(new
                {
                    filter = new
                    {
                        productCodeList = new[] { 1 }
                    },
                    option = new
                    {
                        prices = new[] { new { branchCode = baseBranchCode, priceCodeList = allCodes, isPromotionalPrice = false, isScheduledPrice = false } }
                    },
                    page = 1,
                    pageSize = 1
                });

                var response = await SendValueRequest(company, "product/v2/prices/search", requestBody);
                var codes = new List<int>();

                if (response is not null && (response.IsSuccess || response.StatusCode == 400))
                {
                    // a validação dos códigos vem antes da busca: o produto do filtro não precisa existir
                    var missing = new HashSet<int>();
                    var other = false;

                    // o corpo do 400 é a lista de DomainNotificationMessage (code, message), lida como no TotvsAccountsPayableService
                    try
                    {
                        if (JToken.Parse(response.Content ?? String.Empty) is JArray messages)
                        {
                            foreach (var message in messages.OfType<JObject>())
                            {
                                var match = Regex.Match((string?)message["message"] ?? String.Empty, @"^Price (\d+) not found");

                                if ((string?)message["code"] == "NotFound" && match.Success)
                                    missing.Add(int.Parse(match.Groups[1].Value));
                                else
                                    other = true;
                            }
                        }
                        else
                            other = true;   // 200 com os 999 códigos: não acontece; tratado como falha da descoberta
                    }
                    catch (JsonReaderException)
                    {
                        other = true;
                    }

                    codes = other ? [] : allCodes.Where(x => !missing.Contains(x)).ToList();
                }

                if (codes.Count == 0)
                {
                    var body = response?.Content?.Length > 800 ? response.Content[..800] : response?.Content;
                    _logger.AddMessage($"Empresa {company.doc_company}: não foi possível descobrir os códigos de preço em product/v2/prices/search ({response?.StatusCode} {body}).");

                    return null;
                }

                _cache.Set(key, codes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });

                return codes;
            }
        }

        public async Task<bool?> PostSearchCosts()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchCosts);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var codes = await GetCostCodes(company);

                    if (codes is null)
                        continue;   // a mensagem já foi para o log

                    foreach (var (baseBranchCode, brandClassificationCode) in TotvsProductBrands.BaseBranchCodes)
                    {
                        var brandBranchCodes = await _totvsService.GetBrandBranchCodes(company, baseBranchCode);

                        if (brandBranchCodes.Count == 0)
                        {
                            _logger.AddMessage($"Empresa {company.doc_company}: matriz {baseBranchCode} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                            continue;
                        }

                        // uma chamada por loja: o tempo de resposta cresce com produtos x lojas x códigos (15 lojas numa chamada deram 502, testado)
                        foreach (var branchCode in brandBranchCodes)
                            totalInseridos += await SyncCosts(company, brandClassificationCode, branchCode, codes);
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

            // custos de uma loja. Sem maxChangeFilterDate gravado, a loja está na carga inicial: o filter.change aceita no máximo 1 ano
            // (testado), então ela é lida inteira, sem change, em ordem de produto a partir do maior gravado, e cada página é gravada ao chegar
            // (uma falha para a loja, e a próxima execução continua de onde parou). Depois, a janela vai do maior maxChangeFilterDate − 1 h
            // (ou do começo da carga inicial − 1 h) até agora, em fatias de {WindowDays} dias. Devolve a quantidade de produtos gravados
            async Task<int> SyncCosts(Domain.Totvs.Entities.Parameters.Company company, string brandClassificationCode, int branchCode, List<int> codes)
            {
                var batchTime = _totvsService.GetBatchTime();
                var state = await _totvsProductPricesRepository.GetCostsState(company.doc_company!, branchCode);
                var total = 0;

                if (state?.lastChange is null)
                {
                    var startProductCode = (state?.maxProductCode ?? 0) + 1;
                    var (saved, ok) = await ReadCosts(company, branchCode, batchTime, false, page => CostsBody(brandClassificationCode, branchCode, codes, page, startProductCode, null, null), $"carga inicial a partir do produto {startProductCode}");

                    total += saved;

                    if (!ok)
                        return total;
                }

                var since = (state?.lastChange ?? state?.firstIntegrated ?? batchTime).AddHours(-1);

                for (var windowStart = since; windowStart < batchTime; windowStart = windowStart.AddDays(WindowDays))
                {
                    var windowEnd = windowStart.AddDays(WindowDays) < batchTime ? windowStart.AddDays(WindowDays) : batchTime;
                    var (saved, ok) = await ReadCosts(company, branchCode, batchTime, true, page => CostsBody(brandClassificationCode, branchCode, codes, page, null, windowStart, windowEnd), $"janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}");

                    total += saved;

                    // fatia incompleta para a loja: a próxima execução relê desde o MAX(maxChangeFilterDate)
                    if (!ok)
                        break;
                }

                return total;

                // corpo do product/v2/costs/search de uma loja: na carga inicial, a faixa de produto (startProductCode exige endProductCode, testado);
                // na janela, o filter.change. Sempre a marca (classificação 6000) e a ordem por produto
                static string CostsBody(string brandClassificationCode, int branchCode, List<int> codes, int page, int? startProductCode, DateTime? windowStart, DateTime? windowEnd) =>
                    JsonConvert.SerializeObject(new
                    {
                        filter = new
                        {
                            classifications = new[] { new { type = TotvsProductBrands.ClassificationType, codeList = new[] { brandClassificationCode } } },
                            startProductCode,
                            endProductCode = startProductCode is null ? (int?)null : LastProductCode,
                            change = windowStart is null ? null : new
                            {
                                startDate = $"{windowStart:yyyy-MM-ddTHH:mm:ss}",
                                endDate = $"{windowEnd:yyyy-MM-ddTHH:mm:ss}",
                                inCost = true,
                                branchCostCodeList = new[] { branchCode },
                                costCodeList = codes
                            }
                        },
                        option = new
                        {
                            costs = new[]
                            {
                                new
                                {
                                    branchCode,
                                    costCodeList = codes
                                }
                            }
                        },
                        order = "productCode",
                        page,
                        pageSize = CostsPageSize
                    }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                // lê as páginas de uma consulta da loja e grava cada uma ao chegar. Na janela, descarta o produto cujo maxChangeFilterDate não é
                // mais recente que o gravado (cache de estado por loja). Devolve os produtos gravados e se a leitura chegou ao fim
                async Task<(int saved, bool ok)> ReadCosts(Domain.Totvs.Entities.Parameters.Company company, int branchCode, DateTime batchTime, bool useCache, Func<int, string> body, string context)
                {
                    var saved = 0;
                    var page = 1;
                    var totalPages = 1;

                    while (page <= totalPages)
                    {
                        var result = await GetValuePage<Domain.Totvs.Dtos.Reads.Products.Values.ProductCost>(company, "product/v2/costs/search", body(page), $"loja {branchCode}, {context}, página {page}");

                        if (result is null)
                            return (saved, false);

                        var items = result.Value.items;
                        Dictionary<int, DateTime?>? itemsCache = null;

                        if (useCache && items.Count > 0)
                        {
                            // cache de estado: produto -> maior maxChangeFilterDate gravado, por empresa e loja
                            itemsCache = await _cache.GetOrCreateAsync($"{CostsKey}:{company.doc_company}:{branchCode}", async entry =>
                            {
                                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                                var list = await _totvsProductPricesRepository.GetCostsChanges(
                                    company.doc_company!,
                                    branchCode,
                                    items.Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode)).Where(x => x != 0).Distinct().ToList()
                                );

                                return list
                                    .Where(x => x != null)
                                    .GroupBy(x => x!.productCode)
                                    .ToDictionary(g => g.Key, g => g.First()!.maxChangeFilterDate);
                            });

                            // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                            items.RemoveAll(x =>
                                itemsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode), out var y) &&
                                NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y
                            );
                        }

                        // valida, monta uma linha por loja e código (com o mesmo horário de lote), grava e atualiza o cache da loja.
                        // Devolve a quantidade de produtos gravados.
                        var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.Products.Values.Cost>();
                        var records = new List<(int productCode, DateTime? maxChangeFilterDate, string json)>();

                        // o mesmo produto pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                        foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode) != 0).GroupBy(x => x.dto.productCode).Select(g => g.Last()))
                        {
                            var productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.productCode);
                            var validations = _productCostValidator.Validate(item.dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"custos do produto {productCode} na loja {branchCode}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            var rows = (item.dto.costs ?? [])
                                .Select(x => new Domain.Totvs.Entities.Reads.Products.Values.Cost(company.doc_company!, batchTime, item.dto, x))
                                .Where(x => x.branchCode != 0 && x.costCode != 0)
                                .GroupBy(x => (x.branchCode, x.costCode))
                                .Select(g => g.Last())
                                .ToList();

                            _listSomenteNovos.AddRange(rows);
                            records.Add((productCode, NullableConverters.ToNullableDateTime(item.dto.maxChangeFilterDate), item.json.ToString(Formatting.None)));
                        }

                        if (_listSomenteNovos.Count > 0)
                        {
                            await _totvsProductPricesRepository.BulkInsertIntoTotvsCosts(_listSomenteNovos, _logger.GetExecutionGuid());

                            foreach (var (productCode, maxChangeFilterDate, json) in records)
                            {
                                _logger.AddRecord($"{productCode}|{branchCode}", json);

                                if (itemsCache is not null)
                                    itemsCache[productCode] = maxChangeFilterDate;
                            }

                            saved += records.Count;
                        }

                        totalPages = result.Value.totalPages;
                        page++;
                    }

                    return (saved, true);
                }
            }

            // todos os códigos de custo do cadastro (PRDFL003), descobertos pela própria API e guardados em cache (2 h / 30 min): os códigos de 1 a 999 vão numa chamada, e a API
            // responde 400 com "Cost N not found" para cada um que não existe (testado: 1 a 8); o resto é a lista
            async Task<List<int>?> GetCostCodes(Domain.Totvs.Entities.Parameters.Company company)
            {
                var key = $"{CostCodesKey}:{company.doc_company}";

                if (_cache.TryGetValue(key, out List<int>? cached) && cached is not null)
                    return cached;

                var (baseBranchCode, brandClassificationCode) = TotvsProductBrands.BaseBranchCodes.First();
                var allCodes = Enumerable.Range(1, MaxValueCode).ToList();

                var requestBody = JsonConvert.SerializeObject(new
                {
                    filter = new
                    {
                        productCodeList = new[] { 1 }
                    },
                    option = new
                    {
                        costs = new[] { new { branchCode = baseBranchCode, costCodeList = allCodes } }
                    },
                    page = 1,
                    pageSize = 1
                });

                var response = await SendValueRequest(company, "product/v2/costs/search", requestBody);
                var codes = new List<int>();

                if (response is not null && (response.IsSuccess || response.StatusCode == 400))
                {
                    // a validação dos códigos vem antes da busca: o produto do filtro não precisa existir
                    var missing = new HashSet<int>();
                    var other = false;

                    // o corpo do 400 é a lista de DomainNotificationMessage (code, message), lida como no TotvsAccountsPayableService
                    try
                    {
                        if (JToken.Parse(response.Content ?? String.Empty) is JArray messages)
                        {
                            foreach (var message in messages.OfType<JObject>())
                            {
                                var match = Regex.Match((string?)message["message"] ?? String.Empty, @"^Cost (\d+) not found");

                                if ((string?)message["code"] == "NotFound" && match.Success)
                                    missing.Add(int.Parse(match.Groups[1].Value));
                                else
                                    other = true;
                            }
                        }
                        else
                            other = true;   // 200 com os 999 códigos: não acontece; tratado como falha da descoberta
                    }
                    catch (JsonReaderException)
                    {
                        other = true;
                    }

                    codes = other ? [] : allCodes.Where(x => !missing.Contains(x)).ToList();
                }

                if (codes.Count == 0)
                {
                    var body = response?.Content?.Length > 800 ? response.Content[..800] : response?.Content;
                    _logger.AddMessage($"Empresa {company.doc_company}: não foi possível descobrir os códigos de custo em product/v2/costs/search ({response?.StatusCode} {body}).");

                    return null;
                }

                _cache.Set(key, codes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });

                return codes;
            }
        }

        public async Task<bool?> PostSearchPriceTables()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchPriceTables);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // as tabelas vêm do cabeçalho (GetPriceTablesHeaders, que roda antes, como as filiais antes das buscas)
                    var priceTableCodes = (await _totvsProductPricesRepository.GetActivePriceTableCodes(company.doc_company!)).ToList();

                    if (priceTableCodes.Count == 0)
                        continue;

                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    // option.priceTableCode aceita uma tabela só: uma chamada por tabela, com todas as lojas
                    foreach (var priceTableCode in priceTableCodes)
                        totalInseridos += await SyncPriceTable(company, priceTableCode, branchCodes);
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

            // itens de uma tabela de preço, com o desenho dos preços: sem maxChangeFilterDate gravado, a tabela é lida inteira, sem change,
            // em ordem de produto a partir do maior gravado, cada página gravada ao chegar; depois, a janela do maior maxChangeFilterDate − 1 h
            // (ou do começo da carga inicial − 1 h) até agora, em fatias de {WindowDays} dias. Devolve a quantidade de produtos gravados
            async Task<int> SyncPriceTable(Domain.Totvs.Entities.Parameters.Company company, int priceTableCode, List<int> branchCodes)
            {
                var batchTime = _totvsService.GetBatchTime();
                var state = await _totvsProductPricesRepository.GetPriceTableItemsState(company.doc_company!, priceTableCode);
                var total = 0;

                if (state?.lastChange is null)
                {
                    var startProductCode = (state?.maxProductCode ?? 0) + 1;
                    var (saved, ok) = await ReadPriceTable(company, priceTableCode, batchTime, false, page => PriceTablesBody(priceTableCode, branchCodes, page, startProductCode, null, null), $"carga inicial a partir do produto {startProductCode}");

                    total += saved;

                    if (!ok)
                        return total;
                }

                var since = (state?.lastChange ?? state?.firstIntegrated ?? batchTime).AddHours(-1);

                for (var windowStart = since; windowStart < batchTime; windowStart = windowStart.AddDays(WindowDays))
                {
                    var windowEnd = windowStart.AddDays(WindowDays) < batchTime ? windowStart.AddDays(WindowDays) : batchTime;
                    var (saved, ok) = await ReadPriceTable(company, priceTableCode, batchTime, true, page => PriceTablesBody(priceTableCode, branchCodes, page, null, windowStart, windowEnd), $"janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}");

                    total += saved;

                    if (!ok)
                        break;
                }

                return total;

                // corpo do product/v2/price-tables/search de uma tabela: na carga inicial, a faixa de produto; na janela, o filter.change com
                // inPriceTableItem (que exige as lojas e a tabela)
                static string PriceTablesBody(int priceTableCode, List<int> branchCodes, int page, int? startProductCode, DateTime? windowStart, DateTime? windowEnd) =>
                    JsonConvert.SerializeObject(new
                    {
                        filter = new
                        {
                            startProductCode,
                            endProductCode = startProductCode is null ? (int?)null : LastProductCode,
                            change = windowStart is null ? null : new
                            {
                                startDate = $"{windowStart:yyyy-MM-ddTHH:mm:ss}",
                                endDate = $"{windowEnd:yyyy-MM-ddTHH:mm:ss}",
                                inPriceTableItem = true,
                                branchPriceTableCodeList = branchCodes,
                                priceTableCodeList = new[] { priceTableCode }
                            }
                        },
                        option = new
                        {
                            branchCodeList = branchCodes,
                            priceTableCode
                        },
                        order = "productCode",
                        page,
                        pageSize = PriceTablesPageSize
                    }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                // lê as páginas de uma tabela de preço e grava cada uma ao chegar; na janela, descarta o produto cujo maxChangeFilterDate não é
                // mais recente que o gravado (cache de estado por tabela). Devolve os produtos gravados e se a leitura chegou ao fim
                async Task<(int saved, bool ok)> ReadPriceTable(Domain.Totvs.Entities.Parameters.Company company, int priceTableCode, DateTime batchTime, bool useCache, Func<int, string> body, string context)
                {
                    var saved = 0;
                    var page = 1;
                    var totalPages = 1;

                    while (page <= totalPages)
                    {
                        var result = await GetValuePage<Domain.Totvs.Dtos.Reads.Products.PriceTables.PriceTableProduct>(company, "product/v2/price-tables/search", body(page), $"tabela {priceTableCode}, {context}, página {page}");

                        if (result is null)
                            return (saved, false);

                        var items = result.Value.items;
                        Dictionary<int, DateTime?>? itemsCache = null;

                        if (useCache && items.Count > 0)
                        {
                            // cache de estado: produto -> maxChangeFilterDate gravado, por empresa e tabela
                            itemsCache = await _cache.GetOrCreateAsync($"{PriceTableItemsKey}:{company.doc_company}:{priceTableCode}", async entry =>
                            {
                                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                                var list = await _totvsProductPricesRepository.GetPriceTableItemsChanges(
                                    company.doc_company!,
                                    priceTableCode,
                                    items.Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode)).Where(x => x != 0).Distinct().ToList()
                                );

                                return list
                                    .Where(x => x != null)
                                    .GroupBy(x => x!.productCode)
                                    .ToDictionary(g => g.Key, g => g.First()!.maxChangeFilterDate);
                            });

                            // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                            items.RemoveAll(x =>
                                itemsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode), out var y) &&
                                NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y
                            );
                        }

                        // valida, monta os itens com os preços por loja e escala (com o mesmo horário de lote), grava e atualiza o cache da tabela.
                        // Devolve a quantidade de produtos gravados.
                        var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.PriceTables.PriceTableItem record, string json)>();

                        // o mesmo produto pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                        foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode) != 0).GroupBy(x => x.dto.productCode).Select(g => g.Last()))
                        {
                            var validations = _priceTableProductValidator.Validate(item.dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"tabela de preço {priceTableCode}, produto {item.dto.productCode}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.PriceTables.PriceTableItem(company.doc_company!, batchTime, priceTableCode, item.dto), item.json.ToString(Formatting.None)));
                        }

                        if (_listSomenteNovos.Count > 0)
                        {
                            await _totvsProductPricesRepository.BulkInsertIntoTotvsPriceTablesItems(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

                            foreach (var (record, json) in _listSomenteNovos)
                            {
                                _logger.AddRecord($"{priceTableCode}|{record.productCode}", json);

                                if (itemsCache is not null)
                                    itemsCache[record.productCode] = record.maxChangeFilterDate;
                            }

                            saved += _listSomenteNovos.Count;
                        }

                        totalPages = result.Value.totalPages;
                        page++;
                    }

                    return (saved, true);
                }
            }
        }

        public async Task<bool?> GetPromotion()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetPromotion);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // BranchCode é obrigatório: uma leitura por loja (hoje só as matrizes 1 e 101 têm promoção, testado)
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    foreach (var branchCode in branchCodes)
                        totalInseridos += await SyncPromotions(company, branchCode);
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

            // itens de promoção de uma loja, lidos inteiros: o StartChangeDate/EndChangeDate da rota dá erro na Totvs (400 ORA-00920 em
            // qualquer formato de data, testado), e o maxChangeFilterDate é só a data de cadastro (a troca de valor ou de situação pode não
            // mudá-lo, PR15). O hash do JSON de cada item, comparado com o json_hash gravado (T9), diz o que mudou; cada página é gravada ao
            // chegar. Devolve a quantidade de itens gravados
            async Task<int> SyncPromotions(Domain.Totvs.Entities.Parameters.Company company, int branchCode)
            {
                var batchTime = _totvsService.GetBatchTime();
                Dictionary<(int, int), string>? stored = null;
                var total = 0;
                var page = 1;
                var totalPages = 1;

                while (page <= totalPages)
                {
                    var query = new Dictionary<string, string>
                    {
                        { "BranchCode", branchCode.ToString() },
                        { "Page", page.ToString() },
                        { "PageSize", PromotionsPageSize.ToString() }
                    };

                    // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                    ApiResponse? response = null;

                    for (var attempt = 1; attempt <= 2; attempt++)
                    {
                        var token = await _totvsService.GetToken(company);

                        if (token is null)
                            break;   // a mensagem do login já foi para o log

                        response = await _apiClient.SendAsync(
                            HttpMethod.Get,
                            "product/v2/promotion",
                            headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } },
                            queryParams: query
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
                            _logger.AddMessage($"Erro da API Totvs em product/v2/promotion (empresa {company.doc_company}, loja {branchCode}, página {page}): {response.StatusCode} {body}");
                        }

                        // as páginas já gravadas ficam; a próxima execução relê a loja inteira
                        return total;
                    }

                    Page<Domain.Totvs.Dtos.Reads.Products.Promotions.Promotion>? result = null;
                    JArray json = [];

                    try
                    {
                        result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.Products.Promotions.Promotion>>(response.Content ?? String.Empty);

                        // DateParseHandling.None: o JSON gravado e o hash ficam com as datas como a API mandou
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
                        _logger.AddMessage($"Resposta inválida da API Totvs em product/v2/promotion (empresa {company.doc_company}, loja {branchCode}, página {page}): {body}");

                        return total;
                    }

                    if (result.items.Count > 0)
                    {
                        // o que já está gravado da loja, com o hash de cada item (sem cache: a tabela guarda o hash, T9)
                        stored ??= (await _totvsProductPricesRepository.GetPromotionsHashes(company.doc_company!, branchCode))
                            .Where(x => x != null)
                            .GroupBy(x => (x!.promotionCode, x.productCode))
                            .ToDictionary(g => g.Key, g => g.First()!.json_hash);

                        // valida, monta as entidades (com o mesmo horário de lote e o hash do JSON) e grava só os itens novos ou com hash diferente do
                        // gravado. Devolve a quantidade de itens gravados.
                        List<(Domain.Totvs.Dtos.Reads.Products.Promotions.Promotion dto, string json)> items = result.items.Select((x, i) => (x, ((JObject)json[i]).ToString(Formatting.None))).ToList();

                        var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Promotions.Promotion record, string json)>();

                        foreach (var item in items
                            .Select(x => (x.dto, x.json, key: (ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.promotionCode), ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.productCode))))
                            .Where(x => x.key.Item1 != 0 && x.key.Item2 != 0)
                            .GroupBy(x => x.key)
                            .Select(g => g.Last()))
                        {
                            var hash = _logger.ComputeSha256Hash(item.json);

                            // sem data de alteração confiável: só regrava quando o JSON mudou
                            if (stored.TryGetValue(item.key, out var storedHash) && storedHash == hash)
                                continue;

                            var validations = _promotionValidator.Validate(item.dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"promoção {item.key.Item1} da loja {branchCode}, produto {item.key.Item2}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Promotions.Promotion(company.doc_company!, batchTime, hash, item.dto), item.json));
                        }

                        if (_listSomenteNovos.Count > 0)
                        {
                            await _totvsProductPricesRepository.BulkInsertIntoTotvsPromotions(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

                            foreach (var (record, recordJson) in _listSomenteNovos)
                            {
                                _logger.AddRecord($"{record.branchCode}|{record.promotionCode}|{record.productCode}", recordJson);
                                stored[(record.promotionCode, record.productCode)] = record.json_hash;
                            }

                            total += _listSomenteNovos.Count;
                        }
                    }

                    totalPages = result.totalPages;
                    page++;
                }

                return total;
            }
        }

        // grupo das escritas desta API na configuração (Totvs:Writes:Products:Enabled, T4) e as chaves da PR7 no mesmo grupo
        private const string WritesGroup = "Products";
        private const decimal DefaultMaxVariationPercent = 20m;

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostCreateValues(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Values.ProductValues body, bool confirmVariation) =>
            await WriteValues(user_name, doc_company, EnumJob.TotvsPostCreateValues, "product/v2/values/create", body, confirmVariation, update: false);

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostUpdateValues(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Values.ProductValues body, bool confirmVariation) =>
            await WriteValues(user_name, doc_company, EnumJob.TotvsPostUpdateValues, "product/v2/values/update", body, confirmVariation, update: true);

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostUpdatePromotionValues(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Values.PromotionItems body) =>
            await WritePromotion(user_name, doc_company, EnumJob.TotvsPostUpdatePromotionValues, "product/v2/promotion-values/update", body, update: true);

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostPromotionItems(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Products.Values.PromotionItems body) =>
            await WritePromotion(user_name, doc_company, EnumJob.TotvsPostPromotionItems, "product/v2/promotion-items", body, update: false);

        // ------------------------------------------------------------------------------------------------------

        /// <summary>
        /// um POST da leitura de valores, com uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s.
        /// Devolve null se não houve token (a mensagem do login já foi para o log)
        /// </summary>
        private async Task<ApiResponse?> SendValueRequest(Domain.Totvs.Entities.Parameters.Company company, string route, string requestBody)
        {
            ApiResponse? response = null;

            for (var attempt = 1; attempt <= 2; attempt++)
            {
                var token = await _totvsService.GetToken(company);

                if (token is null)
                    break;

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

            return response;
        }

        /// <summary>
        /// uma página da leitura de valores, com cada produto pareado com o seu JSON, ou null se a página falhou (a mensagem vai para o log)
        /// </summary>
        private async Task<(List<(T dto, JObject json)> items, int totalPages)?> GetValuePage<T>(Domain.Totvs.Entities.Parameters.Company company, string route, string requestBody, string context)
        {
            var response = await SendValueRequest(company, route, requestBody);

            if (response is null || !response.IsSuccess)
            {
                if (response is not null)
                {
                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                    _logger.AddMessage($"Erro da API Totvs em {route} (empresa {company.doc_company}, {context}): {response.StatusCode} {body}");
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
                _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {company.doc_company}, {context}): {body}");

                return null;
            }

            return (result.items.Select((x, i) => (x, (JObject)json[i])).ToList(), result.totalPages);
        }

        // preço e custo (§5.7, PR7): só os códigos (tipo + código, ex.: P1, C2) e as lojas permitidos em Totvs:Writes:Products:AllowedValueCodes e
        // AllowedValueBranches (vazio recusa tudo) e, sem ?confirmVariation=true, variação até MaxValueVariationPercent contra o valor gravado em
        // TotvsPrices/TotvsCosts; na alteração, sem valor gravado não há como conferir e a escrita é recusada. Valor absoluto: sem trava (T5)
        private async Task<Domain.Totvs.Dtos.Writes.Result> WriteValues(string user_name, string doc_company, EnumJob job, string route, Domain.Totvs.Dtos.Writes.Products.Values.ProductValues body, bool confirmVariation, bool update)
        {
            _logger
                .Clear()
                .AddLog(job);

            // códigos dos produtos separados por vírgula, cortados em 400 caracteres
            var key = String.Join(",", body.products!.Select(x => x.productCode!.Value));
            key = key.Length > 400 ? key[..400] : key;

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                var values = body.products!.SelectMany(p => p.values!.Select(v => (product: p.productCode!.Value, branch: v.branchCode!.Value, type: v.valueType!, code: v.valueCode!.Value, value: v.value!.Value))).ToList();
                var companyBranches = await _totvsService.GetBranchCodes(company);
                var outside = values.Select(x => x.branch).Distinct().Where(x => !companyBranches.Contains(x)).ToList();

                if (outside.Count > 0)
                    return _totvsService.BranchNotFound(doc_company, key, outside.Select(x => x.ToString()));

                var allowedCodes = SettingList(_totvsService.GetWriteSetting(WritesGroup, "AllowedValueCodes"));
                var allowedBranches = SettingList(_totvsService.GetWriteSetting(WritesGroup, "AllowedValueBranches"));
                string? refused = null;

                if (allowedCodes.Count == 0 || allowedBranches.Count == 0)
                    refused = $"Escrita de preço e custo sem códigos ou lojas permitidos (Totvs:Writes:{WritesGroup}:AllowedValueCodes e AllowedValueBranches, PR7).";
                else if (values.Where(x => !allowedCodes.Contains($"{x.type}{x.code}") || !allowedBranches.Contains(x.branch.ToString())).Select(x => $"{x.product}/{x.branch}/{x.type}{x.code}").ToList() is { Count: > 0 } denied)
                    refused = $"Código de valor ou loja fora dos permitidos (PR7): {String.Join(", ", denied.Take(20))}.";
                else if (!confirmVariation)
                {
                    var max = Decimal.TryParse(_totvsService.GetWriteSetting(WritesGroup, "MaxValueVariationPercent"), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var configured)
                        ? configured : DefaultMaxVariationPercent;
                    var current = (await _totvsProductPricesRepository.GetCurrentValues(doc_company, values.Select(x => x.product).ToList()))
                        .Where(x => x is not null)
                        .ToDictionary(x => (x!.productCode, x.branchCode, x.valueType, x.valueCode), x => x!.value);
                    var problems = new List<string>();

                    foreach (var x in values)
                    {
                        var stored = current.GetValueOrDefault((x.product, x.branch, x.type, x.code));

                        if (stored is null or 0)
                        {
                            if (update)
                                problems.Add($"{x.product}/{x.branch}/{x.type}{x.code} sem valor gravado");
                        }
                        else if (Math.Abs(x.value - stored.Value) / stored.Value * 100 > max)
                            problems.Add($"{x.product}/{x.branch}/{x.type}{x.code} de {stored:0.00} para {x.value:0.00}");
                    }

                    if (problems.Count > 0)
                        refused = $"Variação acima de {max}% ou sem valor gravado para conferir (PR7; para seguir assim, ?confirmVariation=true): {String.Join(", ", problems.Take(20))}.";
                }

                if (refused is not null)
                {
                    _logger.AddMessage(refused);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = refused };
                }

                return await _totvsService.Write(company, job, user_name, key, HttpMethod.Post, route, body, "Confira os preços e custos dos produtos antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }

            // lista da configuração separada por vírgula, sem espaços nem diferença de maiúsculas
            static HashSet<string> SettingList(string? setting) =>
                (setting ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x => x.ToUpperInvariant()).ToHashSet();
        }

        // valores e itens de promoção (§5.7): a loja precisa ser da empresa; com a promoção em TotvsPromotions, ela precisa estar aberta (A) e, na
        // alteração, o valor de promoção abaixo do anterior. Incluir itens não é idempotente: trava (T5)
        private async Task<Domain.Totvs.Dtos.Writes.Result> WritePromotion(string user_name, string doc_company, EnumJob job, string route, Domain.Totvs.Dtos.Writes.Products.Values.PromotionItems body, bool update)
        {
            _logger
                .Clear()
                .AddLog(job);

            var key = $"{body.branchCode}|{body.promotionCode}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (!(await _totvsService.GetBranchCodes(company)).Contains(body.branchCode!.Value))
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCode.Value.ToString()]);

                var stored = (await _totvsProductPricesRepository.GetPromotionItems(doc_company, body.branchCode.Value, body.promotionCode!.Value)).Where(x => x is not null).ToList();
                string? refused = null;

                if (stored.Count > 0 && stored.Any(x => x!.status != "A"))
                    refused = $"A promoção {key} não está aberta em totvs.TotvsPromotions (situação {stored.First(x => x!.status != "A")!.status}).";
                else if (update)
                {
                    var notBelow = body.items!
                        .Select(i => (item: i, previous: stored.FirstOrDefault(s => s!.productCode == i.productCode)?.previousValue))
                        .Where(x => x.previous is decimal previous && x.item.promotionValue >= previous)
                        .Select(x => $"{x.item.productCode} ({x.item.promotionValue:0.00} >= {x.previous:0.00})")
                        .ToList();

                    if (notBelow.Count > 0)
                        refused = $"Valor de promoção não abaixo do valor anterior: {String.Join(", ", notBelow.Take(20))}.";
                }

                if (refused is not null)
                {
                    _logger.AddMessage(refused);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = refused };
                }

                return await _totvsService.Write(company, job, user_name, key, HttpMethod.Post, route, body, "Confira os itens da promoção na Totvs antes de repetir.", repeatLock: !update);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }
    }
}

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
    /// catálogos da API Product (fase PR-1, docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.1, PR12): tipos e classificações de produto,
    /// categorias, grades, unidades de medida, cores, tipos de campo adicional, agrupadores, itens de instrução, composições e
    /// cabeçalhos e escalas de tabela de preço. Cada método faz as próprias chamadas à API; o comum vem do ITotvsService. Com
    /// maxChangeFilterDate na resposta a janela sai do banco, como nas filiais; as cores, sem ele, são lidas inteiras com hash (T9)
    /// </summary>
    public class TotvsProductCatalogsService : ITotvsProductCatalogsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsProductCatalogsRepository _totvsProductCatalogsRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.ClassificationType> _classificationTypeValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.ProductClassification> _productClassificationValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.Category> _categoryValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.Grid> _gridValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.MeasurementUnit> _measurementUnitValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.Color> _colorValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.AdditionalFieldType> _additionalFieldTypeValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.ProductGrouperConfiguration> _productGrouperConfigurationValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.InstructionItem> _instructionItemValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.Composition> _compositionValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.PriceTable> _priceTableValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.PriceTableScale> _priceTableScaleValidator;

        // máximos das rotas (swagger da API Product) e chaves do cache de estado
        private const int ClassificationTypePageSize = 100;
        private const string ClassificationTypeKey = "Totvs:ClassificationTypes";
        private const int ProductClassificationPageSize = 1000;
        private const string ProductClassificationKey = "Totvs:ProductClassifications";
        private const int CategoryPageSize = 1000;
        private const string CategoryKey = "Totvs:Categories";
        private const int GridPageSize = 1000;
        private const string GridKey = "Totvs:Grids";
        private const int MeasurementUnitPageSize = 1000;
        private const string MeasurementUnitKey = "Totvs:MeasurementUnits";
        private const int ColorPageSize = 500;
        private const int AdditionalFieldTypePageSize = 100;
        private const string AdditionalFieldTypeKey = "Totvs:AdditionalFieldsTypes";
        private const int ProductGrouperConfigurationPageSize = 100;
        private const string ProductGrouperConfigurationKey = "Totvs:ProductGrouperConfigurations";
        private const int InstructionItemPageSize = 100;
        private const string InstructionItemKey = "Totvs:InstructionItems";
        private const int CompositionPageSize = 100;
        private const string CompositionKey = "Totvs:Compositions";
        private const int PriceTablePageSize = 1000;
        private const string PriceTableKey = "Totvs:PriceTables";
        private const int PriceTableScalePageSize = 1000;
        private const string PriceTableScaleKey = "Totvs:PriceTableScales";

        public TotvsProductCatalogsService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsProductCatalogsRepository totvsProductCatalogsRepository,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.ClassificationType> classificationTypeValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.ProductClassification> productClassificationValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.Category> categoryValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.Grid> gridValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.MeasurementUnit> measurementUnitValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.Color> colorValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.AdditionalFieldType> additionalFieldTypeValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.ProductGrouperConfiguration> productGrouperConfigurationValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.InstructionItem> instructionItemValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.Composition> compositionValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.PriceTable> priceTableValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Catalogs.PriceTableScale> priceTableScaleValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsProductCatalogsRepository, _classificationTypeValidator, _productClassificationValidator, _categoryValidator, _gridValidator, _measurementUnitValidator, _colorValidator, _additionalFieldTypeValidator, _productGrouperConfigurationValidator, _instructionItemValidator, _compositionValidator, _priceTableValidator, _priceTableScaleValidator) =
            (apiClient, logger, cache, totvsService, totvsProductCatalogsRepository, classificationTypeValidator, productClassificationValidator, categoryValidator, gridValidator, measurementUnitValidator, colorValidator, additionalFieldTypeValidator, productGrouperConfigurationValidator, instructionItemValidator, compositionValidator, priceTableValidator, priceTableScaleValidator);

        public async Task<bool?> GetClassificationType()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetClassificationType);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetClassificationTypesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.ClassificationType>(
                        company,
                        HttpMethod.Get,
                        "product/v2/classificationType",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", ClassificationTypePageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{ClassificationTypeKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetClassificationTypes(company.doc_company!);

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

                    totalInseridos += await SaveClassificationTypes(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> GetClassifications()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsProductGetClassifications);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetProductClassificationsLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.ProductClassification>(
                        company,
                        HttpMethod.Get,
                        "product/v2/classifications",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Page", page.ToString() },
                            { "PageSize", ProductClassificationPageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{ProductClassificationKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetProductClassifications(company.doc_company!);

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

                    totalInseridos += await SaveProductClassifications(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> GetCategory()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetCategory);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetCategoriesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.Category>(
                        company,
                        HttpMethod.Get,
                        "product/v2/category",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", CategoryPageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{CategoryKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetCategories(company.doc_company!);

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

                    totalInseridos += await SaveCategories(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> GetGrid()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetGrid);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetGridsLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.Grid>(
                        company,
                        HttpMethod.Get,
                        "product/v2/grid",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", GridPageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{GridKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetGrids(company.doc_company!);

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

                    totalInseridos += await SaveGrids(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> GetMeasurementUnit()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetMeasurementUnit);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetMeasurementUnitsLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.MeasurementUnit>(
                        company,
                        HttpMethod.Get,
                        "product/v2/measurement-unit",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Expand", "additionalVariations" },
                            { "Page", page.ToString() },
                            { "PageSize", MeasurementUnitPageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{MeasurementUnitKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetMeasurementUnits(company.doc_company!);

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

                    totalInseridos += await SaveMeasurementUnits(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> PostSearchColors()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchColors);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.Color>(
                        company,
                        HttpMethod.Post,
                        "product/v2/colors/search",
                        null,
                        // filter é obrigatório (vazio: todas as cores; testado). Com filter.change a resposta continua sem data de alteração
                        page => JsonConvert.SerializeObject(new
                        {
                            filter = new { },
                            expand = "additionalColorInformation",
                            page,
                            pageSize = ColorPageSize
                        })
                    );

                    // leitura incompleta não é gravada: só a leitura inteira diz quais cores mudaram
                    if (items is null || items.Count == 0)
                        continue;

                    // o que já está gravado, com o hash do JSON de cada cor (sem cache: a tabela guarda o hash, T9)
                    var stored = (await _totvsProductCatalogsRepository.GetColorsHashes(company.doc_company!))
                        .Where(x => x != null)
                        .GroupBy(x => x!.colorCode.ToUpperInvariant())
                        .ToDictionary(g => g.Key, g => g.First()!.json_hash);

                    totalInseridos += await SaveColors(company.doc_company!, batchTime, items, stored);
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

        public async Task<bool?> GetAdditionalFieldsTypes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetAdditionalFieldsTypes);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetAdditionalFieldsTypesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.AdditionalFieldType>(
                        company,
                        HttpMethod.Get,
                        "product/v2/additional-fields-types",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", AdditionalFieldTypePageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{AdditionalFieldTypeKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetAdditionalFieldsTypes(company.doc_company!);

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

                    totalInseridos += await SaveAdditionalFieldsTypes(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> GetProductGrouperConfiguration()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetProductGrouperConfiguration);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetProductGrouperConfigurationsLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.ProductGrouperConfiguration>(
                        company,
                        HttpMethod.Get,
                        "product/v2/product-grouper-configuration",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "grouperCode" },
                            { "Page", page.ToString() },
                            { "PageSize", ProductGrouperConfigurationPageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{ProductGrouperConfigurationKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetProductGrouperConfigurations(company.doc_company!);

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.grouperCode)
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    items.RemoveAll(x =>
                        itemsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.grouperCode), out var y) &&
                        NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    totalInseridos += await SaveProductGrouperConfigurations(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> GetInstructionItems()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetInstructionItems);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetInstructionItemsLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.InstructionItem>(
                        company,
                        HttpMethod.Get,
                        "product/v2/instruction-items",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", InstructionItemPageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{InstructionItemKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetInstructionItems(company.doc_company!);

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

                    totalInseridos += await SaveInstructionItems(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> PostCompositions()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostCompositions);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetCompositionsLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.Composition>(
                        company,
                        HttpMethod.Post,
                        "product/v2/compositions",
                        null,
                        page => JsonConvert.SerializeObject(new
                        {
                            startChangeDate = $"{since:yyyy-MM-ddTHH:mm:ss}",
                            endChangeDate = $"{batchTime:yyyy-MM-ddTHH:mm:ss}",
                            expand = "fibers",
                            order = "code",
                            page,
                            pageSize = CompositionPageSize
                        })
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{CompositionKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetCompositions(company.doc_company!);

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

                    totalInseridos += await SaveCompositions(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> GetPriceTablesHeaders()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetPriceTablesHeaders);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetPriceTablesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.PriceTable>(
                        company,
                        HttpMethod.Get,
                        "product/v2/price-tables-headers",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", PriceTablePageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{PriceTableKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetPriceTables(company.doc_company!);

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

                    totalInseridos += await SavePriceTables(company.doc_company!, batchTime, items, itemsCache);
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

        public async Task<bool?> GetPriceTableScales()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetPriceTableScales);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsProductCatalogsRepository.GetPriceTableScalesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = await GetCatalogPages<Domain.Totvs.Dtos.Reads.Products.Catalogs.PriceTableScale>(
                        company,
                        HttpMethod.Get,
                        "product/v2/price-table-scales",
                        page => new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Page", page.ToString() },
                            { "PageSize", PriceTableScalePageSize.ToString() }
                        },
                        null
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (items is null || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{PriceTableScaleKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsProductCatalogsRepository.GetPriceTableScales(company.doc_company!);

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.scaleCode)
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    items.RemoveAll(x =>
                        itemsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.scaleCode), out var y) &&
                        NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    totalInseridos += await SavePriceTableScales(company.doc_company!, batchTime, items, itemsCache);
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
        /// lê todas as páginas de uma rota de catálogo (query no GET, corpo no POST). Devolve cada item com o seu JSON, ou null se
        /// alguma página falhou (a mensagem vai para o log). A chamada é a do método: este helper só pagina e repete a tentativa
        /// </summary>
        private async Task<List<(T dto, JObject json)>?> GetCatalogPages<T>(
            Domain.Totvs.Entities.Parameters.Company company,
            HttpMethod method,
            string route,
            Func<int, Dictionary<string, string>>? query,
            Func<int, string>? body)
        {
            var items = new List<(T dto, JObject json)>();
            var page = 1;
            var totalPages = 1;

            while (page <= totalPages)
            {
                var requestBody = body?.Invoke(page);

                // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                ApiResponse? response = null;

                for (var attempt = 1; attempt <= 2; attempt++)
                {
                    var token = await _totvsService.GetToken(company);

                    if (token is null)
                        break;   // a mensagem do login já foi para o log

                    using var content = requestBody is null ? null : new StringContent(requestBody, Encoding.UTF8, "application/json");

                    response = await _apiClient.SendAsync(
                        method,
                        route,
                        content,
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } },
                        queryParams: query?.Invoke(page)
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
                        var body800 = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                        _logger.AddMessage($"Erro da API Totvs em {route} (empresa {company.doc_company}, página {page}): {response.StatusCode} {body800}");
                    }

                    return null;
                }

                Page<T>? result = null;
                JArray json = [];

                try
                {
                    result = JsonConvert.DeserializeObject<Page<T>>(response.Content ?? String.Empty);

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
                    var body800 = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                    _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {company.doc_company}, página {page}): {body800}");

                    return null;
                }

                // cada item pareado com o seu JSON pela posição
                for (var i = 0; i < json.Count; i++)
                    items.Add((result.items[i], (JObject)json[i]));

                totalPages = result.totalPages;
                page++;
            }

            return items;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SaveClassificationTypes(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.ClassificationType dto, JObject json)> items,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Catalogs.ClassificationType>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.ClassificationType record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code)).Select(g => g.Last()))
            {
                var validations = _classificationTypeValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"tipo de classificação {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code)}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.ClassificationType(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsClassificationTypes(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.code;

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SaveProductClassifications(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.ProductClassification dto, JObject json)> items,
            Dictionary<string, Domain.Totvs.Entities.Reads.Products.Catalogs.ProductClassification>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.ProductClassification record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.typeCode) != 0 && !String.IsNullOrWhiteSpace(x.dto.code)).GroupBy(x => $"{ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.typeCode)}|{(x.dto.code ?? String.Empty).ToUpperInvariant()}").Select(g => g.Last()))
            {
                var validations = _productClassificationValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"classificação {$"{ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.typeCode)}|{(item.dto.code ?? String.Empty).ToUpperInvariant()}"}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.ProductClassification(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsProductClassifications(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = $"{record.typeCode}|{record.code.ToUpperInvariant()}";

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SaveCategories(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.Category dto, JObject json)> items,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Catalogs.Category>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.Category record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code)).Select(g => g.Last()))
            {
                var validations = _categoryValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"categoria {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code)}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.Category(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsCategories(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.code;

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SaveGrids(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.Grid dto, JObject json)> items,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Catalogs.Grid>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.Grid record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code)).Select(g => g.Last()))
            {
                var validations = _gridValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"grade {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code)}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.Grid(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsGrids(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.code;

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SaveMeasurementUnits(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.MeasurementUnit dto, JObject json)> items,
            Dictionary<string, Domain.Totvs.Entities.Reads.Products.Catalogs.MeasurementUnit>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.MeasurementUnit record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => !String.IsNullOrWhiteSpace(x.dto.code)).GroupBy(x => (x.dto.code ?? String.Empty).ToUpperInvariant()).Select(g => g.Last()))
            {
                var validations = _measurementUnitValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"unidade de medida {(item.dto.code ?? String.Empty).ToUpperInvariant()}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.MeasurementUnit(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsMeasurementUnits(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.code.ToUpperInvariant();

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote e o hash do JSON) e grava só as cores novas ou com hash
        /// diferente do gravado. Devolve a quantidade de cores gravadas.
        /// </summary>
        private async Task<int> SaveColors(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.Color dto, JObject json)> items,
            IReadOnlyDictionary<string, string> stored)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.Color record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => !String.IsNullOrWhiteSpace(x.dto.colorCode)).GroupBy(x => (x.dto.colorCode ?? String.Empty).ToUpperInvariant()).Select(g => g.Last()))
            {
                var json = item.json.ToString(Formatting.None);
                var hash = _logger.ComputeSha256Hash(json);

                // sem data de alteração: só regrava quando o JSON mudou
                if (stored.TryGetValue((item.dto.colorCode ?? String.Empty).ToUpperInvariant(), out var storedHash) && storedHash == hash)
                    continue;

                var validations = _colorValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"cor {(item.dto.colorCode ?? String.Empty).ToUpperInvariant()}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.Color(doc_company, batchTime, hash, item.dto), json));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsColors(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
                _logger.AddRecord(record.colorCode.ToUpperInvariant().ToString(), json);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SaveAdditionalFieldsTypes(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.AdditionalFieldType dto, JObject json)> items,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Catalogs.AdditionalFieldType>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.AdditionalFieldType record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code)).Select(g => g.Last()))
            {
                var validations = _additionalFieldTypeValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"tipo de campo adicional {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code)}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.AdditionalFieldType(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsAdditionalFieldsTypes(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.code;

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SaveProductGrouperConfigurations(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.ProductGrouperConfiguration dto, JObject json)> items,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Catalogs.ProductGrouperConfiguration>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.ProductGrouperConfiguration record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.grouperCode) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.grouperCode)).Select(g => g.Last()))
            {
                var validations = _productGrouperConfigurationValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"agrupador {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.grouperCode)}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.ProductGrouperConfiguration(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsProductGrouperConfigurations(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.grouperCode;

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SaveInstructionItems(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.InstructionItem dto, JObject json)> items,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Catalogs.InstructionItem>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.InstructionItem record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code)).Select(g => g.Last()))
            {
                var validations = _instructionItemValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"item de instrução {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code)}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.InstructionItem(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsInstructionItems(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.code;

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SaveCompositions(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.Composition dto, JObject json)> items,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Catalogs.Composition>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.Composition record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code)).Select(g => g.Last()))
            {
                var validations = _compositionValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"composição {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code)}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.Composition(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsCompositions(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.code;

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SavePriceTables(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.PriceTable dto, JObject json)> items,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Catalogs.PriceTable>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.PriceTable record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code)).Select(g => g.Last()))
            {
                var validations = _priceTableValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"tabela de preço {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code)}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.PriceTable(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsPriceTables(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.code;

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de registros gravados.
        /// </summary>
        private async Task<int> SavePriceTableScales(
            string doc_company,
            DateTime batchTime,
            List<(Domain.Totvs.Dtos.Reads.Products.Catalogs.PriceTableScale dto, JObject json)> items,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Catalogs.PriceTableScale>? itemsCache)
        {
            var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Products.Catalogs.PriceTableScale record, string json)>();

            // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.scaleCode) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.scaleCode)).Select(g => g.Last()))
            {
                var validations = _priceTableScaleValidator.Validate(item.dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"escala {ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.scaleCode)}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Products.Catalogs.PriceTableScale(doc_company, batchTime, item.dto), item.json.ToString(Formatting.None)));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsProductCatalogsRepository.BulkInsertIntoTotvsPriceTableScales(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

            foreach (var (record, json) in _listSomenteNovos)
            {
                var key = record.scaleCode;

                _logger.AddRecord(key.ToString(), json);

                if (itemsCache is not null)
                    itemsCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }
    }
}

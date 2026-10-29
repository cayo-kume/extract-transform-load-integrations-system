using Application.LinxCommerce.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Enums;
using Domain.LinxCommerce.Extensions;
using Domain.Shared.Extensions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace Application.LinxCommerce.Services
{
    /// <summary>
    /// produtos e SKUs do Linx Commerce (API Catalog). Cada método faz as próprias chamadas; o comum vem do ILinxCommerceService
    /// </summary>
    public class LinxCommerceCatalogService : ILinxCommerceCatalogService
    {
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ILinxCommerceService _linxCommerceService;
        private readonly ILinxCommerceCatalogRepository _linxCommerceCatalogRepository;
        private readonly IValidator<Domain.LinxCommerce.Dtos.Reads.Catalog.Product> _productValidator;
        private readonly IValidator<Domain.LinxCommerce.Dtos.Reads.Catalog.Sku> _skuValidator;

        private const string ProductsKey = "LinxCommerce:Products";
        private const string SkusKey = "LinxCommerce:Skus";

        // carga inicial (banco vazio): tudo (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §10.2, pendência 3)
        private static readonly DateTime CatalogInitialLoad = new(2000, 1, 1);

        public LinxCommerceCatalogService(
            ILoggerService logger,
            IMemoryCache cache,
            ILinxCommerceService linxCommerceService,
            ILinxCommerceCatalogRepository linxCommerceCatalogRepository,
            IValidator<Domain.LinxCommerce.Dtos.Reads.Catalog.Product> productValidator,
            IValidator<Domain.LinxCommerce.Dtos.Reads.Catalog.Sku> skuValidator
        ) =>
            (_logger, _cache, _linxCommerceService, _linxCommerceCatalogRepository, _productValidator, _skuValidator) =
            (logger, cache, linxCommerceService, linxCommerceCatalogRepository, productValidator, skuValidator);

        public async Task<bool?> SearchProduct()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceSearchProduct);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxCommerceService.GetCompanys())
                {
                    var batchTime = _linxCommerceService.GetBatchTime();

                    // o código antigo filtrava por CreatedDate dos últimos 7 dias e nunca lia alterações (4.2, item 3)
                    var lastModified = await _linxCommerceCatalogRepository.GetProductsLastModifiedDate(company.doc_company!);
                    var since = lastModified.HasValue ? LinxCommerceConverters.ToBrasilia(lastModified.Value).AddHours(-1) : CatalogInitialLoad;

                    var found = await _linxCommerceService.Search<Domain.LinxCommerce.Dtos.Reads.Catalog.SearchProductResult>(
                        company,
                        "Catalog/API.svc/web/SearchProduct",
                        _linxCommerceService.ModifiedDateWindow(since, batchTime),
                        "ProductID"
                    );

                    if (found is null)
                        continue;

                    var changed = found
                        .Select(x => (id: ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.ProductID), modified: LinxCommerceConverters.ToNullableDateTimeOffset(x.dto.ModifiedDate)))
                        .Where(x => x.id != 0)
                        .GroupBy(x => x.id)
                        .Select(g => g.Last())
                        .ToList();

                    if (changed.Count == 0)
                        continue;

                    // cache de estado: ProductID -> ModifiedDate gravada, por empresa
                    var productsCache = _cache.GetOrCreate($"{ProductsKey}:{company.doc_company}", entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        return new Dictionary<int, DateTimeOffset?>();
                    })!;

                    var missing = changed.Select(x => x.id).Where(x => !productsCache.ContainsKey(x)).ToList();

                    if (missing.Count > 0)
                        foreach (var stored in (await _linxCommerceCatalogRepository.GetProductsByIds(company.doc_company!, missing)).Where(x => x is not null))
                            productsCache[stored!.ProductID] = stored.ModifiedDate;

                    changed.RemoveAll(x => x.modified.HasValue && productsCache.TryGetValue(x.id, out var stored) && stored.HasValue && x.modified <= stored);

                    var products = new List<Domain.LinxCommerce.Dtos.Reads.Catalog.Product>();
                    var productsJson = new Dictionary<int, string>();
                    var failed = false;

                    foreach (var (id, _) in changed)
                    {
                        var product = await _linxCommerceService.Get<Domain.LinxCommerce.Dtos.Reads.Catalog.Product>(company, "Catalog/API.svc/web/GetProduct", id, $"produto {id}");

                        if (product is null || ConverterValidatorsExtensions.ConvertToInt32Validation(product.Value.dto.ProductID) != id)
                        {
                            if (product is not null)
                                _logger.AddMessage($"GetProduct do produto {id} (empresa {company.doc_company}) devolveu outro ProductID: {product.Value.dto.ProductID}.");

                            failed = true;
                            break;
                        }

                        products.Add(product.Value.dto);
                        productsJson[id] = product.Value.json;
                    }

                    // um produto que falhou ficaria para trás da próxima janela: nada da empresa é gravado
                    if (failed)
                        continue;

                    var _listSomenteNovos = new List<Domain.LinxCommerce.Entities.Reads.Catalog.Product>();

                    foreach (var product in products)
                    {
                        var validations = _productValidator.Validate(product);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"produto {product.ProductID}, {s.PropertyName}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.LinxCommerce.Entities.Reads.Catalog.Product(company.doc_company!, batchTime, product));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _linxCommerceCatalogRepository.BulkInsertIntoLinxCommerceProducts(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        if (productsJson.TryGetValue(record.ProductID, out var json))
                            _logger.AddRecord(record.ProductID.ToString(), json);

                        productsCache[record.ProductID] = record.ModifiedDate;
                    }

                    totalInseridos += _listSomenteNovos.Count;
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

        public async Task<bool?> SearchSKU()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceSearchSKU);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxCommerceService.GetCompanys())
                {
                    var batchTime = _linxCommerceService.GetBatchTime();

                    // o código antigo filtrava por CreatedDate do último dia e nunca lia alterações (4.2, item 3)
                    var lastModified = await _linxCommerceCatalogRepository.GetSkusLastModifiedDate(company.doc_company!);
                    var since = lastModified.HasValue ? LinxCommerceConverters.ToBrasilia(lastModified.Value).AddHours(-1) : CatalogInitialLoad;

                    var found = await _linxCommerceService.Search<Domain.LinxCommerce.Dtos.Reads.Catalog.SearchSkuResult>(
                        company,
                        "Catalog/API.svc/web/SearchSKU",
                        _linxCommerceService.ModifiedDateWindow(since, batchTime),
                        "ProductID"
                    );

                    if (found is null)
                        continue;

                    // o id do SKU é o ProductID (o GetSKU recebe o ProductID, como no código antigo)
                    var changed = found
                        .Select(x => (id: ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.ProductID), modified: LinxCommerceConverters.ToNullableDateTimeOffset(x.dto.ModifiedDate)))
                        .Where(x => x.id != 0)
                        .GroupBy(x => x.id)
                        .Select(g => g.Last())
                        .ToList();

                    if (changed.Count == 0)
                        continue;

                    // cache de estado: ProductID do SKU -> ModifiedDate gravada, por empresa
                    var skusCache = _cache.GetOrCreate($"{SkusKey}:{company.doc_company}", entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        return new Dictionary<int, DateTimeOffset?>();
                    })!;

                    var missing = changed.Select(x => x.id).Where(x => !skusCache.ContainsKey(x)).ToList();

                    if (missing.Count > 0)
                        foreach (var stored in (await _linxCommerceCatalogRepository.GetSkusByIds(company.doc_company!, missing)).Where(x => x is not null))
                            skusCache[stored!.ProductID] = stored.ModifiedDate;

                    changed.RemoveAll(x => x.modified.HasValue && skusCache.TryGetValue(x.id, out var stored) && stored.HasValue && x.modified <= stored);

                    var skus = new List<Domain.LinxCommerce.Dtos.Reads.Catalog.Sku>();
                    var skusJson = new Dictionary<int, string>();
                    var failed = false;

                    foreach (var (id, _) in changed)
                    {
                        var sku = await _linxCommerceService.Get<Domain.LinxCommerce.Dtos.Reads.Catalog.Sku>(company, "Catalog/API.svc/web/GetSKU", id, $"SKU {id}");

                        if (sku is null || ConverterValidatorsExtensions.ConvertToInt32Validation(sku.Value.dto.ProductID) != id)
                        {
                            if (sku is not null)
                                _logger.AddMessage($"GetSKU do SKU {id} (empresa {company.doc_company}) devolveu outro ProductID: {sku.Value.dto.ProductID}.");

                            failed = true;
                            break;
                        }

                        skus.Add(sku.Value.dto);
                        skusJson[id] = sku.Value.json;
                    }

                    // um SKU que falhou ficaria para trás da próxima janela: nada da empresa é gravado
                    if (failed)
                        continue;

                    var _listSomenteNovos = new List<Domain.LinxCommerce.Entities.Reads.Catalog.Sku>();

                    foreach (var sku in skus)
                    {
                        var validations = _skuValidator.Validate(sku);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"SKU {sku.ProductID} ({sku.SKU}), {s.PropertyName}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.LinxCommerce.Entities.Reads.Catalog.Sku(company.doc_company!, batchTime, sku));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _linxCommerceCatalogRepository.BulkInsertIntoLinxCommerceSkus(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        if (skusJson.TryGetValue(record.ProductID, out var json))
                            _logger.AddRecord(record.ProductID.ToString(), json);

                        skusCache[record.ProductID] = record.ModifiedDate;
                    }

                    totalInseridos += _listSomenteNovos.Count;
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
    }
}

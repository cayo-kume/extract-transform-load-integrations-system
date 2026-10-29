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

                    totalInseridos += await SaveProducts(company.doc_company!, batchTime, products, productsJson, productsCache);
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

                    totalInseridos += await SaveSkus(company.doc_company!, batchTime, skus, skusJson, skusCache);
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
        /// busca um produto pelo ProductID, grava (sempre, sem consultar o cache) e registra a chamada em linxcommerce.LinxCommerceRequests
        /// </summary>
        public async Task<bool?> GetProduct(string user_name, string doc_company, int productId)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceGetProduct);

            try
            {
                var company = await _linxCommerceService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em LinxCommerceRequests (igual ao Totvs)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em linxcommerce.LinxCommerceParameters.");
                    return null;
                }

                var response = await _linxCommerceService.ReadOnDemand<Domain.LinxCommerce.Dtos.Reads.Catalog.Product>(company, EnumJob.LinxCommerceGetProduct, user_name, "Catalog/API.svc/web/GetProduct", productId, productId.ToString());

                // "não existe" é o 404 ou a resposta 2xx sem o produto pedido (a conferir com a API real)
                if (response.StatusCode == 404 || (response.IsValid && ConverterValidatorsExtensions.ConvertToInt32Validation(response.Dto!.ProductID) != productId))
                {
                    _logger.AddMessage($"Produto {productId} não encontrado no Linx Commerce (empresa {doc_company}).");
                    return false;
                }

                if (!response.IsValid)
                    return null;   // a mensagem já foi para o log

                // atualiza o cache do SearchProduct, se ele estiver carregado
                _cache.TryGetValue($"{ProductsKey}:{doc_company}", out Dictionary<int, DateTimeOffset?>? productsCache);

                await SaveProducts(doc_company, _linxCommerceService.GetBatchTime(), [response.Dto!], new Dictionary<int, string> { { productId, response.Json! } }, productsCache ?? []);

                _logger.AddMessage($"Concluída com sucesso: produto {productId} inserido ou atualizado!");

                return true;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// busca um SKU pelo ProductID, grava (sempre, sem consultar o cache) e registra a chamada em linxcommerce.LinxCommerceRequests
        /// </summary>
        public async Task<bool?> GetSKU(string user_name, string doc_company, int productId)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceGetSKU);

            try
            {
                var company = await _linxCommerceService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em LinxCommerceRequests (igual ao Totvs)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em linxcommerce.LinxCommerceParameters.");
                    return null;
                }

                var response = await _linxCommerceService.ReadOnDemand<Domain.LinxCommerce.Dtos.Reads.Catalog.Sku>(company, EnumJob.LinxCommerceGetSKU, user_name, "Catalog/API.svc/web/GetSKU", productId, productId.ToString());

                // "não existe" é o 404 ou a resposta 2xx sem o SKU pedido (a conferir com a API real)
                if (response.StatusCode == 404 || (response.IsValid && ConverterValidatorsExtensions.ConvertToInt32Validation(response.Dto!.ProductID) != productId))
                {
                    _logger.AddMessage($"SKU {productId} não encontrado no Linx Commerce (empresa {doc_company}).");
                    return false;
                }

                if (!response.IsValid)
                    return null;   // a mensagem já foi para o log

                // atualiza o cache do SearchSKU, se ele estiver carregado
                _cache.TryGetValue($"{SkusKey}:{doc_company}", out Dictionary<int, DateTimeOffset?>? skusCache);

                await SaveSkus(doc_company, _linxCommerceService.GetBatchTime(), [response.Dto!], new Dictionary<int, string> { { productId, response.Json! } }, skusCache ?? []);

                _logger.AddMessage($"Concluída com sucesso: SKU {productId} inserido ou atualizado!");

                return true;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de produtos gravados.
        /// </summary>
        private async Task<int> SaveProducts(
            string doc_company,
            DateTime batchTime,
            List<Domain.LinxCommerce.Dtos.Reads.Catalog.Product> products,
            IReadOnlyDictionary<int, string> productsJson,
            Dictionary<int, DateTimeOffset?> productsCache)
        {
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

                _listSomenteNovos.Add(new Domain.LinxCommerce.Entities.Reads.Catalog.Product(doc_company, batchTime, product));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxCommerceCatalogRepository.BulkInsertIntoLinxCommerceProducts(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (productsJson.TryGetValue(record.ProductID, out var json))
                    _logger.AddRecord(record.ProductID.ToString(), json);

                productsCache[record.ProductID] = record.ModifiedDate;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de SKUs gravados.
        /// </summary>
        private async Task<int> SaveSkus(
            string doc_company,
            DateTime batchTime,
            List<Domain.LinxCommerce.Dtos.Reads.Catalog.Sku> skus,
            IReadOnlyDictionary<int, string> skusJson,
            Dictionary<int, DateTimeOffset?> skusCache)
        {
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

                _listSomenteNovos.Add(new Domain.LinxCommerce.Entities.Reads.Catalog.Sku(doc_company, batchTime, sku));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxCommerceCatalogRepository.BulkInsertIntoLinxCommerceSkus(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (skusJson.TryGetValue(record.ProductID, out var json))
                    _logger.AddRecord(record.ProductID.ToString(), json);

                skusCache[record.ProductID] = record.ModifiedDate;
            }

            return _listSomenteNovos.Count;
        }
    }
}

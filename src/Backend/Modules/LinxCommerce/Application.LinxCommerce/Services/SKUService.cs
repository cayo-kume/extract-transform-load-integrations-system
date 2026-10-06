using Application.Core.Abstractions.Services;
using Application.LinxCommerce.Abstractions.Services;
using Domain.Core.Abstractions.Api;
using Domain.Core.Enums;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Product;
using Domain.LinxCommerce.Entities.Responses;
using Domain.LinxCommerce.Entities.Sku;
using Domain.LinxCommerce.Interfaces.Repositorys;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.LinxCommerce.Services
{
    public class SKUService : ISKUService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IValidator<Sku> _validator;
        private readonly IValidator<Product> _productValidator;
        private readonly ISKURepository _skuRepository;

        private static List<string?> _errorMessagesList { get; set; } = new List<string?>();

        public SKUService([FromKeyedServices("LinxCommerceAPI")] IApiClient apiClient, ILoggerService logger, IValidator<Sku> validator, IValidator<Product> productValidator, ISKURepository skuRepository) =>
            (_apiClient, _logger, _validator, _productValidator, _skuRepository) = (apiClient, logger, validator, productValidator, skuRepository);

        private static Dictionary<string, string> BuildAuthHeaders(LinxCommerceJobParameter jobParameter)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{jobParameter.UserAuthentication}:{jobParameter.KeyAuthorization}");
            return new Dictionary<string, string>
            {
                { "Authorization", "Basic " + Convert.ToBase64String(plainTextBytes) }
            };
        }

        public async Task<bool?> GetSKU(LinxCommerceJobParameter jobParameter, int productID)
        {
            var response = await _apiClient.PostAsync(
                route: "/v1/Catalog/API.svc/web/GetSKU",
                body: productID,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var skus = Newtonsoft.Json.JsonConvert.DeserializeObject<Sku>(response);

            //await _skuRepository.InsertRecord(jobParameter, skus);

            return true;
        }

        public async Task<bool?> SearchProductByDateInterval(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceProduct);

            var objectRequest = new
            {
                Page = new { PageIndex = 0, PageSize = 10000 },
                Where = $"(CreatedDate>=\"{DateTime.Now.AddDays(-7).Date:yyyy-MM-dd}T00:00:00\" && CreatedDate<=\"{DateTime.Now.Date:yyyy-MM-dd}T23:59:59\")",
                WhereMetadata = "",
                OrderBy = "ProductID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Catalog/API.svc/web/SearchProduct",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var productAPIList = new List<Product>();
            var productsIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchProducts.Root>(response);
            var groupedIDs = productsIDs.Result.GroupBy(x => x.ProductID).Select(g => g.First()).ToList();

            foreach (var productID in groupedIDs)
            {
                string route = String.Empty;

                var getProductResponse = await _apiClient.PostAsync(
                    route: "/v1/Catalog/API.svc/web/GetProduct",
                    body: productID.ProductID,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var product = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(getProductResponse);
                var validations = _productValidator.Validate(product);

                if (validations.Errors.Count() > 0)
                {
                    var message = $"Error when convert record - ProductID: {product.ProductID} | Name: {product.Name}\n";

                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        var msg = validations.Errors[j].ErrorMessage;
                        var property = validations.Errors[j].PropertyName;
                        var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                        message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                    }

                    _errorMessagesList.Add(message);
                }

                productAPIList.Add(new Product(product, getProductResponse));
            }

            if (productAPIList.Count() > 0)
            {
                _skuRepository.BulkInsertIntoTableRaw(jobParameter, productAPIList, _logger.GetExecutionGuid());

                for (int i = 0; i < productAPIList.Count; i++)
                {
                    _logger.AddRecord(productAPIList[i].ProductID.ToString(), productAPIList[i].Responses.Where(pair => pair.Key == productAPIList[i].ProductID).Select(pair => pair.Value).FirstOrDefault());
                    _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - ProductID: {productAPIList[i].ProductID} | Name: {productAPIList[i].Name}")).FirstOrDefault());
                }
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool?> SearchProductByQueue(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceProduct);

            var objectRequest = new
            {
                QueueID = 21,
                Page = new { PageIndex = 0, PageSize = 1000 },
                Where = $"EntityTypeName==\"EZ.Store.Model.Catalog.Product\"",
                WhereMetadata = "",
                OrderBy = "QueueItemID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Queue/API.svc/web/SearchQueueItems",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var productAPIList = new List<Product>();
            var enqueuedItens = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchQueuedItem.Root>(response);
            var enqueuedProducts = enqueuedItens
                                .Result
                                .Where(x => x.EntityTypeName == "EZ.Store.Model.Catalog.Product")
                                .GroupBy(y => y.EntityKeyValue)
                                .Select(g => g.First())
                                .ToList();

            foreach (var enqueuedProduct in enqueuedProducts)
            {
                string route = String.Empty;

                var getProductResponse = await _apiClient.PostAsync(
                    route: "/v1/Catalog/API.svc/web/GetProduct",
                    body: Convert.ToInt32(enqueuedProduct.EntityKeyValue),
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var product = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(getProductResponse);
                var validations = _productValidator.Validate(product);

                if (validations.Errors.Count() > 0)
                {
                    var message = $"Error when convert record - ProductID: {product.ProductID} | Name: {product.Name}\n";

                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        var msg = validations.Errors[j].ErrorMessage;
                        var property = validations.Errors[j].PropertyName;
                        var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                        message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                    }

                    _errorMessagesList.Add(message);
                }

                productAPIList.Add(new Product(product, getProductResponse));
            }

            if (productAPIList.Count() > 0)
            {
                _skuRepository.BulkInsertIntoTableRaw(jobParameter, productAPIList, _logger.GetExecutionGuid());

                for (int i = 0; i < productAPIList.Count; i++)
                {
                    _logger.AddRecord(productAPIList[i].ProductID.ToString(), productAPIList[i].Responses.Where(pair => pair.Key == productAPIList[i].ProductID).Select(pair => pair.Value).FirstOrDefault());
                    _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - ProductID: {productAPIList[i].ProductID} | Name: {productAPIList[i].Name}")).FirstOrDefault());
                }

                //remover da fila de integração apenas se o registro for inserido no banco de dados
                var listQueueItemID = new List<string>();

                foreach (var enqueuedproduct in enqueuedProducts)
                {
                    listQueueItemID.Add(enqueuedproduct.QueueItemID);
                }

                var dequeueObjectRequest = new { QueueItems = listQueueItemID };

                var dequeueResponse = await _apiClient.PostAsync(
                    route: "/v1/Queue/API.svc/web/DequeueQueueItems",
                    body: dequeueObjectRequest,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool?> SearchSKUByDateInterval(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceSku);

            var objectRequest = new
            {
                Page = new { PageIndex = 0, PageSize = 1000 },
                Where = $"(CreatedDate>=\"{DateTime.Now.AddDays(-1).Date:yyyy-MM-dd}T00:00:00\" && CreatedDate<=\"{DateTime.Now.Date:yyyy-MM-dd}T23:59:59\")",
                WhereMetadata = "",
                OrderBy = "ProductID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Catalog/API.svc/web/SearchSKU",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var skuAPIList = new List<Sku>();
            var productsIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchSKU.Root>(response);

            foreach (var productID in productsIDs.Result)
            {
                string route = String.Empty;

                var getSkuResponse = await _apiClient.PostAsync(
                    route: "/v1/Catalog/API.svc/web/GetSKU",
                    body: productID.ProductID,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var sku = Newtonsoft.Json.JsonConvert.DeserializeObject<Sku>(getSkuResponse);
                var validations = _validator.Validate(sku);

                if (validations.Errors.Count() > 0)
                {
                    var message = $"Error when convert record - ProductID: {sku.ProductID} | Name: {sku.Name}\n";

                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        var msg = validations.Errors[j].ErrorMessage;
                        var property = validations.Errors[j].PropertyName;
                        var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                        message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                    }

                    _errorMessagesList.Add(message);
                }

                skuAPIList.Add(new Sku(sku, getSkuResponse));
            }

            if (skuAPIList.Count() > 0)
            {
                _skuRepository.BulkInsertIntoTableRaw(jobParameter, skuAPIList, _logger.GetExecutionGuid());

                for (int i = 0; i < skuAPIList.Count; i++)
                {
                    _logger.AddRecord(skuAPIList[i].ProductID.ToString(), skuAPIList[i].Responses.Where(pair => pair.Key == skuAPIList[i].ProductID).Select(pair => pair.Value).FirstOrDefault());
                    _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - ProductID: {skuAPIList[i].ProductID} | Name: {skuAPIList[i].Name}")).FirstOrDefault());
                }
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool?> SearchSKUByQueue(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceSku);

            var objectRequest = new
            {
                QueueID = 21,
                Page = new { PageIndex = 0, PageSize = 1000 },
                Where = $"EntityKeyName==\"CatalogItemID\" && EntityTypeName==\"EZ.Store.Model.Catalog.StockKeepUnit\"",
                WhereMetadata = "",
                OrderBy = "QueueItemID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Queue/API.svc/web/SearchQueueItems",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var skuAPIList = new List<Sku>();
            var enqueuedItens = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchQueuedItem.Root>(response);
            var enqueuedSKU = enqueuedItens
                                .Result
                                .Where(x => x.EntityKeyName == "CatalogItemID")
                                .GroupBy(y => y.EntityKeyValue)
                                .Select(g => g.First())
                                .ToList();

            foreach (var productID in enqueuedSKU)
            {
                string route = String.Empty;

                var getSkuResponse = await _apiClient.PostAsync(
                    route: "/v1/Catalog/API.svc/web/GetSKU",
                    body: Convert.ToInt32(productID.EntityKeyValue),
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var sku = Newtonsoft.Json.JsonConvert.DeserializeObject<Sku>(getSkuResponse);
                var validations = _validator.Validate(sku);

                if (validations.Errors.Count() > 0)
                {
                    var message = $"Error when convert record - ProductID: {sku.ProductID} | Name: {sku.Name}\n";

                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        var msg = validations.Errors[j].ErrorMessage;
                        var property = validations.Errors[j].PropertyName;
                        var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                        message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                    }

                    _errorMessagesList.Add(message);
                }

                skuAPIList.Add(new Sku(sku, getSkuResponse));
            }

            if (skuAPIList.Count() > 0)
            {
                _skuRepository.BulkInsertIntoTableRaw(jobParameter, skuAPIList, _logger.GetExecutionGuid());

                for (int i = 0; i < skuAPIList.Count; i++)
                {
                    _logger.AddRecord(skuAPIList[i].ProductID.ToString(), skuAPIList[i].Responses.Where(pair => pair.Key == skuAPIList[i].ProductID).Select(pair => pair.Value).FirstOrDefault());
                    _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - ProductID: {skuAPIList[i].ProductID} | Name: {skuAPIList[i].Name}")).FirstOrDefault());
                }

                //remover da fila de integração apenas se o registro for inserido no banco de dados
                var listQueueItemID = new List<string>();

                foreach (var enqueuedsku in enqueuedSKU)
                {
                    listQueueItemID.Add(enqueuedsku.QueueItemID);
                }

                var dequeueObjectRequest = new { QueueItems = listQueueItemID };

                var dequeueResponse = await _apiClient.PostAsync(
                    route: "/v1/Queue/API.svc/web/DequeueQueueItems",
                    body: dequeueObjectRequest,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}

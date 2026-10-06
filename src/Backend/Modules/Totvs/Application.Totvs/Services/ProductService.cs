using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Enums;
using Domain.Shared.Extensions;
using Domain.Shared.Abstractions.Api;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Dtos.Product;
using Domain.Totvs.Entities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Application.Totvs.Services
{
    public class ProductService : IProductService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IValidator<Sku> _productValidator;
        private readonly IProductRepository _productRepository;
        private static List<Domain.Totvs.Dtos.Product.Sku?> _skusCache { get; set; } = new List<Domain.Totvs.Dtos.Product.Sku?>();

        public ProductService([FromKeyedServices("TotvsHmg")] IApiClient apiClient, ILoggerService logger, IValidator<Sku> productValidator, IProductRepository productRepository) =>
            (_apiClient, _logger, _productValidator, _productRepository) = (apiClient, logger, productValidator, productRepository);

        public async Task<bool?> PostSearchReferences()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.TotvsProducts);

            int pageIndex = 1, pageCount = 1;
            var skusList = new List<Domain.Totvs.Entities.Product.Sku>();
            var itens = new List<Sku>();

            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "newbloomerstrnapiv2" },
                { "client_secret", "4860687042" },
                { "username", "INT" },
                { "password", "INTV3" }
            };

            var response = await _apiClient.PostFormAsync(
                route: "/authorization/v2/token",
                formData: parameters
            );

            var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(response);

            while (pageIndex <= pageCount)
            {
                var objectRequest = new
                {
                    filter = new {
                        branchInfo = new {
                            branchCode = 1,
                            isActive = true,
                            isFinishedProduct = true
                        },
                        change = new {
                            //startDate = $"{DateTime.Now.AddDays(-2).Date:yyyy-MM-dd}T00:00:00.000Z",
                            startDate = $"{DateTime.Now.AddMonths(-10).Date:yyyy-MM-dd}T00:00:00.000Z",
                            endDate = $"{DateTime.Now.Date:yyyy-MM-dd}T23:59:59.000Z",
                            inProduct = true
                        }
                    },
                    order = "maxChangeFilterDate",
                    option = new { branchInfoCode = 1 },
                    page = pageIndex,
                    pageSize = 500,
                    expand = "observations,details,composition,filter,barCodes,classifications,additionalFields,webData,webDetail,referenceCategories,suppliers"
                };

                var _response = await _apiClient.PostAsync(
                    route: "/product/v2/references/search",
                    body: objectRequest,
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                );

                try
                {
                    var products = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(_response);

                    if (pageCount == 1)
                        pageCount = products.totalPages;

                    pageIndex++;

                    itens.AddRange(products.items);
                }
                catch (Exception)
                {
                    throw;
                }

            }

            if (_skusCache.Count() == 0)
                _skusCache = await _productRepository.GetExistingSkus(
                    itens.Select(x => x.referenceId).ToList()
                );

            var grupos = itens.GroupBy(x => x.referenceId).ToList();
            foreach (var grupo in grupos.Where(g => g.Count() > 1))
            {
                var serializadoPrimeiro = Newtonsoft.Json.JsonConvert.SerializeObject(grupo.First());
                var divergentes = grupo.Skip(1)
                    .Count(x => Newtonsoft.Json.JsonConvert.SerializeObject(x) != serializadoPrimeiro);

                _logger.AddMessage(
                    $"Duplicidade na API - referenceId {grupo.Key}: {grupo.Count()} ocorrência(s)" +
                    (divergentes > 0
                        ? $" - ATENÇÃO: {divergentes} divergente(s), mantida a primeira."
                        : " - conteúdos idênticos.")
                );
            }
            itens = grupos.Select(g => g.First()).ToList();

            itens.RemoveAll(x => _skusCache.Any(y =>
               x.referenceId == y.referenceId &&
               ConverterValidatorsExtensions.ConvertToDateTimeValidation<string?>(x.maxChangeFilterDate) <= DateTime.Parse(y.maxChangeFilterDate, new CultureInfo("en-US"))
            ));

            foreach (var product in itens)
            {
                var validations = _productValidator.Validate(product);

                if (validations.Errors.Count() > 0)
                {
                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        _logger.AddMessage(
                            message: $"Error when convert record - ProductID: {product.referenceId} | Name: {product.name}\n" +
                                     $"{validations.Errors[j]}"
                        );
                    }
                }

                skusList.Add(new Domain.Totvs.Entities.Product.Sku(product));
            }

            if (skusList.Count() > 0)
            {
                await _productRepository.BulkInsertIntoTableSkusRaw(skusList, _logger.GetExecutionGuid());

                skusList.ForEach(p =>
                    _logger.AddRecord(
                        p.referenceId.ToString(),
                        p.Responses
                            .Where(pair => pair.Key == p.referenceId.ToString())
                            .Select(pair => pair.Value)
                            .FirstOrDefault()
                    )
                );

                _logger.AddMessage(
                    $"Concluída com sucesso: {skusList.Count()} registro(s) novo(s) inserido(s)!"
                );
            }
            else
                _logger.AddMessage(
                    $"Concluída com sucesso: {skusList.Count()} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}

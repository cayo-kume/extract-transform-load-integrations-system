using Application.Core.Abstractions.Services;
using Application.LinxCommerce.Abstractions.Services;
using Domain.Core.Abstractions.Api;
using Domain.Core.Enums;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Responses;
using Domain.LinxCommerce.Entities.SalesRepresentative;
using Domain.LinxCommerce.Interfaces.Repositorys;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.LinxCommerce.Services
{
    public class SalesRepresentativeService : ISalesRepresentativeService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ISalesRepresentativeRepository _salesRepresentativeRepository;
        private readonly IValidator<SalesRepresentative> _validator;

        private static List<string?> _errorMessagesList { get; set; } = new List<string?>();

        public SalesRepresentativeService([FromKeyedServices("LinxCommerceAPI")] IApiClient apiClient, ILoggerService logger, ISalesRepresentativeRepository salesRepresentativeRepository, IValidator<SalesRepresentative> validator) =>
            (_apiClient, _logger, _salesRepresentativeRepository, _validator) = (apiClient, logger, salesRepresentativeRepository, validator);

        private static Dictionary<string, string> BuildAuthHeaders(LinxCommerceJobParameter jobParameter)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{jobParameter.UserAuthentication}:{jobParameter.KeyAuthorization}");
            return new Dictionary<string, string>
            {
                { "Authorization", "Basic " + Convert.ToBase64String(plainTextBytes) }
            };
        }

        public Task<bool?> DeleteSalesRepresentative(int salesRepresentativeId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> GetSalesRepresentative(LinxCommerceJobParameter jobParameter, int? salesRepresentativeId)
        {
            var salesRepresentativeList = new List<SalesRepresentative>();

            var getSaleRepresentativeResponse = await _apiClient.PostAsync(
                route: "/v1/Sales/API.svc/web/GetSalesRepresentative",
                body: new { salesRepresentativeId },
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var saleRepresentative = Newtonsoft.Json.JsonConvert.DeserializeObject<GetSalesRepresentative.Root>(getSaleRepresentativeResponse);
            salesRepresentativeList.Add(saleRepresentative.SalesRepresentative);

            //_salesRepresentativeRepository.BulkInsertIntoTableRaw(jobParameter: jobParameter, registros: salesRepresentativeList);

            return true;
        }

        public async Task<bool?> SaveSalesRepresentative(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceUpdateSalesRepresentative);

            int pageIndex = 0, pageCount = 0;
            var salesRepresentativeResults = new List<SearchSalesRepresentative.Result>();

            //while aqui
            var objectRequest = new
            {
                Page = new { PageIndex = 0, PageSize = 0 },
                Where = $"",
                WhereMetadata = "",
                OrderBy = "SalesRepresentativeID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Sales/API.svc/web/SearchSalesRepresentative",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var salesRepresentativeAPIList = new List<SalesRepresentative>();

            //cache aqui
            var salesRepresentativeIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchSalesRepresentative.Root>(response);

            foreach (var salesRepresentativeID in salesRepresentativeIDs.Result)
            {
                var getSaleRepresentativeObjectRequest = new
                {
                    salesRepresentativeID.SalesRepresentativeID
                };

                var getSaleRepresentativeResponse = await _apiClient.PostAsync(
                    route: "/v1/Sales/API.svc/web/GetSalesRepresentative",
                    body: getSaleRepresentativeObjectRequest,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var saleRepresentative = Newtonsoft.Json.JsonConvert.DeserializeObject<GetSalesRepresentative.Root>(getSaleRepresentativeResponse);

                var jObject = new
                {
                    SalesRepresentativeID = saleRepresentative.SalesRepresentative.SalesRepresentativeID,
                    RemovePhoto = true,
                    RemoveNotInformedAddresses = true,
                    RemoveNotInformedCustomers = true,
                    RemoveNotInformedUsers = true,
                    Portfolio = new { HasPortfolio = saleRepresentative.SalesRepresentative.Portfolio.HasPortfolio, PortfolioAssociationType = "D", Customers = new List<SalesRepresentativeCustomerRelation>() },
                    ShippingRegion = saleRepresentative.SalesRepresentative.ShippingRegion,
                    WebSiteSettings = saleRepresentative.SalesRepresentative.WebSiteSettings,
                    Status = saleRepresentative.SalesRepresentative.Status,
                    SalesRepresentativeType = saleRepresentative.SalesRepresentative.SalesRepresentativeType,
                    Name = saleRepresentative.SalesRepresentative.Name,
                    Identification = saleRepresentative.SalesRepresentative.Identification,
                    FriendlyCode = saleRepresentative.SalesRepresentative.FriendlyCode,
                    Contact = saleRepresentative.SalesRepresentative.Contact,
                    OrderTypeItems = saleRepresentative.SalesRepresentative.OrderTypeItems,
                    AllowQuoteDeletion = saleRepresentative.SalesRepresentative.AllowQuoteDeletion,
                    MaxDiscount = saleRepresentative.SalesRepresentative.MaxDiscount,
                    PortfolioCommission = saleRepresentative.SalesRepresentative.Commission,
                    GeneralCommission = saleRepresentative.SalesRepresentative.Commission,
                };

                var saveSalesRepresentativeResponse = await _apiClient.PostAsync(
                    route: "/v1/Sales/API.svc/web/SaveSalesRepresentative",
                    body: jObject,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                salesRepresentativeAPIList.Add(new SalesRepresentative(saleRepresentative.SalesRepresentative, getSaleRepresentativeResponse));
            }

            if (salesRepresentativeAPIList.Count() > 0)
            {
                salesRepresentativeAPIList.ForEach(s =>
                    _logger.AddRecord(
                        s.SalesRepresentativeID.ToString(),
                        s.Responses
                            .Where(pair => pair.Key == s.SalesRepresentativeID)
                            .Select(pair => pair.Value)
                            .FirstOrDefault()
                    )
                );
            }

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool?> SearchSalesRepresentative(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceSalesRepresentative);

            int pageIndex = 0, pageCount = 0;
            var salesRepresentativeResults = new List<SearchSalesRepresentative.Result>();

            //while aqui
            var objectRequest = new
            {
                Page = new { PageIndex = 0, PageSize = 0 },
                Where = $"",
                WhereMetadata = "",
                OrderBy = "SalesRepresentativeID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Sales/API.svc/web/SearchSalesRepresentative",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var salesRepresentativeAPIList = new List<SalesRepresentative>();

            //cache aqui
            var salesRepresentativeIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchSalesRepresentative.Root>(response);

            foreach (var salesRepresentativeID in salesRepresentativeIDs.Result)
            {
                var getSaleRepresentativeObjectRequest = new
                {
                    salesRepresentativeID.SalesRepresentativeID
                };

                var getSaleRepresentativeResponse = await _apiClient.PostAsync(
                    route: "/v1/Sales/API.svc/web/GetSalesRepresentative",
                    body: getSaleRepresentativeObjectRequest,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var saleRepresentative = Newtonsoft.Json.JsonConvert.DeserializeObject<GetSalesRepresentative.Root>(getSaleRepresentativeResponse);
                var validations = _validator.Validate(saleRepresentative.SalesRepresentative);

                if (validations.Errors.Count() > 0)
                {
                    var message = $"Error when convert record - SalesRepresentativeID: {saleRepresentative.SalesRepresentative.SalesRepresentativeID} | Name: {saleRepresentative.SalesRepresentative.Name}\n";

                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        var msg = validations.Errors[j].ErrorMessage;
                        var property = validations.Errors[j].PropertyName;
                        var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                        message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                    }

                    _errorMessagesList.Add(message);
                }

                salesRepresentativeAPIList.Add(new SalesRepresentative(saleRepresentative.SalesRepresentative, getSaleRepresentativeResponse));
            }

            if (salesRepresentativeAPIList.Count() > 0)
            {
                _salesRepresentativeRepository.BulkInsertIntoTableRaw(jobParameter: jobParameter, registros: salesRepresentativeAPIList, _logger.GetExecutionGuid());

                for (int i = 0; i < salesRepresentativeAPIList.Count; i++)
                {
                    _logger.AddRecord(salesRepresentativeAPIList[i].SalesRepresentativeID.ToString(), salesRepresentativeAPIList[i].Responses.Where(pair => pair.Key == salesRepresentativeAPIList[i].SalesRepresentativeID).Select(pair => pair.Value).FirstOrDefault());
                    _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - SalesRepresentativeID: {salesRepresentativeAPIList[i].SalesRepresentativeID} | Name: {salesRepresentativeAPIList[i].Name}")).FirstOrDefault());
                }
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}

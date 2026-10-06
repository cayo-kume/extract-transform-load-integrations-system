using Application.Core.Abstractions.Services;
using Application.LinxCommerce.Abstractions.Services;
using Domain.Core.Abstractions.Api;
using Domain.Core.Enums;
using Domain.LinxCommerce.Entities.Customer;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Responses;
using Domain.LinxCommerce.Interfaces.Repositorys;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Application.LinxCommerce.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ICustomerRepository _customerRepository;
        private readonly IValidator<Person> _validator;

        private List<string?> _errorMessagesList = new List<string?>();
        private readonly IMemoryCache _cache;
        private const string CustomerHashKey = "LinxCommerce:CustomerHash";

        public CustomerService([FromKeyedServices("LinxCommerceAPI")] IApiClient apiClient, ILoggerService logger, ICustomerRepository customerRepository, IValidator<Person> validator, IMemoryCache cache) =>
            (_apiClient, _logger, _customerRepository, _validator, _cache) = (apiClient, logger, customerRepository, validator, cache);

        private static Dictionary<string, string> BuildAuthHeaders(LinxCommerceJobParameter jobParameter)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{jobParameter.UserAuthentication}:{jobParameter.KeyAuthorization}");
            return new Dictionary<string, string>
            {
                { "Authorization", "Basic " + Convert.ToBase64String(plainTextBytes) }
            };
        }

        public Task<bool?> AddCustomerRelation(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> ApproveCustomerFile(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> DeleteCustomer(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> DeleteCustomerFile(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> DeleteCustomerGroup(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> DeleteCustomerRelation(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> GetCompany(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> GetCustomer(LinxCommerceJobParameter jobParameter, string Identifier)
        {
            var objectRequest = new
            {
                Page = new { PageIndex = 0, PageSize = 1000 },
                Where = $"CustomerID=={Identifier}",
                WhereMetadata = "",
                OrderBy = "CustomerID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Profile/API.svc/web/SearchCustomer",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var customersAPIList = new List<Person>();
            var customersIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchCustomer.Root>(response);

            foreach (var customerID in customersIDs.Result)
            {
                string route = String.Empty;

                if (customerID.CustomerType == "P")
                    route = "GetPerson";
                else
                    route = "GetCompany";

                var getCustomerResponse = await _apiClient.PostAsync(
                    route: "/v1/Profile/API.svc/web/" + route,
                    body: customerID.CustomerID,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(getCustomerResponse);

                customersAPIList.Add(new Person(customer, getCustomerResponse));
            }

            if (customersAPIList.Count() > 0)
            {
                await _customerRepository.InsertRecord(jobParameter, customersAPIList.FirstOrDefault(), new Guid());
            }

            return true;
        }

        public Task<bool?> GetCustomerFile(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> GetCustomerGroup(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> GetCustomerStatus(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> GetCustomerStatusByIntegrationID(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> GetPerson(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> GetQueueCustomerFile(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> GetQueueCustomers(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> GetStatusModerationCustomerMarketplace(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> RejectCustomerFile(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> RunCustomerTransition(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SaveCompany(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SaveCustomer(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SaveCustomerAddress(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SaveCustomerFile(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SaveCustomerGroup(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SaveCustomerStatus(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> SavePerson(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> SearchCustomerByDateInterval(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceCustomers);

            var _listSomenteNovos = new List<Person>();

            var objectRequest = new
            {
                Page = new { PageIndex = 0, PageSize = 1000 },
                Where = $"(CreatedDate>=\"{DateTime.Now.AddDays(-1).Date:yyyy-MM-dd}T00:00:00\" && CreatedDate<=\"{DateTime.Now.Date:yyyy-MM-dd}T23:59:59\")",
                //Where = $"(CreatedDate>=\"2025-07-01T00:00:00\" && CreatedDate<=\"2025-07-31T23:59:59\")",
                WhereMetadata = "",
                OrderBy = "CustomerID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Profile/API.svc/web/SearchCustomer",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var hash = _logger.ComputeSha256Hash(response);

            var cachedHash = _cache.Get<string>(CustomerHashKey) ?? String.Empty;
            if (!cachedHash.Equals(hash))
            {
                var customersIDs = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchCustomer.Root>(response);

                foreach (var customerID in customersIDs.Result)
                {
                    string route = String.Empty;

                    if (customerID.CustomerType == "P")
                        route = "GetPerson";
                    else
                        route = "GetCompany";

                    var getCustomerResponse = await _apiClient.PostAsync(
                        route: "/v1/Profile/API.svc/web/" + route,
                        body: customerID.CustomerID,
                        headers: BuildAuthHeaders(jobParameter)
                    ) ?? string.Empty;

                    var customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(getCustomerResponse);
                    var validations = _validator.Validate(customer);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = $"Error when convert record - cod_cliente: {customer.CustomerID} | doc_cliente: {(String.IsNullOrEmpty(customer.Cpf) ? customer.Cnpj : customer.Cpf)}\n";

                        for (int j = 0; j < validations.Errors.Count(); j++)
                        {
                            var msg = validations.Errors[j].ErrorMessage;
                            var property = validations.Errors[j].PropertyName;
                            var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                            message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                        }

                        _errorMessagesList.Add(message);
                    }

                    _listSomenteNovos.Add(new Person(customer, getCustomerResponse));
                }
            }

            if (_listSomenteNovos.Count() > 0)
            {
                await _customerRepository.BulkInsertIntoTableRaw(jobParameter, _listSomenteNovos, _logger.GetExecutionGuid());

                for (int i = 0; i < _listSomenteNovos.Count; i++)
                {
                    _logger.AddRecord(_listSomenteNovos[i].CustomerID.ToString(), _listSomenteNovos[i].Responses.Where(pair => pair.Key == _listSomenteNovos[i].CustomerID).Select(pair => pair.Value).FirstOrDefault());
                    _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - cod_cliente: {_listSomenteNovos[i].CustomerID} | doc_cliente: {(String.IsNullOrEmpty(_listSomenteNovos[i].Cpf) ? _listSomenteNovos[i].Cnpj : _listSomenteNovos[i].Cpf)}")).FirstOrDefault());
                }

                _cache.Set(CustomerHashKey, hash, TimeSpan.FromHours(2));
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool?> SearchCustomerByQueue(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceCustomers);

            var objectRequest = new
            {
                QueueID = 21,
                Page = new { PageIndex = 0, PageSize = 1000 },
                Where = $"EntityKeyName==\"CustomerID\"",
                WhereMetadata = "",
                OrderBy = "QueueItemID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Profile/API.svc/web/GetQueueCustomers",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var customersAPIList = new List<Person>();
            var enqueuedItens = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchQueuedCustomer.Root>(response);
            var enqueuedCustomers = enqueuedItens
                                .Result
                                .Where(x => x.QueueItem.EntityKeyName == "CustomerID")
                                .GroupBy(y => y.QueueItem.EntityKeyValue)
                                .Select(g => g.First())
                                .ToList();

            foreach (var enqueuedCustomer in enqueuedCustomers)
            {
                string route = String.Empty;

                if (enqueuedCustomer.Customer.CustomerType == 80)
                    route = "GetPerson";
                else
                    route = "GetCompany";

                var getCustomerResponse = await _apiClient.PostAsync(
                    route: "/v1/Profile/API.svc/web/" + route,
                    body: Convert.ToInt32(enqueuedCustomer.Customer.CustomerID),
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var customer = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(getCustomerResponse);
                var validations = _validator.Validate(customer);

                if (validations.Errors.Count() > 0)
                {
                    var message = $"Error when convert record - cod_cliente: {customer.CustomerID} | doc_cliente: {(String.IsNullOrEmpty(customer.Cpf) ? customer.Cnpj : customer.Cpf)}\n";

                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        var msg = validations.Errors[j].ErrorMessage;
                        var property = validations.Errors[j].PropertyName;
                        var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                        message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                    }

                    _errorMessagesList.Add(message);
                }

                customersAPIList.Add(new Person(customer, getCustomerResponse));
            }

            if (customersAPIList.Count() > 0)
            {
                await _customerRepository.BulkInsertIntoTableRaw(jobParameter, customersAPIList, _logger.GetExecutionGuid());

                for (int i = 0; i < customersAPIList.Count; i++)
                {
                    _logger.AddRecord(customersAPIList[i].CustomerID.ToString(), customersAPIList[i].Responses.Where(pair => pair.Key == customersAPIList[i].CustomerID).Select(pair => pair.Value).FirstOrDefault());
                    _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - cod_cliente: {customersAPIList[i].CustomerID} | doc_cliente: {(String.IsNullOrEmpty(customersAPIList[i].Cpf) ? customersAPIList[i].Cnpj : customersAPIList[i].Cpf)}")).FirstOrDefault());
                }

                //remover da fila de integração apenas se o registro for inserido no banco de dados
                var listQueueItemID = new List<string>();

                foreach (var enqueuedCustomer in enqueuedCustomers)
                {
                    listQueueItemID.Add(enqueuedCustomer.QueueItem.QueueItemID);
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

        public Task<bool?> SearchCustomerGroup(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> UpdateModerationCustomerToSellerMarketplace(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> UpdateModerationSellerToCustomerMarketplace(LinxCommerceJobParameter jobParameter)
        {
            throw new NotImplementedException();
        }
    }
}
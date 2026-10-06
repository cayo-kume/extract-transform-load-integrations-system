using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Application.Totvs.Validations.Person.LegalEntity;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Enums;
using Domain.Shared.Extensions;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities;
using Domain.Totvs.Entities.Person.Branch;
using Domain.Totvs.Entities.Person.Customer;
using Domain.Totvs.Entities.Person.LegalEntity;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Application.Totvs.Services
{
    public class PersonService : IPersonService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IValidator<Domain.Totvs.Dtos.Person.LegalEntity.Item> _legalEntityValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Person.Customer.Item> _customerValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Person.Branch.Branch> _branchValidator;
        private readonly IPersonRepository _personRepository;
        private readonly IMemoryCache _cache;
        private const string CustomersCacheKey = "Totvs:Customers";
        private const string LegalEntitiesCacheKey = "Totvs:LegalEntities";
        private const string BranchesCacheKey = "Totvs:Branches";

        public PersonService([FromKeyedServices("TotvsHmg")] IApiClient apiClient, ILoggerService logger, IValidator<Domain.Totvs.Dtos.Person.LegalEntity.Item> legalEntityValidator, IValidator<Domain.Totvs.Dtos.Person.Customer.Item> customerValidator, IValidator<Domain.Totvs.Dtos.Person.Branch.Branch> branchValidator, IPersonRepository personRepository, IMemoryCache cache) =>
            (_apiClient, _logger, _legalEntityValidator, _customerValidator, _branchValidator, _personRepository, _cache) = (apiClient, logger, legalEntityValidator, customerValidator, branchValidator, personRepository, cache);

        public async Task<bool?> PostSearchCustomers()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.TotvsCustomers);

            int pageIndex = 1, pageCount = 1;
            var customersList = new List<Customer>();
            var itens = new List<Domain.Totvs.Dtos.Person.Customer.Item>();
            var somenteNovosList = new List<Domain.Totvs.Dtos.Person.Customer.Item>();

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
                    filter = new
                    {
                        change = new
                        {
                            //startDate = $"{DateTime.Now.AddDays(-14).Date:yyyy-MM-dd}T00:00:00.000Z",
                            startDate = $"2000-01-01T00:00:00.000Z",
                            endDate = $"{DateTime.Now.Date:yyyy-MM-dd}T23:59:59.000Z",
                            inClassification = true,
                            inAddress = true,
                            inPhone = true,
                            inObservation = true,
                            inPerson = true,
                            inCustomer = true,
                            inSupplier = true,
                            inRepresentative = true,
                            inPurchasingGuide = true,
                            inShippingCompany = true,
                            inReference = true,
                            inContact = true,
                            inStatistic = true,
                            inCustomerObservation = true,
                            inEmployee = true,
                            inPreference = true
                        }
                    },
                    order = "maxChangeFilterDate",
                    page = pageIndex,
                    pageSize = 500,
                    expand = "addresses,phones,emails,observations,customerObservations,additionalFields,classifications,references,relateds,shippingCompany,statistics,representatives,preferences,familiars,cpf"
                };

                var _response = await _apiClient.PostAsync(
                    route: "/person/v2/individuals/search",
                    body: objectRequest,
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                );

                try
                {
                    var customers = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Totvs.Dtos.Person.Customer.Root>(_response);

                    if (pageCount == 1)
                        pageCount = customers.totalPages;

                    pageIndex++;

                    itens.AddRange(customers.items);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            var cached = await _cache.GetOrCreateAsync(CustomersCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var list = await _personRepository.GetExistingCustomers(
                    itens.Select(x => x.code).ToList()
                );

                return list
                    .Where(x => x != null)
                    .GroupBy(x => x.code)
                    .ToDictionary(g => g.Key, g => g.First());
            });

            itens.RemoveAll(x =>
               cached.TryGetValue(x.code, out var y) &&
               ConverterValidatorsExtensions.ConvertToDateTimeValidation<string?>(x.maxChangeFilterDate) <= DateTime.Parse(y.maxChangeFilterDate, new CultureInfo("en-US"))
            );

            foreach (var customer in itens)
            {
                var validations = _customerValidator.Validate(customer);

                if (validations.Errors.Count() > 0)
                {
                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        _logger.AddMessage(
                            message: $"Error when convert record - customerCode: {customer.code} | Name: {customer.name}\n" +
                                     $"{validations.Errors[j]}"
                        );
                    }
                }

                customersList.Add(new Customer(customer));
            }

            if (customersList.Count() > 0)
            {
                await _personRepository.BulkInsertIntoTableCustomersRaw(customersList, _logger.GetExecutionGuid());

                foreach (var item in itens.Where(x => x != null))
                    cached[item.code] = item;

                customersList.ForEach(p =>
                    _logger.AddRecord(
                        p.code.ToString(),
                        p.Responses
                            .Where(pair => pair.Key == p.code.ToString())
                            .Select(pair => pair.Value)
                            .FirstOrDefault()
                    )
                );

                _logger.AddMessage(
                    $"Concluída com sucesso: {customersList.Count()} registro(s) novo(s) inserido(s)!"
                );
            }
            else
                _logger.AddMessage(
                    $"Concluída com sucesso: {customersList.Count()} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool?> PostSearchLegalEntity()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.TotvsLegalEntities);

            int pageIndex = 1, pageCount = 1;
            var legalEntitesList = new List<LegalEntity>();
            var itens = new List<Domain.Totvs.Dtos.Person.LegalEntity.Item>();
            var somenteNovosList = new List<Domain.Totvs.Dtos.Person.LegalEntity.Item>();

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
                    filter = new
                    {
                        change = new {
                            //startDate = $"{DateTime.Now.AddDays(-14).Date:yyyy-MM-dd}T00:00:00.000Z",
                            startDate = $"2000-01-01T00:00:00.000Z",
                            endDate = $"{DateTime.Now.Date:yyyy-MM-dd}T23:59:59.000Z",
                            inClassification = true,
                            inAddress = true,
                            inPhone = true,
                            inObservation = true,
                            inPerson = true,
                            inCustomer = true,
                            inSupplier = true,
                            inRepresentative = true,
                            inPurchasingGuide = true,
                            inShippingCompany = true,
                            inReference = true,
                            inContact = true,
                            inStatistic = true,
                            inCustomerObservation = true,
                            inEmployee = true,
                            inPreference = true
                        }
                    },
                    order = "maxChangeFilterDate",
                    page = pageIndex,
                    pageSize = 500,
                    expand = "observations,details,composition,filter,barCodes,classifications,additionalFields,webData,webDetail,referenceCategories,suppliers"
                };

                var _response = await _apiClient.PostAsync(
                    route: "/person/v2/legal-entities/search",
                    body: objectRequest,
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                );

                try
                {
                    var legalEntites = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Totvs.Dtos.Person.LegalEntity.Root>(_response);

                    if (pageCount == 1)
                        pageCount = legalEntites.totalPages;

                    pageIndex++;

                    itens.AddRange(legalEntites.items);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            var cached = await _cache.GetOrCreateAsync(LegalEntitiesCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var list = await _personRepository.GetExistingLegalEntities(
                    itens.Select(x => x.code).ToList()
                );

                return list
                    .Where(x => x != null)
                    .GroupBy(x => x.code)
                    .ToDictionary(g => g.Key, g => g.First());
            });

            itens.RemoveAll(x =>
               cached.TryGetValue(x.code, out var y) &&
               ConverterValidatorsExtensions.ConvertToDateTimeValidation<string?>(x.maxChangeFilterDate) <= DateTime.Parse(y.maxChangeFilterDate, new CultureInfo("en-US"))
            );

            foreach (var legalEntity in itens)
            {
                var validations = _legalEntityValidator.Validate(legalEntity);

                if (validations.Errors.Count() > 0)
                {
                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        _logger.AddMessage(
                            message: $"Error when convert record - legalEntityCode: {legalEntity.code} | Name: {legalEntity.name}\n" +
                                     $"{validations.Errors[j]}"
                        );
                    }
                }

                legalEntitesList.Add(new LegalEntity(legalEntity));
            }

            if (legalEntitesList.Count() > 0)
            {
                await _personRepository.BulkInsertIntoTableLegalEntitiesRaw(legalEntitesList, _logger.GetExecutionGuid());

                foreach (var item in itens.Where(x => x != null))
                    cached[item.code] = item;

                legalEntitesList.ForEach(p =>
                    _logger.AddRecord(
                        p.code.ToString(),
                        p.Responses
                            .Where(pair => pair.Key == p.code.ToString())
                            .Select(pair => pair.Value)
                            .FirstOrDefault()
                    )
                );

                _logger.AddMessage(
                    $"Concluída com sucesso: {legalEntitesList.Count()} registro(s) novo(s) inserido(s)!"
                );
            }
            else
                _logger.AddMessage(
                    $"Concluída com sucesso: {legalEntitesList.Count()} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool?> GetBranchesList()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsBranches);

            var branchesList = new List<Branch>();
            var itens = new List<Domain.Totvs.Dtos.Person.Branch.Branch>();
            var somenteNovosList = new List<Domain.Totvs.Dtos.Person.Branch.Branch>();

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

            var _response = await _apiClient.GetAsync(
                route: "/person/v2/branchesList",
                headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
            );

            try
            {
                var branches = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Totvs.Dtos.Person.Branch.Root>(_response);

                itens.AddRange(branches.items);
            }
            catch (Exception)
            {
                throw;
            }

            var cached = await _cache.GetOrCreateAsync(BranchesCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var list = await _personRepository.GetExistingBranches(
                    itens.Select(x => x.code).ToList()
                );

                return list
                    .Where(x => x != null)
                    .GroupBy(x => x.code)
                    .ToDictionary(g => g.Key, g => g.First());
            });

            itens.RemoveAll(x =>
               cached.TryGetValue(x.code, out var y) &&
               ConverterValidatorsExtensions.ConvertToDateTimeValidation<string?>(x.maxChangeFilterDate) <= DateTime.Parse(y.maxChangeFilterDate, new CultureInfo("en-US"))
            );

            foreach (var branch in itens)
            {
                var validations = _branchValidator.Validate(branch);

                if (validations.Errors.Count() > 0)
                {
                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        _logger.AddMessage(
                            message: $"Error when convert record - branchCode: {branch.code} | Name: {branch.personName}\n" +
                                     $"{validations.Errors[j]}"
                        );
                    }
                }

                branchesList.Add(new Domain.Totvs.Entities.Person.Branch.Branch(branch));
            }

            if (branchesList.Count() > 0)
            {
                await _personRepository.BulkInsertIntoTableBranchesRaw(branchesList, _logger.GetExecutionGuid());

                foreach (var item in itens.Where(x => x != null))
                    cached[item.code] = item;

                _logger.AddMessage(
                    $"Concluída com sucesso: {branchesList.Count()} registro(s) novo(s) inserido(s)!"
                );
            }
            else
                _logger.AddMessage(
                    $"Concluída com sucesso: {branchesList.Count()} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
}
}

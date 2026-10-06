using Application.AfterSale.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.AfterSale.Dtos.Reverses.CourierAttributes;
using Domain.AfterSale.Entities.Reverses.CourierAttributes;
using Domain.AfterSale.Entities.Reverses.Transportations;
using Domain.AfterSale.Enums;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Extensions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Application.AfterSale.Services
{
    public class AfterSaleReversesService : IAfterSaleReversesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IAfterSaleReversesRepository _afterSaleReversesRepository;
        private readonly IAfterSaleRepository _afterSaleRepository;

        private readonly IValidator<Domain.AfterSale.Dtos.Reverses.SearchReverses.Reverse> _summarizedReversesValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reverses.GetReverse.Reverse> _reversesValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reverses.Status.Status> _statusValidator;
        private readonly IValidator<string> _transportationsValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reverses.CourierAttributes.CourierAttribute> _courierAttributeValidator;

        private const string ReversesKey = "AfterSale:Reverses";
        private const string SummarizedReversesKey = "AfterSale:SummarizedReverses";
        private const string StatusReversesKey = "AfterSale:StatusReverses";
        private const string TransportationsReversesKey = "AfterSale:TransportationsReverses";
        private const string CourierAttributesKey = "AfterSale:CourierAttributes";

        public AfterSaleReversesService(
            [FromKeyedServices("AfterSaleAPI")] IApiClient apiClient,
            ILoggerService logger,
            IAfterSaleReversesRepository afterSaleReversesRepository,
            IAfterSaleRepository afterSaleRepository,
            IValidator<Domain.AfterSale.Dtos.Reverses.SearchReverses.Reverse> summarizedReversesValidator,
            IValidator<Domain.AfterSale.Dtos.Reverses.GetReverse.Reverse> reversesValidator,
            IValidator<Domain.AfterSale.Dtos.Reverses.Status.Status> statusValidator,
            IValidator<string> transportationsValidator,
            IValidator<Domain.AfterSale.Dtos.Reverses.CourierAttributes.CourierAttribute> courierAttributeValidator,
            IMemoryCache cache
        ) =>
        (
            _apiClient,
            _logger,
            _afterSaleRepository,
            _afterSaleReversesRepository,
            _summarizedReversesValidator,
            _reversesValidator,
            _statusValidator,
            _transportationsValidator,
            _courierAttributeValidator,
            _cache
        )
            =
        (
            apiClient,
            logger,
            afterSaleRepository,
            afterSaleReversesRepository,
            summarizedReversesValidator,
            reversesValidator,
            statusValidator,
            transportationsValidator,
            courierAttributeValidator,
            cache
        );

        /// <summary>
        /// returns all reverses summarized
        /// </summary>
        public async Task<bool?> GetReverses()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleReverses);

            var listResponses = new List<string>();
            var companys = await _afterSaleRepository.GetCompanys();
            var dictionarysDocCompanyReverses = new Dictionary<string, List<Domain.AfterSale.Dtos.Reverses.SearchReverses.Reverse>>();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();
            var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.SearchReverses.Reverse>();

            foreach (var company in companys)
            {
                var summarizedReverses = new List<Domain.AfterSale.Dtos.Reverses.SearchReverses.Reverse>();

                var parameters = new Dictionary<string, string>
                {
                    //{ "start_date", $"{DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")}" },
                    //{ "end_date", $"{DateTime.Now.ToString("yyyy-MM-dd")}" }

                    { "start_date", $"{DateTime.Now.AddMonths(-1).ToString()}" },
                    { "end_date", $"{DateTime.Now.ToString()}" }
                };

                var encodedParameters = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();

                var response = await _apiClient.GetAsync(
                    route: $"v3/api/reverses?{encodedParameters}",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var page = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.SearchReverses.Root>(response);

                if (!page.success)
                {
                    _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                        company.doc_company,
                        EnumJob.AfterSaleReverses,
                        _logger.GetExecutionGuid(),
                        page.error,
                        page.error_code,
                        page.message,
                        page.message_translated
                    ));
                    continue;
                }

                int contador = 0;
                string? rote = page.next_page_url;
                summarizedReverses.AddRange(page.data);
                listResponses.Add(response);

                if (page.last_page > 1)
                {
                    while (!String.IsNullOrEmpty(rote))
                    {
                        //API da AfterSale lança Too Many Requests após 30 consultas, por isso caso tenha dado erro na consuta das páginas aguardamos por 1 minuto
                        if (contador > 14)
                        {
                            Thread.Sleep(60 * 1000);
                            contador = 0;
                        }

                        var responseByPage = await _apiClient.GetAsync(
                            route: rote.Replace("https://api.send4.com.br/", ""),
                            headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                        );

                        var nextPage = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.SearchReverses.Root>(responseByPage);

                        if (!nextPage.success)
                        {
                            _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                                company.doc_company,
                                EnumJob.AfterSaleReverses,
                                _logger.GetExecutionGuid(),
                                nextPage.error,
                                nextPage.error_code,
                                nextPage.message,
                                nextPage.message_translated
                            ));
                            continue;
                        }

                        contador++;
                        rote = nextPage.next_page_url;
                        summarizedReverses.AddRange(nextPage.data);
                        listResponses.Add(responseByPage);
                        Thread.Sleep(2 * 1000);

                        continue;
                    }
                }

                dictionarysDocCompanyReverses.Add(company.doc_company, summarizedReverses);
            }

            var summarizedReversesCache = await _cache.GetOrCreateAsync(SummarizedReversesKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var list = await _afterSaleReversesRepository.GetSummarizedReversesByIds(
                    //concatena as duas listas de reversas visto que reverse id não replica entre os cnpjs
                    dictionarysDocCompanyReverses.Values.SelectMany(x => x).Select(x => x.id).ToList()
                );

                return list
                    .Where(x => x != null)
                    .GroupBy(x => x.id)
                    .ToDictionary(g => g.Key, g => g.First()); // Dictionary<long, Entities...SearchReverses.Reverse>
            });

            foreach (var dictionary in dictionarysDocCompanyReverses)
            {
                var summarizedReversesList = dictionary.Value;
                var cnpj_emp = dictionary.Key;

                summarizedReversesList.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.id) &&
                   summarizedReversesCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), out var y) &&
                       ConverterValidatorsExtensions.ConvertToDateTimeValidation(x.updated_at) <= y.updated_at
                );

                if (summarizedReversesList.Count() > 0)
                {
                    foreach (var simplifiedReverse in summarizedReversesList)
                    {
                        var validations = _summarizedReversesValidator.Validate(simplifiedReverse);

                        if (validations.Errors.Count() > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s =>
                                message += $"{s.ErrorMessage}\n"
                            );

                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.SearchReverses.Reverse(cnpj_emp, simplifiedReverse));
                    }
                }
            }

            if (_listSomenteNovos.Count() > 0)
            {
                await _afterSaleReversesRepository.BulkInsertIntoAfterSaleSummarizedReverses(_listSomenteNovos, _logger.GetExecutionGuid());
                await _afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors(_listErrors, _logger.GetExecutionGuid());

                foreach (var record in _listSomenteNovos)
                {
                    JToken? reverse = null;

                    foreach (var page in listResponses)
                    {
                        var data = JObject.Parse(page)["data"] as JArray;
                        reverse = data?.FirstOrDefault(r => (long?)r["id"] == record.id);
                        if (reverse != null) break;
                    }

                    if (reverse != null)
                    {
                        _logger.AddRecord(record.id.ToString(), reverse.ToString());
                        summarizedReversesCache[record.id] = record;
                    }
                }
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// get one reverse
        /// </summary>
        /// <param name="id"></param>
        public async Task<bool?> GetReverse()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleReverse);

            var companys = await _afterSaleRepository.GetCompanys();
            var listResponses = new List<string>();
            var summarizedReverses = await _afterSaleReversesRepository.GetSummarizedReversesIdsForGetReverse();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();
            var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.GetReverse.Reverse>();
            var reversesDtosList = new List<Domain.AfterSale.Dtos.Reverses.GetReverse.Root>();

            if (summarizedReverses.Count() > 0)
            {
                foreach (var summarizedReverse in summarizedReverses)
                {
                    var response = await _apiClient.GetAsync(
                        route: $"v3/api/reverses/{summarizedReverse}",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {companys.Where(x => x.doc_company == summarizedReverse.cnpj_emp).Select(x => x.Token).First()}" } }
                    );

                    if (String.IsNullOrEmpty(response))
                        continue;

                    var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.GetReverse.Root>(response);

                    if (!root.success)
                    {
                        _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                            companys.Where(x => x.doc_company == summarizedReverse.cnpj_emp).Select(x => x.doc_company).First(),
                            EnumJob.AfterSaleReverse,
                            _logger.GetExecutionGuid(),
                            summarizedReverse.id,
                            root.error,
                            root.error_code,
                            root.message,
                            root.message_translated
                        ));
                        continue;
                    }

                    listResponses.Add(response);
                    reversesDtosList.Add(root);
                }

                var reversesCache = await _cache.GetOrCreateAsync(ReversesKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetReversesByIds(
                        reversesDtosList.Select(x => x.data.reverse.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x.id)
                        .ToDictionary(g => g.Key, g => g.First()); // Dictionary<long, Entities...Reverse>
                });

                reversesDtosList.RemoveAll(x =>
                   x != null &&
                   reversesCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.data.reverse.id), out var y) &&
                       ConverterValidatorsExtensions.ConvertToDateTimeValidation(x.data.reverse.updated_at) <= y.updated_at
                );

                foreach (var reversesDto in reversesDtosList)
                {
                    var validations = _reversesValidator.Validate(reversesDto.data.reverse);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.GetReverse.Reverse(reversesDto.data.reverse, reversesDto.data.customer, reversesDto.data.tracking_history));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleReverses(_listSomenteNovos, _logger.GetExecutionGuid());
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors(_listErrors, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        JToken? reverse = null;

                        foreach (var page in listResponses)
                        {
                            var data = JObject.Parse(page)["data"]?["reverse"];
                            reverse = data?.FirstOrDefault(r => (long?)r["id"] == record.id);
                            if (reverse != null) break;
                        }

                        if (reverse != null)
                        {
                            _logger.AddRecord(record.id.ToString(), reverse.ToString());
                            reversesCache[record.id] = record;
                        }
                    }
                }
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// get courier data fields needed to process the reverse
        /// </summary>
        public async Task<bool?> GetCourierAttributes()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleCourierAttributes);

            var companys = await _afterSaleRepository.GetCompanys();
            var listResponses = new List<string>();
            var summarizedReverses = await _afterSaleReversesRepository.GetSummarizedReversesIdsForGetCourierAttributes();
            var courierAttributes = new List<Domain.AfterSale.Dtos.Reverses.CourierAttributes.CourierAttribute>();
            var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.CourierAttributes.CourierAttributes>();

            foreach (var summarizedReverse in summarizedReverses)
            {
                foreach (var company in companys)
                {
                    var parameters = new Dictionary<string, string>
                    {
                        { "id", $"{summarizedReverse}" }
                    };

                    var encodedParameters = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();

                    var response = await _apiClient.GetAsync(
                        route: $"/v3/api/reverses?{encodedParameters}/get-courier-attributes",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );

                    if (String.IsNullOrEmpty(response))
                        continue;

                    var courierAttribute = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.CourierAttributes.CourierAttribute>(response);

                    courierAttributes.Add(courierAttribute);
                    listResponses.Add(response);
                }
            }

            var courierAttributesCache = await _cache.GetOrCreateAsync(CourierAttributesKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var list = await _afterSaleReversesRepository.GetCourierAttributesByReverseId(
                    summarizedReverses.Select(x => x).ToList()
                );

                return list
                    .Where(x => x != null)
                    .GroupBy(x => x.reverse_id)
                    .ToDictionary(g => g.Key, g => g.First()); // Dictionary<long, Entities...Reverse>
            });

            //courierAttributes.RemoveAll(x =>
            //   x.reverse.id != null &&
            //   courierAttributesCache.TryGetValue(
            //       ConverterValidatorsExtensions.ConvertToInt64Validation(x.reverse.id), out var y) &&
            //       x. <= y.updated_at
            //);

            foreach (var courierAttribute in courierAttributes)
            {
                var validations = _courierAttributeValidator.Validate(courierAttribute);

                if (validations.Errors.Count() > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s =>
                        message += $"{s.ErrorMessage}\n"
                    );

                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.CourierAttributes.CourierAttributes(courierAttribute));
            }

            if (_listSomenteNovos.Count() > 0)
            {
                //await _afterSaleReversesRepository.BulkInsertIntoAfterSaleC(_listSomenteNovos, _logger.GetExecutionGuid());

                //_listSomenteNovos.ForEach(x =>
                //{
                //    _logger.AddRecord(x.id.ToString(), x.response);
                //});
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// returns the possible status of a reverse.
        /// currently, the status codes are the same for both companies, 
        /// so we only provide the token for one of them.
        /// </summary>
        public async Task<bool?> GetStatus()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleStatusReverses);

            var listResponses = new List<string>();
            var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.Status.Status>();
            var companys = await _afterSaleRepository.GetCompanys();
            var statusReverses = new List<Domain.AfterSale.Dtos.Reverses.Status.Status>();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/reverses/status",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var status = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.Status.Root>(response);
                statusReverses.AddRange(status.data);
                listResponses.Add(response);

                var statusReversesCache = await _cache.GetOrCreateAsync(StatusReversesKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetStatus(/*company.doc_company*/);

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x.id)
                        .ToDictionary(g => g.Key, g => g.First()); // Dictionary<long, Entities...Status>
                });

                statusReverses.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.id) &&
                   statusReversesCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt32Validation(x.id), out var y) &&
                       x.name != y.name &&
                       x.description != y.description
                );

                //statusReverses.RemoveAll(x =>
                //   !string.IsNullOrEmpty(x.id) &&
                //   statusReversesCache.TryGetValue(
                //       ConverterValidatorsExtensions.ConvertToInt32Validation(x.id), out var y) &&
                //       x.doc_company != y.doc_company &&
                //       x.name != y.name &&
                //       x.description != y.description
                //);

                foreach (var statusReverse in statusReverses)
                {
                    var validations = _statusValidator.Validate(statusReverse);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.Status.Status(statusReverse/*, company.doc_company*/));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleStatusReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        JToken? reverse = null;

                        foreach (var page in listResponses)
                        {
                            var data = JObject.Parse(page)["data"] as JArray;
                            reverse = data?.FirstOrDefault(r => (long?)r["id"] == record.id);
                            if (reverse != null) break;
                        }

                        if (reverse != null)
                        {
                            _logger.AddRecord(record.id.ToString(), reverse.ToString());
                            statusReversesCache[record.id] = record;
                        }
                    }
                }

                //If the status codes differ between companies, comment out this `break` so that the `foreach` loop iterates through all companies.
                //Don't forget to add the `doc_company` column to the `AfterSaleStatusReverses` tables and add the `doc_company` property to the `Status` entity.
                //Also comment out the RemoveAll method from cache
                break;
            }

            _logger.AddMessage(
                    $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// returns the possible transportations
        /// </summary>
        public async Task<bool?> GetTransportations()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleTransportationsReverses);

            var listResponses = new List<string>();
            var companys = await _afterSaleRepository.GetCompanys();
            var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reverses.Transportations.Transportation>();
            var transportationsReverses = new List<string>();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                        route: "v3/api/transportations",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reverses.Transportations.Root>(response);
                transportationsReverses.AddRange(root.data);
                listResponses.Add(response);

                var transportationsReversesCache = await _cache.GetOrCreateAsync(TransportationsReversesKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetTransportations(/*company.doc_company*/);

                    return list
                        .Where(x => x != null)
                        .Select(x => x.transportation)
                        .ToList(); // List<string>

                    //return list
                    //    .Where(x => x != null)
                    //    .GroupBy(x => x.doc_company)
                    //    .ToDictionary(g => g.Key, g => g.First()); // Dictionary<long, Entities...Transportation>
                });

                transportationsReverses.RemoveAll(
                    x => transportationsReversesCache.Contains(x)
                );

                //transportationsReversesCache.RemoveAll(x =>
                //   !string.IsNullOrEmpty(x.doc_company) &&
                //   transportationsReversesCache.TryGetValue(
                //       x.transportation, out var y) &&
                //       x.doc_company != y.doc_company &&
                //       x.transportation != y.transportation
                //);

                foreach (var transportationRoot in transportationsReverses)
                {
                    var validations = _transportationsValidator.Validate(transportationRoot);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reverses.Transportations.Transportation(transportationRoot/*, company.doc_company*/));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleTransportationsReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        JArray? data = null;

                        foreach (var _response in listResponses)
                        {
                            var _root = JObject.Parse(_response);
                            data = _root["data"] as JArray;
                            if (data == null) continue;
                        }

                        if (data != null)
                        {
                            _logger.AddRecord(record.transportation, data.Select(t => (string?)t).FirstOrDefault(s => string.Equals(s, record.transportation, StringComparison.OrdinalIgnoreCase)));
                            transportationsReversesCache.Add(record.transportation);
                        }
                    }
                }

                //If the status codes differ between companies, comment out this `break` so that the `foreach` loop iterates through all companies.
                //Don't forget to add the `doc_company` column to the `AfterSaleStatusReverses` tables and add the `doc_company` property to the `Status` entity.
                //Also switch the return value, and comment out the RemoveAll method from cache
                break;
            }

            _logger.AddMessage(
                    $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// receive products by reverse ID
        /// </summary>
        /// <param name="id"></param>
        public Task<bool?> PutReciveProductsById(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// update recived invoice in reverse products. Need pass the products id and the returned invoice
        /// </summary>
        /// <param name="id"></param>
        public Task<bool?> PutReturnedInvoiceById(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// set reverse courier data and process/generate code
        /// </summary>
        /// <param name="id"></param>
        public Task<bool?> PostReverseInvoiceById(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// approve products by reverse ID
        /// </summary>
        /// <param name="id"></param>
        public Task<bool?> PostApproveReverseById(long id)
        {
            throw new NotImplementedException();
        }
    }
}

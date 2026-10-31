using Application.AfterSale.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes;
using Domain.AfterSale.Entities.Parameters;
using Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes;
using Domain.AfterSale.Entities.Reads.Reverses.Transportations;
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
using Application.Shared.Attributes;

namespace Application.AfterSale.Services
{
    public class AfterSaleReversesService : IAfterSaleReversesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IAfterSaleReversesRepository _afterSaleReversesRepository;
        private readonly IAfterSaleRepository _afterSaleRepository;
        private readonly IAfterSaleService _afterSaleService;

        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Reverses.SearchReverses.Reverse> _summarizedReversesValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Reverses.GetReverse.Reverse> _reversesValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Reverses.Status.Status> _statusValidator;
        private readonly IValidator<string> _transportationsValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes.CourierAttribute> _courierAttributeValidator;

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
            IAfterSaleService afterSaleService,
            IValidator<Domain.AfterSale.Dtos.Reads.Reverses.SearchReverses.Reverse> summarizedReversesValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Reverses.GetReverse.Reverse> reversesValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Reverses.Status.Status> statusValidator,
            IValidator<string> transportationsValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes.CourierAttribute> courierAttributeValidator,
            IMemoryCache cache
        ) =>
        (
            _apiClient,
            _logger,
            _afterSaleRepository,
            _afterSaleService,
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
            afterSaleService,
            afterSaleReversesRepository,
            summarizedReversesValidator,
            reversesValidator,
            statusValidator,
            transportationsValidator,
            courierAttributeValidator,
            cache
        );

        /// <summary>
        /// returns all reverses summarized, for each company
        /// </summary>
        [JobVersion("1.0.0")]
        public async Task<bool?> GetReverses()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleReverses);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();

            foreach (var company in companys)
            {
                var summarizedReverses = new List<Domain.AfterSale.Dtos.Reads.Reverses.SearchReverses.Reverse>();
                var listResponses = new List<string>();

                var parameters = new Dictionary<string, string>
                {
                    //{ "start_date", $"{DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")}" },
                    //{ "end_date", $"{DateTime.Now.ToString("yyyy-MM-dd")}" }

                    { "start_date", $"{DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")}" },
                    { "end_date", $"{DateTime.Now.ToString("yyyy-MM-dd")}" }
                };

                var encodedParameters = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();

                var response = await _apiClient.GetAsync(
                    route: $"v3/api/reverses?{encodedParameters}",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var page = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Reverses.SearchReverses.Root>(response);

                if (page is null || !page.success)
                {
                    _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                        company.doc_company,
                        EnumJob.AfterSaleReverses,
                        _logger.GetExecutionGuid(),
                        page?.error,
                        page?.error_code,
                        page?.message,
                        page?.message_translated
                    ));
                    continue;
                }

                int contador = 0;
                string? rote = page.next_page_url;
                summarizedReverses.AddRange(page.data ?? []);
                listResponses.Add(response);

                while (page.last_page > 1 && !String.IsNullOrEmpty(rote))
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

                    var nextPage = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Reverses.SearchReverses.Root>(responseByPage);

                    if (nextPage is null || !nextPage.success)
                    {
                        _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                            company.doc_company,
                            EnumJob.AfterSaleReverses,
                            _logger.GetExecutionGuid(),
                            nextPage?.error,
                            nextPage?.error_code,
                            nextPage?.message,
                            nextPage?.message_translated
                        ));
                        break;
                    }

                    contador++;
                    rote = nextPage.next_page_url;
                    summarizedReverses.AddRange(nextPage.data ?? []);
                    listResponses.Add(responseByPage);
                    Thread.Sleep(2 * 1000);
                }

                if (summarizedReverses.Count == 0)
                    continue;

                var summarizedReversesCache = await _cache.GetOrCreateAsync($"{SummarizedReversesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetSummarizedReversesByIds(
                        company.doc_company,
                        summarizedReverses.Select(x => x.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...SearchReverses.Reverse>
                });

                summarizedReverses.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.id) &&
                   summarizedReversesCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), out var y) &&
                       NullableConverters.ToNullableDateTime(x.updated_at) <= y.updated_at
                );

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse>();

                foreach (var summarizedReverse in summarizedReverses)
                {
                    var validations = _summarizedReversesValidator.Validate(summarizedReverse);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse(company.doc_company, summarizedReverse));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleSummarizedReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    // índice id -> json da reversa, montado só com as páginas desta empresa
                    var reversesJson = new Dictionary<long, string>();

                    foreach (var pageResponse in listResponses)
                    {
                        foreach (var item in (JObject.Parse(pageResponse)["data"] as JArray) ?? [])
                        {
                            var id = (long?)item["id"];

                            if (id != null)
                                reversesJson[id.Value] = item.ToString();
                        }
                    }

                    foreach (var record in _listSomenteNovos)
                    {
                        if (reversesJson.TryGetValue(record.id, out var json))
                            _logger.AddRecord(record.id.ToString(), json);

                        summarizedReversesCache[record.id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            if (_listErrors.Count > 0)
                await _afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors(_listErrors, _logger.GetExecutionGuid());

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// get the details of the summarized reverses not yet integrated, for each company
        /// </summary>
        [JobVersion("1.0.0")]
        public async Task<bool?> GetReverse()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleReverse);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var summarizedReverses = await _afterSaleReversesRepository.GetSummarizedReversesIdsForGetReverse();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();

            foreach (var reversesByCompany in summarizedReverses.Where(x => x != null).GroupBy(x => x!.doc_company))
            {
                var company = companys.FirstOrDefault(x => x.doc_company == reversesByCompany.Key);

                if (company is null)
                {
                    _logger.AddMessage($"CNPJ {reversesByCompany.Key} não encontrado em AfterSaleParameters: {reversesByCompany.Count()} reversa(s) ignorada(s).\n");
                    continue;
                }

                var reversesDtosList = new List<Domain.AfterSale.Dtos.Reads.Reverses.GetReverse.Root>();
                var reversesJson = new Dictionary<long, string>();

                foreach (var summarizedReverse in reversesByCompany)
                {
                    var response = await _apiClient.GetAsync(
                        route: $"v3/api/reverses/{summarizedReverse!.id}",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );

                    if (String.IsNullOrEmpty(response))
                        continue;

                    var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Reverses.GetReverse.Root>(response);

                    if (root is null || !root.success || root.data?.reverse is null)
                    {
                        _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                            company.doc_company,
                            EnumJob.AfterSaleReverse,
                            _logger.GetExecutionGuid(),
                            summarizedReverse.id,
                            root?.error,
                            root?.error_code,
                            root?.message,
                            root?.message_translated
                        ));
                        continue;
                    }

                    reversesDtosList.Add(root);
                    reversesJson[summarizedReverse.id] = JObject.Parse(response)["data"]?["reverse"]?.ToString() ?? response;
                }

                if (reversesDtosList.Count == 0)
                    continue;

                var reversesCache = await _cache.GetOrCreateAsync($"{ReversesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetReversesByIds(
                        company.doc_company,
                        reversesDtosList.Select(x => x.data.reverse.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...Reverse>
                });

                reversesDtosList.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.data.reverse.id) &&
                   reversesCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.data.reverse.id), out var y) &&
                       NullableConverters.ToNullableDateTime(x.data.reverse.updated_at) <= y.updated_at
                );

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse>();

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

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse(company.doc_company, reversesDto.data.reverse, reversesDto.data.customer, reversesDto.data.tracking_history));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        if (reversesJson.TryGetValue(record.id, out var json))
                            _logger.AddRecord(record.id.ToString(), json);

                        reversesCache[record.id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            if (_listErrors.Count > 0)
                await _afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors(_listErrors, _logger.GetExecutionGuid());

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// busca uma reversa por id, grava nas tabelas de reversas (sempre, sem consultar o cache)
        /// e registra a chamada em aftersale.AfterSaleRequests
        /// </summary>
        public async Task<bool?> GetReverseById(string user_name, string doc_company, long id)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleGetReverseById);

            var route = $"v3/api/reverses/{id}";
            bool? result;

            try
            {
                var company = (await _afterSaleRepository.GetCompanys()).FirstOrDefault(x => x?.doc_company == doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em AfterSaleRequests
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em aftersale.AfterSaleParameters.");
                    return null;
                }

                string? response;

                try
                {
                    response = await _apiClient.GetAsync(
                        route: route,
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _afterSaleService.SaveRequest(_logger.GetExecutionGuid(), EnumJob.AfterSaleGetReverseById, user_name, "GET", route, doc_company, id, null, $"Exceção ao chamar a API: {ex.Message}", false);
                    throw;
                }

                Domain.AfterSale.Dtos.Reads.Reverses.GetReverse.Root? root = null;

                try
                {
                    root = String.IsNullOrWhiteSpace(response) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Reverses.GetReverse.Root>(response);
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    // root continua null: tratado abaixo como resposta inválida
                }

                if (root is null)
                {
                    _logger.AddMessage($"Resposta inválida da API AfterSale para a reversa {id} (empresa {doc_company}): {response}");
                    result = null;
                }
                else if (!root.success)
                {
                    // reversa inexistente nesta empresa, ou recusada pela API
                    _logger.AddMessage($"Reversa {id} não encontrada na AfterSale (empresa {doc_company}): {root.message_translated ?? root.message ?? root.error}");

                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors(
                        [
                            new Domain.AfterSale.Entities.Errors.Error(
                                doc_company,
                                EnumJob.AfterSaleGetReverseById,
                                _logger.GetExecutionGuid(),
                                id,
                                root.error,
                                root.error_code,
                                root.message,
                                root.message_translated
                            )
                        ],
                        _logger.GetExecutionGuid()
                    );

                    result = false;
                }
                else if (root.data?.reverse is null || ConverterValidatorsExtensions.ConvertToInt64Validation(root.data.reverse.id) != id)
                {
                    _logger.AddMessage($"Resposta inválida da API AfterSale para a reversa {id} (empresa {doc_company}): {response}");
                    result = null;
                }
                else
                {
                    var validations = _reversesValidator.Validate(root.data.reverse);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    var reverse = new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse(doc_company, root.data.reverse, root.data.customer, root.data.tracking_history);

                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleReverses([reverse], _logger.GetExecutionGuid());

                    _logger.AddRecord(id.ToString(), JObject.Parse(response!)["data"]?["reverse"]?.ToString() ?? response!);

                    // atualiza o cache do GetReverse, se ele estiver carregado, para a próxima execução não regravar a reversa
                    if (_cache.TryGetValue($"{ReversesKey}:{doc_company}", out Dictionary<long, Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse>? reversesCache) && reversesCache is not null)
                        reversesCache[id] = reverse;

                    _logger.AddMessage($"Concluída com sucesso: reversa {id} inserida ou atualizada!");
                    result = true;
                }

                _afterSaleService.SaveRequest(_logger.GetExecutionGuid(), EnumJob.AfterSaleGetReverseById, user_name, "GET", route, doc_company, id, null, response, result == true);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// get courier data fields needed to process the reverse, for each company
        /// </summary>
        [JobVersion("1.0.0")]
        public async Task<bool?> GetCourierAttributes()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleCourierAttributes);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var summarizedReverses = await _afterSaleReversesRepository.GetSummarizedReversesIdsForGetCourierAttributes();

            foreach (var reversesByCompany in summarizedReverses.Where(x => x != null).GroupBy(x => x!.doc_company))
            {
                var company = companys.FirstOrDefault(x => x.doc_company == reversesByCompany.Key);

                if (company is null)
                {
                    _logger.AddMessage($"CNPJ {reversesByCompany.Key} não encontrado em AfterSaleParameters: {reversesByCompany.Count()} reversa(s) ignorada(s).\n");
                    continue;
                }

                // reverse_id consultado -> dto; a API pode devolver reverse.id = 0, então o id vem da consulta
                var courierAttributes = new Dictionary<long, Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes.CourierAttribute>();
                var courierAttributesJson = new Dictionary<long, string>();

                foreach (var summarizedReverse in reversesByCompany)
                {
                    var response = await _apiClient.GetAsync(
                        route: $"v3/api/reverses/{summarizedReverse!.id}/get-courier-attributes",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );

                    if (String.IsNullOrEmpty(response))
                        continue;

                    var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes.Root>(response);

                    if (root is null || !root.success || root.data is null)
                    {
                        _logger.AddMessage($"Reversa {summarizedReverse.id}: resposta sem sucesso ao consultar courier attributes.\n");
                        continue;
                    }

                    courierAttributes[summarizedReverse.id] = root.data;
                    courierAttributesJson[summarizedReverse.id] = JObject.Parse(response)["data"]?.ToString() ?? response;
                }

                if (courierAttributes.Count == 0)
                    continue;

                var courierAttributesCache = await _cache.GetOrCreateAsync($"{CourierAttributesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetCourierAttributesByReverseId(
                        company.doc_company,
                        reversesByCompany.Select(x => x!.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.reverse_id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...CourierAttributes>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes.CourierAttributes>();

                foreach (var (reverseId, courierAttribute) in courierAttributes)
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

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes.CourierAttributes(company.doc_company, reverseId, courierAttribute));
                }

                // mantém só as reversas que ainda não têm courier attributes gravados para a empresa
                _listSomenteNovos.RemoveAll(x => courierAttributesCache.ContainsKey(x.reverse_id));

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleCourierAttributes(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        if (courierAttributesJson.TryGetValue(record.reverse_id, out var json))
                            _logger.AddRecord(record.reverse_id.ToString(), json);

                        courierAttributesCache[record.reverse_id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        ///returns the possible status of a reverse for each company.
        /// </summary>
        [JobVersion("1.0.0")]
        public async Task<bool?> GetStatus()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleStatusReverses);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/reverses/status",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Reverses.Status.Root>(response);
                var statusReverses = root?.data ?? [];

                foreach (var statusReverse in statusReverses)
                    statusReverse.doc_company = company.doc_company;

                var statusReversesCache = await _cache.GetOrCreateAsync($"{StatusReversesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetStatus(company.doc_company);

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<int, Entities...Status>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Reverses.Status.Status>();

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

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Reverses.Status.Status(statusReverse, statusReverse.doc_company));
                }

                // mantém só o que é novo ou foi alterado (remove quando já existe no cache e está igual)
                _listSomenteNovos.RemoveAll(x =>
                    statusReversesCache.TryGetValue(x.id, out var y) &&
                    x.doc_company == y.doc_company &&
                    x.name == y.name &&
                    x.description == y.description
                );

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleStatusReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    var data = JObject.Parse(response)["data"] as JArray;

                    foreach (var record in _listSomenteNovos)
                    {
                        var reverse = data?.FirstOrDefault(r => (int?)r["id"] == record.id);

                        if (reverse != null)
                            _logger.AddRecord(record.id.ToString(), reverse.ToString());

                        statusReversesCache[record.id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            _logger.AddMessage(
                    $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// returns the possible transportations for each company.
        /// </summary>
        [JobVersion("1.0.0")]
        public async Task<bool?> GetTransportations()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleTransportationsReverses);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                        route: "v3/api/transportations",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Reverses.Transportations.Root>(response);
                var transportationsReverses = (root?.data ?? [])
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var transportationsReversesCache = await _cache.GetOrCreateAsync($"{TransportationsReversesKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleReversesRepository.GetTransportations(company.doc_company);

                    return list
                        .Where(x => x != null && !string.IsNullOrEmpty(x.transportation))
                        .Select(x => x!.transportation)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase); // HashSet<string> com as transportadoras já gravadas da empresa
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Reverses.Transportations.Transportation>();

                foreach (var transportationReverse in transportationsReverses)
                {
                    var validations = _transportationsValidator.Validate(transportationReverse);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Reverses.Transportations.Transportation(transportationReverse, company.doc_company));
                }

                // mantém só o que é novo (remove quando a transportadora já existe no cache da empresa)
                _listSomenteNovos.RemoveAll(x => transportationsReversesCache.Contains(x.transportation));

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleTransportationsReverses(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        _logger.AddRecord(record.transportation, record.transportation);
                        transportationsReversesCache.Add(record.transportation);
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            _logger.AddMessage(
                    $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// receive products by reverse ID
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Response.Root> PutReciveProductsById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Request.Root request)
        {
            var body = ToJsonBody(request);
            var route = $"v3/api/reverses/{id}/receive-products";

            return SendReverseAction<Domain.AfterSale.Dtos.Writes.Reverses.ReceiveProducts.Response.Root>(EnumJob.AfterSaleReciveProducts, user_name, doc_company, id, "PUT", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PutAsync(route, body, headers));
        }

        /// <summary>
        /// update returned invoice in reverse products. Need pass the products id and the returned invoice
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.ReturnedInvoice.Response.Root> PutReturnedInvoiceById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.ReturnedInvoice.Request.Root request)
        {
            var body = ToJsonBody(request);
            var route = $"v3/api/reverses/{id}/products/returned-invoice";

            return SendReverseAction<Domain.AfterSale.Dtos.Writes.Reverses.ReturnedInvoice.Response.Root>(EnumJob.AfterSaleReturnedInvoice, user_name, doc_company, id, "PUT", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PutAsync(route, body, headers));
        }

        /// <summary>
        /// set reverse courier data (invoice fields) and process/generate code
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Response.Root> PostReverseInvoiceById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Request.Root request)
        {
            var body = ToJsonBody(request);
            body["id"] = id;
            var route = $"v3/api/reverses/{id}/invoice";

            return SendReverseAction<Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Response.Root>(EnumJob.AfterSaleReverseInvoice, user_name, doc_company, id, "POST", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostAsync(route, body, headers));
        }

        /// <summary>
        /// set reverse courier data (invoice XML file) and process/generate code.
        /// the whole XML is logged as the request body.
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Response.Root> PostReverseInvoiceXmlById(string user_name, string doc_company, long id, byte[] xml, string fileName)
        {
            var route = $"v3/api/reverses/{id}/invoice";

            return SendReverseAction<Domain.AfterSale.Dtos.Writes.Reverses.Invoice.Response.Root>(EnumJob.AfterSaleReverseInvoice, user_name, doc_company, id, "POST", route, Encoding.UTF8.GetString(xml),
                headers => _apiClient.PostMultipartAsync(
                    route: route,
                    fields: new Dictionary<string, string> { { "id", id.ToString() } },
                    fileField: "xml",
                    fileContent: xml,
                    fileName: fileName,
                    fileContentType: "application/xml",
                    headers: headers
                ));
        }

        /// <summary>
        /// approve products by reverse ID
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.Approve.Response.Root> PostApproveReverseById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.Approve.Request.Root request)
        {
            var body = ToJsonBody(request);
            var route = $"v3/api/reverses/{id}/approve";

            return SendReverseAction<Domain.AfterSale.Dtos.Writes.Reverses.Approve.Response.Root>(EnumJob.AfterSaleApproveReverse, user_name, doc_company, id, "POST", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostAsync(route, body, headers));
        }

        /// <summary>
        /// create an administrative record (note) in the reverse
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Reverses.AdministrativeRecords.Response.Root> PostAdministrativeRecordById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Reverses.AdministrativeRecords.Request.Root request)
        {
            var body = ToJsonBody(request);
            var route = $"v3/api/reverses/{id}/administrative-records";

            return SendReverseAction<Domain.AfterSale.Dtos.Writes.Reverses.AdministrativeRecords.Response.Root>(EnumJob.AfterSaleAdministrativeRecordsReverse, user_name, doc_company, id, "POST", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostAsync(route, body, headers));
        }

        // ------------------------------------------------------------------------------------------------------

        /// <summary>
        /// serializes the request ignoring null properties, so the API does not receive fields the caller did not fill
        /// </summary>
        private static JObject ToJsonBody(object request) =>
            JObject.FromObject(request, Newtonsoft.Json.JsonSerializer.Create(new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            }));

        /// <summary>
        /// common flow of the reverse write routes: resolves the company token, calls the API,
        /// logs the call in aftersale.AfterSaleRequests (user, request and response) and saves an
        /// AfterSaleErrors record when the API does not return success
        /// </summary>
        private async Task<TResponse> SendReverseAction<TResponse>(EnumJob job, string user_name, string doc_company, long id, string httpMethod, string route, string requestBody, Func<Dictionary<string, string>, Task<string?>> send)
            where TResponse : Domain.AfterSale.Dtos.Writes.Base.ResponseRoot, new()
        {
            _logger
               .Clear()
               .AddLog(job);

            TResponse result;
            string? response = null;

            try
            {
                var company = (await _afterSaleRepository.GetCompanys()).FirstOrDefault(x => x?.doc_company == doc_company);

                if (company is null)
                {
                    result = new TResponse { success = false, message = $"CNPJ {doc_company} não encontrado em AfterSaleParameters." };
                    _logger.AddMessage($"Reversa {id}: {result.message}");

                    return result;
                }

                try
                {
                    response = await send(new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } });
                }
                catch (Exception ex)
                {
                    //registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _afterSaleService.SaveRequest(_logger.GetExecutionGuid(), job, user_name, httpMethod, route, doc_company, id, requestBody, $"Exceção ao chamar a API: {ex.Message}", false);
                    throw;
                }

                JToken? responseJson = null;

                try
                {
                    responseJson = String.IsNullOrEmpty(response) ? null : JToken.Parse(response);
                }
                catch (Newtonsoft.Json.JsonReaderException) { }

                result = responseJson is JObject responseObject
                    ? responseObject.ToObject<TResponse>() ?? new TResponse()
                    : new TResponse
                    {
                        success = false,
                        message = "Resposta inválida da API.",
                        error = response?.Length > 800 ? response.Substring(0, 800) : response
                    };

                _afterSaleService.SaveRequest(_logger.GetExecutionGuid(), job, user_name, httpMethod, route, doc_company, id, requestBody, response, result.success);

                if (result.success)
                {
                    _logger.AddMessage($"Reversa {id}: concluída com sucesso. {result.message}".Trim());
                }
                else
                {
                    _logger.AddMessage($"Reversa {id}: {result.message_translated ?? result.message ?? result.error ?? "a API não retornou sucesso."}");

                    await _afterSaleReversesRepository.BulkInsertIntoAfterSaleErrors(
                        [
                            new Domain.AfterSale.Entities.Errors.Error(
                                company.doc_company,
                                job,
                                _logger.GetExecutionGuid(),
                                id,
                                result.error,
                                result.error_code,
                                result.message,
                                result.message_translated
                            )
                        ],
                        _logger.GetExecutionGuid()
                    );
                }

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }
    }
}

using Application.AfterSale.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.AfterSale.Enums;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Extensions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Services
{
    public class AfterSaleRefundsService : IAfterSaleRefundsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IAfterSaleRepository _afterSaleRepository;
        private readonly IAfterSaleService _afterSaleService;
        private readonly IAfterSaleRefundsRepository _afterSaleRefundsRepository;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Refund> _refundValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Refund> _summarizedRefundsValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Status.Status> _statusValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Actions.Action> _actionValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Banks.Bank> _bankValidator;
        private readonly IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Types.Type> _typeValidator;

        private const string GetRefundHashesKey = "AfterSale:GetRefundHashes";
        private const string SummarizedRefundsKey = "AfterSale:SummarizedRefunds";
        private const string RefundsKey = "AfterSale:Refunds";
        private const string StatusRefundsKey = "AfterSale:StatusRefunds";
        private const string ActionsRefundsKey = "AfterSale:ActionsRefunds";
        private const string BanksRefundsKey = "AfterSale:BanksRefunds";
        private const string TypesRefundsKey = "AfterSale:TypesRefunds";

        public AfterSaleRefundsService(
            [FromKeyedServices("AfterSaleAPI")] IApiClient apiClient,
            ILoggerService logger,
            IAfterSaleRepository afterSaleRepository,
            IAfterSaleService afterSaleService,
            IAfterSaleRefundsRepository afterSaleRefundsRepository,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Refund> refundValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Refund> summarizedRefundsValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Status.Status> statusValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Actions.Action> actionValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Banks.Bank> bankValidator,
            IValidator<Domain.AfterSale.Dtos.Reads.Refunds.Types.Type> typeValidator,
            IMemoryCache cache
        ) =>
        (_apiClient, _logger, _afterSaleRepository, _afterSaleService, _afterSaleRefundsRepository, _refundValidator, _summarizedRefundsValidator, _statusValidator, _actionValidator, _bankValidator, _typeValidator, _cache) =
        (apiClient, logger, afterSaleRepository, afterSaleService, afterSaleRefundsRepository, refundValidator, summarizedRefundsValidator, statusValidator, actionValidator, bankValidator, typeValidator, cache);

        /// <summary>
        /// returns all refunds summarized, for each company
        /// </summary>
        public async Task<bool?> GetRefunds()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleRefunds);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();

            foreach (var company in companys)
            {
                var summarizedRefunds = new List<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Refund>();
                var listResponses = new List<string>();

                var parameters = new Dictionary<string, string>
                {
                    { "start_date", $"{DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")}" },
                    { "end_date", $"{DateTime.Now.ToString("yyyy-MM-dd")}" }
                };

                var encodedParameters = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();

                var response = await _apiClient.GetAsync(
                    route: $"v3/api/refunds?{encodedParameters}",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var page = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Root>(response);

                if (page is null || !page.success)
                {
                    _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                        company.doc_company,
                        EnumJob.AfterSaleRefunds,
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
                summarizedRefunds.AddRange(page.data ?? []);
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

                    var nextPage = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds.Root>(responseByPage);

                    if (nextPage is null || !nextPage.success)
                    {
                        _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                            company.doc_company,
                            EnumJob.AfterSaleRefunds,
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
                    summarizedRefunds.AddRange(nextPage.data ?? []);
                    listResponses.Add(responseByPage);
                    Thread.Sleep(2 * 1000);
                }

                if (summarizedRefunds.Count == 0)
                    continue;

                var summarizedRefundsCache = await _cache.GetOrCreateAsync($"{SummarizedRefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetSummarizedRefundsByIds(
                        company.doc_company,
                        summarizedRefunds.Select(x => x.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...SearchRefunds.Refund>
                });

                summarizedRefunds.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.id) &&
                   summarizedRefundsCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), out var y) &&
                       NullableConverters.ToNullableDateTime(x.updated_at) <= y.updated_at
                );

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>();

                foreach (var summarizedRefund in summarizedRefunds)
                {
                    var validations = _summarizedRefundsValidator.Validate(summarizedRefund);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund(company.doc_company, summarizedRefund));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleSummarizedRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    // índice id -> json do reembolso, montado só com as páginas desta empresa
                    var refundsJson = new Dictionary<long, string>();

                    foreach (var pageResponse in listResponses)
                    {
                        foreach (var item in (JObject.Parse(pageResponse)["data"] as JArray) ?? [])
                        {
                            var id = (long?)item["id"];

                            if (id != null)
                                refundsJson[id.Value] = item.ToString();
                        }
                    }

                    foreach (var record in _listSomenteNovos)
                    {
                        if (refundsJson.TryGetValue(record.id, out var json))
                            _logger.AddRecord(record.id.ToString(), json);

                        summarizedRefundsCache[record.id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            if (_listErrors.Count > 0)
                await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleErrors(_listErrors, _logger.GetExecutionGuid());

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// returns the possible actions of a refund for each company.
        /// </summary>
        public async Task<bool?> GetActions()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleActionsRefunds);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/refunds/actions",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.Actions.Root>(response);
                var actionsRefunds = root?.data ?? [];

                foreach (var actionRefund in actionsRefunds)
                    actionRefund.doc_company = company.doc_company;

                var actionsRefundsCache = await _cache.GetOrCreateAsync($"{ActionsRefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetActions(company.doc_company);

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<int, Entities...Refunds.Actions.Action>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.Actions.Action>();

                foreach (var actionRefund in actionsRefunds)
                {
                    var validations = _actionValidator.Validate(actionRefund);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.Actions.Action(actionRefund, actionRefund.doc_company));
                }

                // mantém só o que é novo ou foi alterado (remove quando já existe no cache e está igual)
                _listSomenteNovos.RemoveAll(x =>
                    actionsRefundsCache.TryGetValue(x.id, out var y) &&
                    x.doc_company == y.doc_company &&
                    x.name == y.name &&
                    x.description == y.description
                );

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleActionsRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    var data = JObject.Parse(response)["data"] as JArray;

                    foreach (var record in _listSomenteNovos)
                    {
                        var item = data?.FirstOrDefault(r => (int?)r["id"] == record.id);

                        if (item != null)
                            _logger.AddRecord(record.id.ToString(), item.ToString());

                        actionsRefundsCache[record.id] = record;
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
        /// returns the possible banks of a refund for each company.
        /// </summary>
        public async Task<bool?> GetBanks()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleBanksRefunds);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/refunds/banks",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.Banks.Root>(response);
                var banksRefunds = root?.data ?? [];

                foreach (var bankRefund in banksRefunds)
                    bankRefund.doc_company = company.doc_company;

                var banksRefundsCache = await _cache.GetOrCreateAsync($"{BanksRefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetBanks(company.doc_company);

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<int, Entities...Refunds.Banks.Bank>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank>();

                foreach (var bankRefund in banksRefunds)
                {
                    var validations = _bankValidator.Validate(bankRefund);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank(bankRefund, bankRefund.doc_company));
                }

                // mantém só o que é novo ou foi alterado (remove quando já existe no cache e está igual)
                _listSomenteNovos.RemoveAll(x =>
                    banksRefundsCache.TryGetValue(x.id, out var y) &&
                    x.doc_company == y.doc_company &&
                    x.name == y.name &&
                    x.description == y.description
                );

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleBanksRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    var data = JObject.Parse(response)["data"] as JArray;

                    foreach (var record in _listSomenteNovos)
                    {
                        var item = data?.FirstOrDefault(r => (int?)r["id"] == record.id);

                        if (item != null)
                            _logger.AddRecord(record.id.ToString(), item.ToString());

                        banksRefundsCache[record.id] = record;
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
        /// get the details of the summarized refunds not yet integrated, for each company
        /// </summary>
        public async Task<bool?> GetRefund()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleRefund);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();
            var summarizedRefunds = await _afterSaleRefundsRepository.GetSummarizedRefundsIdsForGetRefund();
            var _listErrors = new List<Domain.AfterSale.Entities.Errors.Error>();

            foreach (var refundsByCompany in summarizedRefunds.Where(x => x != null).GroupBy(x => x!.doc_company))
            {
                var company = companys.FirstOrDefault(x => x?.doc_company == refundsByCompany.Key);

                if (company is null)
                {
                    _logger.AddMessage($"CNPJ {refundsByCompany.Key} não encontrado em AfterSaleParameters: {refundsByCompany.Count()} reembolso(s) ignorado(s).\n");
                    continue;
                }

                var refundsDtosList = new List<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Root>();
                var refundsJson = new Dictionary<long, string>();

                foreach (var summarizedRefund in refundsByCompany)
                {
                    var response = await _apiClient.GetAsync(
                        route: $"v3/api/refunds/{summarizedRefund!.id}",
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                    );

                    if (String.IsNullOrEmpty(response))
                        continue;

                    var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Root>(response);

                    if (root is null || !root.success || root.data is null)
                    {
                        _listErrors.Add(new Domain.AfterSale.Entities.Errors.Error(
                            company.doc_company,
                            EnumJob.AfterSaleRefund,
                            _logger.GetExecutionGuid(),
                            summarizedRefund.id,
                            root?.error,
                            root?.error_code,
                            root?.message,
                            root?.message_translated
                        ));
                        continue;
                    }

                    refundsDtosList.Add(root);
                    refundsJson[summarizedRefund.id] = JObject.Parse(response)["data"]?.ToString() ?? response;
                }

                if (refundsDtosList.Count == 0)
                    continue;

                var refundsCache = await _cache.GetOrCreateAsync($"{RefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetRefundsByIds(
                        company.doc_company,
                        refundsDtosList.Select(x => x.data.id).ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<long, Entities...GetRefund.Refund>
                });

                refundsDtosList.RemoveAll(x =>
                   !string.IsNullOrEmpty(x.data.id) &&
                   refundsCache.TryGetValue(
                       ConverterValidatorsExtensions.ConvertToInt64Validation(x.data.id), out var y) &&
                       NullableConverters.ToNullableDateTime(x.data.updated_at) <= y.updated_at
                );

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund>();

                foreach (var refundsDto in refundsDtosList)
                {
                    var validations = _refundValidator.Validate(refundsDto.data);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund(company.doc_company, refundsDto.data));
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        if (refundsJson.TryGetValue(record.id, out var json))
                            _logger.AddRecord(record.id.ToString(), json);

                        refundsCache[record.id] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
                }
            }

            if (_listErrors.Count > 0)
                await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleErrors(_listErrors, _logger.GetExecutionGuid());

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// busca um reembolso por id, grava nas tabelas de reembolsos (sempre, sem consultar cache)
        /// e registra a chamada em general.AfterSaleRequests
        /// </summary>
        public async Task<bool?> GetRefundById(string user_name, string doc_company, long id)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleGetRefundById);

            var route = $"v3/api/refunds/{id}";
            bool? result;

            try
            {
                var company = (await _afterSaleRepository.GetCompanys()).FirstOrDefault(x => x?.doc_company == doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em AfterSaleRequests
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em general.AfterSaleParameters.");
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
                    _afterSaleService.SaveRequest(_logger.GetExecutionGuid(), EnumJob.AfterSaleGetRefundById, user_name, "GET", route, doc_company, id, null, $"Exceção ao chamar a API: {ex.Message}", false);
                    throw;
                }

                Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Root? root = null;

                try
                {
                    root = String.IsNullOrWhiteSpace(response) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.GetRefund.Root>(response);
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    // root continua null: tratado abaixo como resposta inválida
                }

                if (root is null)
                {
                    _logger.AddMessage($"Resposta inválida da API AfterSale para o reembolso {id} (empresa {doc_company}): {response}");
                    result = null;
                }
                else if (!root.success)
                {
                    // reembolso inexistente nesta empresa, ou recusado pela API
                    _logger.AddMessage($"Reembolso {id} não encontrado na AfterSale (empresa {doc_company}): {root.message_translated ?? root.message ?? root.error}");

                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleErrors(
                        [
                            new Domain.AfterSale.Entities.Errors.Error(
                                doc_company,
                                EnumJob.AfterSaleGetRefundById,
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
                else if (root.data is null || ConverterValidatorsExtensions.ConvertToInt64Validation(root.data.id) != id)
                {
                    _logger.AddMessage($"Resposta inválida da API AfterSale para o reembolso {id} (empresa {doc_company}): {response}");
                    result = null;
                }
                else
                {
                    var validations = _refundValidator.Validate(root.data);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    var refund = new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund(doc_company, root.data);

                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleRefunds([refund], _logger.GetExecutionGuid());

                    // atualiza o cache do GetRefund, se ele estiver carregado, para a próxima execução não regravar o reembolso
                    if (_cache.TryGetValue($"{RefundsKey}:{doc_company}", out Dictionary<long, Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund>? refundsCache) && refundsCache is not null)
                        refundsCache[id] = refund;

                    _logger.AddRecord(id.ToString(), JObject.Parse(response!)["data"]?.ToString() ?? response!);

                    _logger.AddMessage($"Concluída com sucesso: reembolso {id} inserido ou atualizado!");
                    result = true;
                }

                _afterSaleService.SaveRequest(_logger.GetExecutionGuid(), EnumJob.AfterSaleGetRefundById, user_name, "GET", route, doc_company, id, null, response, result == true);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// returns the possible status of a refund for each company.
        /// </summary>
        public async Task<bool?> GetStatus()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleStatusRefunds);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/refunds/status",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.Status.Root>(response);
                var statusRefunds = root?.data ?? [];

                foreach (var statusRefund in statusRefunds)
                    statusRefund.doc_company = company.doc_company;

                var statusRefundsCache = await _cache.GetOrCreateAsync($"{StatusRefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetStatus(company.doc_company);

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!); // Dictionary<int, Entities...Refunds.Status.Status>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.Status.Status>();

                foreach (var statusRefund in statusRefunds)
                {
                    var validations = _statusValidator.Validate(statusRefund);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.Status.Status(statusRefund, statusRefund.doc_company));
                }

                // mantém só o que é novo ou foi alterado (remove quando já existe no cache e está igual)
                _listSomenteNovos.RemoveAll(x =>
                    statusRefundsCache.TryGetValue(x.id, out var y) &&
                    x.doc_company == y.doc_company &&
                    x.name == y.name &&
                    x.description == y.description
                );

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleStatusRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    var data = JObject.Parse(response)["data"] as JArray;

                    foreach (var record in _listSomenteNovos)
                    {
                        var item = data?.FirstOrDefault(r => (int?)r["id"] == record.id);

                        if (item != null)
                            _logger.AddRecord(record.id.ToString(), item.ToString());

                        statusRefundsCache[record.id] = record;
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
        /// returns the possible types of a refund for each company. the API does not return an id, so the name is the key.
        /// </summary>
        public async Task<bool?> GetTypes()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.AfterSaleTypesRefunds);

            var totalInseridos = 0;
            var companys = await _afterSaleRepository.GetCompanys();

            foreach (var company in companys)
            {
                var response = await _apiClient.GetAsync(
                    route: "v3/api/refunds/types",
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {company.Token}" } }
                );

                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.AfterSale.Dtos.Reads.Refunds.Types.Root>(response);
                var typesRefunds = (root?.data ?? [])
                    .Where(x => !string.IsNullOrWhiteSpace(x?.name))
                    .ToList();

                foreach (var typeRefund in typesRefunds)
                    typeRefund.doc_company = company.doc_company;

                var typesRefundsCache = await _cache.GetOrCreateAsync($"{TypesRefundsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _afterSaleRefundsRepository.GetTypes(company.doc_company);

                    return list
                        .Where(x => x != null && !string.IsNullOrEmpty(x.name))
                        .GroupBy(x => x!.name!, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(g => g.Key, g => g.First()!, StringComparer.OrdinalIgnoreCase); // Dictionary<string name, Entities...Refunds.Types.Type>
                });

                var _listSomenteNovos = new List<Domain.AfterSale.Entities.Reads.Refunds.Types.Type>();

                foreach (var typeRefund in typesRefunds)
                {
                    var validations = _typeValidator.Validate(typeRefund);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = String.Empty;
                        validations.Errors.ForEach(s =>
                            message += $"{s.ErrorMessage}\n"
                        );

                        _logger.AddMessage(message);
                    }

                    _listSomenteNovos.Add(new Domain.AfterSale.Entities.Reads.Refunds.Types.Type(typeRefund, typeRefund.doc_company));
                }

                // mantém só o que é novo ou foi alterado (remove quando já existe no cache e está igual)
                _listSomenteNovos.RemoveAll(x =>
                    typesRefundsCache.TryGetValue(x.name!, out var y) &&
                    x.doc_company == y.doc_company &&
                    x.description == y.description
                );

                if (_listSomenteNovos.Count() > 0)
                {
                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleTypesRefunds(_listSomenteNovos, _logger.GetExecutionGuid());

                    var data = JObject.Parse(response)["data"] as JArray;

                    foreach (var record in _listSomenteNovos)
                    {
                        var item = data?.FirstOrDefault(r => string.Equals((string?)r["name"], record.name, StringComparison.OrdinalIgnoreCase));

                        if (item != null)
                            _logger.AddRecord(record.name!, item.ToString());

                        typesRefundsCache[record.name!] = record;
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
        /// approve refund by ID
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Approve.Response.Root> PostApproveRefundById(string user_name, string doc_company, long id)
        {
            var body = new JObject();
            var route = $"v3/api/refunds/{id}/approve";

            return SendRefundAction<Domain.AfterSale.Dtos.Writes.Refunds.Approve.Response.Root>(EnumJob.AfterSaleApproveRefund, user_name, doc_company, id, "POST", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostAsync(route, body, headers));
        }

        /// <summary>
        /// cancel refund by ID
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Cancel.Response.Root> PostCancelRefundById(string user_name, string doc_company, long id)
        {
            var body = new JObject();
            var route = $"v3/api/refunds/{id}/cancel";

            return SendRefundAction<Domain.AfterSale.Dtos.Writes.Refunds.Cancel.Response.Root>(EnumJob.AfterSaleCancelRefund, user_name, doc_company, id, "POST", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostAsync(route, body, headers));
        }

        /// <summary>
        /// pay refund by ID (multipart/form-data), with the payment attachment (required in our endpoint).
        /// the whole file is logged in the request body: as text when it is XML/text, as base64 when it is binary (PDF, image).
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.Pay.Response.Root> PostPayRefundById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Refunds.Pay.Request.Root request, byte[] file, string fileName, string fileContentType)
        {
            var route = $"v3/api/refunds/{id}/pay";

            var fields = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(request.voucher_redemption_code))
                fields["voucher_redemption_code"] = request.voucher_redemption_code;

            if (!string.IsNullOrEmpty(request.observation))
                fields["observation"] = request.observation;

            //arquivo gravado inteiro, como no InvoiceXml: texto quando é XML/texto, base64 quando é binário
            var isText = fileContentType.Contains("xml", StringComparison.OrdinalIgnoreCase) ||
                fileContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase);

            var requestLog = JObject.FromObject(fields);
            requestLog["file"] = new JObject
            {
                ["name"] = fileName,
                ["content_type"] = fileContentType,
                ["size"] = file.Length,
                ["encoding"] = isText ? "text" : "base64",
                ["content"] = isText ? Encoding.UTF8.GetString(file) : Convert.ToBase64String(file)
            };

            return SendRefundAction<Domain.AfterSale.Dtos.Writes.Refunds.Pay.Response.Root>(EnumJob.AfterSalePayRefund, user_name, doc_company, id, "POST", route, requestLog.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostMultipartAsync(
                    route: route,
                    fields: fields,
                    fileField: "file",
                    fileContent: file,
                    fileName: fileName,
                    fileContentType: string.IsNullOrEmpty(fileContentType) ? "application/octet-stream" : fileContentType,
                    headers: headers
                ));
        }

        /// <summary>
        /// change total amount of refund by ID
        /// </summary>
        public Task<Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Response.Root> PostChangeTotalAmountRefundById(string user_name, string doc_company, long id, Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Request.Root request)
        {
            var body = ToJsonBody(request);
            var route = $"v3/api/refunds/{id}/change-total-amount";

            return SendRefundAction<Domain.AfterSale.Dtos.Writes.Refunds.ChangeTotalAmount.Response.Root>(EnumJob.AfterSaleChangeTotalAmountRefund, user_name, doc_company, id, "POST", route, body.ToString(Newtonsoft.Json.Formatting.None),
                headers => _apiClient.PostAsync(route, body, headers));
        }

        /// <summary>
        /// serializes the request ignoring null properties, so the API does not receive fields the caller did not fill
        /// </summary>
        private static JObject ToJsonBody(object request) =>
            JObject.FromObject(request, Newtonsoft.Json.JsonSerializer.Create(new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            }));

        /// <summary>
        /// common flow of the refund write routes: resolves the company token, calls the API,
        /// logs the call in general.AfterSaleRequests (user, request and response) and saves an
        /// AfterSaleErrors record when the API does not return success
        /// </summary>
        private async Task<TResponse> SendRefundAction<TResponse>(EnumJob job, string user_name, string doc_company, long id, string httpMethod, string route, string requestBody, Func<Dictionary<string, string>, Task<string?>> send)
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
                    _logger.AddMessage($"Reembolso {id}: {result.message}");

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
                    _logger.AddMessage($"Reembolso {id}: concluído com sucesso. {result.message}".Trim());
                }
                else
                {
                    _logger.AddMessage($"Reembolso {id}: {result.message_translated ?? result.message ?? result.error ?? "a API não retornou sucesso."}");

                    await _afterSaleRefundsRepository.BulkInsertIntoAfterSaleErrors(
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

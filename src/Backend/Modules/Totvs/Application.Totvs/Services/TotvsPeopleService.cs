using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Extensions;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Dtos.Reads.Base;
using Domain.Totvs.Enums;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Application.Totvs.Services
{
    /// <summary>
    /// pessoas da Totvs (API Person): filiais, pessoas físicas e jurídicas. Cada método faz as próprias chamadas à API;
    /// o comum (horário do lote, parâmetros, lojas e token) vem do ITotvsService
    /// </summary>
    public class TotvsPeopleService : ITotvsPeopleService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsPeopleRepository _totvsPeopleRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.Branches.Branch> _branchValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.Customers.Customer> _customerValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.LegalEntities.LegalEntity> _legalEntityValidator;

        private const string BranchesKey = "Totvs:Branches";
        private const string CustomersKey = "Totvs:Customers";
        private const string LegalEntitiesKey = "Totvs:LegalEntities";

        // máximo da rota (swagger da API Person); a holding tem 25 empresas, então é uma página só
        private const int BranchesPageSize = 1000;

        // máximo das buscas de pessoas (swagger da API Person): 500 pessoas físicas com o expand completo levaram 46 s (testado)
        private const int PeoplePageSize = 500;

        // fatias da carga inicial: desde 2000, mas as 188 mil pessoas físicas têm alteração em 2026 (testado); 30 dias evitam
        // milhares de chamadas vazias e mantêm cada fatia gravável sozinha
        private const int PeopleWindowDays = 30;

        // expand da documentação da PF; preferences não está na lista, mas vem (testado)
        private const string CustomersExpand = "phones,addresses,emails,classifications,additionalFields,references,observations,relateds,shippingCompany,statistics," +
            "representatives,customerObservations,familiars,individualAdditionals,preferences";

        // expand da documentação da PJ (o código antigo mandava o do produto, e endereços, telefones e contatos nunca vinham)
        private const string LegalEntitiesExpand = "phones,addresses,emails,classifications,additionalFields,references,observations,relateds,partners,shippingCompany," +
            "contacts,statistics,paymentMethods,preferences,socialNetworks,representatives,customerObservations";

        public TotvsPeopleService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsPeopleRepository totvsPeopleRepository,
            IValidator<Domain.Totvs.Dtos.Reads.People.Branches.Branch> branchValidator,
            IValidator<Domain.Totvs.Dtos.Reads.People.Customers.Customer> customerValidator,
            IValidator<Domain.Totvs.Dtos.Reads.People.LegalEntities.LegalEntity> legalEntityValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsPeopleRepository, _branchValidator, _customerValidator, _legalEntityValidator) =
            (apiClient, logger, cache, totvsService, totvsPeopleRepository, branchValidator, customerValidator, legalEntityValidator);

        public async Task<bool?> GetBranchesList()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetBranchesList);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: carga desde 2000
                    var lastChange = await _totvsPeopleRepository.GetBranchesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var branches = new List<Domain.Totvs.Dtos.Reads.People.Branches.Branch>();
                    var branchesJson = new Dictionary<int, string>();
                    var page = 1;
                    var totalPages = 1;
                    var failed = false;

                    while (page <= totalPages)
                    {
                        var query = new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Expand", "addresses" },   // sem ele os endereços vêm nulos (testado)
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", BranchesPageSize.ToString() }
                        };

                        // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                        ApiResponse? response = null;

                        for (var attempt = 1; attempt <= 2; attempt++)
                        {
                            var token = await _totvsService.GetToken(company);

                            if (token is null)
                                break;   // a mensagem do login já foi para o log

                            response = await _apiClient.SendAsync(
                                HttpMethod.Get,
                                "person/v2/branchesList",
                                headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } },
                                queryParams: query
                            );

                            if (attempt == 1 && response.StatusCode == 401)
                            {
                                _totvsService.RemoveToken(company);
                                continue;
                            }

                            if (attempt == 1 && response.StatusCode == 429)
                            {
                                var seconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var value) ? value : 5;
                                await Task.Delay(TimeSpan.FromSeconds(Math.Clamp(seconds, 1, 60)));
                                continue;
                            }

                            if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(10));
                                continue;
                            }

                            break;
                        }

                        if (response is null || !response.IsSuccess)
                        {
                            if (response is not null)
                            {
                                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                _logger.AddMessage($"Erro da API Totvs em person/v2/branchesList (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
                            }

                            failed = true;
                            break;
                        }

                        Page<Domain.Totvs.Dtos.Reads.People.Branches.Branch>? result = null;

                        try
                        {
                            result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.Branches.Branch>>(response.Content ?? String.Empty);

                            foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                            {
                                var code = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["code"]);

                                if (code != 0)
                                    branchesJson[code] = item.ToString(Formatting.None);
                            }
                        }
                        catch (JsonException)
                        {
                            // result continua null: tratado abaixo como resposta inválida
                        }

                        if (result?.items is null)
                        {
                            var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                            _logger.AddMessage($"Resposta inválida da API Totvs em person/v2/branchesList (empresa {company.doc_company}, página {page}): {body}");

                            failed = true;
                            break;
                        }

                        branches.AddRange(result.items);

                        totalPages = result.totalPages;
                        page++;
                    }

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (failed || branches.Count == 0)
                        continue;

                    // cache de estado: código -> filial gravada (com o maxChangeFilterDate), por empresa
                    var branchesCache = await _cache.GetOrCreateAsync($"{BranchesKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsPeopleRepository.GetBranchesByCodes(
                            company.doc_company!,
                            branches.Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code)).Where(x => x != 0).Distinct().ToList()
                        );

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.code)
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só as novas ou com maxChangeFilterDate mais recente que a gravada (sem data, regrava)
                    branches.RemoveAll(x =>
                        branchesCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.code), out var y) &&
                        NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    totalInseridos += await SaveBranches(company.doc_company!, batchTime, branches, branchesJson, branchesCache);
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

        public async Task<bool?> PostSearchCustomers()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchCustomers);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // as estatísticas só vêm com option.branchStaticDataList (testado): todas as lojas da holding
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    var endDate = _totvsService.GetBatchTime();

                    // as datas da Totvs são do horário local, sem fuso (o "Z" é ignorado pela API, testado). Banco vazio: desde 2000
                    var lastChange = await _totvsPeopleRepository.GetCustomersLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    // a carga inicial é lida em fatias (188 mil pessoas físicas na carga inicial, testado), e cada fatia é gravada ao terminar:
                    // uma falha da API interrompe só o resto, e a próxima execução continua do MAX(maxChangeFilterDate)
                    for (var windowStart = since; windowStart < endDate; windowStart = windowStart.AddDays(PeopleWindowDays))
                    {
                        var windowEnd = windowStart.AddDays(PeopleWindowDays) < endDate ? windowStart.AddDays(PeopleWindowDays) : endDate;

                        var people = new List<Domain.Totvs.Dtos.Reads.People.Customers.Customer>();
                        var peopleJson = new Dictionary<int, string>();
                        var page = 1;
                        var totalPages = 1;
                        var failed = false;

                        while (page <= totalPages)
                        {
                            var requestBody = JsonConvert.SerializeObject(new
                            {
                                filter = new
                                {
                                    change = new
                                    {
                                        startDate = $"{windowStart:yyyy-MM-ddTHH:mm:ss}",
                                        endDate = $"{windowEnd:yyyy-MM-ddTHH:mm:ss}",
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
                                option = new
                                {
                                    branchStaticDataList = branchCodes
                                },
                                order = "maxChangeFilterDate",
                                page,
                                pageSize = PeoplePageSize,
                                expand = CustomersExpand
                            });

                            // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                var token = await _totvsService.GetToken(company);

                                if (token is null)
                                    break;   // a mensagem do login já foi para o log

                                using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                                response = await _apiClient.SendAsync(
                                    HttpMethod.Post,
                                    "person/v2/individuals/search",
                                    content,
                                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } }
                                );

                                if (attempt == 1 && response.StatusCode == 401)
                                {
                                    _totvsService.RemoveToken(company);
                                    continue;
                                }

                                if (attempt == 1 && response.StatusCode == 429)
                                {
                                    var seconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var value) ? value : 5;
                                    await Task.Delay(TimeSpan.FromSeconds(Math.Clamp(seconds, 1, 60)));
                                    continue;
                                }

                                if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                    continue;
                                }

                                break;
                            }

                            if (response is null || !response.IsSuccess)
                            {
                                if (response is not null)
                                {
                                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                    _logger.AddMessage($"Erro da API Totvs em person/v2/individuals/search (empresa {company.doc_company}, janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}, página {page}): {response.StatusCode} {body}");
                                }

                                failed = true;
                                break;
                            }

                            Page<Domain.Totvs.Dtos.Reads.People.Customers.Customer>? result = null;

                            try
                            {
                                result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.Customers.Customer>>(response.Content ?? String.Empty);

                                foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                                {
                                    var code = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["code"]);

                                    if (code != 0)
                                        peopleJson[code] = item.ToString(Formatting.None);
                                }
                            }
                            catch (JsonException)
                            {
                                // result continua null: tratado abaixo como resposta inválida
                            }

                            if (result?.items is null)
                            {
                                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                _logger.AddMessage($"Resposta inválida da API Totvs em person/v2/individuals/search (empresa {company.doc_company}, página {page}): {body}");

                                failed = true;
                                break;
                            }

                            people.AddRange(result.items);

                            totalPages = result.totalPages;
                            page++;
                        }

                        // fatia incompleta não é gravada, e as seguintes também não: a próxima execução continua do MAX(maxChangeFilterDate)
                        if (failed)
                            break;

                        if (people.Count == 0)
                            continue;

                        // cache de estado: código -> pessoa gravada (com o maxChangeFilterDate), por empresa
                        var peopleCache = await _cache.GetOrCreateAsync($"{CustomersKey}:{company.doc_company}", async entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                            var list = await _totvsPeopleRepository.GetCustomersByCodes(
                                company.doc_company!,
                                people.Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code)).Where(x => x != 0).Distinct().ToList()
                            );

                            return list
                                .Where(x => x != null)
                                .GroupBy(x => x!.code)
                                .ToDictionary(g => g.Key, g => g.First()!);
                        });

                        // mantém só as novas ou com maxChangeFilterDate mais recente que a gravada (sem data, regrava)
                        people.RemoveAll(x =>
                            peopleCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.code), out var y) &&
                            NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                        );

                        // um horário de lote por fatia: uma pessoa que volta noutra fatia precisa de horário maior para a exclusão das filhas
                        totalInseridos += await SaveCustomers(company.doc_company!, _totvsService.GetBatchTime(), people, peopleJson, peopleCache);
                    }
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

        public async Task<bool?> PostSearchLegalEntity()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchLegalEntity);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // as estatísticas só vêm com option.branchStaticDataList (testado): todas as lojas da holding
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    var endDate = _totvsService.GetBatchTime();

                    // as datas da Totvs são do horário local, sem fuso (o "Z" é ignorado pela API, testado). Banco vazio: desde 2000
                    var lastChange = await _totvsPeopleRepository.GetLegalEntitiesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    // a carga inicial é lida em fatias (2,2 mil pessoas jurídicas na carga inicial, testado), e cada fatia é gravada ao terminar:
                    // uma falha da API interrompe só o resto, e a próxima execução continua do MAX(maxChangeFilterDate)
                    for (var windowStart = since; windowStart < endDate; windowStart = windowStart.AddDays(PeopleWindowDays))
                    {
                        var windowEnd = windowStart.AddDays(PeopleWindowDays) < endDate ? windowStart.AddDays(PeopleWindowDays) : endDate;

                        var people = new List<Domain.Totvs.Dtos.Reads.People.LegalEntities.LegalEntity>();
                        var peopleJson = new Dictionary<int, string>();
                        var page = 1;
                        var totalPages = 1;
                        var failed = false;

                        while (page <= totalPages)
                        {
                            var requestBody = JsonConvert.SerializeObject(new
                            {
                                filter = new
                                {
                                    change = new
                                    {
                                        startDate = $"{windowStart:yyyy-MM-ddTHH:mm:ss}",
                                        endDate = $"{windowEnd:yyyy-MM-ddTHH:mm:ss}",
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
                                option = new
                                {
                                    branchStaticDataList = branchCodes
                                },
                                order = "maxChangeFilterDate",
                                page,
                                pageSize = PeoplePageSize,
                                expand = LegalEntitiesExpand
                            });

                            // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                var token = await _totvsService.GetToken(company);

                                if (token is null)
                                    break;   // a mensagem do login já foi para o log

                                using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                                response = await _apiClient.SendAsync(
                                    HttpMethod.Post,
                                    "person/v2/legal-entities/search",
                                    content,
                                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } }
                                );

                                if (attempt == 1 && response.StatusCode == 401)
                                {
                                    _totvsService.RemoveToken(company);
                                    continue;
                                }

                                if (attempt == 1 && response.StatusCode == 429)
                                {
                                    var seconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var value) ? value : 5;
                                    await Task.Delay(TimeSpan.FromSeconds(Math.Clamp(seconds, 1, 60)));
                                    continue;
                                }

                                if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                    continue;
                                }

                                break;
                            }

                            if (response is null || !response.IsSuccess)
                            {
                                if (response is not null)
                                {
                                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                    _logger.AddMessage($"Erro da API Totvs em person/v2/legal-entities/search (empresa {company.doc_company}, janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}, página {page}): {response.StatusCode} {body}");
                                }

                                failed = true;
                                break;
                            }

                            Page<Domain.Totvs.Dtos.Reads.People.LegalEntities.LegalEntity>? result = null;

                            try
                            {
                                result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.LegalEntities.LegalEntity>>(response.Content ?? String.Empty);

                                foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                                {
                                    var code = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["code"]);

                                    if (code != 0)
                                        peopleJson[code] = item.ToString(Formatting.None);
                                }
                            }
                            catch (JsonException)
                            {
                                // result continua null: tratado abaixo como resposta inválida
                            }

                            if (result?.items is null)
                            {
                                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                _logger.AddMessage($"Resposta inválida da API Totvs em person/v2/legal-entities/search (empresa {company.doc_company}, página {page}): {body}");

                                failed = true;
                                break;
                            }

                            people.AddRange(result.items);

                            totalPages = result.totalPages;
                            page++;
                        }

                        // fatia incompleta não é gravada, e as seguintes também não: a próxima execução continua do MAX(maxChangeFilterDate)
                        if (failed)
                            break;

                        if (people.Count == 0)
                            continue;

                        // cache de estado: código -> pessoa gravada (com o maxChangeFilterDate), por empresa
                        var peopleCache = await _cache.GetOrCreateAsync($"{LegalEntitiesKey}:{company.doc_company}", async entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                            var list = await _totvsPeopleRepository.GetLegalEntitiesByCodes(
                                company.doc_company!,
                                people.Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code)).Where(x => x != 0).Distinct().ToList()
                            );

                            return list
                                .Where(x => x != null)
                                .GroupBy(x => x!.code)
                                .ToDictionary(g => g.Key, g => g.First()!);
                        });

                        // mantém só as novas ou com maxChangeFilterDate mais recente que a gravada (sem data, regrava)
                        people.RemoveAll(x =>
                            peopleCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.code), out var y) &&
                            NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                        );

                        // um horário de lote por fatia: uma pessoa que volta noutra fatia precisa de horário maior para a exclusão das filhas
                        totalInseridos += await SaveLegalEntities(company.doc_company!, _totvsService.GetBatchTime(), people, peopleJson, peopleCache);
                    }
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
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de filiais gravadas.
        /// </summary>
        private async Task<int> SaveBranches(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.People.Branches.Branch> branches,
            IReadOnlyDictionary<int, string> branchesJson,
            Dictionary<int, Domain.Totvs.Entities.Reads.People.Branches.Branch>? branchesCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.People.Branches.Branch>();

            // a mesma filial pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var branch in branches.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).GroupBy(x => x.code).Select(g => g.Last()))
            {
                var validations = _branchValidator.Validate(branch);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"filial {branch.code}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.Branches.Branch(doc_company, batchTime, branch));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsPeopleRepository.BulkInsertIntoTotvsBranches(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (branchesJson.TryGetValue(record.code, out var json))
                    _logger.AddRecord(record.code.ToString(), json);

                if (branchesCache is not null)
                    branchesCache[record.code] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de pessoas físicas gravadas.
        /// </summary>
        private async Task<int> SaveCustomers(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.People.Customers.Customer> people,
            IReadOnlyDictionary<int, string> peopleJson,
            Dictionary<int, Domain.Totvs.Entities.Reads.People.Customers.Customer>? peopleCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.People.Customers.Customer>();

            // a mesma pessoa pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var person in people.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).GroupBy(x => x.code).Select(g => g.Last()))
            {
                var validations = _customerValidator.Validate(person);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"pessoa física {person.code}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.Customers.Customer(doc_company, batchTime, person));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsPeopleRepository.BulkInsertIntoTotvsPeople(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (peopleJson.TryGetValue(record.code, out var json))
                    _logger.AddRecord(record.code.ToString(), json);

                if (peopleCache is not null)
                    peopleCache[record.code] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de pessoas jurídicas gravadas.
        /// </summary>
        private async Task<int> SaveLegalEntities(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.People.LegalEntities.LegalEntity> people,
            IReadOnlyDictionary<int, string> peopleJson,
            Dictionary<int, Domain.Totvs.Entities.Reads.People.LegalEntities.LegalEntity>? peopleCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.People.LegalEntities.LegalEntity>();

            // a mesma pessoa pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var person in people.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).GroupBy(x => x.code).Select(g => g.Last()))
            {
                var validations = _legalEntityValidator.Validate(person);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"pessoa jurídica {person.code}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.LegalEntities.LegalEntity(doc_company, batchTime, person));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsPeopleRepository.BulkInsertIntoTotvsLegalEntities(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (peopleJson.TryGetValue(record.code, out var json))
                    _logger.AddRecord(record.code.ToString(), json);

                if (peopleCache is not null)
                    peopleCache[record.code] = record;
            }

            return _listSomenteNovos.Count;
        }
    }
}

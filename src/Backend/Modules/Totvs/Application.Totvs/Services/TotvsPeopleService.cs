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

        // grupo das escritas desta API na configuração (Totvs:Writes:People:Enabled, T4)
        private const string WritesGroup = "People";

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

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
                    // Devolve a quantidade de filiais gravadas.
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

                        _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.Branches.Branch(company.doc_company!, batchTime, branch));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _totvsPeopleRepository.BulkInsertIntoTotvsBranches(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        if (branchesJson.TryGetValue(record.code, out var json))
                            _logger.AddRecord(record.code.ToString(), json);

                        if (branchesCache is not null)
                            branchesCache[record.code] = record;
                    }

                    totalInseridos += _listSomenteNovos.Count;
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
        /// busca uma pessoa física por código, grava nas tabelas de pessoas (sempre, sem consultar o cache)
        /// e registra a chamada em totvs.TotvsRequests
        /// </summary>
        public async Task<bool?> GetCustomerByCode(string user_name, string doc_company, int code)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetCustomerByCode);

            // rota registrada no log de requisições (sem o token); o filtro vai no corpo
            const string route = "person/v2/individuals/search";
            bool? result;

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em TotvsRequests (igual ao Movidesk)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em totvs.TotvsParameters.");
                    return null;
                }

                // as estatísticas só vêm com option.branchStaticDataList: todas as lojas da holding (sem lojas, a pessoa vem sem estatísticas)
                var branchCodes = await _totvsService.GetBranchCodes(company);

                // o change desde 2000 (com as mesmas flags da busca por janela) é o que faz o maxChangeFilterDate vir preenchido (testado):
                // sem ele a pessoa seria regravada na próxima execução do job
                var requestBody = JsonConvert.SerializeObject(new
                {
                    filter = new
                    {
                        personCodeList = new[] { code },
                        change = new
                        {
                            startDate = "2000-01-01T00:00:00",
                            endDate = $"{_totvsService.GetBatchTime():yyyy-MM-ddTHH:mm:ss}",
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
                    option = new { branchStaticDataList = branchCodes },
                    page = 1,
                    pageSize = 1,
                    expand = CustomersExpand
                });

                // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                ApiResponse? response = null;

                try
                {
                    for (var attempt = 1; attempt <= 2; attempt++)
                    {
                        var token = await _totvsService.GetToken(company);

                        if (token is null)
                            break;   // a mensagem do login já foi para o log

                        using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                        response = await _apiClient.SendAsync(
                            HttpMethod.Post,
                            route,
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
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _totvsService.SaveRequest(EnumJob.TotvsGetCustomerByCode, user_name, "POST", route, doc_company, code.ToString(), $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                    throw;
                }

                if (response is null)
                {
                    // login falhou: nenhuma chamada à busca
                    _totvsService.SaveRequest(EnumJob.TotvsGetCustomerByCode, user_name, "POST", route, doc_company, code.ToString(), null, false, requestBody);
                    return null;
                }

                if (!response.IsSuccess)
                {
                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                    _logger.AddMessage($"Erro da API Totvs em {route} (empresa {doc_company}, pessoa física {code}): {response.StatusCode} {body}");

                    _totvsService.SaveRequest(EnumJob.TotvsGetCustomerByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, false, requestBody);
                    return null;
                }

                Page<Domain.Totvs.Dtos.Reads.People.Customers.Customer>? page = null;
                string? json = null;

                try
                {
                    page = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.Customers.Customer>>(response.Content ?? String.Empty);
                    json = ((JObject.Parse(response.Content!)["items"] as JArray) ?? []).FirstOrDefault()?.ToString(Formatting.None);
                }
                catch (JsonException)
                {
                    // page continua null: tratado abaixo como resposta inválida
                }

                var person = page?.items?.FirstOrDefault(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) == code);

                if (page?.items is null)
                {
                    _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {doc_company}, pessoa física {code}): {response.Content}");
                    result = null;
                }
                else if (person is null || json is null)
                {
                    // a busca responde 200 com items vazio quando o código não existe
                    _logger.AddMessage($"Pessoa física {code} não encontrada na Totvs (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    // atualiza o cache do PostSearchCustomers, se ele estiver carregado, para a próxima execução não regravar a pessoa
                    _cache.TryGetValue($"{CustomersKey}:{doc_company}", out Dictionary<int, Domain.Totvs.Entities.Reads.People.Customers.Customer>? peopleCache);

                    await SaveCustomers(doc_company, _totvsService.GetBatchTime(), [person], new Dictionary<int, string> { { code, json } }, peopleCache);

                    _logger.AddMessage($"Concluída com sucesso: pessoa física {code} inserida ou atualizada!");
                    result = true;
                }

                _totvsService.SaveRequest(EnumJob.TotvsGetCustomerByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, result == true, requestBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// busca uma pessoa jurídica por código, grava nas tabelas de pessoas (sempre, sem consultar o cache)
        /// e registra a chamada em totvs.TotvsRequests
        /// </summary>
        public async Task<bool?> GetLegalEntityByCode(string user_name, string doc_company, int code)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetLegalEntityByCode);

            // rota registrada no log de requisições (sem o token); o filtro vai no corpo
            const string route = "person/v2/legal-entities/search";
            bool? result;

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em TotvsRequests (igual ao Movidesk)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em totvs.TotvsParameters.");
                    return null;
                }

                // as estatísticas só vêm com option.branchStaticDataList: todas as lojas da holding (sem lojas, a pessoa vem sem estatísticas)
                var branchCodes = await _totvsService.GetBranchCodes(company);

                // o change desde 2000 (com as mesmas flags da busca por janela) é o que faz o maxChangeFilterDate vir preenchido (testado):
                // sem ele a pessoa seria regravada na próxima execução do job
                var requestBody = JsonConvert.SerializeObject(new
                {
                    filter = new
                    {
                        personCodeList = new[] { code },
                        change = new
                        {
                            startDate = "2000-01-01T00:00:00",
                            endDate = $"{_totvsService.GetBatchTime():yyyy-MM-ddTHH:mm:ss}",
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
                    option = new { branchStaticDataList = branchCodes },
                    page = 1,
                    pageSize = 1,
                    expand = LegalEntitiesExpand
                });

                // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s
                ApiResponse? response = null;

                try
                {
                    for (var attempt = 1; attempt <= 2; attempt++)
                    {
                        var token = await _totvsService.GetToken(company);

                        if (token is null)
                            break;   // a mensagem do login já foi para o log

                        using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                        response = await _apiClient.SendAsync(
                            HttpMethod.Post,
                            route,
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
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _totvsService.SaveRequest(EnumJob.TotvsGetLegalEntityByCode, user_name, "POST", route, doc_company, code.ToString(), $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                    throw;
                }

                if (response is null)
                {
                    // login falhou: nenhuma chamada à busca
                    _totvsService.SaveRequest(EnumJob.TotvsGetLegalEntityByCode, user_name, "POST", route, doc_company, code.ToString(), null, false, requestBody);
                    return null;
                }

                if (!response.IsSuccess)
                {
                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                    _logger.AddMessage($"Erro da API Totvs em {route} (empresa {doc_company}, pessoa jurídica {code}): {response.StatusCode} {body}");

                    _totvsService.SaveRequest(EnumJob.TotvsGetLegalEntityByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, false, requestBody);
                    return null;
                }

                Page<Domain.Totvs.Dtos.Reads.People.LegalEntities.LegalEntity>? page = null;
                string? json = null;

                try
                {
                    page = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.LegalEntities.LegalEntity>>(response.Content ?? String.Empty);
                    json = ((JObject.Parse(response.Content!)["items"] as JArray) ?? []).FirstOrDefault()?.ToString(Formatting.None);
                }
                catch (JsonException)
                {
                    // page continua null: tratado abaixo como resposta inválida
                }

                var person = page?.items?.FirstOrDefault(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) == code);

                if (page?.items is null)
                {
                    _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {doc_company}, pessoa jurídica {code}): {response.Content}");
                    result = null;
                }
                else if (person is null || json is null)
                {
                    // a busca responde 200 com items vazio quando o código não existe
                    _logger.AddMessage($"Pessoa jurídica {code} não encontrada na Totvs (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    // atualiza o cache do PostSearchLegalEntity, se ele estiver carregado, para a próxima execução não regravar a pessoa
                    _cache.TryGetValue($"{LegalEntitiesKey}:{doc_company}", out Dictionary<int, Domain.Totvs.Entities.Reads.People.LegalEntities.LegalEntity>? peopleCache);

                    await SaveLegalEntities(doc_company, _totvsService.GetBatchTime(), [person], new Dictionary<int, string> { { code, json } }, peopleCache);

                    _logger.AddMessage($"Concluída com sucesso: pessoa jurídica {code} inserida ou atualizada!");
                    result = true;
                }

                _totvsService.SaveRequest(EnumJob.TotvsGetLegalEntityByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, result == true, requestBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> GetBranches(string user_name, string doc_company, string branchId)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetBranches);

            var key = branchId;

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // devolve o corpo: TotvsBranches é mantida pelo job de 120 min, e o BranchOutDto não tem description, branchGroupName nem datas (PE1)
                return await _totvsService.ReadOnDemand(company, EnumJob.TotvsGetBranches, user_name, key, HttpMethod.Get, $"person/v2/branches/{branchId}", null, null);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> GetPersonStatistics(string user_name, string doc_company, int? customerCode, string? customerCpfCnpj, List<int>? branchCodes)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetPersonStatistics);

            var key = $"{(object?)customerCode ?? customerCpfCnpj}|{String.Join(",", branchCodes ?? [])}";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // BranchCode é obrigatório (400 EmptyField sem ele, testado): sem lojas informadas, vão todas as da empresa
                var companyBranches = await _totvsService.GetBranchCodes(company);

                if (branchCodes is null || branchCodes.Count == 0)
                {
                    branchCodes = companyBranches;
                    key = $"{(object?)customerCode ?? customerCpfCnpj}|{String.Join(",", branchCodes)}";
                }

                var outside = branchCodes.Where(x => !companyBranches.Contains(x)).ToList();

                if (outside.Count > 0)
                    return _totvsService.BranchNotFound(doc_company, key, outside.Select(x => x.ToString()));

                return await _totvsService.ReadOnDemand(company, EnumJob.TotvsGetPersonStatistics, user_name, key, HttpMethod.Get, TotvsQuery.Route("person/v2/person-statistics", ("CustomerCode", customerCode), ("CustomerCpfCnpj", customerCpfCnpj), ("BranchCode", branchCodes)), null, null);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostMessages(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Messages.InsertMessagePerson body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostMessages);

            var key = body.personCode?.ToString() ?? body.personCpfCnpj!;

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // não há leitura de mensagens na API: as sequence ficam só na resposta, guardada em TotvsRequests (PE8). Repetir inclui as
                // mensagens de novo, daí a trava (T5)
                return await _totvsService.Write(company, EnumJob.TotvsPostMessages, user_name, key, HttpMethod.Post, "person/v2/messages", body,
                    "Confira as mensagens da pessoa na Totvs antes de repetir.", repeatLock: true);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PutChangeMessages(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Messages.UpdateMessagePerson body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPutChangeMessages);

            var key = $"{body.filter!.personCode?.ToString() ?? body.filter.personCpfCnpj}|{body.filter.sequence}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // grava valores absolutos: repetir não muda nada, sem trava
                return await _totvsService.Write(company, EnumJob.TotvsPutChangeMessages, user_name, key, HttpMethod.Put, "person/v2/messages/change", body,
                    "Confira a mensagem na Totvs antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> DeleteMessages(string user_name, string doc_company, int personCode, int sequence)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsDeleteMessages);

            var key = $"{personCode}|{sequence}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // sem corpo: os campos vão na query (a rota gravada em TotvsRequests os leva). LastChangeUser não vai: é um código de usuário
                // da Totvs sem descrição no swagger (§3.4.4)
                var route = TotvsQuery.Route("person/v2/messages", ("PersonCode", personCode), ("Sequence", sequence));

                return await _totvsService.Write(company, EnumJob.TotvsDeleteMessages, user_name, key, HttpMethod.Delete, route, null,
                    "Confira a mensagem na Totvs antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostIndividualCustomers(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Customers.IndividualCustomer body) =>
            await WriteCustomer(user_name, doc_company, EnumJob.TotvsPostIndividualCustomers, "person/v2/individual-customers", body.cpf!, body.branchInsertCode!.Value,
                body.limits?.Select(x => x.branchCode!.Value), body);

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostLegalCustomers(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Customers.LegalCustomer body) =>
            await WriteCustomer(user_name, doc_company, EnumJob.TotvsPostLegalCustomers, "person/v2/legal-customers", body.cnpj!, body.branchInsertCode!.Value,
                body.limits?.Select(x => x.branchCode!.Value), body);

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostRemoveClassificationRelationship(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.People.Classifications.RemoveClassificationRelationship body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostRemoveClassificationRelationship);

            var key = body.personCode!.Value.ToString();

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                // tirar de novo o que já saiu não muda nada: sem trava
                return await _totvsService.Write(company, EnumJob.TotvsPostRemoveClassificationRelationship, user_name, key, HttpMethod.Post,
                    "person/v2/classification-relationship/remove", body, "Confira as classificações da pessoa antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        // ------------------------------------------------------------------------------------------------------

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

        // cliente PF e PJ (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.4.1): a mesma rota cria ou altera pelo documento, que é a chave em TotvsRequests;
        // a resposta traz o customerCode. Reenviar o mesmo corpo não muda nada, então não há trava. A loja de cadastro e as dos limites precisam
        // ser da empresa. O cliente chega ao banco pelo próximo job de PF/PJ (PE11)
        private async Task<Domain.Totvs.Dtos.Writes.Result> WriteCustomer(string user_name, string doc_company, EnumJob job, string route, string key, int branchInsertCode, IEnumerable<int>? limitBranches, object body)
        {
            _logger
                .Clear()
                .AddLog(job);

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                var branchCodes = await _totvsService.GetBranchCodes(company);
                var outside = (limitBranches ?? []).Prepend(branchInsertCode).Distinct().Where(x => !branchCodes.Contains(x)).ToList();

                if (outside.Count > 0)
                    return _totvsService.BranchNotFound(doc_company, key, outside.Select(x => x.ToString()));

                return await _totvsService.Write(company, job, user_name, key, HttpMethod.Post, route, body,
                    "Confira o cliente pelo GET Customers/{docCompany}/{code} antes de repetir.");
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }
    }
}

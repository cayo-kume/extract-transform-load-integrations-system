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

namespace Application.Totvs.Services
{
    /// <summary>
    /// catálogos da API Person (fase PE-1, src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.2): classificações e tipos de e-mail, de
    /// telefone e de contato. Cada método faz as próprias chamadas à API; o comum vem do ITotvsService. Com maxChangeFilterDate
    /// na resposta, a janela sai do banco, como nas filiais (T9)
    /// </summary>
    public class TotvsPersonCatalogsService : ITotvsPersonCatalogsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsPersonCatalogsRepository _totvsPersonCatalogsRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.Catalogs.PersonClassification> _personClassificationValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.Catalogs.EmailType> _emailTypeValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.Catalogs.PhoneType> _phoneTypeValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.Catalogs.ContactType> _contactTypeValidator;

        private const string PersonClassificationKey = "Totvs:PersonClassifications";
        private const string EmailTypeKey = "Totvs:EmailTypes";
        private const string PhoneTypeKey = "Totvs:PhoneTypes";
        private const string ContactTypeKey = "Totvs:ContactTypes";

        // máximo das rotas (swagger da API Person); os catálogos da Newbloomers têm de 2 a 6 itens (testado)
        private const int PageSize = 1000;

        public TotvsPersonCatalogsService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsPersonCatalogsRepository totvsPersonCatalogsRepository,
            IValidator<Domain.Totvs.Dtos.Reads.People.Catalogs.PersonClassification> personClassificationValidator,
            IValidator<Domain.Totvs.Dtos.Reads.People.Catalogs.EmailType> emailTypeValidator,
            IValidator<Domain.Totvs.Dtos.Reads.People.Catalogs.PhoneType> phoneTypeValidator,
            IValidator<Domain.Totvs.Dtos.Reads.People.Catalogs.ContactType> contactTypeValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsPersonCatalogsRepository, _personClassificationValidator, _emailTypeValidator, _phoneTypeValidator, _contactTypeValidator) =
            (apiClient, logger, cache, totvsService, totvsPersonCatalogsRepository, personClassificationValidator, emailTypeValidator, phoneTypeValidator, contactTypeValidator);

        public async Task<bool?> GetClassifications()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPersonGetClassifications);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsPersonCatalogsRepository.GetPersonClassificationsLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = new List<Domain.Totvs.Dtos.Reads.People.Catalogs.PersonClassification>();
                    var itemsJson = new Dictionary<string, string>();
                    var page = 1;
                    var totalPages = 1;
                    var failed = false;

                    while (page <= totalPages)
                    {
                        var query = new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Page", page.ToString() },
                            { "PageSize", PageSize.ToString() }
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
                                "person/v2/classifications",
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
                                _logger.AddMessage($"Erro da API Totvs em person/v2/classifications (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
                            }

                            failed = true;
                            break;
                        }

                        Page<Domain.Totvs.Dtos.Reads.People.Catalogs.PersonClassification>? result = null;

                        try
                        {
                            result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.Catalogs.PersonClassification>>(response.Content ?? String.Empty);

                            foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                            {
                                var key = $"{ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["typeCode"])}|{((string?)item["code"] ?? String.Empty).ToUpperInvariant()}";

                                if (!key.EndsWith("|") && !key.StartsWith("0|"))
                                    itemsJson[key] = item.ToString(Formatting.None);
                            }
                        }
                        catch (JsonException)
                        {
                            // result continua null: tratado abaixo como resposta inválida
                        }

                        if (result?.items is null)
                        {
                            var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                            _logger.AddMessage($"Resposta inválida da API Totvs em person/v2/classifications (empresa {company.doc_company}, página {page}): {body}");

                            failed = true;
                            break;
                        }

                        items.AddRange(result.items);

                        totalPages = result.totalPages;
                        page++;
                    }

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (failed || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{PersonClassificationKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsPersonCatalogsRepository.GetPersonClassifications(company.doc_company!);

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => $"{x!.typeCode}|{x!.code.ToUpperInvariant()}")
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    items.RemoveAll(x =>
                        itemsCache!.TryGetValue($"{ConverterValidatorsExtensions.ConvertToInt32Validation(x.typeCode)}|{(x.code ?? String.Empty).ToUpperInvariant()}", out var y) &&
                        NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
                    // Devolve a quantidade de registros gravados.
                    var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.People.Catalogs.PersonClassification>();

                    // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.typeCode) != 0 && !String.IsNullOrWhiteSpace(x.code)).GroupBy(x => $"{ConverterValidatorsExtensions.ConvertToInt32Validation(x.typeCode)}|{(x.code ?? String.Empty).ToUpperInvariant()}").Select(g => g.Last()))
                    {
                        var validations = _personClassificationValidator.Validate(item);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"classificação {$"{ConverterValidatorsExtensions.ConvertToInt32Validation(item.typeCode)}|{(item.code ?? String.Empty).ToUpperInvariant()}"}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.Catalogs.PersonClassification(company.doc_company!, batchTime, item));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _totvsPersonCatalogsRepository.BulkInsertIntoTotvsPersonClassifications(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        var key = $"{record.typeCode}|{record.code.ToUpperInvariant()}";

                        if (itemsJson.TryGetValue(key, out var json))
                            _logger.AddRecord(key.ToString(), json);

                        if (itemsCache is not null)
                            itemsCache[key] = record;
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

        public async Task<bool?> GetEmailTypes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetEmailTypes);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsPersonCatalogsRepository.GetEmailTypesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = new List<Domain.Totvs.Dtos.Reads.People.Catalogs.EmailType>();
                    var itemsJson = new Dictionary<int, string>();
                    var page = 1;
                    var totalPages = 1;
                    var failed = false;

                    while (page <= totalPages)
                    {
                        var query = new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", PageSize.ToString() }
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
                                "person/v2/email-types",
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
                                _logger.AddMessage($"Erro da API Totvs em person/v2/email-types (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
                            }

                            failed = true;
                            break;
                        }

                        Page<Domain.Totvs.Dtos.Reads.People.Catalogs.EmailType>? result = null;

                        try
                        {
                            result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.Catalogs.EmailType>>(response.Content ?? String.Empty);

                            foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                            {
                                var key = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["code"]);

                                if (key != 0)
                                    itemsJson[key] = item.ToString(Formatting.None);
                            }
                        }
                        catch (JsonException)
                        {
                            // result continua null: tratado abaixo como resposta inválida
                        }

                        if (result?.items is null)
                        {
                            var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                            _logger.AddMessage($"Resposta inválida da API Totvs em person/v2/email-types (empresa {company.doc_company}, página {page}): {body}");

                            failed = true;
                            break;
                        }

                        items.AddRange(result.items);

                        totalPages = result.totalPages;
                        page++;
                    }

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (failed || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{EmailTypeKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsPersonCatalogsRepository.GetEmailTypes(company.doc_company!);

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.code)
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    items.RemoveAll(x =>
                        itemsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.code), out var y) &&
                        NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
                    // Devolve a quantidade de registros gravados.
                    var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.People.Catalogs.EmailType>();

                    // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code)).Select(g => g.Last()))
                    {
                        var validations = _emailTypeValidator.Validate(item);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"tipo de e-mail {ConverterValidatorsExtensions.ConvertToInt32Validation(item.code)}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.Catalogs.EmailType(company.doc_company!, batchTime, item));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _totvsPersonCatalogsRepository.BulkInsertIntoTotvsEmailTypes(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        var key = record.code;

                        if (itemsJson.TryGetValue(key, out var json))
                            _logger.AddRecord(key.ToString(), json);

                        if (itemsCache is not null)
                            itemsCache[key] = record;
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

        public async Task<bool?> GetPhoneTypes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetPhoneTypes);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsPersonCatalogsRepository.GetPhoneTypesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = new List<Domain.Totvs.Dtos.Reads.People.Catalogs.PhoneType>();
                    var itemsJson = new Dictionary<int, string>();
                    var page = 1;
                    var totalPages = 1;
                    var failed = false;

                    while (page <= totalPages)
                    {
                        var query = new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", PageSize.ToString() }
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
                                "person/v2/phone-types",
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
                                _logger.AddMessage($"Erro da API Totvs em person/v2/phone-types (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
                            }

                            failed = true;
                            break;
                        }

                        Page<Domain.Totvs.Dtos.Reads.People.Catalogs.PhoneType>? result = null;

                        try
                        {
                            result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.Catalogs.PhoneType>>(response.Content ?? String.Empty);

                            foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                            {
                                var key = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["code"]);

                                if (key != 0)
                                    itemsJson[key] = item.ToString(Formatting.None);
                            }
                        }
                        catch (JsonException)
                        {
                            // result continua null: tratado abaixo como resposta inválida
                        }

                        if (result?.items is null)
                        {
                            var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                            _logger.AddMessage($"Resposta inválida da API Totvs em person/v2/phone-types (empresa {company.doc_company}, página {page}): {body}");

                            failed = true;
                            break;
                        }

                        items.AddRange(result.items);

                        totalPages = result.totalPages;
                        page++;
                    }

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (failed || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{PhoneTypeKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsPersonCatalogsRepository.GetPhoneTypes(company.doc_company!);

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.code)
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    items.RemoveAll(x =>
                        itemsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.code), out var y) &&
                        NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
                    // Devolve a quantidade de registros gravados.
                    var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.People.Catalogs.PhoneType>();

                    // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code)).Select(g => g.Last()))
                    {
                        var validations = _phoneTypeValidator.Validate(item);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"tipo de telefone {ConverterValidatorsExtensions.ConvertToInt32Validation(item.code)}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.Catalogs.PhoneType(company.doc_company!, batchTime, item));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _totvsPersonCatalogsRepository.BulkInsertIntoTotvsPhoneTypes(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        var key = record.code;

                        if (itemsJson.TryGetValue(key, out var json))
                            _logger.AddRecord(key.ToString(), json);

                        if (itemsCache is not null)
                            itemsCache[key] = record;
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

        public async Task<bool?> GetContactTypes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetContactTypes);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // maxChangeFilterDate só vem com StartChangeDate/EndChangeDate (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsPersonCatalogsRepository.GetContactTypesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var items = new List<Domain.Totvs.Dtos.Reads.People.Catalogs.ContactType>();
                    var itemsJson = new Dictionary<int, string>();
                    var page = 1;
                    var totalPages = 1;
                    var failed = false;

                    while (page <= totalPages)
                    {
                        var query = new Dictionary<string, string>
                        {
                            { "StartChangeDate", $"{since:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "code" },
                            { "Page", page.ToString() },
                            { "PageSize", PageSize.ToString() }
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
                                "person/v2/contact-types",
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
                                _logger.AddMessage($"Erro da API Totvs em person/v2/contact-types (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
                            }

                            failed = true;
                            break;
                        }

                        Page<Domain.Totvs.Dtos.Reads.People.Catalogs.ContactType>? result = null;

                        try
                        {
                            result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.Catalogs.ContactType>>(response.Content ?? String.Empty);

                            foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                            {
                                var key = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["code"]);

                                if (key != 0)
                                    itemsJson[key] = item.ToString(Formatting.None);
                            }
                        }
                        catch (JsonException)
                        {
                            // result continua null: tratado abaixo como resposta inválida
                        }

                        if (result?.items is null)
                        {
                            var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                            _logger.AddMessage($"Resposta inválida da API Totvs em person/v2/contact-types (empresa {company.doc_company}, página {page}): {body}");

                            failed = true;
                            break;
                        }

                        items.AddRange(result.items);

                        totalPages = result.totalPages;
                        page++;
                    }

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (failed || items.Count == 0)
                        continue;

                    // cache de estado: chave -> registro gravado (com o maxChangeFilterDate), por empresa; o catálogo é pequeno,
                    // então o cache nasce com a tabela inteira da empresa
                    var itemsCache = await _cache.GetOrCreateAsync($"{ContactTypeKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsPersonCatalogsRepository.GetContactTypes(company.doc_company!);

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.code)
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    items.RemoveAll(x =>
                        itemsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.code), out var y) &&
                        NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
                    // Devolve a quantidade de registros gravados.
                    var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.People.Catalogs.ContactType>();

                    // o mesmo registro pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).GroupBy(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code)).Select(g => g.Last()))
                    {
                        var validations = _contactTypeValidator.Validate(item);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"tipo de contato {ConverterValidatorsExtensions.ConvertToInt32Validation(item.code)}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.Catalogs.ContactType(company.doc_company!, batchTime, item));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _totvsPersonCatalogsRepository.BulkInsertIntoTotvsContactTypes(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        var key = record.code;

                        if (itemsJson.TryGetValue(key, out var json))
                            _logger.AddRecord(key.ToString(), json);

                        if (itemsCache is not null)
                            itemsCache[key] = record;
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
    }
}

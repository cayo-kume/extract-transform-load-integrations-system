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
    /// representantes e recepções de venda da API Person (fase PE-2, docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1). Cada método
    /// faz as próprias chamadas à API; o comum (horário do lote, parâmetros, lojas, token e log sob demanda) vem do ITotvsService
    /// </summary>
    public class TotvsRepresentativesService : ITotvsRepresentativesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsRepresentativesRepository _totvsRepresentativesRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.Representatives.Representative> _representativeValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.ReceptionSales.ReceptionSale> _receptionSaleValidator;

        private const string RepresentativesKey = "Totvs:Representatives";

        // máximo da busca de representantes (swagger da API Person); a base tem 20 representantes (testado), então a janela é lida
        // inteira, sem fatias, como nas filiais
        private const int RepresentativesPageSize = 500;

        // expand da documentação (o swagger repete "addresses")
        private const string RepresentativesExpand = "addresses,classifications,emails,additionalFields,phones,observations,pricetables,customers,salesRegions,representativeParameters";

        // máximo da busca de recepções; a carga inicial é lida em fatias da data do movimento
        private const int ReceptionSalesPageSize = 1000;
        private const int ReceptionSalesWindowDays = 30;

        // a recepção não tem data de alteração: a cada execução relê o movimento desde 2 dias antes do maior já gravado (PE2)
        private const int ReceptionSalesRereadDays = 2;

        public TotvsRepresentativesService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsRepresentativesRepository totvsRepresentativesRepository,
            IValidator<Domain.Totvs.Dtos.Reads.People.Representatives.Representative> representativeValidator,
            IValidator<Domain.Totvs.Dtos.Reads.People.ReceptionSales.ReceptionSale> receptionSaleValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsRepresentativesRepository, _representativeValidator, _receptionSaleValidator) =
            (apiClient, logger, cache, totvsService, totvsRepresentativesRepository, representativeValidator, receptionSaleValidator);

        public async Task<bool?> PostSearchRepresentatives()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchRepresentatives);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // os parâmetros de empresa do representante só vêm para as lojas pedidas em option.branchRepresentativeParameterList
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    // maxChangeFilterDate só vem com filter.change (testado). Banco vazio: desde 2000
                    var lastChange = await _totvsRepresentativesRepository.GetRepresentativesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var representatives = new List<Domain.Totvs.Dtos.Reads.People.Representatives.Representative>();
                    var representativesJson = new Dictionary<int, string>();
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
                                    startDate = $"{since:yyyy-MM-ddTHH:mm:ss}",
                                    endDate = $"{batchTime:yyyy-MM-ddTHH:mm:ss}",
                                    inClassification = true,
                                    inAddress = true,
                                    inPhone = true,
                                    inObservation = true,
                                    inRepresentative = true,
                                    inCustomer = true,
                                    inSalesRegion = true,
                                    inPriceTable = true,
                                    inRepresentativeParameter = true
                                }
                            },
                            option = new
                            {
                                branchRepresentativeParameterList = branchCodes
                            },
                            order = "maxChangeFilterDate",
                            page,
                            pageSize = RepresentativesPageSize,
                            expand = RepresentativesExpand
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
                                "person/v2/representatives/search",
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
                                _logger.AddMessage($"Erro da API Totvs em person/v2/representatives/search (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
                            }

                            failed = true;
                            break;
                        }

                        Page<Domain.Totvs.Dtos.Reads.People.Representatives.Representative>? result = null;

                        try
                        {
                            result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.Representatives.Representative>>(response.Content ?? String.Empty);

                            foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                            {
                                var code = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["code"]);

                                if (code != 0)
                                    representativesJson[code] = item.ToString(Formatting.None);
                            }
                        }
                        catch (JsonException)
                        {
                            // result continua null: tratado abaixo como resposta inválida
                        }

                        if (result?.items is null)
                        {
                            var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                            _logger.AddMessage($"Resposta inválida da API Totvs em person/v2/representatives/search (empresa {company.doc_company}, página {page}): {body}");

                            failed = true;
                            break;
                        }

                        representatives.AddRange(result.items);

                        totalPages = result.totalPages;
                        page++;
                    }

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (failed || representatives.Count == 0)
                        continue;

                    // cache de estado: código -> representante gravado (com o maxChangeFilterDate), por empresa
                    var representativesCache = await _cache.GetOrCreateAsync($"{RepresentativesKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsRepresentativesRepository.GetRepresentativesByCodes(
                            company.doc_company!,
                            representatives.Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code)).Where(x => x != 0).Distinct().ToList()
                        );

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.code)
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    representatives.RemoveAll(x =>
                        representativesCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.code), out var y) &&
                        NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    totalInseridos += await SaveRepresentatives(company.doc_company!, batchTime, representatives, representativesJson, representativesCache);
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
        /// busca um representante por código, grava nas tabelas de representantes (sempre, sem consultar o cache) e registra a chamada
        /// em totvs.TotvsRequests (PE10, no desenho do GetCustomerByCode)
        /// </summary>
        public async Task<bool?> GetRepresentativeByCode(string user_name, string doc_company, int code)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetRepresentativeByCode);

            // rota registrada no log de requisições (sem o token); o filtro vai no corpo
            const string route = "person/v2/representatives/search";
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

                var branchCodes = await _totvsService.GetBranchCodes(company);

                // o change desde 2000 (com as flags da busca por janela) é o que faz o maxChangeFilterDate vir preenchido, como nas pessoas
                var requestBody = JsonConvert.SerializeObject(new
                {
                    filter = new
                    {
                        representativeCodeList = new[] { code },
                        change = new
                        {
                            startDate = "2000-01-01T00:00:00",
                            endDate = $"{_totvsService.GetBatchTime():yyyy-MM-ddTHH:mm:ss}",
                            inClassification = true,
                            inAddress = true,
                            inPhone = true,
                            inObservation = true,
                            inRepresentative = true,
                            inCustomer = true,
                            inSalesRegion = true,
                            inPriceTable = true,
                            inRepresentativeParameter = true
                        }
                    },
                    option = new { branchRepresentativeParameterList = branchCodes },
                    page = 1,
                    pageSize = 1,
                    expand = RepresentativesExpand
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
                    _totvsService.SaveRequest(EnumJob.TotvsGetRepresentativeByCode, user_name, "POST", route, doc_company, code.ToString(), $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                    throw;
                }

                if (response is null)
                {
                    // login falhou: nenhuma chamada à busca
                    _totvsService.SaveRequest(EnumJob.TotvsGetRepresentativeByCode, user_name, "POST", route, doc_company, code.ToString(), null, false, requestBody);
                    return null;
                }

                if (!response.IsSuccess)
                {
                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                    _logger.AddMessage($"Erro da API Totvs em {route} (empresa {doc_company}, representante {code}): {response.StatusCode} {body}");

                    _totvsService.SaveRequest(EnumJob.TotvsGetRepresentativeByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, false, requestBody);
                    return null;
                }

                Page<Domain.Totvs.Dtos.Reads.People.Representatives.Representative>? page = null;
                string? json = null;

                try
                {
                    page = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.Representatives.Representative>>(response.Content ?? String.Empty);
                    json = ((JObject.Parse(response.Content!)["items"] as JArray) ?? []).FirstOrDefault()?.ToString(Formatting.None);
                }
                catch (JsonException)
                {
                    // page continua null: tratado abaixo como resposta inválida
                }

                var representative = page?.items?.FirstOrDefault(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) == code);

                if (page?.items is null)
                {
                    _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {doc_company}, representante {code}): {response.Content}");
                    result = null;
                }
                else if (representative is null || json is null)
                {
                    // a busca responde 200 com items vazio quando o código não existe
                    _logger.AddMessage($"Representante {code} não encontrado na Totvs (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    // atualiza o cache do PostSearchRepresentatives, se ele estiver carregado, para a próxima execução não regravar
                    _cache.TryGetValue($"{RepresentativesKey}:{doc_company}", out Dictionary<int, Domain.Totvs.Entities.Reads.People.Representatives.Representative>? representativesCache);

                    await SaveRepresentatives(doc_company, _totvsService.GetBatchTime(), [representative], new Dictionary<int, string> { { code, json } }, representativesCache);

                    _logger.AddMessage($"Concluída com sucesso: representante {code} inserido ou atualizado!");
                    result = true;
                }

                _totvsService.SaveRequest(EnumJob.TotvsGetRepresentativeByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, result == true, requestBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// sincroniza as recepções de venda pelo GET person/v2/reception-sale/search. A resposta não tem data de alteração: a janela é
        /// pela data do movimento (banco vazio: desde 01/01/2025, em fatias de 30 dias; depois, desde 2 dias antes do maior movimento
        /// gravado), e o hash do JSON de cada recepção, gravado em json_hash, diz o que mudou (T9)
        /// </summary>
        public async Task<bool?> GetSearchReceptionSale()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSearchReceptionSale);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var today = _totvsService.GetBatchTime().Date;

                    var lastMovement = await _totvsRepresentativesRepository.GetReceptionSalesLastMovementDate(company.doc_company!);
                    var since = lastMovement.HasValue ? lastMovement.Value.Date.AddDays(-ReceptionSalesRereadDays) : new DateTime(2025, 1, 1);

                    for (var windowStart = since; windowStart <= today; windowStart = windowStart.AddDays(ReceptionSalesWindowDays))
                    {
                        // a data do movimento é dia inteiro: a fatia vai do primeiro ao último dia, inclusive
                        var windowEnd = windowStart.AddDays(ReceptionSalesWindowDays - 1) < today ? windowStart.AddDays(ReceptionSalesWindowDays - 1) : today;

                        var receptions = new List<Domain.Totvs.Dtos.Reads.People.ReceptionSales.ReceptionSale>();
                        var receptionsJson = new Dictionary<(int, DateTime, int), string>();
                        var page = 1;
                        var totalPages = 1;
                        var failed = false;

                        while (page <= totalPages)
                        {
                            var query = new Dictionary<string, string>
                            {
                                { "StartMovementDate", $"{windowStart:yyyy-MM-dd}" },
                                { "EndMovementDate", $"{windowEnd:yyyy-MM-dd}" },
                                { "Page", page.ToString() },
                                { "PageSize", ReceptionSalesPageSize.ToString() }
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
                                    "person/v2/reception-sale/search",
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

                            // o swagger declara 404 nesta rota; sem recepções no período a API respondeu 200 com items vazio (testado)
                            if (response is not null && response.StatusCode == 404)
                                break;

                            if (response is null || !response.IsSuccess)
                            {
                                if (response is not null)
                                {
                                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                    _logger.AddMessage($"Erro da API Totvs em person/v2/reception-sale/search (empresa {company.doc_company}, movimento {windowStart:dd/MM/yyyy} a {windowEnd:dd/MM/yyyy}, página {page}): {response.StatusCode} {body}");
                                }

                                failed = true;
                                break;
                            }

                            Page<Domain.Totvs.Dtos.Reads.People.ReceptionSales.ReceptionSale>? result = null;

                            try
                            {
                                result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.People.ReceptionSales.ReceptionSale>>(response.Content ?? String.Empty);

                                // a chave sai do DTO (a mesma conversão de data da gravação), pareando cada item com o seu JSON pela posição
                                var items = (JObject.Parse(response.Content!)["items"] as JArray) ?? [];

                                for (var i = 0; i < Math.Min(items.Count, result?.items?.Count ?? 0); i++)
                                {
                                    var key = ReceptionKey(result!.items![i].branchCode, result.items[i].movementDate, result.items[i].sequence);

                                    if (key.branchCode != 0 && key.sequence != 0)
                                        receptionsJson[key] = items[i].ToString(Formatting.None);
                                }
                            }
                            catch (JsonException)
                            {
                                // result continua null: tratado abaixo como resposta inválida
                            }

                            if (result?.items is null)
                            {
                                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                _logger.AddMessage($"Resposta inválida da API Totvs em person/v2/reception-sale/search (empresa {company.doc_company}, página {page}): {body}");

                                failed = true;
                                break;
                            }

                            receptions.AddRange(result.items);

                            totalPages = result.totalPages;
                            page++;
                        }

                        // fatia incompleta não é gravada, e as seguintes também não: a próxima execução relê desde o maior movimento gravado
                        if (failed)
                            break;

                        if (receptions.Count == 0)
                            continue;

                        // o que já está gravado na fatia, com o hash do JSON de cada recepção (sem cache: a tabela guarda o hash, T9)
                        var stored = (await _totvsRepresentativesRepository.GetReceptionSalesHashes(company.doc_company!, windowStart, windowEnd))
                            .Where(x => x != null)
                            .GroupBy(x => (x!.branchCode, x.movementDate.Date, x.sequence))
                            .ToDictionary(g => g.Key, g => g.First()!.json_hash);

                        totalInseridos += await SaveReceptionSales(company.doc_company!, _totvsService.GetBatchTime(), receptions, receptionsJson, stored);
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
        /// chave da recepção: loja + data do movimento (só o dia) + sequência
        /// </summary>
        private static (int branchCode, DateTime movementDate, int sequence) ReceptionKey(string? branchCode, string? movementDate, string? sequence) =>
            (ConverterValidatorsExtensions.ConvertToInt32Validation(branchCode), ConverterValidatorsExtensions.ConvertToDateTimeValidation(movementDate).Date, ConverterValidatorsExtensions.ConvertToInt32Validation(sequence));

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de representantes gravados.
        /// </summary>
        private async Task<int> SaveRepresentatives(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.People.Representatives.Representative> representatives,
            IReadOnlyDictionary<int, string> representativesJson,
            Dictionary<int, Domain.Totvs.Entities.Reads.People.Representatives.Representative>? representativesCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.People.Representatives.Representative>();

            // o mesmo representante pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var representative in representatives.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.code) != 0).GroupBy(x => x.code).Select(g => g.Last()))
            {
                var validations = _representativeValidator.Validate(representative);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"representante {representative.code}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.Representatives.Representative(doc_company, batchTime, representative));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsRepresentativesRepository.BulkInsertIntoTotvsRepresentatives(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (representativesJson.TryGetValue(record.code, out var json))
                    _logger.AddRecord(record.code.ToString(), json);

                if (representativesCache is not null)
                    representativesCache[record.code] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote e o hash do JSON) e grava só as recepções novas ou com hash
        /// diferente do gravado. Devolve a quantidade de recepções gravadas.
        /// </summary>
        private async Task<int> SaveReceptionSales(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.People.ReceptionSales.ReceptionSale> receptions,
            IReadOnlyDictionary<(int, DateTime, int), string> receptionsJson,
            IReadOnlyDictionary<(int, DateTime, int), string> stored)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.People.ReceptionSales.ReceptionSale>();

            foreach (var reception in receptions.GroupBy(x => ReceptionKey(x.branchCode, x.movementDate, x.sequence)).Select(g => g.Last()))
            {
                var key = ReceptionKey(reception.branchCode, reception.movementDate, reception.sequence);

                if (key.branchCode == 0 || key.sequence == 0 || !receptionsJson.TryGetValue(key, out var json))
                    continue;

                var hash = _logger.ComputeSha256Hash(json);

                // sem data de alteração: só regrava quando o JSON mudou
                if (stored.TryGetValue(key, out var storedHash) && storedHash == hash)
                    continue;

                var validations = _receptionSaleValidator.Validate(reception);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"recepção {key.branchCode}/{key.movementDate:dd/MM/yyyy}/{key.sequence}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.People.ReceptionSales.ReceptionSale(doc_company, batchTime, hash, reception));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsRepresentativesRepository.BulkInsertIntoTotvsReceptionSales(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
                if (receptionsJson.TryGetValue((record.branchCode, record.movementDate.Date, record.sequence), out var json))
                    _logger.AddRecord($"{record.branchCode}|{record.movementDate:yyyy-MM-dd}|{record.sequence}", json);

            return _listSomenteNovos.Count;
        }
    }
}

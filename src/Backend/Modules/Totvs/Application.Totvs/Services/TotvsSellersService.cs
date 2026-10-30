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
    /// vendedores e regiões de atuação da Totvs (API Seller). Cada método faz as próprias chamadas à API; o comum (horário do
    /// lote, parâmetros, lojas e token) vem do ITotvsService
    /// </summary>
    public class TotvsSellersService : ITotvsSellersService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsSellersRepository _totvsSellersRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Sellers.Seller> _sellerValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Sellers.OperationalArea> _operationalAreaValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Sellers.OperationalAreaState> _operationalAreaStateValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Sellers.OperationalAreaCep> _operationalAreaCepValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Sellers.OperationalAreaCity> _operationalAreaCityValidator;

        private const string SellersKey = "Totvs:Sellers";

        // máximo da rota (swagger da API Seller)
        private const int SellersPageSize = 500;

        // máximo das quatro rotas de região de atuação (swagger da API Seller)
        private const int OperationalAreasPageSize = 1000;

        public TotvsSellersService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsSellersRepository totvsSellersRepository,
            IValidator<Domain.Totvs.Dtos.Reads.Sellers.Seller> sellerValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Sellers.OperationalArea> operationalAreaValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Sellers.OperationalAreaState> operationalAreaStateValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Sellers.OperationalAreaCep> operationalAreaCepValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Sellers.OperationalAreaCity> operationalAreaCityValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsSellersRepository, _sellerValidator, _operationalAreaValidator, _operationalAreaStateValidator, _operationalAreaCepValidator, _operationalAreaCityValidator) =
            (apiClient, logger, cache, totvsService, totvsSellersRepository, sellerValidator, operationalAreaValidator, operationalAreaStateValidator, operationalAreaCepValidator, operationalAreaCityValidator);

        public async Task<bool?> PostSearchSellers()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchSellers);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // branchCodeList filtra também as filhas: sem todas as lojas, o vendedor vem só com a informação das enviadas (testado)
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    var batchTime = _totvsService.GetBatchTime();

                    // as datas da Totvs são do horário local, sem fuso (o "Z" é ignorado pela API, testado). Banco vazio: desde 2000
                    var lastChange = await _totvsSellersRepository.GetSellersLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);

                    var sellers = new List<Domain.Totvs.Dtos.Reads.Sellers.Seller>();
                    var sellersJson = new Dictionary<int, string>();
                    var page = 1;
                    var totalPages = 1;
                    var failed = false;

                    while (page <= totalPages)
                    {
                        // a rota não tem expand nem order (swagger): as filhas vêm sempre
                        var requestBody = JsonConvert.SerializeObject(new
                        {
                            filter = new
                            {
                                change = new
                                {
                                    startDate = $"{since:yyyy-MM-ddTHH:mm:ss}",
                                    endDate = $"{batchTime:yyyy-MM-ddTHH:mm:ss}"
                                },
                                branchCodeList = branchCodes
                            },
                            page,
                            pageSize = SellersPageSize
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
                                "seller/v2/search",
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
                                _logger.AddMessage($"Erro da API Totvs em seller/v2/search (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
                            }

                            failed = true;
                            break;
                        }

                        Page<Domain.Totvs.Dtos.Reads.Sellers.Seller>? result = null;

                        try
                        {
                            result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.Sellers.Seller>>(response.Content ?? String.Empty);

                            foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                            {
                                var code = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["sellerCode"]);

                                if (code != 0)
                                    sellersJson[code] = item.ToString(Formatting.None);
                            }
                        }
                        catch (JsonException)
                        {
                            // result continua null: tratado abaixo como resposta inválida
                        }

                        if (result?.items is null)
                        {
                            var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                            _logger.AddMessage($"Resposta inválida da API Totvs em seller/v2/search (empresa {company.doc_company}, página {page}): {body}");

                            failed = true;
                            break;
                        }

                        sellers.AddRange(result.items);

                        totalPages = result.totalPages;
                        page++;
                    }

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(maxChangeFilterDate) e pularia as páginas que faltaram
                    if (failed || sellers.Count == 0)
                        continue;

                    // cache de estado: código -> vendedor gravado (com o maxChangeFilterDate), por empresa
                    var sellersCache = await _cache.GetOrCreateAsync($"{SellersKey}:{company.doc_company}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        var list = await _totvsSellersRepository.GetSellersByCodes(
                            company.doc_company!,
                            sellers.Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.sellerCode)).Where(x => x != 0).Distinct().ToList()
                        );

                        return list
                            .Where(x => x != null)
                            .GroupBy(x => x!.sellerCode)
                            .ToDictionary(g => g.Key, g => g.First()!);
                    });

                    // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                    sellers.RemoveAll(x =>
                        sellersCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.sellerCode), out var y) &&
                        NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                    );

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
                    // Devolve a quantidade de vendedores gravados.
                    var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.Sellers.Seller>();

                    // o mesmo vendedor pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    foreach (var seller in sellers.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.sellerCode) != 0).GroupBy(x => x.sellerCode).Select(g => g.Last()))
                    {
                        var validations = _sellerValidator.Validate(seller);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"vendedor {seller.sellerCode}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.Sellers.Seller(company.doc_company!, batchTime, seller));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _totvsSellersRepository.BulkInsertIntoTotvsSellers(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                    {
                        if (sellersJson.TryGetValue(record.sellerCode, out var json))
                            _logger.AddRecord(record.sellerCode.ToString(), json);

                        if (sellersCache is not null)
                            sellersCache[record.sellerCode] = record;
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

        public async Task<bool?> GetOperationalArea()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetOperationalArea);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // as quatro rotas são lidas inteiras, sem filtro de data: sem ele a API não preenche o maxChangeFilterDate, e uma
                    // janela não traria o estado, CEP ou cidade que saiu da região. Qualquer rota incompleta interrompe a empresa
                    var areas = await GetOperationalAreaPages<Domain.Totvs.Dtos.Reads.Sellers.OperationalArea>(company, "seller/v2/operational-area");

                    if (areas is null)
                        continue;

                    var states = await GetOperationalAreaPages<Domain.Totvs.Dtos.Reads.Sellers.OperationalAreaState>(company, "seller/v2/operational-area-state");

                    if (states is null)
                        continue;

                    var ceps = await GetOperationalAreaPages<Domain.Totvs.Dtos.Reads.Sellers.OperationalAreaCep>(company, "seller/v2/operational-area-cep");

                    if (ceps is null)
                        continue;

                    var cities = await GetOperationalAreaPages<Domain.Totvs.Dtos.Reads.Sellers.OperationalAreaCity>(company, "seller/v2/operational-area-city");

                    if (cities is null)
                        continue;

                    if (areas.Count == 0)
                        continue;

                    // o que já está gravado, com o hash da região agrupada (sem cache: a tabela guarda o hash, T9)
                    var stored = (await _totvsSellersRepository.GetOperationalAreasHashes(company.doc_company!))
                        .Where(x => x != null)
                        .GroupBy(x => x!.operationalAreaCode)
                        .ToDictionary(g => g.Key, g => g.First()!.json_hash);

                    // agrupa estados, CEPs e cidades por região, calcula o hash da região agrupada e grava só as novas ou com hash diferente
                    // do gravado (com as filhas). Filha de região que não veio no operational-area vai para o log e é descartada (a FK
                    // recusaria). Devolve a quantidade de regiões gravadas.
                    static int AreaCode(string? code) => ConverterValidatorsExtensions.ConvertToInt32Validation(code);

                    // a mesma região pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    var areasByCode = areas
                        .Where(x => AreaCode(x.dto.operationalAreaCode) != 0)
                        .GroupBy(x => AreaCode(x.dto.operationalAreaCode))
                        .ToDictionary(g => g.Key, g => g.Last());

                    var statesByArea = states.ToLookup(x => AreaCode(x.dto.operationalAreaCode));
                    var cepsByArea = ceps.ToLookup(x => AreaCode(x.dto.operationalAreaCode));
                    var citiesByArea = cities.ToLookup(x => AreaCode(x.dto.operationalAreaCode));

                    foreach (var orphan in statesByArea.Select(g => g.Key).Concat(cepsByArea.Select(g => g.Key)).Concat(citiesByArea.Select(g => g.Key)).Distinct().Where(x => !areasByCode.ContainsKey(x)))
                        _logger.AddMessage($"Região de atuação {orphan} (empresa {company.doc_company}) tem estados, CEPs ou cidades, mas não veio em seller/v2/operational-area: descartados.");

                    var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.Sellers.OperationalArea>();
                    var areasJson = new Dictionary<int, string>();

                    foreach (var (code, area) in areasByCode)
                    {
                        // filhas na ordem da chave: a ordem das páginas não muda o hash
                        var areaStates = statesByArea[code].OrderBy(x => AreaCode(x.dto.stateCode)).ToList();
                        var areaCeps = cepsByArea[code].OrderBy(x => x.dto.cep, StringComparer.Ordinal).ToList();
                        var areaCities = citiesByArea[code].OrderBy(x => AreaCode(x.dto.cityCode)).ToList();

                        var json = new JObject(
                            new JProperty("operationalArea", area.json),
                            new JProperty("states", new JArray(areaStates.Select(x => x.json))),
                            new JProperty("ceps", new JArray(areaCeps.Select(x => x.json))),
                            new JProperty("cities", new JArray(areaCities.Select(x => x.json)))
                        ).ToString(Formatting.None);

                        var hash = _logger.ComputeSha256Hash(json);

                        // sem data de alteração: só regrava quando a região agrupada mudou
                        if (stored.TryGetValue(code, out var storedHash) && storedHash == hash)
                            continue;

                        var messages = new List<string>();

                        _operationalAreaValidator.Validate(area.dto).Errors.ForEach(s => messages.Add($"região de atuação {code}: {s.ErrorMessage}"));
                        areaStates.ForEach(x => _operationalAreaStateValidator.Validate(x.dto).Errors.ForEach(s => messages.Add($"região de atuação {code}, estado {x.dto.stateCode}: {s.ErrorMessage}")));
                        areaCeps.ForEach(x => _operationalAreaCepValidator.Validate(x.dto).Errors.ForEach(s => messages.Add($"região de atuação {code}, CEP {x.dto.cep}: {s.ErrorMessage}")));
                        areaCities.ForEach(x => _operationalAreaCityValidator.Validate(x.dto).Errors.ForEach(s => messages.Add($"região de atuação {code}, cidade {x.dto.cityCode}: {s.ErrorMessage}")));

                        if (messages.Count > 0)
                            _logger.AddMessage(String.Join("\n", messages) + "\n");

                        _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.Sellers.OperationalArea(
                            company.doc_company!,
                            batchTime,
                            hash,
                            area.dto,
                            // sem a chave a linha violaria a PK
                            areaStates.Select(x => x.dto).Where(x => AreaCode(x.stateCode) != 0),
                            areaCeps.Select(x => x.dto).Where(x => !String.IsNullOrWhiteSpace(x.cep)),
                            areaCities.Select(x => x.dto).Where(x => AreaCode(x.cityCode) != 0)
                        ));

                        areasJson[code] = json;
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _totvsSellersRepository.BulkInsertIntoTotvsOperationalAreas(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                        if (areasJson.TryGetValue(record.operationalAreaCode, out var json))
                            _logger.AddRecord(record.operationalAreaCode.ToString(), json);

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

        /// <summary>
        /// lê todas as páginas de uma das quatro rotas de região de atuação (Page/PageSize, sem filtro de data). Devolve cada item
        /// com o seu JSON, sem o maxChangeFilterDate (que, preenchido, mudaria o hash a cada execução), ou null se alguma página
        /// falhou (a mensagem vai para o log)
        /// </summary>
        private async Task<List<(T dto, JObject json)>?> GetOperationalAreaPages<T>(Domain.Totvs.Entities.Parameters.Company company, string route)
        {
            var items = new List<(T dto, JObject json)>();
            var page = 1;
            var totalPages = 1;

            while (page <= totalPages)
            {
                var query = new Dictionary<string, string>
                {
                    { "Page", page.ToString() },
                    { "PageSize", OperationalAreasPageSize.ToString() }
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
                        route,
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
                        _logger.AddMessage($"Erro da API Totvs em {route} (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
                    }

                    return null;
                }

                Page<T>? result = null;
                JArray json = [];

                try
                {
                    result = JsonConvert.DeserializeObject<Page<T>>(response.Content ?? String.Empty);

                    // DateParseHandling.None: o JSON gravado e o hash ficam com as datas como a API mandou
                    using var reader = new JsonTextReader(new StringReader(response.Content!)) { DateParseHandling = DateParseHandling.None };
                    json = (JObject.Load(reader)["items"] as JArray) ?? [];
                }
                catch (JsonException)
                {
                    // result continua null: tratado abaixo como resposta inválida
                }

                if (result?.items is null || json.Count != result.items.Count)
                {
                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                    _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {company.doc_company}, página {page}): {body}");

                    return null;
                }

                // cada item pareado com o seu JSON pela posição
                for (var i = 0; i < json.Count; i++)
                {
                    var item = (JObject)json[i];
                    item.Remove("maxChangeFilterDate");
                    items.Add((result.items[i], item));
                }

                totalPages = result.totalPages;
                page++;
            }

            return items;
        }
    }
}

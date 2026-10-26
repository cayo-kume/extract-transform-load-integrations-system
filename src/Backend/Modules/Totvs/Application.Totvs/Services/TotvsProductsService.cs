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
    /// referências e produtos da Totvs (API Product). O método faz as próprias chamadas à API; o comum (horário do lote,
    /// parâmetros, lojas e token) vem do ITotvsService
    /// </summary>
    public class TotvsProductsService : ITotvsProductsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsProductsRepository _totvsProductsRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Products.Reference> _referenceValidator;

        private const string SkusKey = "Totvs:Skus";

        // empresa base de cada marca (option.branchInfoCode é obrigatório e aceita uma empresa só) e o código da marca na
        // classificação 6000 (SETOR): sem esse filtro as duas chamadas trazem o catálogo inteiro (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.4).
        // Códigos da classificação conferidos na API: 2 = MISHA, 4 = OPEN ERA (1 = GERAL, itens de consumo, fica de fora). Uma marca nova entra aqui
        private const int BrandClassificationType = 6000;

        private static readonly Dictionary<int, string> BaseBranchCodes = new()
        {
            { 1, "2" },     // Misha
            { 101, "4" }    // Open Era
        };

        // com o expand completo, 1000 por página levou 105 s numa marca e deu 502 (timeout do gateway da Totvs) na outra; 200 respondeu (testado)
        private const int ReferencesPageSize = 200;

        // a carga inicial (10 meses) tem ~20 mil referências por marca: a janela é lida em fatias e cada fatia é gravada ao terminar,
        // para uma falha da API não descartar a carga inteira (a próxima execução continua do MAX(maxChangeFilterDate))
        private const int WindowDays = 7;

        // primeira carga da matriz (sem registros): janela de 30 dias, como em notas, pedidos e pessoas; depois volta para 7
        private const int InitialWindowDays = 30;

        private const string Expand = "observations,details,barCodes,classifications,additionalFields,composition,suppliers,manufacturers," +
            "referenceCodeSequences,webData,webDetail,referenceCategories,measurementTableOmni,branchesProductBlocked,conservationInstructions,integrationGroupCodes";

        public TotvsProductsService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsProductsRepository totvsProductsRepository,
            IValidator<Domain.Totvs.Dtos.Reads.Products.Reference> referenceValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsProductsRepository, _referenceValidator) =
            (apiClient, logger, cache, totvsService, totvsProductsRepository, referenceValidator);

        public async Task<bool?> PostSearchReferences()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchReferences);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    foreach (var (baseBranchCode, brandClassificationCode) in BaseBranchCodes)
                    {
                        // lojas da marca (mesma raiz de CNPJ da matriz), para trazer também a referência alterada só na informação de uma loja
                        var brandBranchCodes = await _totvsService.GetBrandBranchCodes(company, baseBranchCode);

                        if (brandBranchCodes.Count == 0)
                        {
                            _logger.AddMessage($"Empresa {company.doc_company}: matriz {baseBranchCode} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                            continue;
                        }

                        var endDate = _totvsService.GetBatchTime();

                        // as datas da Totvs são do horário local, sem fuso (o "Z" é ignorado pela API, testado). Banco vazio: desde 2000
                        var lastChange = await _totvsProductsRepository.GetSkusLastChange(company.doc_company!, baseBranchCode);
                        var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2000, 1, 1);
                        var windowDays = lastChange.HasValue ? WindowDays : InitialWindowDays;

                        for (var windowStart = since; windowStart < endDate; windowStart = windowStart.AddDays(windowDays))
                        {
                            var windowEnd = windowStart.AddDays(windowDays) < endDate ? windowStart.AddDays(windowDays) : endDate;

                            var references = new List<Domain.Totvs.Dtos.Reads.Products.Reference>();
                            var referencesJson = new Dictionary<int, string>();
                            var page = 1;
                            var totalPages = 1;
                            var failed = false;

                            while (page <= totalPages)
                            {
                                var requestBody = JsonConvert.SerializeObject(new
                                {
                                    filter = new
                                    {
                                        branchInfo = new
                                        {
                                            branchCode = baseBranchCode,
                                            isActive = true,
                                            isFinishedProduct = true
                                        },
                                        classifications = new[]
                                        {
                                            new
                                            {
                                                type = BrandClassificationType,
                                                codeList = new[] { brandClassificationCode }
                                            }
                                        },
                                        change = new
                                        {
                                            startDate = $"{windowStart:yyyy-MM-ddTHH:mm:ss}",
                                            endDate = $"{windowEnd:yyyy-MM-ddTHH:mm:ss}",
                                            inProduct = true,
                                            inBranchInfo = true,
                                            branchInfoCodeList = brandBranchCodes
                                        }
                                    },
                                    // measurementTableOmni só vem com o tipo de medida, e o valor é numérico (2 = medida aprovada; o nome do enum dá 400, testado)
                                    option = new
                                    {
                                        branchInfoCode = baseBranchCode,
                                        measurementTableOmniType = 2
                                    },
                                    order = "maxChangeFilterDate",
                                    page,
                                    pageSize = ReferencesPageSize,
                                    expand = Expand
                                });

                                // uma nova tentativa: 401 renova o token, 429 espera o Retry-After e 502/503/504 (a rota é lenta e o
                                // gateway da Totvs corta, testado) espera 10 s
                                ApiResponse? response = null;

                                for (var attempt = 1; attempt <= 2; attempt++)
                                {
                                    var token = await _totvsService.GetToken(company);

                                    if (token is null)
                                        break;   // a mensagem do login já foi para o log

                                    using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                                    response = await _apiClient.SendAsync(
                                        HttpMethod.Post,
                                        "product/v2/references/search",
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
                                        _logger.AddMessage($"Erro da API Totvs em product/v2/references/search (empresa {company.doc_company}, matriz {baseBranchCode}, janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}, página {page}): {response.StatusCode} {body}");
                                    }

                                    failed = true;
                                    break;
                                }

                                Page<Domain.Totvs.Dtos.Reads.Products.Reference>? result = null;

                                try
                                {
                                    result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.Products.Reference>>(response.Content ?? String.Empty);

                                    foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                                    {
                                        var referenceId = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["referenceId"]);

                                        if (referenceId != 0)
                                            referencesJson[referenceId] = item.ToString(Formatting.None);
                                    }
                                }
                                catch (JsonException)
                                {
                                    // result continua null: tratado abaixo como resposta inválida (já veio resposta truncada no teste)
                                }

                                if (result?.items is null)
                                {
                                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                    _logger.AddMessage($"Resposta inválida da API Totvs em product/v2/references/search (empresa {company.doc_company}, matriz {baseBranchCode}, página {page}): {body}");

                                    failed = true;
                                    break;
                                }

                                references.AddRange(result.items);

                                // a página volta com menos itens que o pageSize (o filtro de ativo/acabado é aplicado depois da paginação, testado):
                                // o fim é o totalPages, não uma página curta
                                totalPages = result.totalPages;
                                page++;
                            }

                            // fatia incompleta não é gravada, e as seguintes também não: a próxima execução continua do MAX(maxChangeFilterDate)
                            if (failed)
                                break;

                            if (references.Count == 0)
                                continue;

                            // cache de estado: referência -> referência gravada (com o maxChangeFilterDate), por empresa e matriz
                            var skusCache = await _cache.GetOrCreateAsync($"{SkusKey}:{company.doc_company}:{baseBranchCode}", async entry =>
                            {
                                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                                var list = await _totvsProductsRepository.GetSkusByIds(
                                    company.doc_company!,
                                    baseBranchCode,
                                    references.Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.referenceId)).Where(x => x != 0).Distinct().ToList()
                                );

                                return list
                                    .Where(x => x != null)
                                    .GroupBy(x => x!.referenceId)
                                    .ToDictionary(g => g.Key, g => g.First()!);
                            });

                            // mantém só as novas ou com maxChangeFilterDate mais recente que a gravada (sem data, regrava)
                            references.RemoveAll(x =>
                                skusCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(x.referenceId), out var y) &&
                                NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                            );

                            // um horário de lote por fatia: uma referência que volta noutra fatia precisa de horário maior para a exclusão das filhas
                            totalInseridos += await SaveReferences(company.doc_company!, baseBranchCode, _totvsService.GetBatchTime(), references, referencesJson, skusCache);
                        }
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
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa e matriz.
        /// Devolve a quantidade de referências gravadas.
        /// </summary>
        private async Task<int> SaveReferences(
            string doc_company,
            int baseBranchCode,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.Products.Reference> references,
            IReadOnlyDictionary<int, string> referencesJson,
            Dictionary<int, Domain.Totvs.Entities.Reads.Products.Sku>? skusCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.Products.Sku>();

            // a mesma referência pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var reference in references.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.referenceId) != 0).GroupBy(x => x.referenceId).Select(g => g.Last()))
            {
                var validations = _referenceValidator.Validate(reference);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"referência {reference.referenceId} (matriz {baseBranchCode}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.Products.Sku(doc_company, batchTime, baseBranchCode, reference));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            // o mesmo produto em duas referências do lote violaria a PK de TotvsSkusColorsProducts (não aconteceu na amostra real)
            var repeatedProducts = _listSomenteNovos.SelectMany(x => x.colors).SelectMany(x => x.products).GroupBy(x => x.code).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            if (repeatedProducts.Count > 0)
                _logger.AddMessage($"Matriz {baseBranchCode}: produto(s) em mais de uma referência do lote, gravado(s) só na última: {String.Join(", ", repeatedProducts)}");

            foreach (var product in repeatedProducts)
                foreach (var color in _listSomenteNovos.SelectMany(x => x.colors).Where(c => c.products.Any(p => p.code == product)).SkipLast(1))
                    color.products.RemoveAll(p => p.code == product);

            await _totvsProductsRepository.BulkInsertIntoTotvsSkus(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (referencesJson.TryGetValue(record.referenceId, out var json))
                    _logger.AddRecord($"{baseBranchCode}|{record.referenceId}", json);

                if (skusCache is not null)
                    skusCache[record.referenceId] = record;
            }

            return _listSomenteNovos.Count;
        }
    }
}

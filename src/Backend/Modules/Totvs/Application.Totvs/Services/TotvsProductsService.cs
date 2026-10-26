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

        // referência inteira na busca por código: a paginação é por produto e 1000 é o máximo da rota (102 produtos vieram numa página)
        private const int ReferencePageSize = 1000;

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

                            // a paginação da rota é por produto (count = produtos), e uma referência cujos produtos cruzam o limite da página
                            // vem em partes: as partes são unidas aqui por referenceId antes de virar entidade (decisão 34)
                            var merged = new Dictionary<int, JObject>();
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

                                JArray? items = null;
                                var pages = 0;

                                try
                                {
                                    // DateParseHandling.None: senão o JObject converte as datas e o ToObject as devolve na cultura do servidor
                                    using var reader = new JsonTextReader(new StringReader(response.Content ?? String.Empty)) { DateParseHandling = DateParseHandling.None };
                                    var root = JObject.Load(reader);

                                    items = root["items"] as JArray;
                                    pages = (int?)root["totalPages"] ?? 0;
                                }
                                catch (JsonException)
                                {
                                    // items continua null: tratado abaixo como resposta inválida (já veio resposta truncada no teste)
                                }

                                if (items is null)
                                {
                                    var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                    _logger.AddMessage($"Resposta inválida da API Totvs em product/v2/references/search (empresa {company.doc_company}, matriz {baseBranchCode}, página {page}): {body}");

                                    failed = true;
                                    break;
                                }

                                foreach (var item in items.OfType<JObject>())
                                {
                                    var referenceId = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["referenceId"]);

                                    if (referenceId == 0)
                                        continue;

                                    if (!merged.TryGetValue(referenceId, out var existing))
                                    {
                                        merged[referenceId] = item;
                                        continue;
                                    }

                                    // a parte seguinte da mesma referência: soma cores, produtos e medidas, e fica o maior maxChangeFilterDate
                                    MergeColors(existing, item);

                                    if (NullableConverters.ToNullableDateTime((string?)item["maxChangeFilterDate"]) > NullableConverters.ToNullableDateTime((string?)existing["maxChangeFilterDate"]))
                                        existing["maxChangeFilterDate"] = item["maxChangeFilterDate"];
                                }

                                // items são as referências dos produtos da página, então vêm menos itens que o pageSize: o fim é o totalPages
                                totalPages = pages;
                                page++;
                            }

                            // fatia incompleta não é gravada, e as seguintes também não: a próxima execução continua do MAX(maxChangeFilterDate)
                            if (failed)
                                break;

                            if (merged.Count == 0)
                                continue;

                            var references = merged.Values.Select(x => x.ToObject<Domain.Totvs.Dtos.Reads.Products.Reference>()).Where(x => x is not null).Select(x => x!).ToList();
                            var referencesJson = merged.ToDictionary(x => x.Key, x => x.Value.ToString(Formatting.None));

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
        /// busca a referência de um produto por código, grava nas tabelas de produtos (sempre, sem consultar o cache) e registra
        /// as chamadas em totvs.TotvsRequests. Duas etapas, porque a busca por productCodeList devolve a referência só com o
        /// produto filtrado (gravar assim apagaria os irmãos no merge): 1) productCodeList nas matrizes das marcas, na ordem de
        /// BaseBranchCodes, para achar a referência; 2) referenceCodeList (igualdade exata) sem filter.change, para a referência
        /// inteira. A paginação da rota é por produto (count = produtos), então as páginas são unidas por cor e produto.
        /// Sem o change o maxChangeFilterDate vem nulo: a referência fica sem a data, e o job a regrava quando ela mudar
        /// </summary>
        public async Task<bool?> GetProductByCode(string user_name, string doc_company, int code)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetProductByCode);

            // rota registrada no log de requisições (sem o token); o filtro vai no corpo
            const string route = "product/v2/references/search";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em TotvsRequests (igual ao Movidesk)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em totvs.TotvsParameters.");
                    return null;
                }

                // uma chamada à busca, com o mesmo option do job; null quando o login falhou. Quem chama registra em TotvsRequests
                async Task<(ApiResponse? response, string requestBody)> Search(object filter, int baseBranchCode, int page, int pageSize)
                {
                    var requestBody = JsonConvert.SerializeObject(new
                    {
                        filter,
                        option = new
                        {
                            branchInfoCode = baseBranchCode,
                            measurementTableOmniType = 2
                        },
                        page,
                        pageSize,
                        expand = Expand
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
                        _totvsService.SaveRequest(EnumJob.TotvsGetProductByCode, user_name, "POST", route, doc_company, code.ToString(), $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                        throw;
                    }

                    return (response, requestBody);
                }

                // 1) a referência do produto: a primeira marca em que ele existe
                var referenceId = 0;
                string? referenceCode = null;
                var foundBaseBranchCode = 0;

                foreach (var (baseBranchCode, brandClassificationCode) in BaseBranchCodes)
                {
                    // sem lojas da marca em TotvsBranches o job não roda para ela; aqui a busca por código não precisa delas
                    var (response, requestBody) = await Search(new
                    {
                        productCodeList = new[] { code },
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
                        }
                    }, baseBranchCode, 1, 1);

                    if (response is null)
                    {
                        // login falhou: nenhuma chamada à busca
                        _totvsService.SaveRequest(EnumJob.TotvsGetProductByCode, user_name, "POST", route, doc_company, code.ToString(), null, false, requestBody);
                        return null;
                    }

                    if (!response.IsSuccess)
                    {
                        var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                        _logger.AddMessage($"Erro da API Totvs em {route} (empresa {doc_company}, matriz {baseBranchCode}, produto {code}): {response.StatusCode} {body}");

                        _totvsService.SaveRequest(EnumJob.TotvsGetProductByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, false, requestBody);
                        return null;
                    }

                    Page<Domain.Totvs.Dtos.Reads.Products.Reference>? page = null;

                    try
                    {
                        page = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.Products.Reference>>(response.Content ?? String.Empty);
                    }
                    catch (JsonException)
                    {
                        // page continua null: tratado abaixo como resposta inválida
                    }

                    if (page?.items is null)
                    {
                        _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {doc_company}, matriz {baseBranchCode}, produto {code}): {response.Content}");

                        _totvsService.SaveRequest(EnumJob.TotvsGetProductByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, false, requestBody);
                        return null;
                    }

                    var reference = page.items.FirstOrDefault(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.referenceId) != 0);

                    // 200 com items vazio: o produto não é desta marca (ou não existe)
                    _totvsService.SaveRequest(EnumJob.TotvsGetProductByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, reference is not null, requestBody);

                    if (reference is null)
                        continue;

                    referenceId = ConverterValidatorsExtensions.ConvertToInt32Validation(reference.referenceId);
                    referenceCode = reference.ReferenceCode;
                    foundBaseBranchCode = baseBranchCode;
                    break;
                }

                if (referenceId == 0)
                {
                    _logger.AddMessage($"Produto {code} não encontrado na Totvs (empresa {doc_company}).");
                    return false;
                }

                if (String.IsNullOrWhiteSpace(referenceCode))
                {
                    _logger.AddMessage($"Produto {code} (referência {referenceId}, matriz {foundBaseBranchCode}): a referência veio sem ReferenceCode, não dá para lê-la inteira.");
                    return null;
                }

                // 2) a referência inteira, página a página (1000 produtos por página, o máximo da rota), unindo cores e produtos
                JObject? merged = null;
                var pageNumber = 1;
                var totalPages = 1;

                while (pageNumber <= totalPages)
                {
                    var (response, requestBody) = await Search(new
                    {
                        referenceCodeList = new[] { referenceCode },
                        branchInfo = new
                        {
                            branchCode = foundBaseBranchCode,
                            isActive = true,
                            isFinishedProduct = true
                        },
                        classifications = new[]
                        {
                            new
                            {
                                type = BrandClassificationType,
                                codeList = new[] { BaseBranchCodes[foundBaseBranchCode] }
                            }
                        }
                    }, foundBaseBranchCode, pageNumber, ReferencePageSize);

                    if (response is null)
                    {
                        _totvsService.SaveRequest(EnumJob.TotvsGetProductByCode, user_name, "POST", route, doc_company, code.ToString(), null, false, requestBody);
                        return null;
                    }

                    if (!response.IsSuccess)
                    {
                        var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                        _logger.AddMessage($"Erro da API Totvs em {route} (empresa {doc_company}, matriz {foundBaseBranchCode}, referência {referenceCode}, página {pageNumber}): {response.StatusCode} {body}");

                        _totvsService.SaveRequest(EnumJob.TotvsGetProductByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, false, requestBody);
                        return null;
                    }

                    JObject? item = null;
                    var pages = 0;

                    try
                    {
                        // sem DateParseHandling.None o JObject converte as datas em DateTime e o ToObject as devolve como string na
                        // cultura do servidor, e os validadores e conversores (en-US) as rejeitam (testado: 20 mensagens, uma por produto)
                        using var reader = new JsonTextReader(new StringReader(response.Content ?? String.Empty)) { DateParseHandling = DateParseHandling.None };
                        var root = JObject.Load(reader);

                        pages = (int?)root["totalPages"] ?? 0;
                        item = ((root["items"] as JArray) ?? [])
                            .OfType<JObject>()
                            .FirstOrDefault(x => ConverterValidatorsExtensions.ConvertToInt32Validation((string?)x["referenceId"]) == referenceId);
                    }
                    catch (JsonException)
                    {
                        // item continua null: tratado abaixo como resposta inválida
                    }

                    _totvsService.SaveRequest(EnumJob.TotvsGetProductByCode, user_name, "POST", route, doc_company, code.ToString(), response.Content, item is not null, requestBody);

                    if (item is null)
                    {
                        _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {doc_company}, matriz {foundBaseBranchCode}): a referência {referenceCode} ({referenceId}) não veio na página {pageNumber}.");
                        return null;
                    }

                    if (merged is null)
                        merged = item;
                    else
                        MergeColors(merged, item);

                    totalPages = pages;
                    pageNumber++;
                }

                var dto = merged!.ToObject<Domain.Totvs.Dtos.Reads.Products.Reference>();

                if (dto is null)
                {
                    _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {doc_company}): a referência {referenceCode} ({referenceId}) não pôde ser lida.");
                    return null;
                }

                // atualiza o cache do PostSearchReferences da matriz, se ele estiver carregado
                _cache.TryGetValue($"{SkusKey}:{doc_company}:{foundBaseBranchCode}", out Dictionary<int, Domain.Totvs.Entities.Reads.Products.Sku>? skusCache);

                await SaveReferences(doc_company, foundBaseBranchCode, _totvsService.GetBatchTime(), [dto], new Dictionary<int, string> { { referenceId, merged.ToString(Formatting.None) } }, skusCache);

                var products = (dto.colors ?? []).Sum(c => c.products?.Count ?? 0);
                _logger.AddMessage($"Concluída com sucesso: produto {code} (referência {referenceCode} / {referenceId}, matriz {foundBaseBranchCode}, {products} produto(s)) inserido ou atualizado, sem maxChangeFilterDate!");

                return true;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// une numa referência as cores e os produtos de outra página da mesma referência (cor por code, produto por code,
        /// medida por sku); as outras listas vêm inteiras em toda página e ficam as da primeira
        /// </summary>
        private static void MergeColors(JObject target, JObject source)
        {
            if (target["colors"] is not JArray targetColors)
            {
                targetColors = [];
                target["colors"] = targetColors;
            }

            foreach (var color in (source["colors"] as JArray ?? []).OfType<JObject>())
            {
                var code = (string?)color["code"];
                var existing = targetColors.OfType<JObject>().FirstOrDefault(c => (string?)c["code"] == code);

                if (existing is null)
                {
                    targetColors.Add(color.DeepClone());
                    continue;
                }

                MergeList(existing, color, "products", "code");
                MergeList(existing, color, "measurementTableOmni", "sku");
            }
        }

        private static void MergeList(JObject target, JObject source, string listName, string keyName)
        {
            if (target[listName] is not JArray targetList)
            {
                targetList = [];
                target[listName] = targetList;
            }

            var keys = targetList.OfType<JObject>().Select(x => (string?)x[keyName]).ToHashSet();

            foreach (var item in (source[listName] as JArray ?? []).OfType<JObject>())
                if (keys.Add((string?)item[keyName]))
                    targetList.Add(item.DeepClone());
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

            // as páginas da mesma referência já foram unidas por quem chamou; o GroupBy só protege a PK
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

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
    /// vendedores da Totvs (API Seller). O método faz as próprias chamadas à API; o comum (horário do lote,
    /// parâmetros, lojas e token) vem do ITotvsService
    /// </summary>
    public class TotvsSellersService : ITotvsSellersService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsSellersRepository _totvsSellersRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Sellers.Seller> _sellerValidator;

        private const string SellersKey = "Totvs:Sellers";

        // máximo da rota (swagger da API Seller)
        private const int SellersPageSize = 500;

        public TotvsSellersService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsSellersRepository totvsSellersRepository,
            IValidator<Domain.Totvs.Dtos.Reads.Sellers.Seller> sellerValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsSellersRepository, _sellerValidator) =
            (apiClient, logger, cache, totvsService, totvsSellersRepository, sellerValidator);

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

                    // as datas da Totvs são do horário local, sem fuso (o "Z" é ignorado pela API, testado). Banco vazio: 5 meses
                    var lastChange = await _totvsSellersRepository.GetSellersLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : batchTime.AddMonths(-5);

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

                    totalInseridos += await SaveSellers(company.doc_company!, batchTime, sellers, sellersJson, sellersCache);
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
        /// Devolve a quantidade de vendedores gravados.
        /// </summary>
        private async Task<int> SaveSellers(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.Sellers.Seller> sellers,
            IReadOnlyDictionary<int, string> sellersJson,
            Dictionary<int, Domain.Totvs.Entities.Reads.Sellers.Seller>? sellersCache)
        {
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

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.Sellers.Seller(doc_company, batchTime, seller));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsSellersRepository.BulkInsertIntoTotvsSellers(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (sellersJson.TryGetValue(record.sellerCode, out var json))
                    _logger.AddRecord(record.sellerCode.ToString(), json);

                if (sellersCache is not null)
                    sellersCache[record.sellerCode] = record;
            }

            return _listSomenteNovos.Count;
        }
    }
}

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
    /// contas a pagar da Totvs (API Accounts Payable): duplicatas e fechamentos de comissão. Cada método faz as próprias
    /// chamadas à API; o comum (horário do lote, parâmetros, lojas e token) vem do ITotvsService
    /// </summary>
    public class TotvsAccountsPayableService : ITotvsAccountsPayableService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsAccountsPayableRepository _totvsAccountsPayableRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.AccountsPayable.Duplicate> _duplicateValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.AccountsPayable.CommissionPaid> _commissionPaidValidator;

        private const string DuplicatesKey = "Totvs:Duplicates";
        private const string CommissionsPaidHashKey = "Totvs:CommissionsPaidHash";

        // máximo das duas rotas (swagger da API Accounts Payable)
        private const int PageSize = 100;

        // a carga inicial das duplicatas (10 meses) é lida em fatias, e cada fatia é gravada ao terminar (como nos demais)
        private const int DuplicatesWindowDays = 30;

        // as comissões não têm filtro de alteração: os últimos 10 meses pela data do fechamento são relidos a cada execução
        private const int CommissionsPaidMonths = 10;

        public TotvsAccountsPayableService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsAccountsPayableRepository totvsAccountsPayableRepository,
            IValidator<Domain.Totvs.Dtos.Reads.AccountsPayable.Duplicate> duplicateValidator,
            IValidator<Domain.Totvs.Dtos.Reads.AccountsPayable.CommissionPaid> commissionPaidValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsAccountsPayableRepository, _duplicateValidator, _commissionPaidValidator) =
            (apiClient, logger, cache, totvsService, totvsAccountsPayableRepository, duplicateValidator, commissionPaidValidator);

        public async Task<bool?> PostSearchDuplicates()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchDuplicates);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // branchCodeList é obrigatório (máx. 900 itens, swagger): todas as lojas da holding
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    var endDate = _totvsService.GetBatchTime();

                    // as datas da Totvs são do horário local, sem fuso (o "Z" é ignorado pela API, testado). Banco vazio: 10 meses
                    var lastChange = await _totvsAccountsPayableRepository.GetDuplicatesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : endDate.AddMonths(-10);

                    for (var windowStart = since; windowStart < endDate; windowStart = windowStart.AddDays(DuplicatesWindowDays))
                    {
                        var windowEnd = windowStart.AddDays(DuplicatesWindowDays) < endDate ? windowStart.AddDays(DuplicatesWindowDays) : endDate;

                        var duplicates = new List<Domain.Totvs.Dtos.Reads.AccountsPayable.Duplicate>();
                        var duplicatesJson = new Dictionary<(int, int, long, int), string>();
                        var page = 1;
                        var totalPages = 1;
                        var failed = false;

                        while (page <= totalPages)
                        {
                            // expense vem sempre; o único expand da rota é classifications (o "expense" do código antigo era ignorado, testado)
                            var requestBody = JsonConvert.SerializeObject(new
                            {
                                filter = new
                                {
                                    change = new
                                    {
                                        startDate = $"{windowStart:yyyy-MM-ddTHH:mm:ss}",
                                        endDate = $"{windowEnd:yyyy-MM-ddTHH:mm:ss}"
                                    },
                                    branchCodeList = branchCodes
                                },
                                order = "maxChangeFilterDate",
                                page,
                                pageSize = PageSize,
                                expand = "classifications"
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
                                    "accounts-payable/v2/duplicates/search",
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
                                    _logger.AddMessage($"Erro da API Totvs em accounts-payable/v2/duplicates/search (empresa {company.doc_company}, janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}, página {page}): {response.StatusCode} {body}");
                                }

                                failed = true;
                                break;
                            }

                            Page<Domain.Totvs.Dtos.Reads.AccountsPayable.Duplicate>? result = null;

                            try
                            {
                                result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.AccountsPayable.Duplicate>>(response.Content ?? String.Empty);

                                foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                                {
                                    var key = (
                                        ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["branchCode"]),
                                        ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["supplierCode"]),
                                        ConverterValidatorsExtensions.ConvertToInt64Validation((string?)item["duplicateCode"]),
                                        ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["installmentCode"])
                                    );

                                    if (key.Item1 != 0 && key.Item3 != 0)
                                        duplicatesJson[key] = item.ToString(Formatting.None);
                                }
                            }
                            catch (JsonException)
                            {
                                // result continua null: tratado abaixo como resposta inválida
                            }

                            if (result?.items is null)
                            {
                                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                _logger.AddMessage($"Resposta inválida da API Totvs em accounts-payable/v2/duplicates/search (empresa {company.doc_company}, página {page}): {body}");

                                failed = true;
                                break;
                            }

                            duplicates.AddRange(result.items);

                            totalPages = result.totalPages;
                            page++;
                        }

                        // fatia incompleta não é gravada, e as seguintes também não: a próxima execução continua do MAX(maxChangeFilterDate)
                        if (failed)
                            break;

                        if (duplicates.Count == 0)
                            continue;

                        // cache de estado: (loja, fornecedor, duplicata, parcela) -> parcela gravada (com o maxChangeFilterDate), por empresa
                        var duplicatesCache = await _cache.GetOrCreateAsync($"{DuplicatesKey}:{company.doc_company}", async entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                            var list = await _totvsAccountsPayableRepository.GetDuplicatesByKeys(
                                company.doc_company!,
                                duplicates.Select(DuplicateKey).Where(x => x.branchCode != 0 && x.duplicateCode != 0).Distinct().ToList()
                            );

                            return list
                                .Where(x => x != null)
                                .GroupBy(x => (x!.branchCode, x.supplierCode, x.duplicateCode, x.installmentCode))
                                .ToDictionary(g => g.Key, g => g.First()!);
                        });

                        // mantém só as novas ou com maxChangeFilterDate mais recente que a gravada (sem data, regrava)
                        duplicates.RemoveAll(x =>
                            duplicatesCache!.TryGetValue(DuplicateKey(x), out var y) &&
                            NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                        );

                        // um horário de lote por fatia: uma parcela que volta noutra fatia precisa de horário maior para a exclusão das filhas
                        totalInseridos += await SaveDuplicates(company.doc_company!, _totvsService.GetBatchTime(), duplicates, duplicatesJson, duplicatesCache);
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

        public async Task<bool?> PostSearchComissionsPaid()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchComissionsPaid);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // closingCompanyCode é obrigatório e é uma empresa só (sem ele a API responde 400, testado): uma chamada por loja
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    var endDate = _totvsService.GetBatchTime();
                    var startDate = endDate.AddMonths(-CommissionsPaidMonths);

                    var commissions = new List<Domain.Totvs.Dtos.Reads.AccountsPayable.CommissionPaid>();
                    var commissionsJson = new Dictionary<(int, int, string), string>();

                    foreach (var branchCode in branchCodes)
                    {
                        var page = 1;
                        var totalPages = 1;

                        while (page <= totalPages)
                        {
                            // a rota não tem filtro de alteração nem maxChangeFilterDate: a janela é pela data do fechamento
                            var requestBody = JsonConvert.SerializeObject(new
                            {
                                filter = new
                                {
                                    closingCompanyCode = branchCode,
                                    startClosingDate = $"{startDate:yyyy-MM-ddTHH:mm:ss}",
                                    endClosingDate = $"{endDate:yyyy-MM-ddTHH:mm:ss}"
                                },
                                order = "closingDate",
                                page,
                                pageSize = PageSize,
                                expand = "accountMovements"
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
                                    "accounts-payable/v2/comissions-paid/search",
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
                                    _logger.AddMessage($"Erro da API Totvs em accounts-payable/v2/comissions-paid/search (empresa {company.doc_company}, loja {branchCode}, página {page}): {response.StatusCode} {body}");
                                }

                                // sem janela pelo banco, uma loja com erro não atrapalha as outras: ela é lida de novo na próxima execução
                                break;
                            }

                            Page<Domain.Totvs.Dtos.Reads.AccountsPayable.CommissionPaid>? result = null;

                            try
                            {
                                result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.AccountsPayable.CommissionPaid>>(response.Content ?? String.Empty);

                                foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                                {
                                    var key = (
                                        ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["closingBranchCode"]),
                                        ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["closingCode"]),
                                        ConverterValidatorsExtensions.StringLengthValidation((string?)item["commissionedCpfCnpj"], 14)
                                    );

                                    if (key.Item1 != 0 && key.Item2 != 0)
                                        commissionsJson[key] = item.ToString(Formatting.None);
                                }
                            }
                            catch (JsonException)
                            {
                                // result continua null: tratado abaixo como resposta inválida
                            }

                            if (result?.items is null)
                            {
                                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                _logger.AddMessage($"Resposta inválida da API Totvs em accounts-payable/v2/comissions-paid/search (empresa {company.doc_company}, loja {branchCode}, página {page}): {body}");
                                break;
                            }

                            commissions.AddRange(result.items);

                            totalPages = result.totalPages;
                            page++;
                        }
                    }

                    if (commissions.Count == 0)
                        continue;

                    // cache de hash por fechamento (sem data de alteração na rota): só grava o que mudou desde a última leitura
                    var hashCache = _cache.GetOrCreate($"{CommissionsPaidHashKey}:{company.doc_company}", entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        return new Dictionary<(int, int, string), string>();
                    })!;

                    var changed = commissions
                        .Where(x => commissionsJson.TryGetValue(CommissionKey(x), out var json) && !(hashCache.TryGetValue(CommissionKey(x), out var hash) && hash == _logger.ComputeSha256Hash(json)))
                        .ToList();

                    totalInseridos += await SaveCommissionsPaid(company.doc_company!, _totvsService.GetBatchTime(), changed, commissionsJson, hashCache);
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
        /// chave da duplicata: o mesmo duplicateCode volta uma vez por parcela e pode se repetir entre fornecedores e lojas (testado)
        /// </summary>
        private static (int branchCode, int supplierCode, long duplicateCode, int installmentCode) DuplicateKey(Domain.Totvs.Dtos.Reads.AccountsPayable.Duplicate x) =>
            (ConverterValidatorsExtensions.ConvertToInt32Validation(x.branchCode), ConverterValidatorsExtensions.ConvertToInt32Validation(x.supplierCode),
             ConverterValidatorsExtensions.ConvertToInt64Validation(x.duplicateCode), ConverterValidatorsExtensions.ConvertToInt32Validation(x.installmentCode));

        /// <summary>
        /// chave do fechamento de comissão: loja e número do fechamento + CPF/CNPJ do comissionado (uma linha por comissionado)
        /// </summary>
        private static (int, int, string) CommissionKey(Domain.Totvs.Dtos.Reads.AccountsPayable.CommissionPaid x) =>
            (ConverterValidatorsExtensions.ConvertToInt32Validation(x.closingBranchCode), ConverterValidatorsExtensions.ConvertToInt32Validation(x.closingCode),
             ConverterValidatorsExtensions.StringLengthValidation(x.commissionedCpfCnpj, 14));

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de parcelas gravadas.
        /// </summary>
        private async Task<int> SaveDuplicates(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.AccountsPayable.Duplicate> duplicates,
            IReadOnlyDictionary<(int, int, long, int), string> duplicatesJson,
            Dictionary<(int branchCode, int supplierCode, long duplicateCode, int installmentCode), Domain.Totvs.Entities.Reads.AccountsPayable.Duplicate>? duplicatesCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.AccountsPayable.Duplicate>();

            // a mesma parcela pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var duplicate in duplicates.Where(x => DuplicateKey(x).branchCode != 0 && DuplicateKey(x).duplicateCode != 0).GroupBy(DuplicateKey).Select(g => g.Last()))
            {
                var validations = _duplicateValidator.Validate(duplicate);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"duplicata {duplicate.branchCode}/{duplicate.supplierCode}/{duplicate.duplicateCode}/{duplicate.installmentCode}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.AccountsPayable.Duplicate(doc_company, batchTime, duplicate));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsAccountsPayableRepository.BulkInsertIntoTotvsDuplicates(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                var key = (record.branchCode, record.supplierCode, record.duplicateCode, record.installmentCode);

                if (duplicatesJson.TryGetValue(key, out var json))
                    _logger.AddRecord($"{record.branchCode}|{record.supplierCode}|{record.duplicateCode}|{record.installmentCode}", json);

                if (duplicatesCache is not null)
                    duplicatesCache[key] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache de hash da empresa.
        /// Devolve a quantidade de fechamentos gravados.
        /// </summary>
        private async Task<int> SaveCommissionsPaid(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.AccountsPayable.CommissionPaid> commissions,
            IReadOnlyDictionary<(int, int, string), string> commissionsJson,
            Dictionary<(int, int, string), string> hashCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.AccountsPayable.CommissionPaid>();

            foreach (var commission in commissions.GroupBy(CommissionKey).Select(g => g.Last()))
            {
                var validations = _commissionPaidValidator.Validate(commission);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"fechamento de comissão {commission.closingBranchCode}/{commission.closingCode}/{commission.commissionedCpfCnpj}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.AccountsPayable.CommissionPaid(doc_company, batchTime, commission));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsAccountsPayableRepository.BulkInsertIntoTotvsCommissionsPaid(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                var key = (record.closingBranchCode, record.closingCode, record.commissionedCpfCnpj);

                if (commissionsJson.TryGetValue(key, out var json))
                {
                    _logger.AddRecord($"{record.closingBranchCode}|{record.closingCode}|{record.commissionedCpfCnpj}", json);
                    hashCache[key] = _logger.ComputeSha256Hash(json);
                }
            }

            return _listSomenteNovos.Count;
        }
    }
}

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

        // grupo das escritas desta API na configuração (Totvs:Writes:AccountsPayable:Enabled, T4)
        private const string WritesGroup = "AccountsPayable";

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
                        // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
                        // Devolve a quantidade de parcelas gravadas.
                        var batchTime = _totvsService.GetBatchTime();

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

                            _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.AccountsPayable.Duplicate(company.doc_company!, batchTime, duplicate));
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _totvsAccountsPayableRepository.BulkInsertIntoTotvsDuplicates(_listSomenteNovos, _logger.GetExecutionGuid());

                        foreach (var record in _listSomenteNovos)
                        {
                            var key = (record.branchCode, record.supplierCode, record.duplicateCode, record.installmentCode);

                            if (duplicatesJson.TryGetValue(key, out var json))
                                _logger.AddRecord($"{record.branchCode}|{record.supplierCode}|{record.duplicateCode}|{record.installmentCode}", json);

                            if (duplicatesCache is not null)
                                duplicatesCache[key] = record;
                        }

                        totalInseridos += _listSomenteNovos.Count;
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

                    // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache de hash da empresa.
                    // Devolve a quantidade de fechamentos gravados.
                    var batchTime = _totvsService.GetBatchTime();

                    var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.AccountsPayable.CommissionPaid>();

                    foreach (var commission in changed.GroupBy(CommissionKey).Select(g => g.Last()))
                    {
                        var validations = _commissionPaidValidator.Validate(commission);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"fechamento de comissão {commission.closingBranchCode}/{commission.closingCode}/{commission.commissionedCpfCnpj}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.AccountsPayable.CommissionPaid(company.doc_company!, batchTime, commission));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

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
        /// troca o estágio de uma parcela (docs/PLANO-TOTVS-METODOS-FALTANTES.md §1.2 e §2.3). A Totvs valida a transição: a
        /// busca de duplicatas não devolve o estágio, então não há como conferir antes pelo banco (D4)
        /// </summary>
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostDuplicatesInstallmentsStage(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.AccountsPayable.ChangeInstallmentStage body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostDuplicatesInstallmentsStage);

            // rota registrada no log de requisições (sem o token)
            const string route = "accounts-payable/v2/duplicates/installments/stage";
            var key = $"{body.branchCnpj}|{(String.IsNullOrEmpty(body.supplierCpfCnpj) ? body.supplierCode.ToString() : body.supplierCpfCnpj)}|{body.duplicateCode}|{body.installmentCode}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                {
                    var disabled = $"Escritas de contas a pagar desligadas (Totvs:Writes:{WritesGroup}:Enabled).";
                    _logger.AddMessage(disabled);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = disabled };
                }

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em TotvsRequests (igual às leituras sob demanda)
                    var notFound = $"Empresa {doc_company} não cadastrada em totvs.TotvsParameters.";
                    _logger.AddMessage(notFound);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = notFound };
                }

                if (await _totvsService.GetBranchCodeByCnpj(company, body.branchCnpj!) is null)
                {
                    var branchNotFound = $"CNPJ {body.branchCnpj} não é uma loja da empresa {doc_company} em totvs.TotvsBranches.";
                    _logger.AddMessage(branchNotFound);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = branchNotFound };
                }

                // nulos não vão: o fornecedor que não foi informado fica fora do corpo; o enum vai como número
                var requestBody = JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                // uma nova tentativa só em 401 (renova o token) e 429 (espera o Retry-After): nos dois a Totvs não processou o
                // pedido. Em 502/503/504 não há nova tentativa: a escrita pode ter sido feita antes de o gateway cair
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

                        break;
                    }
                }
                catch (Exception ex)
                {
                    // sem resposta (timeout, rede): a escrita pode ter sido feita; conferir antes de repetir
                    var failure = $"Exceção ao chamar a API: {ex.Message.TrimEnd('.')}";
                    _totvsService.SaveRequest(EnumJob.TotvsPostDuplicatesInstallmentsStage, user_name, "POST", route, doc_company, key, failure, false, requestBody, 0);
                    _logger.AddMessage($"Parcela {key} (empresa {doc_company}): {failure}. Confira o estágio na Totvs antes de repetir.");

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, apiCalled = true, message = $"{failure}. Confira o estágio na Totvs antes de repetir." };
                }

                if (response is null)
                {
                    // login falhou: nenhuma chamada à rota
                    _totvsService.SaveRequest(EnumJob.TotvsPostDuplicatesInstallmentsStage, user_name, "POST", route, doc_company, key, null, false, requestBody);

                    return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = $"Falha na autenticação da Totvs (empresa {doc_company}). Veja o log da execução." };
                }

                _totvsService.SaveRequest(EnumJob.TotvsPostDuplicatesInstallmentsStage, user_name, "POST", route, doc_company, key, response.Content, response.IsSuccess, requestBody, response.StatusCode);

                if (!response.IsSuccess)
                {
                    var message = TotvsMessage(response);
                    _logger.AddMessage($"Parcela {key} (empresa {doc_company}): {response.StatusCode} {message}");

                    return new Domain.Totvs.Dtos.Writes.Result
                    {
                        success = false,
                        key = key,
                        statusCode = response.StatusCode,
                        apiCalled = true,
                        message = response.StatusCode >= 500 ? $"{message.TrimEnd('.')}. Confira o estágio na Totvs antes de repetir." : message,
                        response = response.Content,
                        retryAfterSeconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var seconds) ? seconds : null
                    };
                }

                // o banco local não é atualizado aqui: a busca de duplicatas não traz o estágio (§2.3)
                _logger.AddMessage($"Concluída com sucesso: parcela {key} (empresa {doc_company}) passou para o estágio {(int)body.newStage!}.");

                return new Domain.Totvs.Dtos.Writes.Result { success = true, key = key, statusCode = response.StatusCode, apiCalled = true, response = response.Content };
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// inclui uma duplicata a pagar (docs/PLANO-TOTVS-METODOS-FALTANTES.md §2.1) pela base comum das escritas. Não idempotente: o mesmo
        /// corpo em 10 minutos volta 409 (T5). A duplicata chega ao banco pelo próximo PostSearchDuplicates (D3)
        /// </summary>
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostDuplicates(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.AccountsPayable.Duplicate body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostDuplicates);

            var key = $"{body.branchCnpj}|{body.supplierCpfCnpj}|{body.duplicateCode}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                if (await _totvsService.GetBranchCodeByCnpj(company, body.branchCnpj!) is null)
                    return _totvsService.BranchNotFound(doc_company, key, [body.branchCnpj!]);

                return await _totvsService.Write(company, EnumJob.TotvsPostDuplicates, user_name, key, HttpMethod.Post, "accounts-payable/v2/duplicates", body,
                    "Confira a duplicata pelo duplicates/search antes de repetir.", repeatLock: true);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// agrupa parcelas numa duplicata nova (docs/PLANO-TOTVS-METODOS-FALTANTES.md §2.2) pela base comum das escritas. A loja do corpo e as
        /// das parcelas agrupadas precisam ser da empresa; o mesmo corpo em 10 minutos volta 409 (T5). A resposta traz a chave gerada
        /// </summary>
        public async Task<Domain.Totvs.Dtos.Writes.Result> PostGroupDuplicates(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.AccountsPayable.GroupDuplicate body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostGroupDuplicates);

            var key = $"{body.branchCnpj}|{body.supplierCpfCnpj}|{body.duplicateCode}";

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                var outside = new List<string>();

                foreach (var cnpj in body.groupInstallments!.Select(x => x.branchCnpj!).Prepend(body.branchCnpj!).Distinct())
                    if (await _totvsService.GetBranchCodeByCnpj(company, cnpj) is null)
                        outside.Add(cnpj);

                if (outside.Count > 0)
                    return _totvsService.BranchNotFound(doc_company, key, outside);

                return await _totvsService.Write(company, EnumJob.TotvsPostGroupDuplicates, user_name, key, HttpMethod.Post, "accounts-payable/v2/group-duplicates", body,
                    "Confira a duplicata agrupadora pelo duplicates/search antes de repetir.", repeatLock: true);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// mensagens da Totvs num erro: o 400 traz DomainNotificationMessage[] (code, message, detailedMessage); outro corpo vai
        /// como veio, cortado
        /// </summary>
        private static string TotvsMessage(ApiResponse response)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(response.Content) && JToken.Parse(response.Content) is JArray messages)
                {
                    var text = String.Join(" | ", messages.OfType<JObject>().Select(m => $"{m["code"]}: {m["message"]}").Where(m => m != ": "));

                    if (!String.IsNullOrEmpty(text))
                        return text;
                }
            }
            catch (JsonReaderException)
            {
                // corpo em texto: usa o corpo como veio
            }

            if (String.IsNullOrWhiteSpace(response.Content))
                return $"A Totvs devolveu {response.StatusCode} sem corpo.";

            return response.Content.Length > 800 ? response.Content[..800] : response.Content;
        }
    }
}

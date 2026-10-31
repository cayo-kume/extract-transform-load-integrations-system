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
using Application.Shared.Attributes;

namespace Application.Totvs.Services
{
    /// <summary>
    /// notas fiscais da Totvs. O método faz as próprias chamadas à API; o comum (horário do lote, parâmetros, lojas e token)
    /// vem do ITotvsService
    /// </summary>
    public class TotvsInvoicesService : ITotvsInvoicesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsService _totvsService;
        private readonly ITotvsInvoicesRepository _totvsInvoicesRepository;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Invoices.Invoice> _invoiceValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Invoices.InvoiceDisable> _invoiceDisableValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Invoices.CostCenter> _costCenterValidator;

        private const string InvoicesKey = "Totvs:Invoices";
        private const string InvoicesDisableKey = "Totvs:InvoicesDisable";

        // máximo das rotas de inutilização e de centro de custo (o swagger do cost-center diz 1000, mas a rota recusa mais de 100, testado)
        private const int FiscalCatalogsPageSize = 100;

        // decisão do documento (§10.1); com o expand completo, 100 notas levam poucos segundos (testado)
        private const int InvoicesPageSize = 100;

        // a carga inicial (5 meses) é lida em fatias, e cada fatia é gravada ao terminar (como nos pedidos)
        private const int InvoicesWindowDays = 30;

        // expand da documentação (swagger da API Fiscal), sem referencedInvoices: a API Totvs está com problemas para devolver
        // os dados quando esse campo é buscado. Para algumas devoluções de venda a busca responde 400
        // (InvoiceDomainService.GetReferencedInvoices, "Object reference not set...") e a página inteira se perde (testado: loja 7,
        // sequência 42572, 21/10/2026; 9 de 1.272 notas em 21 a 23/10). Sem ele, TotvsInvoicesItemsProductsReferenced fica vazia.
        // Se for necessário, dá para usá-lo trocando pela linha abaixo: o ReadPage já trata o 400, relendo a página em partes e
        // lendo sem referencedInvoices só a nota que falha (a leitura fica bem mais lenta: 3 dias levaram ~7 min em vez de 13 chamadas)
        private const string Expand = "shippingCompany,salesOrder,person,items,barCodes,taxes,payments,rateDifferential,relatedProductionOrder,observationNF,observationNFE,referencedTaxInvoices";
        //private const string Expand = "shippingCompany,salesOrder,person,items,barCodes,taxes,payments,referencedInvoices,rateDifferential,relatedProductionOrder,observationNF,observationNFE,referencedTaxInvoices";

        private const string ReferencedInvoicesError = "GetReferencedInvoices";

        public TotvsInvoicesService(
            [FromKeyedServices("TotvsAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            ITotvsService totvsService,
            ITotvsInvoicesRepository totvsInvoicesRepository,
            IValidator<Domain.Totvs.Dtos.Reads.Invoices.Invoice> invoiceValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Invoices.InvoiceDisable> invoiceDisableValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Invoices.CostCenter> costCenterValidator
        ) =>
            (_apiClient, _logger, _cache, _totvsService, _totvsInvoicesRepository, _invoiceValidator, _invoiceDisableValidator, _costCenterValidator) =
            (apiClient, logger, cache, totvsService, totvsInvoicesRepository, invoiceValidator, invoiceDisableValidator, costCenterValidator);

        [JobVersion("1.0.0")]
        public async Task<bool?> PostSearchInvoices()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchInvoices);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // branchCodeList é obrigatório (swagger): todas as lojas da holding
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    var endDate = _totvsService.GetBatchTime();

                    // as datas da Totvs são do horário local, sem fuso (o "Z" é ignorado pela API, testado). Banco vazio: desde 01/01/2025
                    var lastChange = await _totvsInvoicesRepository.GetInvoicesLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2025, 1, 1);

                    for (var windowStart = since; windowStart < endDate; windowStart = windowStart.AddDays(InvoicesWindowDays))
                    {
                        var windowEnd = windowStart.AddDays(InvoicesWindowDays) < endDate ? windowStart.AddDays(InvoicesWindowDays) : endDate;

                        var invoices = new List<Domain.Totvs.Dtos.Reads.Invoices.Invoice>();
                        var invoicesJson = new Dictionary<(int, int, DateTime), string>();

                        // uma página da busca; null quando a chamada falhou (a mensagem já foi para o log)
                        async Task<Page<Domain.Totvs.Dtos.Reads.Invoices.Invoice>?> Search(int page, int pageSize, string expand)
                        {
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
                                pageSize,
                                expand
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
                                    "fiscal/v2/invoices/search",
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

                            if (response is null)
                                return null;

                            // o erro do referencedInvoices (só com ele no Expand) é tratado por quem chamou (ReadPage), sem mensagem aqui
                            if (response.StatusCode == 400 && expand.Contains("referencedInvoices") && response.Content?.Contains(ReferencedInvoicesError) == true)
                                throw new ReferencedInvoicesException();

                            if (!response.IsSuccess)
                            {
                                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                _logger.AddMessage($"Erro da API Totvs em fiscal/v2/invoices/search (empresa {company.doc_company}, janela {windowStart:dd/MM/yyyy HH:mm} a {windowEnd:dd/MM/yyyy HH:mm}, página {page} de {pageSize}): {response.StatusCode} {body}");

                                return null;
                            }

                            Page<Domain.Totvs.Dtos.Reads.Invoices.Invoice>? result = null;

                            try
                            {
                                result = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.Invoices.Invoice>>(response.Content ?? String.Empty);

                                foreach (var item in (JObject.Parse(response.Content!)["items"] as JArray) ?? [])
                                {
                                    var key = (ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["branchCode"]), ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["invoiceSequence"]), item.Value<DateTime?>("invoiceDate") ?? default);

                                    if (key.Item1 != 0 && key.Item2 != 0 && key.Item3 != default)
                                        invoicesJson[key] = item.ToString(Formatting.None);
                                }
                            }
                            catch (Exception ex) when (ex is JsonException or FormatException or InvalidCastException)
                            {
                                // result continua null: tratado abaixo como resposta inválida
                            }

                            if (result?.items is null)
                            {
                                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                                _logger.AddMessage($"Resposta inválida da API Totvs em fiscal/v2/invoices/search (empresa {company.doc_company}, página {page} de {pageSize}): {body}");

                                return null;
                            }

                            return result;
                        }

                        // lê a página; com referencedInvoices no Expand (hoje comentado), no 400 dele relê o mesmo trecho em páginas 10 vezes menores (100 -> 10 -> 1)
                        // e só a nota que falha sozinha vem sem referencedInvoices. A ordem é por maxChangeFilterDate, que não muda
                        // entre as chamadas; uma nota que muda no meio da leitura volta na próxima execução (janela de 1 h)
                        async Task<Page<Domain.Totvs.Dtos.Reads.Invoices.Invoice>?> ReadPage(int page, int pageSize)
                        {
                            try
                            {
                                return await Search(page, pageSize, Expand);
                            }
                            catch (ReferencedInvoicesException) when (pageSize == 1)
                            {
                                var result = await Search(page, 1, Expand.Replace("referencedInvoices,", String.Empty));

                                foreach (var x in result?.items ?? [])
                                    _logger.AddMessage($"Nota {x.branchCode}/{x.invoiceSequence} de {x.invoiceDate} (empresa {company.doc_company}) gravada sem as notas referenciadas dos produtos (referencedInvoices): a API Totvs devolve erro 400 em {ReferencedInvoicesError} para ela.");

                                return result;
                            }
                            catch (ReferencedInvoicesException)
                            {
                                var size = pageSize / 10;
                                var items = new List<Domain.Totvs.Dtos.Reads.Invoices.Invoice>();
                                var count = 0;

                                for (var subPage = (page - 1) * 10 + 1; subPage <= page * 10; subPage++)
                                {
                                    var part = await ReadPage(subPage, size);

                                    if (part is null)
                                        return null;

                                    items.AddRange(part.items!);
                                    count = part.count;   // total da busca, igual em qualquer tamanho de página

                                    // fim da busca
                                    if (part.items!.Count < size)
                                        break;
                                }

                                return new Page<Domain.Totvs.Dtos.Reads.Invoices.Invoice> { items = items, count = count, totalPages = (count + pageSize - 1) / pageSize };
                            }
                        }

                        var page = 1;
                        var totalPages = 1;
                        var failed = false;

                        while (page <= totalPages)
                        {
                            var result = await ReadPage(page, InvoicesPageSize);

                            if (result is null)
                            {
                                failed = true;
                                break;
                            }

                            invoices.AddRange(result.items!);

                            totalPages = result.totalPages;
                            page++;
                        }

                        // fatia incompleta não é gravada, e as seguintes também não: a próxima execução continua do MAX(maxChangeFilterDate)
                        if (failed)
                            break;

                        if (invoices.Count == 0)
                            continue;

                        // cache de estado: (loja, sequência, data) -> nota gravada (com o maxChangeFilterDate), por empresa
                        var invoicesCache = await _cache.GetOrCreateAsync($"{InvoicesKey}:{company.doc_company}", async entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                            var list = await _totvsInvoicesRepository.GetInvoicesByKeys(
                                company.doc_company!,
                                invoices.Select(Key).Where(IsValidKey).Distinct().ToList()
                            );

                            return list
                                .Where(x => x != null)
                                .GroupBy(x => (x!.branchCode, x.invoiceSequence, x.invoiceDate))
                                .ToDictionary(g => g.Key, g => g.First()!);
                        });

                        // mantém só as novas ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                        invoices.RemoveAll(x =>
                            invoicesCache!.TryGetValue(Key(x), out var y) &&
                            NullableConverters.ToNullableDateTime(x.maxChangeFilterDate) <= y.maxChangeFilterDate
                        );

                        // um horário de lote por fatia: uma nota que volta noutra fatia precisa de horário maior para a exclusão das filhas
                        totalInseridos += await SaveInvoices(company.doc_company!, _totvsService.GetBatchTime(), invoices, invoicesJson, invoicesCache);
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
        /// busca uma nota fiscal por loja + sequência, grava nas tabelas de notas (sempre, sem consultar o cache)
        /// e registra a chamada em totvs.TotvsRequests. A API limita o filter.change a 6 meses, então a nota é procurada
        /// primeiro nos últimos 6 meses (com maxChangeFilterDate) e, se não vier, sem o change (gravada sem a data)
        /// </summary>
        public async Task<bool?> GetInvoiceByKey(string user_name, string doc_company, int branchCode, int invoiceSequence)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetInvoiceByKey);

            // rota registrada no log de requisições (sem o token); o filtro vai no corpo
            const string route = "fiscal/v2/invoices/search";
            var key = $"{branchCode}|{invoiceSequence}";
            bool? result = null;

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em TotvsRequests (igual ao Movidesk)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em totvs.TotvsParameters.");
                    return null;
                }

                var batchTime = _totvsService.GetBatchTime();

                foreach (var withChange in new[] { true, false })
                {
                    // 1ª tentativa: change dos últimos 6 meses (o máximo da rota, testado), que preenche o maxChangeFilterDate;
                    // 2ª: sem change, para notas mais antigas (o job as regrava se voltarem numa janela, porque ficam sem a data)
                    var requestBody = JsonConvert.SerializeObject(new
                    {
                        filter = new
                        {
                            branchCodeList = new[] { branchCode },
                            invoiceSequenceList = new[] { invoiceSequence },
                            change = withChange
                                ? new { startDate = $"{batchTime.AddMonths(-6).AddDays(1):yyyy-MM-ddTHH:mm:ss}", endDate = $"{batchTime:yyyy-MM-ddTHH:mm:ss}" }
                                : null
                        },
                        page = 1,
                        pageSize = 1,
                        expand = Expand
                    }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

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
                        _totvsService.SaveRequest(EnumJob.TotvsGetInvoiceByKey, user_name, "POST", route, doc_company, key, $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                        throw;
                    }

                    if (response is null)
                    {
                        // login falhou: nenhuma chamada à busca
                        _totvsService.SaveRequest(EnumJob.TotvsGetInvoiceByKey, user_name, "POST", route, doc_company, key, null, false, requestBody);
                        return null;
                    }

                    if (!response.IsSuccess)
                    {
                        var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                        _logger.AddMessage($"Erro da API Totvs em {route} (empresa {doc_company}, nota {key}): {response.StatusCode} {body}");

                        _totvsService.SaveRequest(EnumJob.TotvsGetInvoiceByKey, user_name, "POST", route, doc_company, key, response.Content, false, requestBody);
                        return null;
                    }

                    Page<Domain.Totvs.Dtos.Reads.Invoices.Invoice>? page = null;
                    string? json = null;

                    try
                    {
                        page = JsonConvert.DeserializeObject<Page<Domain.Totvs.Dtos.Reads.Invoices.Invoice>>(response.Content ?? String.Empty);
                        json = ((JObject.Parse(response.Content!)["items"] as JArray) ?? []).FirstOrDefault()?.ToString(Formatting.None);
                    }
                    catch (JsonException)
                    {
                        // page continua null: tratado abaixo como resposta inválida
                    }

                    var invoice = page?.items?.FirstOrDefault(x => Key(x).branchCode == branchCode && Key(x).invoiceSequence == invoiceSequence && IsValidKey(Key(x)));

                    if (page?.items is null)
                    {
                        _logger.AddMessage($"Resposta inválida da API Totvs em {route} (empresa {doc_company}, nota {key}): {response.Content}");
                        result = null;
                    }
                    else if (invoice is null || json is null)
                    {
                        // 200 com items vazio: não está na janela (1ª tentativa) ou não existe (2ª)
                        if (!withChange)
                            _logger.AddMessage($"Nota {key} não encontrada na Totvs (empresa {doc_company}).");

                        result = false;
                    }
                    else
                    {
                        if (!withChange)
                            _logger.AddMessage($"Nota {key} está fora dos últimos 6 meses de alteração: gravada sem maxChangeFilterDate.");

                        // atualiza o cache do PostSearchInvoices, se ele estiver carregado, para a próxima execução não regravar a nota
                        _cache.TryGetValue($"{InvoicesKey}:{doc_company}", out Dictionary<(int branchCode, int invoiceSequence, DateTime invoiceDate), Domain.Totvs.Entities.Reads.Invoices.Invoice>? invoicesCache);

                        await SaveInvoices(doc_company, _totvsService.GetBatchTime(), [invoice], new Dictionary<(int, int, DateTime), string> { { Key(invoice), json } }, invoicesCache);

                        _logger.AddMessage($"Concluída com sucesso: nota {key} de {Key(invoice).invoiceDate:dd/MM/yyyy} inserida ou atualizada!");
                        result = true;
                    }

                    _totvsService.SaveRequest(EnumJob.TotvsGetInvoiceByKey, user_name, "POST", route, doc_company, key, response.Content, result == true, requestBody);

                    // só a 1ª tentativa sem resultado passa para a 2ª
                    if (result != false)
                        break;
                }

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// sinaliza o 400 do expand referencedInvoices para o ReadPage reler a página em partes
        /// </summary>
        private sealed class ReferencedInvoicesException : Exception { }

        [JobVersion("1.0.0")]
        public async Task<bool?> GetInvoicesDisable()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetInvoicesDisable);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    // BranchCodeList é obrigatório (400 EmptyField, testado): as lojas vão repetidas na rota (BranchCodeList=1&BranchCodeList=2),
                    // porque os parâmetros de query do IApiClient são um dicionário; o BuildUrl junta o resto com &
                    var branchCodes = await _totvsService.GetBranchCodes(company);

                    if (branchCodes.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em totvs.TotvsBranches: rode o GetBranchesList antes.");
                        continue;
                    }

                    var route = "fiscal/v2/invoices/disable?" + String.Join("&", branchCodes.Select(x => $"BranchCodeList={x}"));
                    var endDate = _totvsService.GetBatchTime();

                    // banco vazio: desde 01/01/2025 (igual às notas), em fatias de 30 dias; depois, MAX(maxChangeFilterDate) − 1 h
                    var lastChange = await _totvsInvoicesRepository.GetInvoicesDisableLastChange(company.doc_company!);
                    var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : new DateTime(2025, 1, 1);

                    for (var windowStart = since; windowStart < endDate; windowStart = windowStart.AddDays(InvoicesWindowDays))
                    {
                        var windowEnd = windowStart.AddDays(InvoicesWindowDays) < endDate ? windowStart.AddDays(InvoicesWindowDays) : endDate;

                        var items = await GetFiscalPages<Domain.Totvs.Dtos.Reads.Invoices.InvoiceDisable>(company, route, page => new Dictionary<string, string>
                        {
                            { "StartDate", $"{windowStart:yyyy-MM-ddTHH:mm:ss}" },
                            { "EndDate", $"{windowEnd:yyyy-MM-ddTHH:mm:ss}" },
                            { "Order", "maxChangeFilterDate" },
                            { "Page", page.ToString() },
                            { "PageSize", FiscalCatalogsPageSize.ToString() }
                        });

                        // fatia incompleta para a leitura: a próxima execução continua do MAX(maxChangeFilterDate)
                        if (items is null)
                            break;

                        if (items.Count == 0)
                            continue;

                        static string Key(Domain.Totvs.Dtos.Reads.Invoices.InvoiceDisable x) =>
                            $"{ConverterValidatorsExtensions.ConvertToInt32Validation(x.branchCode)}|{ConverterValidatorsExtensions.ConvertToInt32Validation(x.documentType)}|{(x.serialCode ?? String.Empty).ToUpperInvariant()}|{ConverterValidatorsExtensions.ConvertToInt32Validation(x.initialInvoiceNumber)}";

                        // cache de estado: chave -> maxChangeFilterDate gravado, por empresa (poucas faixas: a tabela inteira da empresa)
                        var itemsCache = await _cache.GetOrCreateAsync($"{InvoicesDisableKey}:{company.doc_company}", async entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                            entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                            var list = await _totvsInvoicesRepository.GetInvoicesDisable(company.doc_company!);

                            return list
                                .Where(x => x != null)
                                .GroupBy(x => $"{x!.branchCode}|{x.documentType}|{x.serialCode.ToUpperInvariant()}|{x.initialInvoiceNumber}")
                                .ToDictionary(g => g.Key, g => g.First()!.maxChangeFilterDate);
                        });

                        // mantém só os novos ou com maxChangeFilterDate mais recente que o gravado (sem data, regrava)
                        items.RemoveAll(x =>
                            itemsCache!.TryGetValue(Key(x.dto), out var y) &&
                            NullableConverters.ToNullableDateTime(x.dto.maxChangeFilterDate) <= y
                        );

                        // valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa. Sem loja ou número inicial
                        // (a chave), a faixa vai para o log e não é gravada. Devolve a quantidade de faixas gravadas.
                        var batchTime = _totvsService.GetBatchTime();

                        var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Invoices.InvoiceDisable record, string json)>();

                        foreach (var item in items)
                        {
                            var validations = _invoiceDisableValidator.Validate(item.dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"inutilização {item.dto.branchCode}/{item.dto.documentType}/{item.dto.serialCode}/{item.dto.initialInvoiceNumber}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            var record = new Domain.Totvs.Entities.Reads.Invoices.InvoiceDisable(company.doc_company!, batchTime, item.dto);

                            if (record.branchCode != 0 && record.initialInvoiceNumber != 0)
                                _listSomenteNovos.Add((record, item.json.ToString(Formatting.None)));
                        }

                        // a mesma faixa pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                        var records = _listSomenteNovos.GroupBy(x => $"{x.record.branchCode}|{x.record.documentType}|{x.record.serialCode.ToUpperInvariant()}|{x.record.initialInvoiceNumber}").Select(g => g.Last()).ToList();

                        if (records.Count == 0)
                            continue;

                        await _totvsInvoicesRepository.BulkInsertIntoTotvsInvoicesDisable(records.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

                        foreach (var (record, json) in records)
                        {
                            var key = $"{record.branchCode}|{record.documentType}|{record.serialCode.ToUpperInvariant()}|{record.initialInvoiceNumber}";

                            _logger.AddRecord(key, json);

                            if (itemsCache is not null)
                                itemsCache[key] = record.maxChangeFilterDate;
                        }

                        totalInseridos += records.Count;
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

        [JobVersion("1.0.0")]
        public async Task<bool?> GetCostCenter()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetCostCenter);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _totvsService.GetCompanys())
                {
                    var batchTime = _totvsService.GetBatchTime();

                    // a rota exige StartChangeDate/EndChangeDate, mas a resposta não traz maxChangeFilterDate: a leitura é inteira (desde 2000)
                    // e o hash do JSON de cada centro, comparado com o json_hash gravado, diz o que mudou (F5, T9). IsInactive não é enviado:
                    // sem ele vêm os ativos e os inativos (testado)
                    var items = await GetFiscalPages<Domain.Totvs.Dtos.Reads.Invoices.CostCenter>(company, "fiscal/v2/cost-center", page => new Dictionary<string, string>
                    {
                        { "StartChangeDate", "2000-01-01T00:00:00" },
                        { "EndChangeDate", $"{batchTime:yyyy-MM-ddTHH:mm:ss}" },
                        { "Page", page.ToString() },
                        { "PageSize", FiscalCatalogsPageSize.ToString() }
                    });

                    // leitura incompleta não é gravada: só a leitura inteira diz quais centros mudaram
                    if (items is null || items.Count == 0)
                        continue;

                    var stored = (await _totvsInvoicesRepository.GetCostCentersHashes(company.doc_company!))
                        .Where(x => x != null)
                        .GroupBy(x => x!.code)
                        .ToDictionary(g => g.Key, g => g.First()!.json_hash);

                    // valida, monta as entidades (com o mesmo horário de lote e o hash do JSON) e grava só os centros novos ou com hash diferente
                    // do gravado. Devolve a quantidade de centros gravados.
                    var _listSomenteNovos = new List<(Domain.Totvs.Entities.Reads.Invoices.CostCenter record, string json)>();

                    foreach (var item in items.Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.code) != 0).GroupBy(x => x.dto.code).Select(g => g.Last()))
                    {
                        var json = item.json.ToString(Formatting.None);
                        var hash = _logger.ComputeSha256Hash(json);

                        // sem data de alteração: só regrava quando o JSON mudou
                        if (stored.TryGetValue(ConverterValidatorsExtensions.ConvertToInt32Validation(item.dto.code), out var storedHash) && storedHash == hash)
                            continue;

                        var validations = _costCenterValidator.Validate(item.dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"centro de custo {item.dto.code}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add((new Domain.Totvs.Entities.Reads.Invoices.CostCenter(company.doc_company!, batchTime, hash, item.dto), json));
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _totvsInvoicesRepository.BulkInsertIntoTotvsCostCenters(_listSomenteNovos.Select(x => x.record).ToList(), _logger.GetExecutionGuid());

                    foreach (var (record, json) in _listSomenteNovos)
                        _logger.AddRecord(record.code.ToString(), json);

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

        public async Task<Domain.Totvs.Dtos.Writes.Result> GetXmlContents(string user_name, string doc_company, string accessKey)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetXmlContents);

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return CompanyNotFound(doc_company, accessKey);

                var (xml, error) = await ReadInvoiceXml(company, EnumJob.TotvsGetXmlContents, user_name, accessKey);

                if (error is not null)
                    return error;

                _logger.AddMessage($"Concluída com sucesso: XML da nota {accessKey} ({(string?)xml!["invoiceStatus"]}) devolvido.");

                return new Domain.Totvs.Dtos.Writes.Result
                {
                    success = true,
                    key = accessKey,
                    statusCode = StatusCodes.OK,
                    apiCalled = true,
                    data = System.Text.Json.JsonDocument.Parse(xml.ToString(Formatting.None)).RootElement.Clone()
                };
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostSearchDanfe(string user_name, string doc_company, string accessKey, int? nfeDocumentType)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchDanfe);

            const string route = "fiscal/v2/danfe-search";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return CompanyNotFound(doc_company, accessKey);

                // a rota recebe a chave, não o XML: o XML sai do xml-contents (a linha dele em TotvsRequests leva o job do DANFE)
                var (xml, error) = await ReadInvoiceXml(company, EnumJob.TotvsPostSearchDanfe, user_name, accessKey);

                if (error is not null)
                    return error;

                var mainInvoiceXml = (string?)xml!["mainInvoiceXml"] ?? String.Empty;

                // o xml-contents já devolve o XML em base64 (começa por PD94bWwg, "<?xml"; testado), que é o que o danfe-search pede;
                // se um dia vier em texto, é codificado aqui
                if (mainInvoiceXml.TrimStart().StartsWith('<'))
                    mainInvoiceXml = Convert.ToBase64String(Encoding.UTF8.GetBytes(mainInvoiceXml));

                var requestBody = JsonConvert.SerializeObject(new { mainInvoiceXml, nfeDocumentType }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                // TotvsRequests leva só a chave e o tipo: o XML não é gravado
                var requestSummary = JsonConvert.SerializeObject(new { accessKey, nfeDocumentType });
                var response = await CallFiscalOnDemand(company, EnumJob.TotvsPostSearchDanfe, user_name, accessKey, HttpMethod.Post, route, requestBody, requestSummary);

                if (response is null || !response.IsSuccess)
                {
                    _totvsService.SaveRequest(EnumJob.TotvsPostSearchDanfe, user_name, "POST", route, doc_company, accessKey, response?.Content, false, requestSummary);
                    return FiscalError(accessKey, route, response);
                }

                var pdf = Base64Content(response.Content, "danfePdfBase64");

                if (pdf is null)
                {
                    _totvsService.SaveRequest(EnumJob.TotvsPostSearchDanfe, user_name, "POST", route, doc_company, accessKey, Truncate(response.Content), false, requestSummary);
                    return InvalidResponse(accessKey, route, response.Content);
                }

                _totvsService.SaveRequest(EnumJob.TotvsPostSearchDanfe, user_name, "POST", route, doc_company, accessKey, JsonConvert.SerializeObject(new { pdfLength = pdf.Length }), true, requestSummary);
                _logger.AddMessage($"Concluída com sucesso: DANFE da nota {accessKey} devolvido ({pdf.Length} bytes).");

                return new Domain.Totvs.Dtos.Writes.Result
                {
                    success = true,
                    key = accessKey,
                    statusCode = StatusCodes.OK,
                    apiCalled = true,
                    file = pdf,
                    fileName = $"{accessKey}.pdf",
                    contentType = "application/pdf"
                };
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostTransactionPrint(string user_name, string doc_company, int branchCode, int invoiceSequence, DateTime invoiceDate, int transactionPrintTemplate, int transactionPrintType)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostTransactionPrint);

            const string route = "fiscal/v2/transaction/print";
            var key = $"{branchCode}|{invoiceSequence}|{invoiceDate:yyyy-MM-dd}";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return CompanyNotFound(doc_company, key);

                // o swagger não marca nenhum campo como obrigatório; aqui todos são (§8.6). A rota roda um processo Uniface no ERP
                // (~12 s, testado): é chamada só sob demanda, nunca em laço
                var requestBody = JsonConvert.SerializeObject(new
                {
                    branchCode,
                    invoiceDate = $"{invoiceDate:yyyy-MM-dd}",
                    invoiceSequence,
                    transactionPrintTemplate,
                    transactionPrintType
                });

                var response = await CallFiscalOnDemand(company, EnumJob.TotvsPostTransactionPrint, user_name, key, HttpMethod.Post, route, requestBody, requestBody);

                if (response is null || !response.IsSuccess)
                {
                    _totvsService.SaveRequest(EnumJob.TotvsPostTransactionPrint, user_name, "POST", route, doc_company, key, response?.Content, false, requestBody);
                    return FiscalError(key, route, response);
                }

                JObject? json = null;

                try
                {
                    json = JObject.Parse(response.Content ?? String.Empty);
                }
                catch (JsonReaderException)
                {
                    // json continua null: tratado abaixo como resposta inválida
                }

                var status = (string?)json?["unifaceResponseStatus"];
                var unifaceMessage = (string?)json?["unifaceMessage"];
                var pdf = json is null ? null : Base64Content(response.Content, "content");
                var summary = JsonConvert.SerializeObject(new { unifaceResponseStatus = status, unifaceMessage, contentLength = pdf?.Length ?? 0 });

                if (json is null)
                {
                    _totvsService.SaveRequest(EnumJob.TotvsPostTransactionPrint, user_name, "POST", route, doc_company, key, Truncate(response.Content), false, requestBody);
                    return InvalidResponse(key, route, response.Content);
                }

                // 200 com erro do processo (status diferente de Success ou sem conteúdo): 422 com a mensagem do Uniface
                if (status != "Success" || pdf is null || pdf.Length == 0)
                {
                    _totvsService.SaveRequest(EnumJob.TotvsPostTransactionPrint, user_name, "POST", route, doc_company, key, summary, false, requestBody);
                    _logger.AddMessage($"Impressão da transação {key} (empresa {doc_company}): {status} {unifaceMessage}");

                    return new Domain.Totvs.Dtos.Writes.Result
                    {
                        success = false,
                        key = key,
                        statusCode = StatusCodes.UnprocessableEntity,
                        apiCalled = true,
                        message = String.IsNullOrWhiteSpace(unifaceMessage) ? $"A Totvs devolveu {status} sem conteúdo." : unifaceMessage
                    };
                }

                _totvsService.SaveRequest(EnumJob.TotvsPostTransactionPrint, user_name, "POST", route, doc_company, key, summary, true, requestBody);
                _logger.AddMessage($"Concluída com sucesso: impressão da transação {key} devolvida ({pdf.Length} bytes).");

                return new Domain.Totvs.Dtos.Writes.Result
                {
                    success = true,
                    key = key,
                    statusCode = StatusCodes.OK,
                    apiCalled = true,
                    file = pdf,
                    fileName = $"{branchCode}-{invoiceSequence}-{invoiceDate:yyyyMMdd}.pdf",
                    contentType = "application/pdf"
                };
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        // status das leituras sem persistência (§8.1)
        private static class StatusCodes
        {
            public const int OK = 200;
            public const int NotFound = 404;
            public const int UnprocessableEntity = 422;
            public const int BadGateway = 502;
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> GetSearchInvoicesItemDetail(string user_name, string doc_company, int? branchCode, int? invoiceSequence, DateTime? invoiceDate, string? accessKey)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetSearchInvoicesItemDetail);

            // as duas formas da chave da nota, uma ou outra (a Totvs recusa as duas juntas: 400 CannotBeInformed, testado)
            var key = accessKey ?? $"{branchCode}|{invoiceSequence}|{invoiceDate:yyyy-MM-dd}";
            var query = accessKey is not null
                ? $"AccessKey={accessKey}"
                : $"BranchCode={branchCode}&InvoiceDate={invoiceDate:yyyy-MM-dd}&InvoiceSequence={invoiceSequence}";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return CompanyNotFound(doc_company, key);

                // os itens já são gravados pelo invoices/search; esta rota só devolve o que é dela (transações e notas referenciadas por
                // produto, inTransactionAgroup) e não grava: regravar as tabelas por aqui apagaria cest e additionalItemInformation (§8.7)
                return await ReadFiscalJson(company, EnumJob.TotvsGetSearchInvoicesItemDetail, user_name, key, HttpMethod.Get, $"fiscal/v2/invoices/item-detail-search?{query}&Expand=barcodes", null, false);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> GetInvoicesPendingConditionalProducts(string user_name, string doc_company, int branchCode, int? personCode, string? personCpfCnpj, bool onlyPendingItem)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsGetInvoicesPendingConditionalProducts);

            var key = personCode is not null ? $"{branchCode}|{personCode}" : $"{branchCode}|{personCpfCnpj}";

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return CompanyNotFound(doc_company, key);

                // a loja precisa ser da empresa (422 sem chamar a API, §8.8)
                if (!(await _totvsService.GetBranchCodes(company)).Contains(branchCode))
                    return BranchNotFound(doc_company, key, [branchCode]);

                // PersonCode ou PersonCpfCnpj, um dos dois (a Totvs recusa nenhum e os dois: 400 FieldsHaveNoValue / FieldsHaveValue, testado)
                var person = personCode is not null ? $"PersonCode={personCode}" : $"PersonCpfCnpj={personCpfCnpj}";

                // o saldo do momento muda a cada venda ou devolução e a rota não tem filtro de alteração: não há o que gravar (§8.8)
                return await ReadFiscalJson(company, EnumJob.TotvsGetInvoicesPendingConditionalProducts, user_name, key, HttpMethod.Get,
                    $"fiscal/v2/invoices/pending-conditional-products?BranchCode={branchCode}&{person}&OnlyPendingItem={(onlyPendingItem ? "true" : "false")}", null, true);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostSearchInvoiceProducts(string user_name, string doc_company, Domain.Totvs.Dtos.Reads.Invoices.InvoiceProductSearch body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostSearchInvoiceProducts);

            // chave em TotvsRequests: as lojas e o primeiro filtro restritivo informado (§8.9)
            var filter = body.filter!;
            var restrictive = filter.invoiceCodeList?.Count > 0 ? $"invoiceCodeList={String.Join(",", filter.invoiceCodeList)}"
                : filter.ProductCodeList?.Count > 0 ? $"ProductCodeList={String.Join(",", filter.ProductCodeList)}"
                : $"acessKeyList={String.Join(",", filter.acessKeyList ?? [])}";
            var key = $"{String.Join(",", filter.branchCodeList ?? [])}|{restrictive}";
            key = key.Length > 200 ? key[..200] : key;

            try
            {
                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return CompanyNotFound(doc_company, key);

                var branchCodes = await _totvsService.GetBranchCodes(company);
                var outside = (filter.branchCodeList ?? []).Where(x => !branchCodes.Contains(x)).ToList();

                if (outside.Count > 0)
                    return BranchNotFound(doc_company, key, outside);

                // o corpo vai como veio (nomes do swagger, enums pelo nome); nulos omitidos
                var requestBody = JsonConvert.SerializeObject(body, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "yyyy-MM-ddTHH:mm:ss"
                });

                // a página volta como veio: quem chama pagina (§8.9)
                return await ReadFiscalJson(company, EnumJob.TotvsPostSearchInvoiceProducts, user_name, key, HttpMethod.Post, "fiscal/v2/invoice-products/search", requestBody, false);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        // grupo das escritas da API Fiscal na configuração (Totvs:Writes:Invoices:Enabled, T4)
        private const string WritesGroup = "Invoices";

        public async Task<Domain.Totvs.Dtos.Writes.Result> PostInvoicesManifestations(string user_name, string doc_company, Domain.Totvs.Dtos.Writes.Fiscal.Manifestation body)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.TotvsPostInvoicesManifestations);

            var key = String.Join("|", body.items!.Select(x => x.accessKey));

            try
            {
                if (!_totvsService.IsWriteEnabled(WritesGroup))
                    return _totvsService.WriteDisabled(WritesGroup, key);

                var company = await _totvsService.GetCompany(doc_company);

                if (company is null)
                    return _totvsService.CompanyNotFound(doc_company, key);

                var outside = new List<string>();

                foreach (var cnpj in body.items!.Select(x => x.branchCnpj!).Distinct())
                    if (await _totvsService.GetBranchCodeByCnpj(company, cnpj) is null)
                        outside.Add(cnpj);

                if (outside.Count > 0)
                    return _totvsService.BranchNotFound(doc_company, key, outside);

                // o XML da nota não vai para totvs.TotvsRequests: no lugar dele, o tamanho (F1, §8.1)
                var logged = JObject.FromObject(body, JsonSerializer.Create(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                foreach (var item in logged["items"]!.OfType<JObject>())
                    if (item["invoiceXml"] is JToken xml)
                    {
                        item["invoiceXmlLength"] = xml.ToString().Length;
                        item.Remove("invoiceXml");
                    }

                // inclusão ou alteração no painel: repetir dá no mesmo, sem trava. O efeito fiscal é a confirmar (F1)
                return await _totvsService.Write(company, EnumJob.TotvsPostInvoicesManifestations, user_name, key, HttpMethod.Post, "fiscal/v2/invoices/manifestations", body,
                    "Confira o painel de manifestação (FISFP153) antes de repetir.", loggedBody: logged);
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        // ------------------------------------------------------------------------------------------------------

        /// <summary>
        /// chave da nota: loja + sequência + data (5.2.1)
        /// </summary>
        private static (int branchCode, int invoiceSequence, DateTime invoiceDate) Key(Domain.Totvs.Dtos.Reads.Invoices.Invoice x) =>
            (ConverterValidatorsExtensions.ConvertToInt32Validation(x.branchCode), ConverterValidatorsExtensions.ConvertToInt32Validation(x.invoiceSequence), ConverterValidatorsExtensions.ConvertToDateTimeValidation(x.invoiceDate));

        /// <summary>
        /// sem loja, sequência ou data a nota não tem chave (a data vazia vira 01/01/1990 no conversor)
        /// </summary>
        private static bool IsValidKey((int branchCode, int invoiceSequence, DateTime invoiceDate) key) =>
            key.branchCode != 0 && key.invoiceSequence != 0 && key.invoiceDate != new DateTime(1990, 1, 1);

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de notas gravadas.
        /// </summary>
        private async Task<int> SaveInvoices(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Totvs.Dtos.Reads.Invoices.Invoice> invoices,
            IReadOnlyDictionary<(int, int, DateTime), string> invoicesJson,
            Dictionary<(int branchCode, int invoiceSequence, DateTime invoiceDate), Domain.Totvs.Entities.Reads.Invoices.Invoice>? invoicesCache)
        {
            var _listSomenteNovos = new List<Domain.Totvs.Entities.Reads.Invoices.Invoice>();

            // a mesma nota pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var invoice in invoices.Where(x => IsValidKey(Key(x))).GroupBy(Key).Select(g => g.Last()))
            {
                var validations = _invoiceValidator.Validate(invoice);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"nota {invoice.branchCode}/{invoice.invoiceSequence} de {invoice.invoiceDate}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // vínculo com pedido sem loja ou número não tem chave e é descartado pela entidade
                var semChave = (invoice.salesOrder ?? []).Count(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.branchCode) == 0 || ConverterValidatorsExtensions.ConvertToInt32Validation(x.orderCode) == 0);

                if (semChave > 0)
                    _logger.AddMessage($"nota {invoice.branchCode}/{invoice.invoiceSequence} de {invoice.invoiceDate}: {semChave} pedido(s) de venda sem loja ou número descartado(s).");

                _listSomenteNovos.Add(new Domain.Totvs.Entities.Reads.Invoices.Invoice(doc_company, batchTime, invoice));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _totvsInvoicesRepository.BulkInsertIntoTotvsInvoices(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (invoicesJson.TryGetValue((record.branchCode, record.invoiceSequence, record.invoiceDate), out var json))
                    _logger.AddRecord($"{record.branchCode}|{record.invoiceSequence}|{record.invoiceDate:yyyy-MM-dd}", json);

                if (invoicesCache is not null)
                    invoicesCache[(record.branchCode, record.invoiceSequence, record.invoiceDate)] = record;
            }

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// lê todas as páginas de um GET da API Fiscal (inutilizações e centros de custo). Devolve cada item com o seu JSON, ou null se
        /// alguma página falhou (a mensagem vai para o log)
        /// </summary>
        private async Task<List<(T dto, JObject json)>?> GetFiscalPages<T>(Domain.Totvs.Entities.Parameters.Company company, string route, Func<int, Dictionary<string, string>> query)
        {
            var items = new List<(T dto, JObject json)>();
            var page = 1;
            var totalPages = 1;
            var routeName = route.Split('?')[0];

            while (page <= totalPages)
            {
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
                        queryParams: query(page)
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
                        _logger.AddMessage($"Erro da API Totvs em {routeName} (empresa {company.doc_company}, página {page}): {response.StatusCode} {body}");
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
                    _logger.AddMessage($"Resposta inválida da API Totvs em {routeName} (empresa {company.doc_company}, página {page}): {body}");

                    return null;
                }

                for (var i = 0; i < json.Count; i++)
                    items.Add((result.items[i], (JObject)json[i]));

                totalPages = result.totalPages;
                page++;
            }

            return items;
        }

        /// <summary>
        /// lê o XML da nota pelo GET fiscal/v2/xml-contents/{accessKey} e registra a chamada em TotvsRequests com um resumo (situação e
        /// tamanho de cada XML), nunca o conteúdo. Devolve o JSON da Totvs ou o Result do erro
        /// </summary>
        private async Task<(JObject? xml, Domain.Totvs.Dtos.Writes.Result? error)> ReadInvoiceXml(Domain.Totvs.Entities.Parameters.Company company, EnumJob job, string user_name, string accessKey)
        {
            var route = $"fiscal/v2/xml-contents/{accessKey}";
            var response = await CallFiscalOnDemand(company, job, user_name, accessKey, HttpMethod.Get, route, null, null);

            if (response is null || !response.IsSuccess)
            {
                _totvsService.SaveRequest(job, user_name, "GET", route, company.doc_company!, accessKey, response?.Content, false);
                return (null, FiscalError(accessKey, route, response));
            }

            JObject? json = null;

            try
            {
                json = JObject.Parse(response.Content ?? String.Empty);
            }
            catch (JsonReaderException)
            {
                // json continua null: tratado abaixo como resposta inválida
            }

            if (json is null || String.IsNullOrWhiteSpace((string?)json["mainInvoiceXml"]))
            {
                _totvsService.SaveRequest(job, user_name, "GET", route, company.doc_company!, accessKey, Truncate(response.Content), false);
                return (null, InvalidResponse(accessKey, route, response.Content));
            }

            // a resposta real traz invoiceStatus (o swagger diz processingType) e o cancelInvoiceXml só nas canceladas
            var summary = JsonConvert.SerializeObject(new
            {
                invoiceStatus = (string?)json["invoiceStatus"],
                mainInvoiceXmlLength = ((string?)json["mainInvoiceXml"])?.Length ?? 0,
                cancelInvoiceXmlLength = ((string?)json["cancelInvoiceXml"])?.Length ?? 0
            });

            _totvsService.SaveRequest(job, user_name, "GET", route, company.doc_company!, accessKey, summary, true);

            return (json, null);
        }

        /// <summary>
        /// uma chamada sob demanda à API Fiscal, com uma nova tentativa (são consultas, idempotentes, inclusive os POST; §8.1): 401
        /// renova o token, 429 espera o Retry-After e 502/503/504 espera 10 s. A exceção (timeout, rede) é registrada em TotvsRequests
        /// antes de subir. Devolve null se o login falhou (a mensagem já foi para o log); quem chama registra a resposta
        /// </summary>
        private async Task<ApiResponse?> CallFiscalOnDemand(Domain.Totvs.Entities.Parameters.Company company, EnumJob job, string user_name, string key, HttpMethod method, string route, string? requestBody, string? requestSummary)
        {
            ApiResponse? response = null;

            try
            {
                for (var attempt = 1; attempt <= 2; attempt++)
                {
                    var token = await _totvsService.GetToken(company);

                    if (token is null)
                        break;

                    using var content = requestBody is null ? null : new StringContent(requestBody, Encoding.UTF8, "application/json");

                    response = await _apiClient.SendAsync(
                        method,
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
                _totvsService.SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, $"Exceção ao chamar a API: {ex.Message}", false, requestSummary);
                throw;
            }

            return response;
        }

        /// <summary>
        /// Result do erro da Totvs numa leitura sem persistência: o 400 KeyNotFound (chave de acesso inexistente, ou XML não encontrado
        /// numa nota cancelada; testado) e o 404 viram 404; 429 e 5xx seguem como vieram; o resto (400) vira 422 com as mensagens
        /// </summary>
        private Domain.Totvs.Dtos.Writes.Result FiscalError(string key, string route, ApiResponse? response)
        {
            if (response is null)
                return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 0, apiCalled = false, message = "Falha no login na Totvs: veja o log da execução." };

            var message = TotvsMessage(response);
            _logger.AddMessage($"Erro da API Totvs em {route} ({key}): {response.StatusCode} {message}");

            var notFound = response.StatusCode == 404 || (response.StatusCode == 400 && (response.Content ?? String.Empty).Contains("\"KeyNotFound\""));

            return new Domain.Totvs.Dtos.Writes.Result
            {
                success = false,
                key = key,
                statusCode = notFound ? StatusCodes.NotFound : response.StatusCode,
                apiCalled = true,
                message = message,
                response = Truncate(response.Content),
                retryAfterSeconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var seconds) ? seconds : null
            };

            // as mensagens da Totvs (lista de DomainNotificationMessage), como no TotvsAccountsPayableService
            static string TotvsMessage(ApiResponse response)
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

                return String.IsNullOrWhiteSpace(response.Content) ? $"A Totvs devolveu {response.StatusCode} sem corpo." : Truncate(response.Content)!;
            }
        }

        private Domain.Totvs.Dtos.Writes.Result CompanyNotFound(string doc_company, string key)
        {
            // nenhuma chamada à API: não há o que registrar em TotvsRequests (igual ao Movidesk)
            _logger.AddMessage($"Empresa {doc_company} não cadastrada em totvs.TotvsParameters.");

            return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 0, apiCalled = false, message = $"Empresa {doc_company} não cadastrada." };
        }

        private Domain.Totvs.Dtos.Writes.Result InvalidResponse(string key, string route, string? content)
        {
            _logger.AddMessage($"Resposta inválida da API Totvs em {route} ({key}): {Truncate(content)}");

            // JSON inválido ou sem o conteúdo esperado: 502 (§8.1)
            return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = StatusCodes.BadGateway, apiCalled = true, message = "Resposta inválida da Totvs.", response = Truncate(content) };
        }

        /// <summary>
        /// o arquivo em base64 de um campo da resposta, decodificado; null se o campo não existe ou não é base64
        /// </summary>
        private static byte[]? Base64Content(string? content, string field)
        {
            try
            {
                var value = (string?)JObject.Parse(content ?? String.Empty)[field];

                return String.IsNullOrWhiteSpace(value) ? null : Convert.FromBase64String(value);
            }
            catch (Exception ex) when (ex is JsonReaderException or FormatException)
            {
                return null;
            }
        }

        private static string? Truncate(string? content) => content?.Length > 800 ? content[..800] : content;

        /// <summary>
        /// uma leitura sem persistência que devolve o JSON da Totvs (T11): chama com a nova tentativa, registra em TotvsRequests o corpo
        /// enviado e o recebido inteiros, e devolve o Result com o JSON em data, ou o do erro (o 400 KeyNotFound vira 404). Com emptyIs404,
        /// a resposta com items vazio também vira 404 (§8.8)
        /// </summary>
        private async Task<Domain.Totvs.Dtos.Writes.Result> ReadFiscalJson(Domain.Totvs.Entities.Parameters.Company company, EnumJob job, string user_name, string key, HttpMethod method, string route, string? requestBody, bool emptyIs404)
        {
            var routeName = route.Split('?')[0];
            var response = await CallFiscalOnDemand(company, job, user_name, key, method, route, requestBody, requestBody);

            if (response is null || !response.IsSuccess)
            {
                _totvsService.SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, response?.Content, false, requestBody);
                return FiscalError(key, routeName, response);
            }

            System.Text.Json.JsonElement data;

            try
            {
                data = System.Text.Json.JsonDocument.Parse(response.Content ?? String.Empty).RootElement.Clone();
            }
            catch (System.Text.Json.JsonException)
            {
                _totvsService.SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, Truncate(response.Content), false, requestBody);
                return InvalidResponse(key, routeName, response.Content);
            }

            var empty = emptyIs404 && data.ValueKind == System.Text.Json.JsonValueKind.Object && data.TryGetProperty("items", out var items)
                && items.ValueKind == System.Text.Json.JsonValueKind.Array && items.GetArrayLength() == 0;

            _totvsService.SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, response.Content, !empty, requestBody);

            if (empty)
            {
                _logger.AddMessage($"Sem registros em {routeName} ({key}).");
                return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = StatusCodes.NotFound, apiCalled = true, message = "Nenhum registro encontrado.", data = data };
            }

            _logger.AddMessage($"Concluída com sucesso: {routeName} ({key}) devolvido.");

            return new Domain.Totvs.Dtos.Writes.Result { success = true, key = key, statusCode = StatusCodes.OK, apiCalled = true, data = data };
        }

        private Domain.Totvs.Dtos.Writes.Result BranchNotFound(string doc_company, string key, List<int> branchCodes)
        {
            // nenhuma chamada à API: não há o que registrar em TotvsRequests
            var message = $"Loja(s) {String.Join(", ", branchCodes)} fora de totvs.TotvsBranches da empresa {doc_company}.";
            _logger.AddMessage(message);

            return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 0, apiCalled = false, message = message };
        }
    }
}

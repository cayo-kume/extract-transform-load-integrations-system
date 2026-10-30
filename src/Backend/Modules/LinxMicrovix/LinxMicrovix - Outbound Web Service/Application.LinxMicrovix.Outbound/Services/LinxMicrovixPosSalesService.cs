using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.Enums;
using Domain.LinxMicrovix.Outbound.Extensions;
using Domain.Shared.Abstractions.Api;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Application.LinxMicrovix.Outbound.Services
{
    /// <summary>
    /// Vendas POS temporárias do Web Service de Saída Padrão (ERP). Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixPosSalesService : ILinxMicrovixPosSalesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixPosSalesRepository _linxMicrovixPosSalesRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosTmp> _linxVendasPosTmpValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosProdutosTmp> _linxVendasPosProdutosTmpValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosAgrupamentoTmp> _linxVendasPosAgrupamentoTmpValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixPosSalesService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixPosSalesRepository linxMicrovixPosSalesRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosTmp> linxVendasPosTmpValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosProdutosTmp> linxVendasPosProdutosTmpValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosAgrupamentoTmp> linxVendasPosAgrupamentoTmpValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixPosSalesRepository, _linxVendasPosTmpValidator, _linxVendasPosProdutosTmpValidator, _linxVendasPosAgrupamentoTmpValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixPosSalesRepository, linxVendasPosTmpValidator, linxVendasPosProdutosTmpValidator, linxVendasPosAgrupamentoTmpValidator);

        public async Task<bool?> LinxVendasPosTmp()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxVendasPosTmp);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.Erp);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.LinxGrupoLojas: rode o LinxGrupoLojas antes.");
                        continue;
                    }

                    foreach (var cnpj_emp in stores)
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxVendasPosTmp, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosTmp dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxVendasPosTmp"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp)
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxVendasPosTmp", content);

                                if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                    continue;
                                }

                                break;
                            }

                            if (response is null || !response.IsSuccess)
                            {
                                var body = response?.Content?.Length > 800 ? response.Content[..800] : response?.Content;
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxVendasPosTmp (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

                                failed = true;
                                break;
                            }

                            XElement? root = null;

                            try
                            {
                                root = XDocument.Parse(response.Content?.TrimStart('\uFEFF') ?? String.Empty).Root;
                            }
                            catch (XmlException)
                            {
                                // root continua null: tratado abaixo como resposta inválida
                            }

                            if (root is null || !String.Equals(root.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                            {
                                var message = root?.Descendants("Message").FirstOrDefault()?.Value ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content);
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxVendasPosTmp (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxVendasPosTmp (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

                                failed = true;
                                break;
                            }

                            // resposta vazia: não há mais registros com timestamp maior que o informado
                            if (pageRows.Count == 0)
                                break;

                            foreach (var r in pageRows)
                            {
                                // <D /> é nulo; a linha é registrada em auditing.Records com o nome de cada coluna
                                var values = r.Elements("D").Select(d => d.IsEmpty ? null : d.Value).ToList();
                                var row = columns.Select((c, i) => (c, v: i < values.Count ? values[i] : null)).GroupBy(x => x.c).ToDictionary(g => g.Key, g => g.First().v);

                                rows.Add((
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosTmp
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_vendas_pos = row.GetValueOrDefault("id_vendas_pos"),
                                    numero_ficha = row.GetValueOrDefault("numero_ficha"),
                                    numero_sequencial = row.GetValueOrDefault("numero_sequencial"),
                                    cod_cliente = row.GetValueOrDefault("cod_cliente"),
                                    data_lcto = row.GetValueOrDefault("data_lcto"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    cliente_indicador = row.GetValueOrDefault("cliente_indicador"),
                                    observacao_dav = row.GetValueOrDefault("observacao_dav"),
                                    observacao = row.GetValueOrDefault("observacao"),
                                    codigo_sistema_terceiro = row.GetValueOrDefault("codigo_sistema_terceiro"),
                                    cancelado = row.GetValueOrDefault("cancelado"),
                                    timestamp = row.GetValueOrDefault("timestamp")
                                    },
                                    new XElement("R", row.Select(x => new XElement(x.Key, x.Value))).ToString(SaveOptions.DisableFormatting)
                                ));
                            }

                            // a resposta vem em ordem crescente de timestamp (testado): a próxima página começa no maior desta
                            var lastTimestamp = rows.Max(x => MicrovixConverters.ToInt64(x.dto.timestamp));

                            if (lastTimestamp <= timestamp)
                                break;

                            timestamp = lastTimestamp;
                        }

                        // leitura incompleta não é gravada: a próxima execução sai do MAX(timestamp) e pularia o que faltou
                        if (failed || rows.Count == 0)
                            continue;

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_vendas_pos; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales.LinxVendasPosTmp>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_vendas_pos).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxVendasPosTmpValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxVendasPosTmp {dto.id_vendas_pos} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_vendas_pos))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales.LinxVendasPosTmp(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_vendas_pos}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixPosSalesRepository.BulkInsertIntoLinxVendasPosTmp(_listSomenteNovos, _logger.GetExecutionGuid());

                        foreach (var record in records)
                            _logger.AddRecord(record.Key, record.Value);

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

        public async Task<bool?> LinxVendasPosProdutosTmp()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxVendasPosProdutosTmp);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.Erp);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.LinxGrupoLojas: rode o LinxGrupoLojas antes.");
                        continue;
                    }

                    foreach (var cnpj_emp in stores)
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxVendasPosProdutosTmp, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosProdutosTmp dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxVendasPosProdutosTmp"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp)
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxVendasPosProdutosTmp", content);

                                if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                    continue;
                                }

                                break;
                            }

                            if (response is null || !response.IsSuccess)
                            {
                                var body = response?.Content?.Length > 800 ? response.Content[..800] : response?.Content;
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxVendasPosProdutosTmp (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

                                failed = true;
                                break;
                            }

                            XElement? root = null;

                            try
                            {
                                root = XDocument.Parse(response.Content?.TrimStart('\uFEFF') ?? String.Empty).Root;
                            }
                            catch (XmlException)
                            {
                                // root continua null: tratado abaixo como resposta inválida
                            }

                            if (root is null || !String.Equals(root.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                            {
                                var message = root?.Descendants("Message").FirstOrDefault()?.Value ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content);
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxVendasPosProdutosTmp (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxVendasPosProdutosTmp (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

                                failed = true;
                                break;
                            }

                            // resposta vazia: não há mais registros com timestamp maior que o informado
                            if (pageRows.Count == 0)
                                break;

                            foreach (var r in pageRows)
                            {
                                // <D /> é nulo; a linha é registrada em auditing.Records com o nome de cada coluna
                                var values = r.Elements("D").Select(d => d.IsEmpty ? null : d.Value).ToList();
                                var row = columns.Select((c, i) => (c, v: i < values.Count ? values[i] : null)).GroupBy(x => x.c).ToDictionary(g => g.Key, g => g.First().v);

                                rows.Add((
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosProdutosTmp
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_vendas_pos_produtos = row.GetValueOrDefault("id_vendas_pos_produtos"),
                                    numero_ficha = row.GetValueOrDefault("numero_ficha"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    codigoproduto = row.GetValueOrDefault("codigoproduto"),
                                    preco_unitario = row.GetValueOrDefault("preco_unitario"),
                                    preco_tabela = row.GetValueOrDefault("preco_tabela"),
                                    quantidade = row.GetValueOrDefault("quantidade"),
                                    vendedor = row.GetValueOrDefault("vendedor"),
                                    desconto_item = row.GetValueOrDefault("desconto_item"),
                                    id_tabela_preco = row.GetValueOrDefault("id_tabela_preco"),
                                    altura = row.GetValueOrDefault("altura"),
                                    comprimento = row.GetValueOrDefault("comprimento"),
                                    largura = row.GetValueOrDefault("largura"),
                                    cfop_item = row.GetValueOrDefault("cfop_item"),
                                    excluido = row.GetValueOrDefault("excluido"),
                                    aliq_icms = row.GetValueOrDefault("aliq_icms"),
                                    cst_fiscal = row.GetValueOrDefault("cst_fiscal"),
                                    indice_ordenacao_espelho = row.GetValueOrDefault("indice_ordenacao_espelho"),
                                    id_vendas_pos = row.GetValueOrDefault("id_vendas_pos"),
                                    id_cst_icms_fiscal = row.GetValueOrDefault("id_cst_icms_fiscal"),
                                    id_cst_ipi_fiscal = row.GetValueOrDefault("id_cst_ipi_fiscal"),
                                    id_cst_pis_fiscal = row.GetValueOrDefault("id_cst_pis_fiscal"),
                                    id_cst_cofins_fiscal = row.GetValueOrDefault("id_cst_cofins_fiscal"),
                                    aliq_iss = row.GetValueOrDefault("aliq_iss"),
                                    id_cfop_fiscal = row.GetValueOrDefault("id_cfop_fiscal"),
                                    aliq_pis = row.GetValueOrDefault("aliq_pis"),
                                    aliq_cofins = row.GetValueOrDefault("aliq_cofins"),
                                    desconto_item_valor = row.GetValueOrDefault("desconto_item_valor"),
                                    lote = row.GetValueOrDefault("lote"),
                                    deposito = row.GetValueOrDefault("deposito"),
                                    previsao_entrega = row.GetValueOrDefault("previsao_entrega"),
                                    transacao_movimento_reserva = row.GetValueOrDefault("transacao_movimento_reserva"),
                                    data_exclusao = row.GetValueOrDefault("data_exclusao"),
                                    desconto_total_item = row.GetValueOrDefault("desconto_total_item"),
                                    id_motivo_desconto = row.GetValueOrDefault("id_motivo_desconto"),
                                    motivo_desconto_observacao = row.GetValueOrDefault("motivo_desconto_observacao"),
                                    crm_cliente_indicador = row.GetValueOrDefault("crm_cliente_indicador"),
                                    cod_cliente_indicador = row.GetValueOrDefault("cod_cliente_indicador"),
                                    timestamp = row.GetValueOrDefault("timestamp")
                                    },
                                    new XElement("R", row.Select(x => new XElement(x.Key, x.Value))).ToString(SaveOptions.DisableFormatting)
                                ));
                            }

                            // a resposta vem em ordem crescente de timestamp (testado): a próxima página começa no maior desta
                            var lastTimestamp = rows.Max(x => MicrovixConverters.ToInt64(x.dto.timestamp));

                            if (lastTimestamp <= timestamp)
                                break;

                            timestamp = lastTimestamp;
                        }

                        // leitura incompleta não é gravada: a próxima execução sai do MAX(timestamp) e pularia o que faltou
                        if (failed || rows.Count == 0)
                            continue;

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_vendas_pos_produtos; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales.LinxVendasPosProdutosTmp>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_vendas_pos_produtos).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxVendasPosProdutosTmpValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxVendasPosProdutosTmp {dto.id_vendas_pos_produtos} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_vendas_pos_produtos))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales.LinxVendasPosProdutosTmp(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_vendas_pos_produtos}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixPosSalesRepository.BulkInsertIntoLinxVendasPosProdutosTmp(_listSomenteNovos, _logger.GetExecutionGuid());

                        foreach (var record in records)
                            _logger.AddRecord(record.Key, record.Value);

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

        public async Task<bool?> LinxVendasPosAgrupamentoTmp()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxVendasPosAgrupamentoTmp);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.Erp);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.LinxGrupoLojas: rode o LinxGrupoLojas antes.");
                        continue;
                    }

                    // a resposta não identifica a loja: o dado é do portal, lido pela primeira loja
                    foreach (var cnpj_emp in stores.Take(1))
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxVendasPosAgrupamentoTmp, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosAgrupamentoTmp dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxVendasPosAgrupamentoTmp"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp)
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxVendasPosAgrupamentoTmp", content);

                                if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                    continue;
                                }

                                break;
                            }

                            if (response is null || !response.IsSuccess)
                            {
                                var body = response?.Content?.Length > 800 ? response.Content[..800] : response?.Content;
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxVendasPosAgrupamentoTmp (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

                                failed = true;
                                break;
                            }

                            XElement? root = null;

                            try
                            {
                                root = XDocument.Parse(response.Content?.TrimStart('\uFEFF') ?? String.Empty).Root;
                            }
                            catch (XmlException)
                            {
                                // root continua null: tratado abaixo como resposta inválida
                            }

                            if (root is null || !String.Equals(root.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                            {
                                var message = root?.Descendants("Message").FirstOrDefault()?.Value ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content);
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxVendasPosAgrupamentoTmp (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxVendasPosAgrupamentoTmp (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

                                failed = true;
                                break;
                            }

                            // resposta vazia: não há mais registros com timestamp maior que o informado
                            if (pageRows.Count == 0)
                                break;

                            foreach (var r in pageRows)
                            {
                                // <D /> é nulo; a linha é registrada em auditing.Records com o nome de cada coluna
                                var values = r.Elements("D").Select(d => d.IsEmpty ? null : d.Value).ToList();
                                var row = columns.Select((c, i) => (c, v: i < values.Count ? values[i] : null)).GroupBy(x => x.c).ToDictionary(g => g.Key, g => g.First().v);

                                rows.Add((
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales.LinxVendasPosAgrupamentoTmp
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_vendas_pos_agrupamento_tmp = row.GetValueOrDefault("id_vendas_pos_agrupamento_tmp"),
                                    id_vendas_pos = row.GetValueOrDefault("id_vendas_pos"),
                                    id_vendas_pos_origem = row.GetValueOrDefault("id_vendas_pos_origem"),
                                    timestamp = row.GetValueOrDefault("timestamp")
                                    },
                                    new XElement("R", row.Select(x => new XElement(x.Key, x.Value))).ToString(SaveOptions.DisableFormatting)
                                ));
                            }

                            // a resposta vem em ordem crescente de timestamp (testado): a próxima página começa no maior desta
                            var lastTimestamp = rows.Max(x => MicrovixConverters.ToInt64(x.dto.timestamp));

                            if (lastTimestamp <= timestamp)
                                break;

                            timestamp = lastTimestamp;
                        }

                        // leitura incompleta não é gravada: a próxima execução sai do MAX(timestamp) e pularia o que faltou
                        if (failed || rows.Count == 0)
                            continue;

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_vendas_pos_agrupamento_tmp; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales.LinxVendasPosAgrupamentoTmp>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_vendas_pos_agrupamento_tmp)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxVendasPosAgrupamentoTmpValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxVendasPosAgrupamentoTmp {dto.portal} {dto.id_vendas_pos_agrupamento_tmp}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_vendas_pos_agrupamento_tmp))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales.LinxVendasPosAgrupamentoTmp(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_vendas_pos_agrupamento_tmp}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixPosSalesRepository.BulkInsertIntoLinxVendasPosAgrupamentoTmp(_listSomenteNovos, _logger.GetExecutionGuid());

                        foreach (var record in records)
                            _logger.AddRecord(record.Key, record.Value);

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
    }
}

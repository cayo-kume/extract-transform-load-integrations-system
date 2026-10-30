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
    /// Configurações tributárias do Web Service de Saída Padrão (ERP). Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixTaxSettingsService : ILinxMicrovixTaxSettingsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixTaxSettingsRepository _linxMicrovixTaxSettingsRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributarias> _linxConfiguracoesTributariasValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhes> _linxConfiguracoesTributariasDetalhesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhesSimplificado> _linxConfiguracoesTributariasDetalhesSimplificadoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasEmpresas> _linxConfiguracoesTributariasEmpresasValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixTaxSettingsService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixTaxSettingsRepository linxMicrovixTaxSettingsRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributarias> linxConfiguracoesTributariasValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhes> linxConfiguracoesTributariasDetalhesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhesSimplificado> linxConfiguracoesTributariasDetalhesSimplificadoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasEmpresas> linxConfiguracoesTributariasEmpresasValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixTaxSettingsRepository, _linxConfiguracoesTributariasValidator, _linxConfiguracoesTributariasDetalhesValidator, _linxConfiguracoesTributariasDetalhesSimplificadoValidator, _linxConfiguracoesTributariasEmpresasValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixTaxSettingsRepository, linxConfiguracoesTributariasValidator, linxConfiguracoesTributariasDetalhesValidator, linxConfiguracoesTributariasDetalhesSimplificadoValidator, linxConfiguracoesTributariasEmpresasValidator);

        public async Task<bool?> LinxConfiguracoesTributarias()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxConfiguracoesTributarias);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxConfiguracoesTributarias, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributarias dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxConfiguracoesTributarias"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxConfiguracoesTributarias", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxConfiguracoesTributarias (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxConfiguracoesTributarias (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxConfiguracoesTributarias (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributarias
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                    id_config_tributaria = row.GetValueOrDefault("id_config_tributaria"),
                                    desc_config_tributaria = row.GetValueOrDefault("desc_config_tributaria"),
                                    sigla_config_tributaria = row.GetValueOrDefault("sigla_config_tributaria"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    ativa = row.GetValueOrDefault("ativa"),
                                    uf = row.GetValueOrDefault("uf"),
                                    sistema_tributacao = row.GetValueOrDefault("sistema_tributacao"),
                                    tipo_atividade = row.GetValueOrDefault("tipo_atividade"),
                                    id_origem_mercadoria = row.GetValueOrDefault("id_origem_mercadoria"),
                                    utiliza_uso_consumo = row.GetValueOrDefault("utiliza_uso_consumo"),
                                    id_classificacao_cest_produto = row.GetValueOrDefault("id_classificacao_cest_produto"),
                                    codigo_ws = row.GetValueOrDefault("codigo_ws")
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_config_tributaria; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings.LinxConfiguracoesTributarias>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_config_tributaria).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxConfiguracoesTributariasValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxConfiguracoesTributarias {dto.id_config_tributaria} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_config_tributaria))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings.LinxConfiguracoesTributarias(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_config_tributaria}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixTaxSettingsRepository.BulkInsertIntoLinxConfiguracoesTributarias(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxConfiguracoesTributariasDetalhes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxConfiguracoesTributariasDetalhes);

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
                        // o método exige um id_config_tributaria por chamada: os valores vêm do LinxConfiguracoesTributarias já gravado
                        var sourceValues = await _linxMicrovixService.GetSourceValues(company, EnumJob.LinxConfiguracoesTributarias, "id_config_tributaria", cnpj_emp, null);

                        if (sourceValues.Count == 0)
                        {
                            _logger.AddMessage($"Empresa {company.doc_company}, loja {cnpj_emp} sem id_config_tributaria em linxmicrovix.LinxConfiguracoesTributarias: rode o LinxConfiguracoesTributarias antes.");
                            continue;
                        }

                        foreach (var id_config_tributaria in sourceValues)
                        {
                            // timestamp exclusivo, por id_config_tributaria: um valor novo começa do 0 sem pular os outros
                            var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxConfiguracoesTributariasDetalhes, cnpj_emp, "id_config_tributaria", id_config_tributaria);
                            var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhes dto, string xml)>();
                            var failed = false;

                            for (var page = 1; page <= MaxPages; page++)
                            {
                                var requestBody = new XDocument(
                                    new XDeclaration("1.0", "utf-8", null),
                                    new XElement("LinxMicrovix",
                                        new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                        new XElement("ResponseFormat", "xml"),
                                        new XElement("Command",
                                            new XElement("Name", "LinxConfiguracoesTributariasDetalhes"),
                                            new XElement("Parameters",
                                                new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                                new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                                new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                                new XElement("Parameter", new XAttribute("id", "id_config_tributaria"), id_config_tributaria)
                                            )
                                        )
                                    )
                                );

                                // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                                ApiResponse? response = null;

                                for (var attempt = 1; attempt <= 2; attempt++)
                                {
                                    using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                    response = await _apiClient.SendAsync(HttpMethod.Post, "LinxConfiguracoesTributariasDetalhes", content);

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
                                    _logger.AddMessage($"Erro da API Linx Microvix em LinxConfiguracoesTributariasDetalhes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}, id_config_tributaria {id_config_tributaria}): {response?.StatusCode} {body}");

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
                                    _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxConfiguracoesTributariasDetalhes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}, id_config_tributaria {id_config_tributaria}): {message}");

                                    failed = true;
                                    break;
                                }

                                var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                                var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                                // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                                if (columns.Count == 0 && pageRows.Count > 0)
                                {
                                    _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxConfiguracoesTributariasDetalhes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}, id_config_tributaria {id_config_tributaria}): {pageRows[0].Value}");

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
                                        new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhes
                                        {
                                        portal = row.GetValueOrDefault("portal"),
                                        cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                        id_config_tributaria = row.GetValueOrDefault("id_config_tributaria"),
                                        desc_classe_fiscal = row.GetValueOrDefault("desc_classe_fiscal"),
                                        cod_natureza_operacao = row.GetValueOrDefault("cod_natureza_operacao"),
                                        desc_natureza_operacao = row.GetValueOrDefault("desc_natureza_operacao"),
                                        cfop_fiscal = row.GetValueOrDefault("cfop_fiscal"),
                                        desc_cfop_fiscal = row.GetValueOrDefault("desc_cfop_fiscal"),
                                        aliq_icms = row.GetValueOrDefault("aliq_icms"),
                                        valor_tributado_icms = row.GetValueOrDefault("valor_tributado_icms"),
                                        aliq_pis = row.GetValueOrDefault("aliq_pis"),
                                        aliq_cofins = row.GetValueOrDefault("aliq_cofins"),
                                        perc_reducao_icms = row.GetValueOrDefault("perc_reducao_icms"),
                                        perc_reducao_icms_st = row.GetValueOrDefault("perc_reducao_icms_st"),
                                        margem_st = row.GetValueOrDefault("margem_st"),
                                        aliquota_st = row.GetValueOrDefault("aliquota_st"),
                                        margem_st_simulador = row.GetValueOrDefault("margem_st_simulador"),
                                        aliquota_st_simulador = row.GetValueOrDefault("aliquota_st_simulador"),
                                        desconto_icms = row.GetValueOrDefault("desconto_icms"),
                                        cst_icms_fiscal = row.GetValueOrDefault("cst_icms_fiscal"),
                                        desc_cst_icms_fiscal = row.GetValueOrDefault("desc_cst_icms_fiscal"),
                                        cst_ipi_fiscal = row.GetValueOrDefault("cst_ipi_fiscal"),
                                        desc_cst_ipi_fiscal = row.GetValueOrDefault("desc_cst_ipi_fiscal"),
                                        cst_pis_fiscal = row.GetValueOrDefault("cst_pis_fiscal"),
                                        desc_cst_pis_fiscal = row.GetValueOrDefault("desc_cst_pis_fiscal"),
                                        cst_cofins_fiscal = row.GetValueOrDefault("cst_cofins_fiscal"),
                                        desc_cst_cofins_fiscal = row.GetValueOrDefault("desc_cst_cofins_fiscal"),
                                        desc_obs_padrao = row.GetValueOrDefault("desc_obs_padrao"),
                                        icms_credito = row.GetValueOrDefault("icms_credito"),
                                        ipi_credito = row.GetValueOrDefault("ipi_credito"),
                                        cod_enquadramento_ipi = row.GetValueOrDefault("cod_enquadramento_ipi"),
                                        desc_enquadramento_ipi = row.GetValueOrDefault("desc_enquadramento_ipi"),
                                        perc_aliquota_interna_uf_destinatario = row.GetValueOrDefault("perc_aliquota_interna_uf_destinatario"),
                                        perc_aliquota_interestadual_uf_envolvidas = row.GetValueOrDefault("perc_aliquota_interestadual_uf_envolvidas"),
                                        csosn_fiscal = row.GetValueOrDefault("csosn_fiscal"),
                                        desc_csosn_fiscal = row.GetValueOrDefault("desc_csosn_fiscal"),
                                        forma_tributacao_pis = row.GetValueOrDefault("forma_tributacao_pis"),
                                        forma_tributacao_cofins = row.GetValueOrDefault("forma_tributacao_cofins"),
                                        id_ramo_atividade = row.GetValueOrDefault("id_ramo_atividade"),
                                        empresa = row.GetValueOrDefault("empresa"),
                                        usa_base_icms_para_calculo_st = row.GetValueOrDefault("usa_base_icms_para_calculo_st"),
                                        id_classe_fiscal = row.GetValueOrDefault("id_classe_fiscal"),
                                        id_cfop_fiscal = row.GetValueOrDefault("id_cfop_fiscal"),
                                        id_cst_icms_fiscal = row.GetValueOrDefault("id_cst_icms_fiscal"),
                                        id_cst_ipi_fiscal = row.GetValueOrDefault("id_cst_ipi_fiscal"),
                                        id_cst_pis_fiscal = row.GetValueOrDefault("id_cst_pis_fiscal"),
                                        id_cst_cofins_fiscal = row.GetValueOrDefault("id_cst_cofins_fiscal"),
                                        id_obs = row.GetValueOrDefault("id_obs"),
                                        tributado = row.GetValueOrDefault("tributado"),
                                        retido = row.GetValueOrDefault("retido"),
                                        icms_base_naotributado = row.GetValueOrDefault("icms_base_naotributado"),
                                        ipi_base_naotributado = row.GetValueOrDefault("ipi_base_naotributado"),
                                        id_csosn_fiscal = row.GetValueOrDefault("id_csosn_fiscal"),
                                        somar_icms_st = row.GetValueOrDefault("somar_icms_st"),
                                        id_sped_tipo_base_credito = row.GetValueOrDefault("id_sped_tipo_base_credito"),
                                        aliq_pis_servico = row.GetValueOrDefault("aliq_pis_servico"),
                                        aliq_cofins_servico = row.GetValueOrDefault("aliq_cofins_servico"),
                                        id_cst_pis_fiscal_servico = row.GetValueOrDefault("id_cst_pis_fiscal_servico"),
                                        id_cst_cofins_fiscal_servico = row.GetValueOrDefault("id_cst_cofins_fiscal_servico"),
                                        csrf = row.GetValueOrDefault("csrf"),
                                        codigo_retencao_csrf = row.GetValueOrDefault("codigo_retencao_csrf"),
                                        aliq_pis_csrf = row.GetValueOrDefault("aliq_pis_csrf"),
                                        aliq_cofins_csrf = row.GetValueOrDefault("aliq_cofins_csrf"),
                                        aliq_csll_csrf = row.GetValueOrDefault("aliq_csll_csrf"),
                                        id_sped_tipo_base_credito_servico = row.GetValueOrDefault("id_sped_tipo_base_credito_servico"),
                                        receita = row.GetValueOrDefault("receita"),
                                        fci_informa_parcela_importada = row.GetValueOrDefault("fci_informa_parcela_importada"),
                                        fci_informa_numero = row.GetValueOrDefault("fci_informa_numero"),
                                        fci_informa_conteudo_importacao = row.GetValueOrDefault("fci_informa_conteudo_importacao"),
                                        fci_informa_valor_importacao = row.GetValueOrDefault("fci_informa_valor_importacao"),
                                        iss_tipo_tributacao = row.GetValueOrDefault("iss_tipo_tributacao"),
                                        icms_st_antecipado_margem = row.GetValueOrDefault("icms_st_antecipado_margem"),
                                        icms_st_antecipado_aliquota = row.GetValueOrDefault("icms_st_antecipado_aliquota"),
                                        icms_st_antecipado_percentual_reducao = row.GetValueOrDefault("icms_st_antecipado_percentual_reducao"),
                                        icms_st_antecipado_valor_integra_custo_medio = row.GetValueOrDefault("icms_st_antecipado_valor_integra_custo_medio"),
                                        id_enquadramento_ipi = row.GetValueOrDefault("id_enquadramento_ipi"),
                                        usa_regime_estimativa_simplifica = row.GetValueOrDefault("usa_regime_estimativa_simplifica"),
                                        aliq_carga_tributaria_media = row.GetValueOrDefault("aliq_carga_tributaria_media"),
                                        fcp_integra_custo_medio = row.GetValueOrDefault("fcp_integra_custo_medio"),
                                        aliq_fcp = row.GetValueOrDefault("aliq_fcp"),
                                        fcp_credito = row.GetValueOrDefault("fcp_credito"),
                                        fcp_reducao_icms = row.GetValueOrDefault("fcp_reducao_icms"),
                                        aliq_icms_efetivo = row.GetValueOrDefault("aliq_icms_efetivo"),
                                        perc_reducao_icms_efetivo = row.GetValueOrDefault("perc_reducao_icms_efetivo"),
                                        icms_fcp_st_antecipado_aliquota = row.GetValueOrDefault("icms_fcp_st_antecipado_aliquota"),
                                        difal_base_dupla = row.GetValueOrDefault("difal_base_dupla"),
                                        aliq_desoneracao_icms = row.GetValueOrDefault("aliq_desoneracao_icms"),
                                        aliq_desoneracao_fcp = row.GetValueOrDefault("aliq_desoneracao_fcp"),
                                        cod_beneficio_fiscal = row.GetValueOrDefault("cod_beneficio_fiscal"),
                                        id_motivo_desoneracao_icms = row.GetValueOrDefault("id_motivo_desoneracao_icms"),
                                        deduzir_icms_custo = row.GetValueOrDefault("deduzir_icms_custo"),
                                        deduzir_icms_desonerado = row.GetValueOrDefault("deduzir_icms_desonerado"),
                                        somar_ipi_base_difal_fcp = row.GetValueOrDefault("somar_ipi_base_difal_fcp"),
                                        tipo_base_dupla = row.GetValueOrDefault("tipo_base_dupla"),
                                        utiliza_lucro_base_icms = row.GetValueOrDefault("utiliza_lucro_base_icms"),
                                        utiliza_lucro_base_pis = row.GetValueOrDefault("utiliza_lucro_base_pis"),
                                        utiliza_lucro_base_cofins = row.GetValueOrDefault("utiliza_lucro_base_cofins"),
                                        codigo_ws = row.GetValueOrDefault("codigo_ws"),
                                        timestamp = row.GetValueOrDefault("timestamp"),
                                        aliquota_diferimento_icms = row.GetValueOrDefault("aliquota_diferimento_icms"),
                                        aliquota_diferimento_fcp = row.GetValueOrDefault("aliquota_diferimento_fcp")
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

                            // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_config_tributaria + cod_natureza_operacao + id_classe_fiscal; fica a linha de maior timestamp
                            var batchTime = _linxMicrovixService.GetBatchTime();

                            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhes>();
                            var records = new Dictionary<string, string>();

                            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.id_config_tributaria, x.dto.cod_natureza_operacao, x.dto.id_classe_fiscal)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                            {
                                var validations = _linxConfiguracoesTributariasDetalhesValidator.Validate(dto);

                                if (validations.Errors.Count > 0)
                                {
                                    var message = String.Empty;
                                    validations.Errors.ForEach(s => message += $"LinxConfiguracoesTributariasDetalhes {dto.id_config_tributaria} {dto.cod_natureza_operacao} {dto.id_classe_fiscal} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                    _logger.AddMessage(message);
                                }

                                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                                if (String.IsNullOrEmpty(dto.id_config_tributaria) && String.IsNullOrEmpty(dto.cod_natureza_operacao) && String.IsNullOrEmpty(dto.id_classe_fiscal))
                                    continue;

                                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhes(company.doc_company!, cnpj_emp, batchTime, dto);

                                _listSomenteNovos.Add(entity);
                                records[$"{entity.cnpj_emp}|{entity.id_config_tributaria}|{entity.cod_natureza_operacao}|{entity.id_classe_fiscal}"] = xml;
                            }

                            if (_listSomenteNovos.Count == 0)
                                continue;

                            await _linxMicrovixTaxSettingsRepository.BulkInsertIntoLinxConfiguracoesTributariasDetalhes(_listSomenteNovos, _logger.GetExecutionGuid());

                            foreach (var record in records)
                                _logger.AddRecord(record.Key, record.Value);

                            totalInseridos += _listSomenteNovos.Count;
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

        public async Task<bool?> LinxConfiguracoesTributariasDetalhesSimplificado()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxConfiguracoesTributariasDetalhesSimplificado);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxConfiguracoesTributariasDetalhesSimplificado, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhesSimplificado dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxConfiguracoesTributariasDetalhesSimplificado"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxConfiguracoesTributariasDetalhesSimplificado", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxConfiguracoesTributariasDetalhesSimplificado (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxConfiguracoesTributariasDetalhesSimplificado (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxConfiguracoesTributariasDetalhesSimplificado (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhesSimplificado
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_config_tributaria_detalhe = row.GetValueOrDefault("id_config_tributaria_detalhe"),
                                    id_config_tributaria = row.GetValueOrDefault("id_config_tributaria"),
                                    cod_natureza_operacao = row.GetValueOrDefault("cod_natureza_operacao"),
                                    id_classe_fiscal = row.GetValueOrDefault("id_classe_fiscal"),
                                    id_cst_icms_fiscal = row.GetValueOrDefault("id_cst_icms_fiscal"),
                                    id_csosn_fiscal = row.GetValueOrDefault("id_csosn_fiscal"),
                                    id_cfop_fiscal = row.GetValueOrDefault("id_cfop_fiscal"),
                                    ipi_credito = row.GetValueOrDefault("ipi_credito"),
                                    icms_credito = row.GetValueOrDefault("icms_credito"),
                                    aliq_icms = row.GetValueOrDefault("aliq_icms"),
                                    perc_reducao_icms = row.GetValueOrDefault("perc_reducao_icms"),
                                    aliquota_st = row.GetValueOrDefault("aliquota_st"),
                                    margem_st = row.GetValueOrDefault("margem_st"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_config_tributaria_detalhe; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhesSimplificado>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_config_tributaria_detalhe)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxConfiguracoesTributariasDetalhesSimplificadoValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxConfiguracoesTributariasDetalhesSimplificado {dto.portal} {dto.id_config_tributaria_detalhe}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_config_tributaria_detalhe))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasDetalhesSimplificado(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_config_tributaria_detalhe}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixTaxSettingsRepository.BulkInsertIntoLinxConfiguracoesTributariasDetalhesSimplificado(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxConfiguracoesTributariasEmpresas()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxConfiguracoesTributariasEmpresas);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxConfiguracoesTributariasEmpresas, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasEmpresas dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxConfiguracoesTributariasEmpresas"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxConfiguracoesTributariasEmpresas", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxConfiguracoesTributariasEmpresas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxConfiguracoesTributariasEmpresas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxConfiguracoesTributariasEmpresas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasEmpresas
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_config_tributaria = row.GetValueOrDefault("id_config_tributaria"),
                                    empresa = row.GetValueOrDefault("empresa"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_config_tributaria + empresa; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasEmpresas>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.id_config_tributaria, x.dto.empresa)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxConfiguracoesTributariasEmpresasValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxConfiguracoesTributariasEmpresas {dto.id_config_tributaria} {dto.empresa} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_config_tributaria) && String.IsNullOrEmpty(dto.empresa))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings.LinxConfiguracoesTributariasEmpresas(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_config_tributaria}|{entity.empresa}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixTaxSettingsRepository.BulkInsertIntoLinxConfiguracoesTributariasEmpresas(_listSomenteNovos, _logger.GetExecutionGuid());

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

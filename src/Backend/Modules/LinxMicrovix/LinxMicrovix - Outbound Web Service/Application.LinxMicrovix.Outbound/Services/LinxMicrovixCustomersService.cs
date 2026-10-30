using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.Enums;
using Domain.LinxMicrovix.Outbound.Extensions;
using Domain.Shared.Abstractions.Api;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Application.LinxMicrovix.Outbound.Services
{
    /// <summary>
    /// Clientes e fornecedores do Web Service de Saída Padrão (ERP). Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixCustomersService : ILinxMicrovixCustomersService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixCustomersRepository _linxMicrovixCustomersRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornec> _linxClientesFornecValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecCamposAdicionais> _linxClientesFornecCamposAdicionaisValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecClasses> _linxClientesFornecClassesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecContatos> _linxClientesFornecContatosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecContatosParentesco> _linxClientesFornecContatosParentescoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecCreditoAvulso> _linxClientesFornecCreditoAvulsoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecVendedores> _linxClientesFornecVendedoresValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesEnderecosEntrega> _linxClientesEnderecosEntregaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesRede> _linxClientesRedeValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxRamosAtividade> _linxRamosAtividadeValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixCustomersService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixCustomersRepository linxMicrovixCustomersRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornec> linxClientesFornecValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecCamposAdicionais> linxClientesFornecCamposAdicionaisValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecClasses> linxClientesFornecClassesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecContatos> linxClientesFornecContatosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecContatosParentesco> linxClientesFornecContatosParentescoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecCreditoAvulso> linxClientesFornecCreditoAvulsoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecVendedores> linxClientesFornecVendedoresValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesEnderecosEntrega> linxClientesEnderecosEntregaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesRede> linxClientesRedeValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxRamosAtividade> linxRamosAtividadeValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixCustomersRepository, _linxClientesFornecValidator, _linxClientesFornecCamposAdicionaisValidator, _linxClientesFornecClassesValidator, _linxClientesFornecContatosValidator, _linxClientesFornecContatosParentescoValidator, _linxClientesFornecCreditoAvulsoValidator, _linxClientesFornecVendedoresValidator, _linxClientesEnderecosEntregaValidator, _linxClientesRedeValidator, _linxRamosAtividadeValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixCustomersRepository, linxClientesFornecValidator, linxClientesFornecCamposAdicionaisValidator, linxClientesFornecClassesValidator, linxClientesFornecContatosValidator, linxClientesFornecContatosParentescoValidator, linxClientesFornecCreditoAvulsoValidator, linxClientesFornecVendedoresValidator, linxClientesEnderecosEntregaValidator, linxClientesRedeValidator, linxRamosAtividadeValidator);

        public async Task<bool?> LinxClientesFornec()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesFornec);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    // datas obrigatórias: intervalo largo (a leitura é pelo timestamp, em páginas)
                    var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxClientesFornec, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornec dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxClientesFornec"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                            new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "data_fim"), endDate),
                                            new XElement("Parameter", new XAttribute("id", "dt_update_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "dt_update_fim"), endDate)
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxClientesFornec", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxClientesFornec (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornec (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornec (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornec
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cod_cliente = row.GetValueOrDefault("cod_cliente"),
                                    razao_cliente = row.GetValueOrDefault("razao_cliente"),
                                    nome_cliente = row.GetValueOrDefault("nome_cliente"),
                                    doc_cliente = row.GetValueOrDefault("doc_cliente"),
                                    tipo_cliente = row.GetValueOrDefault("tipo_cliente"),
                                    endereco_cliente = row.GetValueOrDefault("endereco_cliente"),
                                    numero_rua_cliente = row.GetValueOrDefault("numero_rua_cliente"),
                                    complement_end_cli = row.GetValueOrDefault("complement_end_cli"),
                                    bairro_cliente = row.GetValueOrDefault("bairro_cliente"),
                                    cep_cliente = row.GetValueOrDefault("cep_cliente"),
                                    cidade_cliente = row.GetValueOrDefault("cidade_cliente"),
                                    uf_cliente = row.GetValueOrDefault("uf_cliente"),
                                    pais = row.GetValueOrDefault("pais"),
                                    fone_cliente = row.GetValueOrDefault("fone_cliente"),
                                    email_cliente = row.GetValueOrDefault("email_cliente"),
                                    sexo = row.GetValueOrDefault("sexo"),
                                    data_cadastro = row.GetValueOrDefault("data_cadastro"),
                                    data_nascimento = row.GetValueOrDefault("data_nascimento"),
                                    cel_cliente = row.GetValueOrDefault("cel_cliente"),
                                    ativo = row.GetValueOrDefault("ativo"),
                                    dt_update = row.GetValueOrDefault("dt_update"),
                                    inscricao_estadual = row.GetValueOrDefault("inscricao_estadual"),
                                    incricao_municipal = row.GetValueOrDefault("incricao_municipal"),
                                    identidade_cliente = row.GetValueOrDefault("identidade_cliente"),
                                    cartao_fidelidade = row.GetValueOrDefault("cartao_fidelidade"),
                                    cod_ibge_municipio = row.GetValueOrDefault("cod_ibge_municipio"),
                                    classe_cliente = row.GetValueOrDefault("classe_cliente"),
                                    matricula_conveniado = row.GetValueOrDefault("matricula_conveniado"),
                                    tipo_cadastro = row.GetValueOrDefault("tipo_cadastro"),
                                    empresa_cadastro = row.GetValueOrDefault("empresa_cadastro"),
                                    id_estado_civil = row.GetValueOrDefault("id_estado_civil"),
                                    fax_cliente = row.GetValueOrDefault("fax_cliente"),
                                    site_cliente = row.GetValueOrDefault("site_cliente"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    cliente_anonimo = row.GetValueOrDefault("cliente_anonimo"),
                                    limite_compras = row.GetValueOrDefault("limite_compras"),
                                    codigo_ws = row.GetValueOrDefault("codigo_ws"),
                                    limite_credito_compra = row.GetValueOrDefault("limite_credito_compra"),
                                    id_classe_fiscal = row.GetValueOrDefault("id_classe_fiscal"),
                                    obs = row.GetValueOrDefault("obs"),
                                    mae = row.GetValueOrDefault("mae"),
                                    cliente_contribuinte = row.GetValueOrDefault("cliente_contribuinte"),
                                    id_tipo_registro_profissional = row.GetValueOrDefault("id_tipo_registro_profissional"),
                                    registro_profissional = row.GetValueOrDefault("registro_profissional"),
                                    recebe_newsletter = row.GetValueOrDefault("recebe_newsletter"),
                                    diferencial_icms_fornec = row.GetValueOrDefault("diferencial_icms_fornec"),
                                    perc_diferencial_icms_fornec = row.GetValueOrDefault("perc_diferencial_icms_fornec"),
                                    id_classe_preferencial = row.GetValueOrDefault("id_classe_preferencial"),
                                    id_subclasse_preferencial = row.GetValueOrDefault("id_subclasse_preferencial"),
                                    id_gestao_fiscal_perfil_tributario_crm = row.GetValueOrDefault("id_gestao_fiscal_perfil_tributario_crm"),
                                    fone_2 = row.GetValueOrDefault("fone_2")
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

                        totalInseridos += await SaveLinxClientesFornec(company.doc_company!, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxClientesFornecCamposAdicionais()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesFornecCamposAdicionais);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    // datas obrigatórias: intervalo largo (a leitura é pelo timestamp, em páginas)
                    var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.Erp);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.LinxGrupoLojas: rode o LinxGrupoLojas antes.");
                        continue;
                    }

                    foreach (var cnpj_emp in stores)
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxClientesFornecCamposAdicionais, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecCamposAdicionais dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxClientesFornecCamposAdicionais"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                            new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "data_fim"), endDate)
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxClientesFornecCamposAdicionais", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxClientesFornecCamposAdicionais (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecCamposAdicionais (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecCamposAdicionais (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecCamposAdicionais
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cod_cliente = row.GetValueOrDefault("cod_cliente"),
                                    campo = row.GetValueOrDefault("campo"),
                                    valor = row.GetValueOrDefault("valor"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + cod_cliente + campo; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecCamposAdicionais>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.cod_cliente, x.dto.campo)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxClientesFornecCamposAdicionaisValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxClientesFornecCamposAdicionais {dto.cod_cliente} {dto.campo} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.cod_cliente) && String.IsNullOrEmpty(dto.campo))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecCamposAdicionais(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.cod_cliente}|{entity.campo}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixCustomersRepository.BulkInsertIntoLinxClientesFornecCamposAdicionais(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxClientesFornecClasses()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesFornecClasses);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxClientesFornecClasses, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecClasses dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxClientesFornecClasses"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxClientesFornecClasses", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxClientesFornecClasses (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecClasses (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecClasses (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecClasses
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cod_cliente = row.GetValueOrDefault("cod_cliente"),
                                    cod_classe = row.GetValueOrDefault("cod_classe"),
                                    nome_classe = row.GetValueOrDefault("nome_classe"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + cod_cliente + cod_classe; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecClasses>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.cod_cliente, x.dto.cod_classe)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxClientesFornecClassesValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxClientesFornecClasses {dto.cod_cliente} {dto.cod_classe} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.cod_cliente) && String.IsNullOrEmpty(dto.cod_classe))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecClasses(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.cod_cliente}|{entity.cod_classe}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixCustomersRepository.BulkInsertIntoLinxClientesFornecClasses(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxClientesFornecContatos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesFornecContatos);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    // datas obrigatórias: intervalo largo (a leitura é pelo timestamp, em páginas)
                    var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxClientesFornecContatos, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecContatos dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxClientesFornecContatos"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                            new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "data_fim"), endDate)
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxClientesFornecContatos", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxClientesFornecContatos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecContatos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecContatos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecContatos
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cod_cliente = row.GetValueOrDefault("cod_cliente"),
                                    nome_contato = row.GetValueOrDefault("nome_contato"),
                                    sexo_contato = row.GetValueOrDefault("sexo_contato"),
                                    contatos_clientes_parentesco = row.GetValueOrDefault("contatos_clientes_parentesco"),
                                    fone1_contato = row.GetValueOrDefault("fone1_contato"),
                                    fone2_contato = row.GetValueOrDefault("fone2_contato"),
                                    celular_contato = row.GetValueOrDefault("celular_contato"),
                                    email_contato = row.GetValueOrDefault("email_contato"),
                                    data_nasc_contato = row.GetValueOrDefault("data_nasc_contato"),
                                    tipo_contato = row.GetValueOrDefault("tipo_contato"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + cod_cliente + nome_contato + tipo_contato; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecContatos>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.cod_cliente, x.dto.nome_contato, x.dto.tipo_contato)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxClientesFornecContatosValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxClientesFornecContatos {dto.portal} {dto.cod_cliente} {dto.nome_contato} {dto.tipo_contato}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.cod_cliente) && String.IsNullOrEmpty(dto.nome_contato) && String.IsNullOrEmpty(dto.tipo_contato))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecContatos(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.cod_cliente}|{entity.nome_contato}|{entity.tipo_contato}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixCustomersRepository.BulkInsertIntoLinxClientesFornecContatos(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxClientesFornecContatosParentesco()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesFornecContatosParentesco);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxClientesFornecContatosParentesco, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecContatosParentesco dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxClientesFornecContatosParentesco"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxClientesFornecContatosParentesco", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxClientesFornecContatosParentesco (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecContatosParentesco (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecContatosParentesco (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecContatosParentesco
                                    {
                                    id_parentesco = row.GetValueOrDefault("id_parentesco"),
                                    descricao_parentesco = row.GetValueOrDefault("descricao_parentesco"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    portal = row.GetValueOrDefault("portal")
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_parentesco; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecContatosParentesco>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_parentesco)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxClientesFornecContatosParentescoValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxClientesFornecContatosParentesco {dto.portal} {dto.id_parentesco}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_parentesco))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecContatosParentesco(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_parentesco}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixCustomersRepository.BulkInsertIntoLinxClientesFornecContatosParentesco(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxClientesFornecCreditoAvulso()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesFornecCreditoAvulso);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    // datas obrigatórias: intervalo largo (a leitura é pelo timestamp, em páginas)
                    var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.Erp);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.LinxGrupoLojas: rode o LinxGrupoLojas antes.");
                        continue;
                    }

                    foreach (var cnpj_emp in stores)
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxClientesFornecCreditoAvulso, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecCreditoAvulso dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxClientesFornecCreditoAvulso"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                            new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "data_fim"), endDate)
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxClientesFornecCreditoAvulso", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxClientesFornecCreditoAvulso (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecCreditoAvulso (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecCreditoAvulso (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecCreditoAvulso
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    cod_cliente = row.GetValueOrDefault("cod_cliente"),
                                    controle = row.GetValueOrDefault("controle"),
                                    data = row.GetValueOrDefault("data"),
                                    cd = row.GetValueOrDefault("cd"),
                                    valor = row.GetValueOrDefault("valor"),
                                    motivo = row.GetValueOrDefault("motivo"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    identificador = row.GetValueOrDefault("identificador"),
                                    cnpj_emp = row.GetValueOrDefault("cnpj_emp")
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + cod_cliente + controle; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecCreditoAvulso>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.cod_cliente, x.dto.controle)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxClientesFornecCreditoAvulsoValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxClientesFornecCreditoAvulso {dto.cod_cliente} {dto.controle} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.cod_cliente) && String.IsNullOrEmpty(dto.controle))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecCreditoAvulso(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.cod_cliente}|{entity.controle}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixCustomersRepository.BulkInsertIntoLinxClientesFornecCreditoAvulso(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxClientesFornecVendedores()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesFornecVendedores);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxClientesFornecVendedores, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecVendedores dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxClientesFornecVendedores"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxClientesFornecVendedores", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxClientesFornecVendedores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecVendedores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesFornecVendedores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornecVendedores
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cod_cliente = row.GetValueOrDefault("cod_cliente"),
                                    cod_vendedor = row.GetValueOrDefault("cod_vendedor"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + cod_cliente + cod_vendedor; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecVendedores>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.cod_cliente, x.dto.cod_vendedor)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxClientesFornecVendedoresValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxClientesFornecVendedores {dto.cod_cliente} {dto.cod_vendedor} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.cod_cliente) && String.IsNullOrEmpty(dto.cod_vendedor))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornecVendedores(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.cod_cliente}|{entity.cod_vendedor}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixCustomersRepository.BulkInsertIntoLinxClientesFornecVendedores(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxClientesEnderecosEntrega()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesEnderecosEntrega);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxClientesEnderecosEntrega, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesEnderecosEntrega dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxClientesEnderecosEntrega"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxClientesEnderecosEntrega", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxClientesEnderecosEntrega (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesEnderecosEntrega (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesEnderecosEntrega (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesEnderecosEntrega
                                    {
                                    id_endereco_entrega = row.GetValueOrDefault("id_endereco_entrega"),
                                    cod_cliente = row.GetValueOrDefault("cod_cliente"),
                                    endereco_cliente = row.GetValueOrDefault("endereco_cliente"),
                                    numero_rua_cliente = row.GetValueOrDefault("numero_rua_cliente"),
                                    complemento_end_cli = row.GetValueOrDefault("complemento_end_cli"),
                                    cep_cliente = row.GetValueOrDefault("cep_cliente"),
                                    bairro_cliente = row.GetValueOrDefault("bairro_cliente"),
                                    cidade_cliente = row.GetValueOrDefault("cidade_cliente"),
                                    uf_cliente = row.GetValueOrDefault("uf_cliente"),
                                    descricao = row.GetValueOrDefault("descricao"),
                                    principal = row.GetValueOrDefault("principal"),
                                    fone_cliente = row.GetValueOrDefault("fone_cliente"),
                                    fone_celular = row.GetValueOrDefault("fone_celular"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    portal = row.GetValueOrDefault("portal")
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_endereco_entrega; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesEnderecosEntrega>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_endereco_entrega)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxClientesEnderecosEntregaValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxClientesEnderecosEntrega {dto.portal} {dto.id_endereco_entrega}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_endereco_entrega))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesEnderecosEntrega(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_endereco_entrega}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixCustomersRepository.BulkInsertIntoLinxClientesEnderecosEntrega(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxClientesRede()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesRede);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxClientesRede, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesRede dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxClientesRede"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxClientesRede", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxClientesRede (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesRede (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxClientesRede (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesRede
                                    {
                                    id_clientes_rede = row.GetValueOrDefault("id_clientes_rede"),
                                    doc_cliente = row.GetValueOrDefault("doc_cliente"),
                                    nome_razao_social = row.GetValueOrDefault("nome_razao_social"),
                                    data_cadastro = row.GetValueOrDefault("data_cadastro"),
                                    pf_pj = row.GetValueOrDefault("pf_pj"),
                                    tipo = row.GetValueOrDefault("tipo"),
                                    cidade = row.GetValueOrDefault("cidade"),
                                    uf = row.GetValueOrDefault("uf"),
                                    pais = row.GetValueOrDefault("pais"),
                                    id_estado_civil = row.GetValueOrDefault("id_estado_civil"),
                                    compras_a_prazo = row.GetValueOrDefault("compras_a_prazo"),
                                    aceita_boleto_bancario = row.GetValueOrDefault("aceita_boleto_bancario"),
                                    nome_fantasia = row.GetValueOrDefault("nome_fantasia"),
                                    numero_endereco = row.GetValueOrDefault("numero_endereco"),
                                    complemento = row.GetValueOrDefault("complemento"),
                                    bairro = row.GetValueOrDefault("bairro"),
                                    cep = row.GetValueOrDefault("cep"),
                                    telefone = row.GetValueOrDefault("telefone"),
                                    celular = row.GetValueOrDefault("celular"),
                                    fax = row.GetValueOrDefault("fax"),
                                    email = row.GetValueOrDefault("email"),
                                    site = row.GetValueOrDefault("site"),
                                    data_nascimento = row.GetValueOrDefault("data_nascimento"),
                                    naturalidade = row.GetValueOrDefault("naturalidade"),
                                    sexo = row.GetValueOrDefault("sexo"),
                                    nome_pai = row.GetValueOrDefault("nome_pai"),
                                    nome_mae = row.GetValueOrDefault("nome_mae"),
                                    identidade_cliente = row.GetValueOrDefault("identidade_cliente"),
                                    data_emissao_identidade_cliente = row.GetValueOrDefault("data_emissao_identidade_cliente"),
                                    uf_emissao_identidade_cliente = row.GetValueOrDefault("uf_emissao_identidade_cliente"),
                                    orgao_emissao_identidade_cliente = row.GetValueOrDefault("orgao_emissao_identidade_cliente"),
                                    nome_empresa_titular = row.GetValueOrDefault("nome_empresa_titular"),
                                    telefone_empresa_titular = row.GetValueOrDefault("telefone_empresa_titular"),
                                    funcao_titular = row.GetValueOrDefault("funcao_titular"),
                                    tempo_servico_titular = row.GetValueOrDefault("tempo_servico_titular"),
                                    renda_titular = row.GetValueOrDefault("renda_titular"),
                                    inscricao_estadual = row.GetValueOrDefault("inscricao_estadual"),
                                    inscricao_municipal = row.GetValueOrDefault("inscricao_municipal"),
                                    cliente_optante_simples_municipal = row.GetValueOrDefault("cliente_optante_simples_municipal"),
                                    cliente_optante_simples_federal = row.GetValueOrDefault("cliente_optante_simples_federal"),
                                    limite_compras = row.GetValueOrDefault("limite_compras"),
                                    cartao_fidelidade = row.GetValueOrDefault("cartao_fidelidade"),
                                    desabilitado = row.GetValueOrDefault("desabilitado"),
                                    motivo_bloqueio = row.GetValueOrDefault("motivo_bloqueio"),
                                    portal_origem = row.GetValueOrDefault("portal_origem"),
                                    empresa_origem = row.GetValueOrDefault("empresa_origem"),
                                    cod_cliente_portal_origem = row.GetValueOrDefault("cod_cliente_portal_origem"),
                                    codigo_ws = row.GetValueOrDefault("codigo_ws"),
                                    integrado_ws = row.GetValueOrDefault("integrado_ws"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_clientes_rede; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesRede>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_clientes_rede).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxClientesRedeValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxClientesRede {dto.id_clientes_rede} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_clientes_rede))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesRede(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_clientes_rede}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixCustomersRepository.BulkInsertIntoLinxClientesRede(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxRamosAtividade()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxRamosAtividade);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxRamosAtividade, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxRamosAtividade dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxRamosAtividade"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxRamosAtividade", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxRamosAtividade (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxRamosAtividade (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxRamosAtividade (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxRamosAtividade
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_ramo_atividade = row.GetValueOrDefault("id_ramo_atividade"),
                                    nome_ramo_atividade = row.GetValueOrDefault("nome_ramo_atividade"),
                                    codigo_ramo_atividade = row.GetValueOrDefault("codigo_ramo_atividade"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_ramo_atividade; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxRamosAtividade>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_ramo_atividade).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxRamosAtividadeValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxRamosAtividade {dto.id_ramo_atividade} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_ramo_atividade))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxRamosAtividade(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_ramo_atividade}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixCustomersRepository.BulkInsertIntoLinxRamosAtividade(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxClientesFornecByDocCliente(string user_name, string doc_company, string doc_cliente)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxClientesFornecByDocCliente);

            // rota registrada no log de requisições; o corpo enviado vai sem a chave
            const string route = "LinxClientesFornec";
            bool? result;

            try
            {
                var company = await _linxMicrovixService.GetCompany(doc_company);

                if (company?.erp_chave is null)
                {
                    // nenhuma chamada à API: não há o que registrar em LinxMicrovixRequests (igual ao Movidesk e ao Totvs)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em linxmicrovix.LinxMicrovixParameters ou sem a chave do ERP.");
                    return null;
                }

                // dado do portal: lido pela primeira loja do cliente, como no método recorrente
                var cnpj_emp = (await _linxMicrovixService.GetStores(company, EnumApi.Erp)).FirstOrDefault();

                if (cnpj_emp is null)
                {
                    _logger.AddMessage($"Empresa {doc_company} sem lojas do ERP gravadas: rode o LinxGrupoLojas antes.");
                    return null;
                }

                var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

                // timestamp 0: a busca pela chave traz o registro inteiro, em qualquer versão
                long timestamp = 0;
                var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornec dto, string xml)>();
                string? logBody = null;
                string? lastResponse = null;
                var failed = false;

                try
                {
                    for (var page = 1; page <= MaxPages; page++)
                    {
                        var requestBody = new XDocument(
                            new XDeclaration("1.0", "utf-8", null),
                            new XElement("LinxMicrovix",
                                new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                new XElement("ResponseFormat", "xml"),
                                new XElement("Command",
                                    new XElement("Name", "LinxClientesFornec"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                        new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                        new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                        new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                        new XElement("Parameter", new XAttribute("id", "data_fim"), endDate),
                                        new XElement("Parameter", new XAttribute("id", "dt_update_inicial"), "2000-01-01"),
                                        new XElement("Parameter", new XAttribute("id", "dt_update_fim"), endDate),
                                        new XElement("Parameter", new XAttribute("id", "doc_cliente"), doc_cliente)
                                    )
                                )
                            )
                        );

                        logBody = Regex.Replace($"{requestBody.Declaration}{requestBody}", "(<Parameter id=\"chave\">)[^<]*", "$1(oculta)");

                        // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                        ApiResponse? response = null;

                        for (var attempt = 1; attempt <= 2; attempt++)
                        {
                            using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                            response = await _apiClient.SendAsync(HttpMethod.Post, route, content);

                            if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(10));
                                continue;
                            }

                            break;
                        }

                        lastResponse = response?.Content;

                        if (response is null || !response.IsSuccess)
                        {
                            var body = response?.Content?.Length > 800 ? response.Content[..800] : response?.Content;
                            _logger.AddMessage($"Erro da API Linx Microvix em {route} (empresa {doc_company}, {doc_cliente}): {response?.StatusCode} {body}");

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
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {doc_cliente}): {message}");

                            failed = true;
                            break;
                        }

                        var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                        var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                        // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                        if (columns.Count == 0 && pageRows.Count > 0)
                        {
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {doc_cliente}): {pageRows[0].Value}");

                            failed = true;
                            break;
                        }

                        // resposta vazia: não há mais registros
                        if (pageRows.Count == 0)
                            break;

                        foreach (var r in pageRows)
                        {
                            var values = r.Elements("D").Select(d => d.IsEmpty ? null : d.Value).ToList();
                            var row = columns.Select((c, i) => (c, v: i < values.Count ? values[i] : null)).GroupBy(x => x.c).ToDictionary(g => g.Key, g => g.First().v);

                            rows.Add((
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornec
                                {
                                portal = row.GetValueOrDefault("portal"),
                                cod_cliente = row.GetValueOrDefault("cod_cliente"),
                                razao_cliente = row.GetValueOrDefault("razao_cliente"),
                                nome_cliente = row.GetValueOrDefault("nome_cliente"),
                                doc_cliente = row.GetValueOrDefault("doc_cliente"),
                                tipo_cliente = row.GetValueOrDefault("tipo_cliente"),
                                endereco_cliente = row.GetValueOrDefault("endereco_cliente"),
                                numero_rua_cliente = row.GetValueOrDefault("numero_rua_cliente"),
                                complement_end_cli = row.GetValueOrDefault("complement_end_cli"),
                                bairro_cliente = row.GetValueOrDefault("bairro_cliente"),
                                cep_cliente = row.GetValueOrDefault("cep_cliente"),
                                cidade_cliente = row.GetValueOrDefault("cidade_cliente"),
                                uf_cliente = row.GetValueOrDefault("uf_cliente"),
                                pais = row.GetValueOrDefault("pais"),
                                fone_cliente = row.GetValueOrDefault("fone_cliente"),
                                email_cliente = row.GetValueOrDefault("email_cliente"),
                                sexo = row.GetValueOrDefault("sexo"),
                                data_cadastro = row.GetValueOrDefault("data_cadastro"),
                                data_nascimento = row.GetValueOrDefault("data_nascimento"),
                                cel_cliente = row.GetValueOrDefault("cel_cliente"),
                                ativo = row.GetValueOrDefault("ativo"),
                                dt_update = row.GetValueOrDefault("dt_update"),
                                inscricao_estadual = row.GetValueOrDefault("inscricao_estadual"),
                                incricao_municipal = row.GetValueOrDefault("incricao_municipal"),
                                identidade_cliente = row.GetValueOrDefault("identidade_cliente"),
                                cartao_fidelidade = row.GetValueOrDefault("cartao_fidelidade"),
                                cod_ibge_municipio = row.GetValueOrDefault("cod_ibge_municipio"),
                                classe_cliente = row.GetValueOrDefault("classe_cliente"),
                                matricula_conveniado = row.GetValueOrDefault("matricula_conveniado"),
                                tipo_cadastro = row.GetValueOrDefault("tipo_cadastro"),
                                empresa_cadastro = row.GetValueOrDefault("empresa_cadastro"),
                                id_estado_civil = row.GetValueOrDefault("id_estado_civil"),
                                fax_cliente = row.GetValueOrDefault("fax_cliente"),
                                site_cliente = row.GetValueOrDefault("site_cliente"),
                                timestamp = row.GetValueOrDefault("timestamp"),
                                cliente_anonimo = row.GetValueOrDefault("cliente_anonimo"),
                                limite_compras = row.GetValueOrDefault("limite_compras"),
                                codigo_ws = row.GetValueOrDefault("codigo_ws"),
                                limite_credito_compra = row.GetValueOrDefault("limite_credito_compra"),
                                id_classe_fiscal = row.GetValueOrDefault("id_classe_fiscal"),
                                obs = row.GetValueOrDefault("obs"),
                                mae = row.GetValueOrDefault("mae"),
                                cliente_contribuinte = row.GetValueOrDefault("cliente_contribuinte"),
                                id_tipo_registro_profissional = row.GetValueOrDefault("id_tipo_registro_profissional"),
                                registro_profissional = row.GetValueOrDefault("registro_profissional"),
                                recebe_newsletter = row.GetValueOrDefault("recebe_newsletter"),
                                diferencial_icms_fornec = row.GetValueOrDefault("diferencial_icms_fornec"),
                                perc_diferencial_icms_fornec = row.GetValueOrDefault("perc_diferencial_icms_fornec"),
                                id_classe_preferencial = row.GetValueOrDefault("id_classe_preferencial"),
                                id_subclasse_preferencial = row.GetValueOrDefault("id_subclasse_preferencial"),
                                id_gestao_fiscal_perfil_tributario_crm = row.GetValueOrDefault("id_gestao_fiscal_perfil_tributario_crm"),
                                fone_2 = row.GetValueOrDefault("fone_2")
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
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _linxMicrovixService.SaveRequest(EnumJob.LinxClientesFornecByDocCliente, user_name, "POST", route, doc_company, $"{doc_cliente}", $"Exceção ao chamar a API: {ex.Message}", false, logBody);
                    throw;
                }

                if (failed)
                    result = null;
                else if (rows.Count == 0)
                {
                    // a busca responde sem linhas quando a chave não existe
                    _logger.AddMessage($"LinxClientesFornec {doc_cliente} não encontrado no Linx Microvix (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    await SaveLinxClientesFornec(doc_company, _linxMicrovixService.GetBatchTime(), rows);

                    _logger.AddMessage($"Concluída com sucesso: LinxClientesFornec {doc_cliente} inserido ou atualizado ({rows.Count} linha(s))!");
                    result = true;
                }

                _linxMicrovixService.SaveRequest(EnumJob.LinxClientesFornecByDocCliente, user_name, "POST", route, doc_company, $"{doc_cliente}", lastResponse, result == true, logBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        // ------------------------------------------------------------------------------------------------------

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + cod_cliente; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxClientesFornec(string doc_company, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers.LinxClientesFornec dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornec>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.cod_cliente)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxClientesFornecValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxClientesFornec {dto.portal} {dto.cod_cliente}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.cod_cliente))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers.LinxClientesFornec(doc_company, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.portal}|{entity.cod_cliente}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixCustomersRepository.BulkInsertIntoLinxClientesFornec(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }
    }
}

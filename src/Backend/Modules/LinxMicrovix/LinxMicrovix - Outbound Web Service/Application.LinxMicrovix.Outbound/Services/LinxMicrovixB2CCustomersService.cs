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
    /// Clientes do B2C do WS B2C. Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixB2CCustomersService : ILinxMicrovixB2CCustomersService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixB2CCustomersRepository _linxMicrovixB2CCustomersRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientes> _b2CConsultaClientesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatos> _b2CConsultaClientesContatosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatosParentesco> _b2CConsultaClientesContatosParentescoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEnderecosEntrega> _b2CConsultaClientesEnderecosEntregaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEstadoCivil> _b2CConsultaClientesEstadoCivilValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldo> _b2CConsultaClientesSaldoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldoLinx> _b2CConsultaClientesSaldoLinxValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixB2CCustomersService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixB2CCustomersRepository linxMicrovixB2CCustomersRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientes> b2CConsultaClientesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatos> b2CConsultaClientesContatosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatosParentesco> b2CConsultaClientesContatosParentescoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEnderecosEntrega> b2CConsultaClientesEnderecosEntregaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEstadoCivil> b2CConsultaClientesEstadoCivilValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldo> b2CConsultaClientesSaldoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldoLinx> b2CConsultaClientesSaldoLinxValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixB2CCustomersRepository, _b2CConsultaClientesValidator, _b2CConsultaClientesContatosValidator, _b2CConsultaClientesContatosParentescoValidator, _b2CConsultaClientesEnderecosEntregaValidator, _b2CConsultaClientesEstadoCivilValidator, _b2CConsultaClientesSaldoValidator, _b2CConsultaClientesSaldoLinxValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixB2CCustomersRepository, b2CConsultaClientesValidator, b2CConsultaClientesContatosValidator, b2CConsultaClientesContatosParentescoValidator, b2CConsultaClientesEnderecosEntregaValidator, b2CConsultaClientesEstadoCivilValidator, b2CConsultaClientesSaldoValidator, b2CConsultaClientesSaldoLinxValidator);

        public async Task<bool?> B2CConsultaClientes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaClientes);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.B2C);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.B2CConsultaEmpresas: rode o B2CConsultaEmpresas antes.");
                        continue;
                    }

                    // a resposta não identifica a loja: o dado é do portal, lido pela primeira loja
                    foreach (var cnpj_emp in stores.Take(1))
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaClientes, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientes dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaClientes"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaClientes", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaClientes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientes
                                    {
                                    cod_cliente_b2c = row.GetValueOrDefault("cod_cliente_b2c"),
                                    cod_cliente_erp = row.GetValueOrDefault("cod_cliente_erp"),
                                    doc_cliente = row.GetValueOrDefault("doc_cliente"),
                                    nm_cliente = row.GetValueOrDefault("nm_cliente"),
                                    nm_mae = row.GetValueOrDefault("nm_mae"),
                                    nm_pai = row.GetValueOrDefault("nm_pai"),
                                    nm_conjuge = row.GetValueOrDefault("nm_conjuge"),
                                    dt_cadastro = row.GetValueOrDefault("dt_cadastro"),
                                    dt_nasc_cliente = row.GetValueOrDefault("dt_nasc_cliente"),
                                    end_cliente = row.GetValueOrDefault("end_cliente"),
                                    complemento_end_cliente = row.GetValueOrDefault("complemento_end_cliente"),
                                    nr_rua_cliente = row.GetValueOrDefault("nr_rua_cliente"),
                                    bairro_cliente = row.GetValueOrDefault("bairro_cliente"),
                                    cep_cliente = row.GetValueOrDefault("cep_cliente"),
                                    cidade_cliente = row.GetValueOrDefault("cidade_cliente"),
                                    uf_cliente = row.GetValueOrDefault("uf_cliente"),
                                    fone_cliente = row.GetValueOrDefault("fone_cliente"),
                                    fone_comercial = row.GetValueOrDefault("fone_comercial"),
                                    cel_cliente = row.GetValueOrDefault("cel_cliente"),
                                    email_cliente = row.GetValueOrDefault("email_cliente"),
                                    rg_cliente = row.GetValueOrDefault("rg_cliente"),
                                    rg_orgao_emissor = row.GetValueOrDefault("rg_orgao_emissor"),
                                    estado_civil_cliente = row.GetValueOrDefault("estado_civil_cliente"),
                                    empresa_cliente = row.GetValueOrDefault("empresa_cliente"),
                                    cargo_cliente = row.GetValueOrDefault("cargo_cliente"),
                                    sexo_cliente = row.GetValueOrDefault("sexo_cliente"),
                                    dt_update = row.GetValueOrDefault("dt_update"),
                                    ativo = row.GetValueOrDefault("ativo"),
                                    receber_email = row.GetValueOrDefault("receber_email"),
                                    dt_expedicao_rg = row.GetValueOrDefault("dt_expedicao_rg"),
                                    naturalidade = row.GetValueOrDefault("naturalidade"),
                                    tempo_residencia = row.GetValueOrDefault("tempo_residencia"),
                                    renda = row.GetValueOrDefault("renda"),
                                    numero_compl_rua_cliente = row.GetValueOrDefault("numero_compl_rua_cliente"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    tipo_pessoa = row.GetValueOrDefault("tipo_pessoa"),
                                    portal = row.GetValueOrDefault("portal"),
                                    aceita_programa_fidelidade = row.GetValueOrDefault("aceita_programa_fidelidade"),
                                    inscricao_cliente = row.GetValueOrDefault("inscricao_cliente")
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

                        totalInseridos += await SaveB2CConsultaClientes(company.doc_company!, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaClientesContatos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaClientesContatos);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.B2C);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.B2CConsultaEmpresas: rode o B2CConsultaEmpresas antes.");
                        continue;
                    }

                    foreach (var cnpj_emp in stores)
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaClientesContatos, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatos dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaClientesContatos"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaClientesContatos", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaClientesContatos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesContatos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesContatos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatos
                                    {
                                    id_clientes_contatos = row.GetValueOrDefault("id_clientes_contatos"),
                                    id_contato_b2c = row.GetValueOrDefault("id_contato_b2c"),
                                    nome_contato = row.GetValueOrDefault("nome_contato"),
                                    data_nasc_contato = row.GetValueOrDefault("data_nasc_contato"),
                                    sexo_contato = row.GetValueOrDefault("sexo_contato"),
                                    id_parentesco = row.GetValueOrDefault("id_parentesco"),
                                    fone_contato = row.GetValueOrDefault("fone_contato"),
                                    celular_contato = row.GetValueOrDefault("celular_contato"),
                                    email_contato = row.GetValueOrDefault("email_contato"),
                                    cod_cliente_erp = row.GetValueOrDefault("cod_cliente_erp"),
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

                        totalInseridos += await SaveB2CConsultaClientesContatos(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaClientesContatosParentesco()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaClientesContatosParentesco);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.B2C);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.B2CConsultaEmpresas: rode o B2CConsultaEmpresas antes.");
                        continue;
                    }

                    // a resposta não identifica a loja: o dado é do portal, lido pela primeira loja
                    foreach (var cnpj_emp in stores.Take(1))
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaClientesContatosParentesco, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatosParentesco dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaClientesContatosParentesco"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaClientesContatosParentesco", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaClientesContatosParentesco (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesContatosParentesco (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesContatosParentesco (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatosParentesco
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

                        totalInseridos += await SaveB2CConsultaClientesContatosParentesco(company.doc_company!, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaClientesEnderecosEntrega()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaClientesEnderecosEntrega);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.B2C);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.B2CConsultaEmpresas: rode o B2CConsultaEmpresas antes.");
                        continue;
                    }

                    // a resposta não identifica a loja: o dado é do portal, lido pela primeira loja
                    foreach (var cnpj_emp in stores.Take(1))
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaClientesEnderecosEntrega, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEnderecosEntrega dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaClientesEnderecosEntrega"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaClientesEnderecosEntrega", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaClientesEnderecosEntrega (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesEnderecosEntrega (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesEnderecosEntrega (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEnderecosEntrega
                                    {
                                    id_endereco_entrega = row.GetValueOrDefault("id_endereco_entrega"),
                                    cod_cliente_erp = row.GetValueOrDefault("cod_cliente_erp"),
                                    cod_cliente_b2c = row.GetValueOrDefault("cod_cliente_b2c"),
                                    endereco_cliente = row.GetValueOrDefault("endereco_cliente"),
                                    numero_rua_cliente = row.GetValueOrDefault("numero_rua_cliente"),
                                    complemento_end_cli = row.GetValueOrDefault("complemento_end_cli"),
                                    cep_cliente = row.GetValueOrDefault("cep_cliente"),
                                    bairro_cliente = row.GetValueOrDefault("bairro_cliente"),
                                    cidade_cliente = row.GetValueOrDefault("cidade_cliente"),
                                    uf_cliente = row.GetValueOrDefault("uf_cliente"),
                                    descricao = row.GetValueOrDefault("descricao"),
                                    principal = row.GetValueOrDefault("principal"),
                                    id_cidade = row.GetValueOrDefault("id_cidade"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    id_endereco_entrega_b2c = row.GetValueOrDefault("id_endereco_entrega_b2c"),
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

                        totalInseridos += await SaveB2CConsultaClientesEnderecosEntrega(company.doc_company!, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaClientesEstadoCivil()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaClientesEstadoCivil);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.B2C);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.B2CConsultaEmpresas: rode o B2CConsultaEmpresas antes.");
                        continue;
                    }

                    // a resposta não identifica a loja: o dado é do portal, lido pela primeira loja
                    foreach (var cnpj_emp in stores.Take(1))
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaClientesEstadoCivil, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEstadoCivil dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaClientesEstadoCivil"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaClientesEstadoCivil", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaClientesEstadoCivil (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesEstadoCivil (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesEstadoCivil (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEstadoCivil
                                    {
                                    id_estado_civil = row.GetValueOrDefault("id_estado_civil"),
                                    estado_civil = row.GetValueOrDefault("estado_civil"),
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

                        totalInseridos += await SaveB2CConsultaClientesEstadoCivil(company.doc_company!, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaClientesSaldo()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaClientesSaldo);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.B2C);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.B2CConsultaEmpresas: rode o B2CConsultaEmpresas antes.");
                        continue;
                    }

                    foreach (var cnpj_emp in stores)
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaClientesSaldo, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldo dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaClientesSaldo"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaClientesSaldo", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaClientesSaldo (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesSaldo (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesSaldo (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldo
                                    {
                                    saldo = row.GetValueOrDefault("saldo"),
                                    cod_cliente_erp = row.GetValueOrDefault("cod_cliente_erp"),
                                    empresa = row.GetValueOrDefault("empresa"),
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

                        totalInseridos += await SaveB2CConsultaClientesSaldo(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaClientesSaldoLinx()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaClientesSaldoLinx);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.B2C);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.B2CConsultaEmpresas: rode o B2CConsultaEmpresas antes.");
                        continue;
                    }

                    foreach (var cnpj_emp in stores)
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaClientesSaldoLinx, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldoLinx dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaClientesSaldoLinx"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaClientesSaldoLinx", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaClientesSaldoLinx (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesSaldoLinx (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClientesSaldoLinx (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldoLinx
                                    {
                                    cod_cliente_erp = row.GetValueOrDefault("cod_cliente_erp"),
                                    cod_cliente_b2c = row.GetValueOrDefault("cod_cliente_b2c"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    valor = row.GetValueOrDefault("valor"),
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

                        totalInseridos += await SaveB2CConsultaClientesSaldoLinx(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaClientesByDocCliente(string user_name, string doc_company, string doc_cliente)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaClientesByDocCliente);

            // rota registrada no log de requisições; o corpo enviado vai sem a chave
            const string route = "B2CConsultaClientes";
            bool? result;

            try
            {
                var company = await _linxMicrovixService.GetCompany(doc_company);

                if (company?.b2c_chave is null)
                {
                    // nenhuma chamada à API: não há o que registrar em LinxMicrovixRequests (igual ao Movidesk e ao Totvs)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em linxmicrovix.LinxMicrovixParameters ou sem a chave do B2C.");
                    return null;
                }

                // dado do portal: lido pela primeira loja do cliente, como no método recorrente
                var cnpj_emp = (await _linxMicrovixService.GetStores(company, EnumApi.B2C)).FirstOrDefault();

                if (cnpj_emp is null)
                {
                    _logger.AddMessage($"Empresa {doc_company} sem lojas do B2C gravadas: rode o B2CConsultaEmpresas antes.");
                    return null;
                }

                // timestamp 0: a busca pela chave traz o registro inteiro, em qualquer versão
                long timestamp = 0;
                var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientes dto, string xml)>();
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
                                new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                new XElement("ResponseFormat", "xml"),
                                new XElement("Command",
                                    new XElement("Name", "B2CConsultaClientes"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
                                        new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                        new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
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
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientes
                                {
                                cod_cliente_b2c = row.GetValueOrDefault("cod_cliente_b2c"),
                                cod_cliente_erp = row.GetValueOrDefault("cod_cliente_erp"),
                                doc_cliente = row.GetValueOrDefault("doc_cliente"),
                                nm_cliente = row.GetValueOrDefault("nm_cliente"),
                                nm_mae = row.GetValueOrDefault("nm_mae"),
                                nm_pai = row.GetValueOrDefault("nm_pai"),
                                nm_conjuge = row.GetValueOrDefault("nm_conjuge"),
                                dt_cadastro = row.GetValueOrDefault("dt_cadastro"),
                                dt_nasc_cliente = row.GetValueOrDefault("dt_nasc_cliente"),
                                end_cliente = row.GetValueOrDefault("end_cliente"),
                                complemento_end_cliente = row.GetValueOrDefault("complemento_end_cliente"),
                                nr_rua_cliente = row.GetValueOrDefault("nr_rua_cliente"),
                                bairro_cliente = row.GetValueOrDefault("bairro_cliente"),
                                cep_cliente = row.GetValueOrDefault("cep_cliente"),
                                cidade_cliente = row.GetValueOrDefault("cidade_cliente"),
                                uf_cliente = row.GetValueOrDefault("uf_cliente"),
                                fone_cliente = row.GetValueOrDefault("fone_cliente"),
                                fone_comercial = row.GetValueOrDefault("fone_comercial"),
                                cel_cliente = row.GetValueOrDefault("cel_cliente"),
                                email_cliente = row.GetValueOrDefault("email_cliente"),
                                rg_cliente = row.GetValueOrDefault("rg_cliente"),
                                rg_orgao_emissor = row.GetValueOrDefault("rg_orgao_emissor"),
                                estado_civil_cliente = row.GetValueOrDefault("estado_civil_cliente"),
                                empresa_cliente = row.GetValueOrDefault("empresa_cliente"),
                                cargo_cliente = row.GetValueOrDefault("cargo_cliente"),
                                sexo_cliente = row.GetValueOrDefault("sexo_cliente"),
                                dt_update = row.GetValueOrDefault("dt_update"),
                                ativo = row.GetValueOrDefault("ativo"),
                                receber_email = row.GetValueOrDefault("receber_email"),
                                dt_expedicao_rg = row.GetValueOrDefault("dt_expedicao_rg"),
                                naturalidade = row.GetValueOrDefault("naturalidade"),
                                tempo_residencia = row.GetValueOrDefault("tempo_residencia"),
                                renda = row.GetValueOrDefault("renda"),
                                numero_compl_rua_cliente = row.GetValueOrDefault("numero_compl_rua_cliente"),
                                timestamp = row.GetValueOrDefault("timestamp"),
                                tipo_pessoa = row.GetValueOrDefault("tipo_pessoa"),
                                portal = row.GetValueOrDefault("portal"),
                                aceita_programa_fidelidade = row.GetValueOrDefault("aceita_programa_fidelidade"),
                                inscricao_cliente = row.GetValueOrDefault("inscricao_cliente")
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
                    _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaClientesByDocCliente, user_name, "POST", route, doc_company, $"{doc_cliente}", $"Exceção ao chamar a API: {ex.Message}", false, logBody);
                    throw;
                }

                if (failed)
                    result = null;
                else if (rows.Count == 0)
                {
                    // a busca responde sem linhas quando a chave não existe
                    _logger.AddMessage($"B2CConsultaClientes {doc_cliente} não encontrado no Linx Microvix (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    await SaveB2CConsultaClientes(doc_company, _linxMicrovixService.GetBatchTime(), rows);

                    _logger.AddMessage($"Concluída com sucesso: B2CConsultaClientes {doc_cliente} inserido ou atualizado ({rows.Count} linha(s))!");
                    result = true;
                }

                _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaClientesByDocCliente, user_name, "POST", route, doc_company, $"{doc_cliente}", lastResponse, result == true, logBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + cod_cliente_erp; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaClientes(string doc_company, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientes dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientes>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.cod_cliente_erp)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaClientesValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaClientes {dto.portal} {dto.cod_cliente_erp}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.cod_cliente_erp))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientes(doc_company, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.portal}|{entity.cod_cliente_erp}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2CCustomersRepository.BulkInsertIntoB2CConsultaClientes(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_clientes_contatos; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaClientesContatos(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatos dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesContatos>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_clientes_contatos).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaClientesContatosValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaClientesContatos {dto.id_clientes_contatos} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.id_clientes_contatos))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesContatos(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.id_clientes_contatos}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2CCustomersRepository.BulkInsertIntoB2CConsultaClientesContatos(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_parentesco; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaClientesContatosParentesco(string doc_company, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesContatosParentesco dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesContatosParentesco>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_parentesco)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaClientesContatosParentescoValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaClientesContatosParentesco {dto.portal} {dto.id_parentesco}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_parentesco))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesContatosParentesco(doc_company, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.portal}|{entity.id_parentesco}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2CCustomersRepository.BulkInsertIntoB2CConsultaClientesContatosParentesco(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_endereco_entrega; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaClientesEnderecosEntrega(string doc_company, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEnderecosEntrega dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesEnderecosEntrega>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_endereco_entrega)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaClientesEnderecosEntregaValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaClientesEnderecosEntrega {dto.portal} {dto.id_endereco_entrega}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_endereco_entrega))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesEnderecosEntrega(doc_company, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.portal}|{entity.id_endereco_entrega}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2CCustomersRepository.BulkInsertIntoB2CConsultaClientesEnderecosEntrega(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_estado_civil; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaClientesEstadoCivil(string doc_company, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesEstadoCivil dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesEstadoCivil>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_estado_civil)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaClientesEstadoCivilValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaClientesEstadoCivil {dto.portal} {dto.id_estado_civil}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_estado_civil))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesEstadoCivil(doc_company, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.portal}|{entity.id_estado_civil}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2CCustomersRepository.BulkInsertIntoB2CConsultaClientesEstadoCivil(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + cod_cliente_erp + empresa; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaClientesSaldo(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldo dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesSaldo>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.cod_cliente_erp, x.dto.empresa)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaClientesSaldoValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaClientesSaldo {dto.cod_cliente_erp} {dto.empresa} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.cod_cliente_erp) && String.IsNullOrEmpty(dto.empresa))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesSaldo(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.cod_cliente_erp}|{entity.empresa}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2CCustomersRepository.BulkInsertIntoB2CConsultaClientesSaldo(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + cod_cliente_erp + empresa; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaClientesSaldoLinx(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers.B2CConsultaClientesSaldoLinx dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesSaldoLinx>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.cod_cliente_erp, x.dto.empresa)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaClientesSaldoLinxValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaClientesSaldoLinx {dto.cod_cliente_erp} {dto.empresa} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.cod_cliente_erp) && String.IsNullOrEmpty(dto.empresa))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers.B2CConsultaClientesSaldoLinx(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.cod_cliente_erp}|{entity.empresa}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2CCustomersRepository.BulkInsertIntoB2CConsultaClientesSaldoLinx(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }
    }
}

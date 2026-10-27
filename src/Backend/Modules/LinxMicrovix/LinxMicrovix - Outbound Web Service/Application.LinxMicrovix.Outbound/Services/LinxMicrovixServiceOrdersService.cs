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
    /// Ordens de serviço do Web Service de Saída Padrão (ERP). Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixServiceOrdersService : ILinxMicrovixServiceOrdersService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixServiceOrdersRepository _linxMicrovixServiceOrdersRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServico> _linxOrdensServicoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServicoProdutos> _linxOrdensServicoProdutosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicos> _linxServicosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicosDetalhes> _linxServicosDetalhesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoClientes> _linxServicoClientesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaComandas> _linxServicoAgendaComandasValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaEventos> _linxServicoAgendaEventosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdemServicoExternaDav> _linxOrdemServicoExternaDavValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxMovimentoOrdemServicoExterna> _linxMovimentoOrdemServicoExternaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxValeOrdemServicoExterna> _linxValeOrdemServicoExternaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxPagamentoParcialDav> _linxPagamentoParcialDavValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixServiceOrdersService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixServiceOrdersRepository linxMicrovixServiceOrdersRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServico> linxOrdensServicoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServicoProdutos> linxOrdensServicoProdutosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicos> linxServicosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicosDetalhes> linxServicosDetalhesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoClientes> linxServicoClientesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaComandas> linxServicoAgendaComandasValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaEventos> linxServicoAgendaEventosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdemServicoExternaDav> linxOrdemServicoExternaDavValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxMovimentoOrdemServicoExterna> linxMovimentoOrdemServicoExternaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxValeOrdemServicoExterna> linxValeOrdemServicoExternaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxPagamentoParcialDav> linxPagamentoParcialDavValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixServiceOrdersRepository, _linxOrdensServicoValidator, _linxOrdensServicoProdutosValidator, _linxServicosValidator, _linxServicosDetalhesValidator, _linxServicoClientesValidator, _linxServicoAgendaComandasValidator, _linxServicoAgendaEventosValidator, _linxOrdemServicoExternaDavValidator, _linxMovimentoOrdemServicoExternaValidator, _linxValeOrdemServicoExternaValidator, _linxPagamentoParcialDavValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixServiceOrdersRepository, linxOrdensServicoValidator, linxOrdensServicoProdutosValidator, linxServicosValidator, linxServicosDetalhesValidator, linxServicoClientesValidator, linxServicoAgendaComandasValidator, linxServicoAgendaEventosValidator, linxOrdemServicoExternaDavValidator, linxMovimentoOrdemServicoExternaValidator, linxValeOrdemServicoExternaValidator, linxPagamentoParcialDavValidator);

        public async Task<bool?> LinxOrdensServico()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxOrdensServico);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxOrdensServico, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServico dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxOrdensServico"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxOrdensServico", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxOrdensServico (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxOrdensServico (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxOrdensServico (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServico
                                    {
                                    numero_os = row.GetValueOrDefault("numero_os"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    data_os = row.GetValueOrDefault("data_os"),
                                    data_envio_laboratorio = row.GetValueOrDefault("data_envio_laboratorio"),
                                    cancelado = row.GetValueOrDefault("cancelado"),
                                    id_laboratorio = row.GetValueOrDefault("id_laboratorio"),
                                    id_posicao_os_ramo_optico = row.GetValueOrDefault("id_posicao_os_ramo_optico"),
                                    compartilhar_hub_laboratorios = row.GetValueOrDefault("compartilhar_hub_laboratorios"),
                                    cod_cliente_os = row.GetValueOrDefault("cod_cliente_os"),
                                    cod_vendedor = row.GetValueOrDefault("cod_vendedor"),
                                    numero_sequencial_antecipacao_financeira = row.GetValueOrDefault("numero_sequencial_antecipacao_financeira"),
                                    chave_nfe_laboratorio = row.GetValueOrDefault("chave_nfe_laboratorio"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    portal = row.GetValueOrDefault("portal"),
                                    previsao_entrega_os = row.GetValueOrDefault("previsao_entrega_os")
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

                        totalInseridos += await SaveLinxOrdensServico(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxOrdensServicoProdutos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxOrdensServicoProdutos);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxOrdensServicoProdutos, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServicoProdutos dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxOrdensServicoProdutos"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxOrdensServicoProdutos", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxOrdensServicoProdutos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxOrdensServicoProdutos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxOrdensServicoProdutos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServicoProdutos
                                    {
                                    numero_os = row.GetValueOrDefault("numero_os"),
                                    cod_produto_serv = row.GetValueOrDefault("cod_produto_serv"),
                                    id_ordservprod = row.GetValueOrDefault("id_ordservprod"),
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

                        totalInseridos += await SaveLinxOrdensServicoProdutos(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxServicos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxServicos);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxServicos, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicos dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxServicos"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                            new XElement("Parameter", new XAttribute("id", "dt_update_inicio"), "2000-01-01"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxServicos", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxServicos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicos
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cod_servico = row.GetValueOrDefault("cod_servico"),
                                    nome = row.GetValueOrDefault("nome"),
                                    id_setor = row.GetValueOrDefault("id_setor"),
                                    desc_setor = row.GetValueOrDefault("desc_setor"),
                                    id_linha = row.GetValueOrDefault("id_linha"),
                                    desc_linha = row.GetValueOrDefault("desc_linha"),
                                    id_marca = row.GetValueOrDefault("id_marca"),
                                    desc_marca = row.GetValueOrDefault("desc_marca"),
                                    dt_update = row.GetValueOrDefault("dt_update"),
                                    operacao_servico = row.GetValueOrDefault("operacao_servico"),
                                    servico_km = row.GetValueOrDefault("servico_km"),
                                    desativado = row.GetValueOrDefault("desativado"),
                                    cod_lc11603 = row.GetValueOrDefault("cod_lc11603"),
                                    codigo_nbs = row.GetValueOrDefault("codigo_nbs"),
                                    dt_inclusao = row.GetValueOrDefault("dt_inclusao"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
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

                        totalInseridos += await SaveLinxServicos(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxServicosDetalhes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxServicosDetalhes);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxServicosDetalhes, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicosDetalhes dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxServicosDetalhes"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxServicosDetalhes", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxServicosDetalhes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicosDetalhes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicosDetalhes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicosDetalhes
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                    cod_servico = row.GetValueOrDefault("cod_servico"),
                                    preco_custo_hn = row.GetValueOrDefault("preco_custo_hn"),
                                    preco_primera_hn = row.GetValueOrDefault("preco_primera_hn"),
                                    preco_demais_hn = row.GetValueOrDefault("preco_demais_hn"),
                                    preco_custo_he = row.GetValueOrDefault("preco_custo_he"),
                                    preco_primera_he = row.GetValueOrDefault("preco_primera_he"),
                                    preco_demais_he = row.GetValueOrDefault("preco_demais_he"),
                                    preco_custo_had = row.GetValueOrDefault("preco_custo_had"),
                                    preco_primera_had = row.GetValueOrDefault("preco_primera_had"),
                                    preco_demais_had = row.GetValueOrDefault("preco_demais_had"),
                                    id_config_tributaria = row.GetValueOrDefault("id_config_tributaria"),
                                    desc_config_tributaria = row.GetValueOrDefault("desc_config_tributaria"),
                                    cod_trib_servico_municipio = row.GetValueOrDefault("cod_trib_servico_municipio"),
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

                        totalInseridos += await SaveLinxServicosDetalhes(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxServicoClientes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxServicoClientes);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxServicoClientes, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoClientes dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxServicoClientes"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxServicoClientes", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxServicoClientes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicoClientes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicoClientes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoClientes
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_cliente = row.GetValueOrDefault("id_cliente"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    id_servico = row.GetValueOrDefault("id_servico"),
                                    qtd_adquirido = row.GetValueOrDefault("qtd_adquirido"),
                                    qtd_agendado = row.GetValueOrDefault("qtd_agendado"),
                                    qtd_utilizado = row.GetValueOrDefault("qtd_utilizado"),
                                    qtd_cancelado = row.GetValueOrDefault("qtd_cancelado"),
                                    dt_insercao = row.GetValueOrDefault("dt_insercao"),
                                    tipo_desconto = row.GetValueOrDefault("tipo_desconto"),
                                    valor_desconto_por_item = row.GetValueOrDefault("valor_desconto_por_item"),
                                    id_dav_item = row.GetValueOrDefault("id_dav_item"),
                                    transacao_movimento = row.GetValueOrDefault("transacao_movimento"),
                                    ficha = row.GetValueOrDefault("ficha"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    identificador = row.GetValueOrDefault("identificador")
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

                        totalInseridos += await SaveLinxServicoClientes(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxServicoAgendaComandas()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxServicoAgendaComandas);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxServicoAgendaComandas, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaComandas dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxServicoAgendaComandas"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxServicoAgendaComandas", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxServicoAgendaComandas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicoAgendaComandas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicoAgendaComandas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaComandas
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_comanda = row.GetValueOrDefault("id_comanda"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    cliente = row.GetValueOrDefault("cliente"),
                                    data = row.GetValueOrDefault("data"),
                                    noiva = row.GetValueOrDefault("noiva"),
                                    excluida = row.GetValueOrDefault("excluida"),
                                    fechada = row.GetValueOrDefault("fechada"),
                                    observacoes = row.GetValueOrDefault("observacoes"),
                                    tipo_desconto = row.GetValueOrDefault("tipo_desconto"),
                                    valor_desconto = row.GetValueOrDefault("valor_desconto"),
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

                        totalInseridos += await SaveLinxServicoAgendaComandas(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxServicoAgendaEventos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxServicoAgendaEventos);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxServicoAgendaEventos, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaEventos dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxServicoAgendaEventos"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxServicoAgendaEventos", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxServicoAgendaEventos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicoAgendaEventos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxServicoAgendaEventos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaEventos
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_evento = row.GetValueOrDefault("id_evento"),
                                    id_espaco = row.GetValueOrDefault("id_espaco"),
                                    data_evento = row.GetValueOrDefault("data_evento"),
                                    minutos_duracao = row.GetValueOrDefault("minutos_duracao"),
                                    tipo_agendamento = row.GetValueOrDefault("tipo_agendamento"),
                                    id_cliente_servico = row.GetValueOrDefault("id_cliente_servico"),
                                    id_profissional = row.GetValueOrDefault("id_profissional"),
                                    situacao = row.GetValueOrDefault("situacao"),
                                    data_situacao = row.GetValueOrDefault("data_situacao"),
                                    observacao = row.GetValueOrDefault("observacao"),
                                    data_chegada = row.GetValueOrDefault("data_chegada"),
                                    data_execucao = row.GetValueOrDefault("data_execucao"),
                                    recorrente = row.GetValueOrDefault("recorrente"),
                                    id_comanda = row.GetValueOrDefault("id_comanda"),
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

                        totalInseridos += await SaveLinxServicoAgendaEventos(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxOrdemServicoExternaDav()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxOrdemServicoExternaDav);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxOrdemServicoExternaDav, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdemServicoExternaDav dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxOrdemServicoExternaDav"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxOrdemServicoExternaDav", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxOrdemServicoExternaDav (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxOrdemServicoExternaDav (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxOrdemServicoExternaDav (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdemServicoExternaDav
                                    {
                                    id_vendas_pos_tmp_ordem_servico_externa = row.GetValueOrDefault("id_vendas_pos_tmp_ordem_servico_externa"),
                                    id_vendas_pos = row.GetValueOrDefault("id_vendas_pos"),
                                    codigo_ordem_servico_externa = row.GetValueOrDefault("codigo_ordem_servico_externa"),
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

                        totalInseridos += await SaveLinxOrdemServicoExternaDav(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxMovimentoOrdemServicoExterna()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoOrdemServicoExterna);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimentoOrdemServicoExterna, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxMovimentoOrdemServicoExterna dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimentoOrdemServicoExterna"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimentoOrdemServicoExterna", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimentoOrdemServicoExterna (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoOrdemServicoExterna (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoOrdemServicoExterna (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxMovimentoOrdemServicoExterna
                                    {
                                    id_movimento_ordem_servico_externa = row.GetValueOrDefault("id_movimento_ordem_servico_externa"),
                                    identificador = row.GetValueOrDefault("identificador"),
                                    codigo_ordem_servico = row.GetValueOrDefault("codigo_ordem_servico"),
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

                        totalInseridos += await SaveLinxMovimentoOrdemServicoExterna(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxValeOrdemServicoExterna()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxValeOrdemServicoExterna);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxValeOrdemServicoExterna, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxValeOrdemServicoExterna dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxValeOrdemServicoExterna"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxValeOrdemServicoExterna", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxValeOrdemServicoExterna (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxValeOrdemServicoExterna (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxValeOrdemServicoExterna (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxValeOrdemServicoExterna
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                    id_vale_ordem_servico_externa = row.GetValueOrDefault("id_vale_ordem_servico_externa"),
                                    numero_controle = row.GetValueOrDefault("numero_controle"),
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

                        totalInseridos += await SaveLinxValeOrdemServicoExterna(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxPagamentoParcialDav()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxPagamentoParcialDav);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxPagamentoParcialDav, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxPagamentoParcialDav dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxPagamentoParcialDav"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxPagamentoParcialDav", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxPagamentoParcialDav (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPagamentoParcialDav (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];

                            // a API repete coluna neste método (valor): a segunda ocorrência vira <nome>_2
                            columns = columns.Select((c, i) => columns.Take(i).Contains(c) ? $"{c}_{columns.Take(i).Count(x => x == c) + 1}" : c).ToList();
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPagamentoParcialDav (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxPagamentoParcialDav
                                    {
                                    id_vendas_pos = row.GetValueOrDefault("id_vendas_pos"),
                                    valor = row.GetValueOrDefault("valor"),
                                    ajuste_valor_desc_acresc_plano = row.GetValueOrDefault("ajuste_valor_desc_acresc_plano"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    id_pagamento_parcial_tmp = row.GetValueOrDefault("id_pagamento_parcial_tmp"),
                                    plano = row.GetValueOrDefault("plano"),
                                    forma_pgto = row.GetValueOrDefault("forma_pgto"),
                                    valor_2 = row.GetValueOrDefault("valor_2"),
                                    id_bandeira = row.GetValueOrDefault("id_bandeira"),
                                    qtde_parcelas = row.GetValueOrDefault("qtde_parcelas"),
                                    credito_debito = row.GetValueOrDefault("credito_debito"),
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

                        totalInseridos += await SaveLinxPagamentoParcialDav(company.doc_company!, _linxMicrovixService.GetBatchTime(), rows);
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
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + numero_os; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxOrdensServico(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServico dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxOrdensServico>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.numero_os).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxOrdensServicoValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxOrdensServico {dto.numero_os} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.numero_os))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxOrdensServico(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.numero_os}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxOrdensServico(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_ordservprod; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxOrdensServicoProdutos(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdensServicoProdutos dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxOrdensServicoProdutos>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_ordservprod).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxOrdensServicoProdutosValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxOrdensServicoProdutos {dto.id_ordservprod} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.id_ordservprod))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxOrdensServicoProdutos(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.id_ordservprod}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxOrdensServicoProdutos(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + cod_servico; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxServicos(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicos dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicos>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.cod_servico).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxServicosValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxServicos {dto.cod_servico} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.cod_servico))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicos(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.cod_servico}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxServicos(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + cod_servico; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxServicosDetalhes(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicosDetalhes dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicosDetalhes>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.cod_servico).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxServicosDetalhesValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxServicosDetalhes {dto.cod_servico} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.cod_servico))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicosDetalhes(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.cod_servico}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxServicosDetalhes(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_cliente + id_servico + id_dav_item; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxServicoClientes(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoClientes dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicoClientes>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.id_cliente, x.dto.id_servico, x.dto.id_dav_item)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxServicoClientesValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxServicoClientes {dto.id_cliente} {dto.id_servico} {dto.id_dav_item} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.id_cliente) && String.IsNullOrEmpty(dto.id_servico) && String.IsNullOrEmpty(dto.id_dav_item))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicoClientes(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.id_cliente}|{entity.id_servico}|{entity.id_dav_item}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxServicoClientes(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_comanda; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxServicoAgendaComandas(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaComandas dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicoAgendaComandas>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_comanda).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxServicoAgendaComandasValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxServicoAgendaComandas {dto.id_comanda} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.id_comanda))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicoAgendaComandas(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.id_comanda}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxServicoAgendaComandas(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_evento; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxServicoAgendaEventos(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaEventos dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicoAgendaEventos>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_evento).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxServicoAgendaEventosValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxServicoAgendaEventos {dto.id_evento} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.id_evento))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxServicoAgendaEventos(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.id_evento}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxServicoAgendaEventos(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_vendas_pos_tmp_ordem_servico_externa; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxOrdemServicoExternaDav(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxOrdemServicoExternaDav dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxOrdemServicoExternaDav>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_vendas_pos_tmp_ordem_servico_externa).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxOrdemServicoExternaDavValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxOrdemServicoExternaDav {dto.id_vendas_pos_tmp_ordem_servico_externa} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.id_vendas_pos_tmp_ordem_servico_externa))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxOrdemServicoExternaDav(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.id_vendas_pos_tmp_ordem_servico_externa}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxOrdemServicoExternaDav(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_movimento_ordem_servico_externa; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxMovimentoOrdemServicoExterna(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxMovimentoOrdemServicoExterna dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxMovimentoOrdemServicoExterna>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_movimento_ordem_servico_externa).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxMovimentoOrdemServicoExternaValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxMovimentoOrdemServicoExterna {dto.id_movimento_ordem_servico_externa} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.id_movimento_ordem_servico_externa))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxMovimentoOrdemServicoExterna(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.id_movimento_ordem_servico_externa}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxMovimentoOrdemServicoExterna(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_vale_ordem_servico_externa; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxValeOrdemServicoExterna(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxValeOrdemServicoExterna dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxValeOrdemServicoExterna>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_vale_ordem_servico_externa).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxValeOrdemServicoExternaValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxValeOrdemServicoExterna {dto.id_vale_ordem_servico_externa} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.id_vale_ordem_servico_externa))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxValeOrdemServicoExterna(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.id_vale_ordem_servico_externa}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxValeOrdemServicoExterna(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_pagamento_parcial_tmp; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxPagamentoParcialDav(string doc_company, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders.LinxPagamentoParcialDav dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxPagamentoParcialDav>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_pagamento_parcial_tmp)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxPagamentoParcialDavValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxPagamentoParcialDav {dto.portal} {dto.id_pagamento_parcial_tmp}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_pagamento_parcial_tmp))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders.LinxPagamentoParcialDav(doc_company, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.portal}|{entity.id_pagamento_parcial_tmp}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixServiceOrdersRepository.BulkInsertIntoLinxPagamentoParcialDav(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }
    }
}

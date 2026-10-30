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
    /// Pedidos e notas do B2C do WS B2C. Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixB2COrdersService : ILinxMicrovixB2COrdersService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixB2COrdersRepository _linxMicrovixB2COrdersRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidos> _b2CConsultaPedidosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosItens> _b2CConsultaPedidosItensValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosPlanos> _b2CConsultaPedidosPlanosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosStatus> _b2CConsultaPedidosStatusValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosIdentificador> _b2CConsultaPedidosIdentificadorValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosTipos> _b2CConsultaPedidosTiposValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaStatus> _b2CConsultaStatusValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaCodigoRastreio> _b2CConsultaCodigoRastreioValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFe> _b2CConsultaNFeValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFeSituacao> _b2CConsultaNFeSituacaoValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixB2COrdersService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixB2COrdersRepository linxMicrovixB2COrdersRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidos> b2CConsultaPedidosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosItens> b2CConsultaPedidosItensValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosPlanos> b2CConsultaPedidosPlanosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosStatus> b2CConsultaPedidosStatusValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosIdentificador> b2CConsultaPedidosIdentificadorValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosTipos> b2CConsultaPedidosTiposValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaStatus> b2CConsultaStatusValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaCodigoRastreio> b2CConsultaCodigoRastreioValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFe> b2CConsultaNFeValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFeSituacao> b2CConsultaNFeSituacaoValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixB2COrdersRepository, _b2CConsultaPedidosValidator, _b2CConsultaPedidosItensValidator, _b2CConsultaPedidosPlanosValidator, _b2CConsultaPedidosStatusValidator, _b2CConsultaPedidosIdentificadorValidator, _b2CConsultaPedidosTiposValidator, _b2CConsultaStatusValidator, _b2CConsultaCodigoRastreioValidator, _b2CConsultaNFeValidator, _b2CConsultaNFeSituacaoValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixB2COrdersRepository, b2CConsultaPedidosValidator, b2CConsultaPedidosItensValidator, b2CConsultaPedidosPlanosValidator, b2CConsultaPedidosStatusValidator, b2CConsultaPedidosIdentificadorValidator, b2CConsultaPedidosTiposValidator, b2CConsultaStatusValidator, b2CConsultaCodigoRastreioValidator, b2CConsultaNFeValidator, b2CConsultaNFeSituacaoValidator);

        public async Task<bool?> B2CConsultaPedidos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPedidos);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    // datas obrigatórias: intervalo largo (a leitura é pelo timestamp, em páginas)
                    var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaPedidos, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidos dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaPedidos"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                            new XElement("Parameter", new XAttribute("id", "data_origem_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "data_origem_fim"), endDate)
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaPedidos", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaPedidos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidos
                                    {
                                    id_pedido = row.GetValueOrDefault("id_pedido"),
                                    dt_pedido = row.GetValueOrDefault("dt_pedido"),
                                    cod_cliente_erp = row.GetValueOrDefault("cod_cliente_erp"),
                                    cod_cliente_b2c = row.GetValueOrDefault("cod_cliente_b2c"),
                                    vl_frete = row.GetValueOrDefault("vl_frete"),
                                    forma_pgto = row.GetValueOrDefault("forma_pgto"),
                                    plano_pagamento = row.GetValueOrDefault("plano_pagamento"),
                                    anotacao = row.GetValueOrDefault("anotacao"),
                                    taxa_impressao = row.GetValueOrDefault("taxa_impressao"),
                                    finalizado = row.GetValueOrDefault("finalizado"),
                                    valor_frete_gratis = row.GetValueOrDefault("valor_frete_gratis"),
                                    tipo_frete = row.GetValueOrDefault("tipo_frete"),
                                    id_status = row.GetValueOrDefault("id_status"),
                                    cod_transportador = row.GetValueOrDefault("cod_transportador"),
                                    tipo_cobranca_frete = row.GetValueOrDefault("tipo_cobranca_frete"),
                                    ativo = row.GetValueOrDefault("ativo"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    id_tabela_preco = row.GetValueOrDefault("id_tabela_preco"),
                                    valor_credito = row.GetValueOrDefault("valor_credito"),
                                    cod_vendedor = row.GetValueOrDefault("cod_vendedor"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    dt_insert = row.GetValueOrDefault("dt_insert"),
                                    dt_disponivel_faturamento = row.GetValueOrDefault("dt_disponivel_faturamento"),
                                    mensagem_falha_faturamento = row.GetValueOrDefault("mensagem_falha_faturamento"),
                                    portal = row.GetValueOrDefault("portal"),
                                    order_id = row.GetValueOrDefault("order_id"),
                                    fulfillment_id = row.GetValueOrDefault("fulfillment_id"),
                                    id_tipo_b2c = row.GetValueOrDefault("id_tipo_b2c"),
                                    ecommerce_origem = row.GetValueOrDefault("ecommerce_origem"),
                                    id_endereco_entrega = row.GetValueOrDefault("id_endereco_entrega"),
                                    marketplace_loja = row.GetValueOrDefault("marketplace_loja"),
                                    desconto = row.GetValueOrDefault("desconto"),
                                    order_id_alternativo = row.GetValueOrDefault("order_id_alternativo"),
                                    cnpj_origem_venda = row.GetValueOrDefault("cnpj_origem_venda"),
                                    data_previsao_entrega = row.GetValueOrDefault("data_previsao_entrega"),
                                    marketplace_full = row.GetValueOrDefault("marketplace_full")
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

                        totalInseridos += await SaveB2CConsultaPedidos(company.doc_company!, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaPedidosItens()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPedidosItens);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaPedidosItens, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosItens dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaPedidosItens"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaPedidosItens", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaPedidosItens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosItens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosItens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosItens
                                    {
                                    id_pedido_item = row.GetValueOrDefault("id_pedido_item"),
                                    id_pedido = row.GetValueOrDefault("id_pedido"),
                                    codigoproduto = row.GetValueOrDefault("codigoproduto"),
                                    quantidade = row.GetValueOrDefault("quantidade"),
                                    vl_unitario = row.GetValueOrDefault("vl_unitario"),
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

                        totalInseridos += await SaveB2CConsultaPedidosItens(company.doc_company!, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaPedidosPlanos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPedidosPlanos);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaPedidosPlanos, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosPlanos dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaPedidosPlanos"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaPedidosPlanos", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaPedidosPlanos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosPlanos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosPlanos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosPlanos
                                    {
                                    id_pedido_planos = row.GetValueOrDefault("id_pedido_planos"),
                                    id_pedido = row.GetValueOrDefault("id_pedido"),
                                    plano_pagamento = row.GetValueOrDefault("plano_pagamento"),
                                    valor_plano = row.GetValueOrDefault("valor_plano"),
                                    nsu_sitef = row.GetValueOrDefault("nsu_sitef"),
                                    cod_autorizacao = row.GetValueOrDefault("cod_autorizacao"),
                                    texto_comprovante = row.GetValueOrDefault("texto_comprovante"),
                                    cod_loja_sitef = row.GetValueOrDefault("cod_loja_sitef"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_pedido_planos; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosPlanos>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_pedido_planos)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaPedidosPlanosValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaPedidosPlanos {dto.portal} {dto.id_pedido_planos}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_pedido_planos))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosPlanos(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_pedido_planos}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaPedidosPlanos(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaPedidosStatus()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPedidosStatus);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    // datas obrigatórias: intervalo largo (a leitura é pelo timestamp, em páginas)
                    var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaPedidosStatus, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosStatus dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaPedidosStatus"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaPedidosStatus", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaPedidosStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosStatus
                                    {
                                    id = row.GetValueOrDefault("id"),
                                    id_status = row.GetValueOrDefault("id_status"),
                                    id_pedido = row.GetValueOrDefault("id_pedido"),
                                    data_hora = row.GetValueOrDefault("data_hora"),
                                    anotacao = row.GetValueOrDefault("anotacao"),
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

                        totalInseridos += await SaveB2CConsultaPedidosStatus(company.doc_company!, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaPedidosIdentificador()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPedidosIdentificador);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    // datas obrigatórias: intervalo largo (a leitura é pelo timestamp, em páginas)
                    var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaPedidosIdentificador, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosIdentificador dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaPedidosIdentificador"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                            new XElement("Parameter", new XAttribute("id", "data_origem_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "data_origem_fim"), endDate)
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaPedidosIdentificador", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaPedidosIdentificador (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosIdentificador (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosIdentificador (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosIdentificador
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    identificador = row.GetValueOrDefault("identificador"),
                                    id_venda = row.GetValueOrDefault("id_venda"),
                                    order_id = row.GetValueOrDefault("order_id"),
                                    id_cliente = row.GetValueOrDefault("id_cliente"),
                                    valor_frete = row.GetValueOrDefault("valor_frete"),
                                    data_origem = row.GetValueOrDefault("data_origem"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + identificador; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosIdentificador>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.identificador)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaPedidosIdentificadorValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaPedidosIdentificador {dto.portal} {dto.identificador}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.identificador))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosIdentificador(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.identificador}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaPedidosIdentificador(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaPedidosTipos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPedidosTipos);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaPedidosTipos, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosTipos dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaPedidosTipos"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaPedidosTipos", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaPedidosTipos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosTipos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPedidosTipos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosTipos
                                    {
                                    id_tipo_b2c = row.GetValueOrDefault("id_tipo_b2c"),
                                    descricao = row.GetValueOrDefault("descricao"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_tipo_b2c; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosTipos>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_tipo_b2c)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaPedidosTiposValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaPedidosTipos {dto.portal} {dto.id_tipo_b2c}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_tipo_b2c))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosTipos(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_tipo_b2c}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaPedidosTipos(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaStatus()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaStatus);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaStatus, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaStatus dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaStatus"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaStatus", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaStatus
                                    {
                                    id_status = row.GetValueOrDefault("id_status"),
                                    descricao_status = row.GetValueOrDefault("descricao_status"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_status; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaStatus>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_status)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaStatusValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaStatus {dto.portal} {dto.id_status}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_status))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaStatus(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_status}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaStatus(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaCodigoRastreio()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaCodigoRastreio);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaCodigoRastreio, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaCodigoRastreio dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaCodigoRastreio"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaCodigoRastreio", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaCodigoRastreio (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaCodigoRastreio (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaCodigoRastreio (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaCodigoRastreio
                                    {
                                    id_pedido = row.GetValueOrDefault("id_pedido"),
                                    documento = row.GetValueOrDefault("documento"),
                                    serie = row.GetValueOrDefault("serie"),
                                    codigo_rastreio = row.GetValueOrDefault("codigo_rastreio"),
                                    sequencia_volume = row.GetValueOrDefault("sequencia_volume"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_pedido + codigo_rastreio; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaCodigoRastreio>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_pedido, x.dto.codigo_rastreio)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaCodigoRastreioValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaCodigoRastreio {dto.portal} {dto.id_pedido} {dto.codigo_rastreio}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_pedido) && String.IsNullOrEmpty(dto.codigo_rastreio))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaCodigoRastreio(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_pedido}|{entity.codigo_rastreio}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaCodigoRastreio(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaNFe()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaNFe);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.b2c_chave is null)
                        continue;   // cliente sem o WS B2C

                    // datas obrigatórias: intervalo largo (a leitura é pelo timestamp, em páginas)
                    var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

                    var stores = await _linxMicrovixService.GetStores(company, EnumApi.B2C);

                    if (stores.Count == 0)
                    {
                        _logger.AddMessage($"Empresa {company.doc_company} sem lojas em linxmicrovix.B2CConsultaEmpresas: rode o B2CConsultaEmpresas antes.");
                        continue;
                    }

                    foreach (var cnpj_emp in stores)
                    {
                        // timestamp exclusivo: a API devolve só os maiores que o informado; banco vazio (0) traz o histórico inteiro
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaNFe, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFe dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaNFe"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaNFe", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaNFe (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaNFe (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaNFe (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFe
                                    {
                                    id_nfe = row.GetValueOrDefault("id_nfe"),
                                    id_pedido = row.GetValueOrDefault("id_pedido"),
                                    documento = row.GetValueOrDefault("documento"),
                                    data_emissao = row.GetValueOrDefault("data_emissao"),
                                    chave_nfe = row.GetValueOrDefault("chave_nfe"),
                                    situacao = row.GetValueOrDefault("situacao"),
                                    xml = row.GetValueOrDefault("xml"),
                                    excluido = row.GetValueOrDefault("excluido"),
                                    identificador_microvix = row.GetValueOrDefault("identificador_microvix"),
                                    dt_insert = row.GetValueOrDefault("dt_insert"),
                                    valor_nota = row.GetValueOrDefault("valor_nota"),
                                    serie = row.GetValueOrDefault("serie"),
                                    frete = row.GetValueOrDefault("frete"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    portal = row.GetValueOrDefault("portal"),
                                    nProt = row.GetValueOrDefault("nProt"),
                                    codigo_modelo_nf = row.GetValueOrDefault("codigo_modelo_nf"),
                                    justificativa = row.GetValueOrDefault("justificativa"),
                                    tpAmb = row.GetValueOrDefault("tpAmb")
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

                        totalInseridos += await SaveB2CConsultaNFe(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> B2CConsultaNFeSituacao()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaNFeSituacao);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaNFeSituacao, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFeSituacao dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaNFeSituacao"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaNFeSituacao", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaNFeSituacao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaNFeSituacao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaNFeSituacao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFeSituacao
                                    {
                                    id_nfe_situacao = row.GetValueOrDefault("id_nfe_situacao"),
                                    descricao = row.GetValueOrDefault("descricao"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_nfe_situacao; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaNFeSituacao>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_nfe_situacao)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaNFeSituacaoValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaNFeSituacao {dto.portal} {dto.id_nfe_situacao}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_nfe_situacao))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaNFeSituacao(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_nfe_situacao}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaNFeSituacao(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaPedidosByIdPedido(string user_name, string doc_company, string id_pedido)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPedidosByIdPedido);

            // rota registrada no log de requisições; o corpo enviado vai sem a chave
            const string route = "B2CConsultaPedidos";
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

                var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

                // timestamp 0: a busca pela chave traz o registro inteiro, em qualquer versão
                long timestamp = 0;
                var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidos dto, string xml)>();
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
                                    new XElement("Name", "B2CConsultaPedidos"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
                                        new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                        new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                        new XElement("Parameter", new XAttribute("id", "data_origem_inicial"), "2000-01-01"),
                                        new XElement("Parameter", new XAttribute("id", "data_origem_fim"), endDate),
                                        new XElement("Parameter", new XAttribute("id", "id_pedido"), id_pedido)
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
                            _logger.AddMessage($"Erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {response?.StatusCode} {body}");

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
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {message}");

                            failed = true;
                            break;
                        }

                        var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                        var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                        // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                        if (columns.Count == 0 && pageRows.Count > 0)
                        {
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {pageRows[0].Value}");

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
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidos
                                {
                                id_pedido = row.GetValueOrDefault("id_pedido"),
                                dt_pedido = row.GetValueOrDefault("dt_pedido"),
                                cod_cliente_erp = row.GetValueOrDefault("cod_cliente_erp"),
                                cod_cliente_b2c = row.GetValueOrDefault("cod_cliente_b2c"),
                                vl_frete = row.GetValueOrDefault("vl_frete"),
                                forma_pgto = row.GetValueOrDefault("forma_pgto"),
                                plano_pagamento = row.GetValueOrDefault("plano_pagamento"),
                                anotacao = row.GetValueOrDefault("anotacao"),
                                taxa_impressao = row.GetValueOrDefault("taxa_impressao"),
                                finalizado = row.GetValueOrDefault("finalizado"),
                                valor_frete_gratis = row.GetValueOrDefault("valor_frete_gratis"),
                                tipo_frete = row.GetValueOrDefault("tipo_frete"),
                                id_status = row.GetValueOrDefault("id_status"),
                                cod_transportador = row.GetValueOrDefault("cod_transportador"),
                                tipo_cobranca_frete = row.GetValueOrDefault("tipo_cobranca_frete"),
                                ativo = row.GetValueOrDefault("ativo"),
                                empresa = row.GetValueOrDefault("empresa"),
                                id_tabela_preco = row.GetValueOrDefault("id_tabela_preco"),
                                valor_credito = row.GetValueOrDefault("valor_credito"),
                                cod_vendedor = row.GetValueOrDefault("cod_vendedor"),
                                timestamp = row.GetValueOrDefault("timestamp"),
                                dt_insert = row.GetValueOrDefault("dt_insert"),
                                dt_disponivel_faturamento = row.GetValueOrDefault("dt_disponivel_faturamento"),
                                mensagem_falha_faturamento = row.GetValueOrDefault("mensagem_falha_faturamento"),
                                portal = row.GetValueOrDefault("portal"),
                                order_id = row.GetValueOrDefault("order_id"),
                                fulfillment_id = row.GetValueOrDefault("fulfillment_id"),
                                id_tipo_b2c = row.GetValueOrDefault("id_tipo_b2c"),
                                ecommerce_origem = row.GetValueOrDefault("ecommerce_origem"),
                                id_endereco_entrega = row.GetValueOrDefault("id_endereco_entrega"),
                                marketplace_loja = row.GetValueOrDefault("marketplace_loja"),
                                desconto = row.GetValueOrDefault("desconto"),
                                order_id_alternativo = row.GetValueOrDefault("order_id_alternativo"),
                                cnpj_origem_venda = row.GetValueOrDefault("cnpj_origem_venda"),
                                data_previsao_entrega = row.GetValueOrDefault("data_previsao_entrega"),
                                marketplace_full = row.GetValueOrDefault("marketplace_full")
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
                    _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaPedidosByIdPedido, user_name, "POST", route, doc_company, $"{id_pedido}", $"Exceção ao chamar a API: {ex.Message}", false, logBody);
                    throw;
                }

                if (failed)
                    result = null;
                else if (rows.Count == 0)
                {
                    // a busca responde sem linhas quando a chave não existe
                    _logger.AddMessage($"B2CConsultaPedidos {id_pedido} não encontrado no Linx Microvix (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    await SaveB2CConsultaPedidos(doc_company, _linxMicrovixService.GetBatchTime(), rows);

                    _logger.AddMessage($"Concluída com sucesso: B2CConsultaPedidos {id_pedido} inserido ou atualizado ({rows.Count} linha(s))!");
                    result = true;
                }

                _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaPedidosByIdPedido, user_name, "POST", route, doc_company, $"{id_pedido}", lastResponse, result == true, logBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<bool?> B2CConsultaPedidosItensByIdPedido(string user_name, string doc_company, string id_pedido)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPedidosItensByIdPedido);

            // rota registrada no log de requisições; o corpo enviado vai sem a chave
            const string route = "B2CConsultaPedidosItens";
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
                var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosItens dto, string xml)>();
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
                                    new XElement("Name", "B2CConsultaPedidosItens"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
                                        new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                        new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                        new XElement("Parameter", new XAttribute("id", "id_pedido"), id_pedido)
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
                            _logger.AddMessage($"Erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {response?.StatusCode} {body}");

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
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {message}");

                            failed = true;
                            break;
                        }

                        var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                        var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                        // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                        if (columns.Count == 0 && pageRows.Count > 0)
                        {
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {pageRows[0].Value}");

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
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosItens
                                {
                                id_pedido_item = row.GetValueOrDefault("id_pedido_item"),
                                id_pedido = row.GetValueOrDefault("id_pedido"),
                                codigoproduto = row.GetValueOrDefault("codigoproduto"),
                                quantidade = row.GetValueOrDefault("quantidade"),
                                vl_unitario = row.GetValueOrDefault("vl_unitario"),
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
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaPedidosItensByIdPedido, user_name, "POST", route, doc_company, $"{id_pedido}", $"Exceção ao chamar a API: {ex.Message}", false, logBody);
                    throw;
                }

                if (failed)
                    result = null;
                else if (rows.Count == 0)
                {
                    // a busca responde sem linhas quando a chave não existe
                    _logger.AddMessage($"B2CConsultaPedidosItens {id_pedido} não encontrado no Linx Microvix (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    await SaveB2CConsultaPedidosItens(doc_company, _linxMicrovixService.GetBatchTime(), rows);

                    _logger.AddMessage($"Concluída com sucesso: B2CConsultaPedidosItens {id_pedido} inserido ou atualizado ({rows.Count} linha(s))!");
                    result = true;
                }

                _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaPedidosItensByIdPedido, user_name, "POST", route, doc_company, $"{id_pedido}", lastResponse, result == true, logBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<bool?> B2CConsultaPedidosStatusByIdPedido(string user_name, string doc_company, string id_pedido)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPedidosStatusByIdPedido);

            // rota registrada no log de requisições; o corpo enviado vai sem a chave
            const string route = "B2CConsultaPedidosStatus";
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

                var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

                // timestamp 0: a busca pela chave traz o registro inteiro, em qualquer versão
                long timestamp = 0;
                var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosStatus dto, string xml)>();
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
                                    new XElement("Name", "B2CConsultaPedidosStatus"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
                                        new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                        new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                        new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                        new XElement("Parameter", new XAttribute("id", "data_fim"), endDate),
                                        new XElement("Parameter", new XAttribute("id", "id_pedido"), id_pedido)
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
                            _logger.AddMessage($"Erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {response?.StatusCode} {body}");

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
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {message}");

                            failed = true;
                            break;
                        }

                        var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                        var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                        // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                        if (columns.Count == 0 && pageRows.Count > 0)
                        {
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {pageRows[0].Value}");

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
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosStatus
                                {
                                id = row.GetValueOrDefault("id"),
                                id_status = row.GetValueOrDefault("id_status"),
                                id_pedido = row.GetValueOrDefault("id_pedido"),
                                data_hora = row.GetValueOrDefault("data_hora"),
                                anotacao = row.GetValueOrDefault("anotacao"),
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
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaPedidosStatusByIdPedido, user_name, "POST", route, doc_company, $"{id_pedido}", $"Exceção ao chamar a API: {ex.Message}", false, logBody);
                    throw;
                }

                if (failed)
                    result = null;
                else if (rows.Count == 0)
                {
                    // a busca responde sem linhas quando a chave não existe
                    _logger.AddMessage($"B2CConsultaPedidosStatus {id_pedido} não encontrado no Linx Microvix (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    await SaveB2CConsultaPedidosStatus(doc_company, _linxMicrovixService.GetBatchTime(), rows);

                    _logger.AddMessage($"Concluída com sucesso: B2CConsultaPedidosStatus {id_pedido} inserido ou atualizado ({rows.Count} linha(s))!");
                    result = true;
                }

                _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaPedidosStatusByIdPedido, user_name, "POST", route, doc_company, $"{id_pedido}", lastResponse, result == true, logBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        public async Task<bool?> B2CConsultaNFeByIdPedido(string user_name, string doc_company, string cnpj_emp, string id_pedido)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaNFeByIdPedido);

            // rota registrada no log de requisições; o corpo enviado vai sem a chave
            const string route = "B2CConsultaNFe";
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

                // a loja precisa ser do cliente (gravada pelo B2CConsultaEmpresas): o CNPJ da rota não vira chamada para outra empresa
                if (!(await _linxMicrovixService.GetStores(company, EnumApi.B2C)).Contains(cnpj_emp))
                {
                    _logger.AddMessage($"Loja {cnpj_emp} não está entre as lojas do B2C da empresa {doc_company}.");
                    return null;
                }

                var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

                // timestamp 0: a busca pela chave traz o registro inteiro, em qualquer versão
                long timestamp = 0;
                var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFe dto, string xml)>();
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
                                    new XElement("Name", "B2CConsultaNFe"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
                                        new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                        new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                        new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                        new XElement("Parameter", new XAttribute("id", "data_fim"), endDate),
                                        new XElement("Parameter", new XAttribute("id", "id_pedido"), id_pedido)
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
                            _logger.AddMessage($"Erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {response?.StatusCode} {body}");

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
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {message}");

                            failed = true;
                            break;
                        }

                        var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                        var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                        // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                        if (columns.Count == 0 && pageRows.Count > 0)
                        {
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {id_pedido}): {pageRows[0].Value}");

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
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFe
                                {
                                id_nfe = row.GetValueOrDefault("id_nfe"),
                                id_pedido = row.GetValueOrDefault("id_pedido"),
                                documento = row.GetValueOrDefault("documento"),
                                data_emissao = row.GetValueOrDefault("data_emissao"),
                                chave_nfe = row.GetValueOrDefault("chave_nfe"),
                                situacao = row.GetValueOrDefault("situacao"),
                                xml = row.GetValueOrDefault("xml"),
                                excluido = row.GetValueOrDefault("excluido"),
                                identificador_microvix = row.GetValueOrDefault("identificador_microvix"),
                                dt_insert = row.GetValueOrDefault("dt_insert"),
                                valor_nota = row.GetValueOrDefault("valor_nota"),
                                serie = row.GetValueOrDefault("serie"),
                                frete = row.GetValueOrDefault("frete"),
                                timestamp = row.GetValueOrDefault("timestamp"),
                                portal = row.GetValueOrDefault("portal"),
                                nProt = row.GetValueOrDefault("nProt"),
                                codigo_modelo_nf = row.GetValueOrDefault("codigo_modelo_nf"),
                                justificativa = row.GetValueOrDefault("justificativa"),
                                tpAmb = row.GetValueOrDefault("tpAmb")
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
                    _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaNFeByIdPedido, user_name, "POST", route, doc_company, $"{cnpj_emp}|{id_pedido}", $"Exceção ao chamar a API: {ex.Message}", false, logBody);
                    throw;
                }

                if (failed)
                    result = null;
                else if (rows.Count == 0)
                {
                    // a busca responde sem linhas quando a chave não existe
                    _logger.AddMessage($"B2CConsultaNFe {id_pedido} não encontrado no Linx Microvix (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    await SaveB2CConsultaNFe(doc_company, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);

                    _logger.AddMessage($"Concluída com sucesso: B2CConsultaNFe {id_pedido} inserido ou atualizado ({rows.Count} linha(s))!");
                    result = true;
                }

                _linxMicrovixService.SaveRequest(EnumJob.B2CConsultaNFeByIdPedido, user_name, "POST", route, doc_company, $"{cnpj_emp}|{id_pedido}", lastResponse, result == true, logBody);

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
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_pedido; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaPedidos(string doc_company, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidos dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidos>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_pedido)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaPedidosValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaPedidos {dto.portal} {dto.id_pedido}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_pedido))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidos(doc_company, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.portal}|{entity.id_pedido}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaPedidos(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_pedido_item; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaPedidosItens(string doc_company, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosItens dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosItens>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_pedido_item)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaPedidosItensValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaPedidosItens {dto.portal} {dto.id_pedido_item}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_pedido_item))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosItens(doc_company, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.portal}|{entity.id_pedido_item}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaPedidosItens(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaPedidosStatus(string doc_company, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaPedidosStatus dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosStatus>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaPedidosStatusValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaPedidosStatus {dto.portal} {dto.id}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaPedidosStatus(doc_company, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.portal}|{entity.id}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaPedidosStatus(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_nfe; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveB2CConsultaNFe(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders.B2CConsultaNFe dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaNFe>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_nfe).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _b2CConsultaNFeValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"B2CConsultaNFe {dto.id_nfe} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.id_nfe))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders.B2CConsultaNFe(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.id_nfe}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixB2COrdersRepository.BulkInsertIntoB2CConsultaNFe(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }
    }
}

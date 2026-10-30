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
    /// Pedidos de venda do Web Service de Saída Padrão (ERP). Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixOrdersService : ILinxMicrovixOrdersService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixOrdersRepository _linxMicrovixOrdersRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVenda> _linxPedidosVendaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPlanosPedidoVenda> _linxPlanosPedidoVendaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaLocal> _linxPedidosVendaChecklistEntregaLocalValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaArmazenamento> _linxPedidosVendaChecklistEntregaArmazenamentoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaDificuldade> _linxPedidosVendaChecklistEntregaDificuldadeValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxOrcamentoPosicao> _linxOrcamentoPosicaoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidos> _linxB2CPedidosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidosItens> _linxB2CPedidosItensValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidosStatus> _linxB2CPedidosStatusValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CStatus> _linxB2CStatusValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixOrdersService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixOrdersRepository linxMicrovixOrdersRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVenda> linxPedidosVendaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPlanosPedidoVenda> linxPlanosPedidoVendaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaLocal> linxPedidosVendaChecklistEntregaLocalValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaArmazenamento> linxPedidosVendaChecklistEntregaArmazenamentoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaDificuldade> linxPedidosVendaChecklistEntregaDificuldadeValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxOrcamentoPosicao> linxOrcamentoPosicaoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidos> linxB2CPedidosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidosItens> linxB2CPedidosItensValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidosStatus> linxB2CPedidosStatusValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CStatus> linxB2CStatusValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixOrdersRepository, _linxPedidosVendaValidator, _linxPlanosPedidoVendaValidator, _linxPedidosVendaChecklistEntregaLocalValidator, _linxPedidosVendaChecklistEntregaArmazenamentoValidator, _linxPedidosVendaChecklistEntregaDificuldadeValidator, _linxOrcamentoPosicaoValidator, _linxB2CPedidosValidator, _linxB2CPedidosItensValidator, _linxB2CPedidosStatusValidator, _linxB2CStatusValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixOrdersRepository, linxPedidosVendaValidator, linxPlanosPedidoVendaValidator, linxPedidosVendaChecklistEntregaLocalValidator, linxPedidosVendaChecklistEntregaArmazenamentoValidator, linxPedidosVendaChecklistEntregaDificuldadeValidator, linxOrcamentoPosicaoValidator, linxB2CPedidosValidator, linxB2CPedidosItensValidator, linxB2CPedidosStatusValidator, linxB2CStatusValidator);

        public async Task<bool?> LinxPedidosVenda()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxPedidosVenda);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    // datas obrigatórias: intervalo largo (a leitura é pelo timestamp, em páginas) e filtra_somente_timestamp = 1, que faz a API ignorar as datas (testado)
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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxPedidosVenda, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVenda dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxPedidosVenda"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                            new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "data_fim"), endDate),
                                            new XElement("Parameter", new XAttribute("id", "data_alt_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "data_alt_fim"), endDate),
                                            new XElement("Parameter", new XAttribute("id", "filtra_somente_timestamp"), "1")
                                        )
                                    )
                                )
                            );

                            // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                            ApiResponse? response = null;

                            for (var attempt = 1; attempt <= 2; attempt++)
                            {
                                using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxPedidosVenda", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxPedidosVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPedidosVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPedidosVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVenda
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                    cod_pedido = row.GetValueOrDefault("cod_pedido"),
                                    data_lancamento = row.GetValueOrDefault("data_lancamento"),
                                    hora_lancamento = row.GetValueOrDefault("hora_lancamento"),
                                    transacao = row.GetValueOrDefault("transacao"),
                                    usuario = row.GetValueOrDefault("usuario"),
                                    codigo_cliente = row.GetValueOrDefault("codigo_cliente"),
                                    cod_produto = row.GetValueOrDefault("cod_produto"),
                                    quantidade = row.GetValueOrDefault("quantidade"),
                                    valor_unitario = row.GetValueOrDefault("valor_unitario"),
                                    cod_vendedor = row.GetValueOrDefault("cod_vendedor"),
                                    valor_frete = row.GetValueOrDefault("valor_frete"),
                                    valor_total = row.GetValueOrDefault("valor_total"),
                                    desconto_item = row.GetValueOrDefault("desconto_item"),
                                    cod_plano_pagamento = row.GetValueOrDefault("cod_plano_pagamento"),
                                    plano_pagamento = row.GetValueOrDefault("plano_pagamento"),
                                    obs = row.GetValueOrDefault("obs"),
                                    aprovado = row.GetValueOrDefault("aprovado"),
                                    cancelado = row.GetValueOrDefault("cancelado"),
                                    data_aprovacao = row.GetValueOrDefault("data_aprovacao"),
                                    data_alteracao = row.GetValueOrDefault("data_alteracao"),
                                    tipo_frete = row.GetValueOrDefault("tipo_frete"),
                                    natureza_operacao = row.GetValueOrDefault("natureza_operacao"),
                                    tabela_preco = row.GetValueOrDefault("tabela_preco"),
                                    nome_tabela_preco = row.GetValueOrDefault("nome_tabela_preco"),
                                    previsao_entrega = row.GetValueOrDefault("previsao_entrega"),
                                    realizado_por = row.GetValueOrDefault("realizado_por"),
                                    pontuacao_ser = row.GetValueOrDefault("pontuacao_ser"),
                                    venda_externa = row.GetValueOrDefault("venda_externa"),
                                    nf_gerada = row.GetValueOrDefault("nf_gerada"),
                                    status = row.GetValueOrDefault("status"),
                                    numero_projeto_officina = row.GetValueOrDefault("numero_projeto_officina"),
                                    cod_natureza_operacao = row.GetValueOrDefault("cod_natureza_operacao"),
                                    margem_contribuicao = row.GetValueOrDefault("margem_contribuicao"),
                                    doc_origem = row.GetValueOrDefault("doc_origem"),
                                    posicao_item = row.GetValueOrDefault("posicao_item"),
                                    orcamento_origem = row.GetValueOrDefault("orcamento_origem"),
                                    transacao_origem = row.GetValueOrDefault("transacao_origem"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    desconto = row.GetValueOrDefault("desconto"),
                                    transacao_ws = row.GetValueOrDefault("transacao_ws"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    transportador = row.GetValueOrDefault("transportador"),
                                    deposito = row.GetValueOrDefault("deposito"),
                                    texto_agrupamento = row.GetValueOrDefault("texto_agrupamento"),
                                    posicao = row.GetValueOrDefault("posicao"),
                                    encerrado = row.GetValueOrDefault("encerrado")
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

                        totalInseridos += await SaveLinxPedidosVenda(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxPlanosPedidoVenda()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxPlanosPedidoVenda);

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
                        // o método exige um cod_pedido por chamada: os valores vêm do LinxPedidosVenda já gravado (só os gravados depois da última leitura deste método)
                        var sourceValues = await _linxMicrovixService.GetSourceValues(company, EnumJob.LinxPedidosVenda, "cod_pedido", cnpj_emp, EnumJob.LinxPlanosPedidoVenda);

                        foreach (var cod_pedido in sourceValues)
                        {
                            // timestamp exclusivo, por cod_pedido: um valor novo começa do 0 sem pular os outros
                            var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxPlanosPedidoVenda, cnpj_emp, "cod_pedido", cod_pedido);
                            var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPlanosPedidoVenda dto, string xml)>();
                            var failed = false;

                            for (var page = 1; page <= MaxPages; page++)
                            {
                                var requestBody = new XDocument(
                                    new XDeclaration("1.0", "utf-8", null),
                                    new XElement("LinxMicrovix",
                                        new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                        new XElement("ResponseFormat", "xml"),
                                        new XElement("Command",
                                            new XElement("Name", "LinxPlanosPedidoVenda"),
                                            new XElement("Parameters",
                                                new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                                new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                                new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                                new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                                new XElement("Parameter", new XAttribute("id", "data_fim"), endDate),
                                                new XElement("Parameter", new XAttribute("id", "dt_update_inicial"), "2000-01-01"),
                                                new XElement("Parameter", new XAttribute("id", "dt_update_fim"), endDate),
                                                new XElement("Parameter", new XAttribute("id", "cod_pedido"), cod_pedido)
                                            )
                                        )
                                    )
                                );

                                // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                                ApiResponse? response = null;

                                for (var attempt = 1; attempt <= 2; attempt++)
                                {
                                    using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                    response = await _apiClient.SendAsync(HttpMethod.Post, "LinxPlanosPedidoVenda", content);

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
                                    _logger.AddMessage($"Erro da API Linx Microvix em LinxPlanosPedidoVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}, cod_pedido {cod_pedido}): {response?.StatusCode} {body}");

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
                                    _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPlanosPedidoVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}, cod_pedido {cod_pedido}): {message}");

                                    failed = true;
                                    break;
                                }

                                var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                                var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                                // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                                if (columns.Count == 0 && pageRows.Count > 0)
                                {
                                    _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPlanosPedidoVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}, cod_pedido {cod_pedido}): {pageRows[0].Value}");

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
                                        new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPlanosPedidoVenda
                                        {
                                        portal = row.GetValueOrDefault("portal"),
                                        cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                        cod_pedido = row.GetValueOrDefault("cod_pedido"),
                                        plano = row.GetValueOrDefault("plano"),
                                        desc_plano = row.GetValueOrDefault("desc_plano"),
                                        total = row.GetValueOrDefault("total"),
                                        qtde_parcelas = row.GetValueOrDefault("qtde_parcelas"),
                                        indice_plano = row.GetValueOrDefault("indice_plano"),
                                        valor_desc_acresc_plano = row.GetValueOrDefault("valor_desc_acresc_plano"),
                                        cod_forma_pgto = row.GetValueOrDefault("cod_forma_pgto"),
                                        forma_pgto = row.GetValueOrDefault("forma_pgto"),
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

                            // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + cod_pedido + plano; fica a linha de maior timestamp
                            var batchTime = _linxMicrovixService.GetBatchTime();

                            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPlanosPedidoVenda>();
                            var records = new Dictionary<string, string>();

                            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.cod_pedido, x.dto.plano)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                            {
                                var validations = _linxPlanosPedidoVendaValidator.Validate(dto);

                                if (validations.Errors.Count > 0)
                                {
                                    var message = String.Empty;
                                    validations.Errors.ForEach(s => message += $"LinxPlanosPedidoVenda {dto.cod_pedido} {dto.plano} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                    _logger.AddMessage(message);
                                }

                                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                                if (String.IsNullOrEmpty(dto.cod_pedido) && String.IsNullOrEmpty(dto.plano))
                                    continue;

                                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPlanosPedidoVenda(company.doc_company!, cnpj_emp, batchTime, dto);

                                _listSomenteNovos.Add(entity);
                                records[$"{entity.cnpj_emp}|{entity.cod_pedido}|{entity.plano}"] = xml;
                            }

                            if (_listSomenteNovos.Count == 0)
                                continue;

                            await _linxMicrovixOrdersRepository.BulkInsertIntoLinxPlanosPedidoVenda(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxPedidosVendaChecklistEntregaLocal()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxPedidosVendaChecklistEntregaLocal);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxPedidosVendaChecklistEntregaLocal, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaLocal dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxPedidosVendaChecklistEntregaLocal"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxPedidosVendaChecklistEntregaLocal", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxPedidosVendaChecklistEntregaLocal (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPedidosVendaChecklistEntregaLocal (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPedidosVendaChecklistEntregaLocal (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaLocal
                                    {
                                    id_checklist_entrega_local = row.GetValueOrDefault("id_checklist_entrega_local"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_checklist_entrega_local; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaLocal>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_checklist_entrega_local).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxPedidosVendaChecklistEntregaLocalValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxPedidosVendaChecklistEntregaLocal {dto.id_checklist_entrega_local} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_checklist_entrega_local))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaLocal(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_checklist_entrega_local}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixOrdersRepository.BulkInsertIntoLinxPedidosVendaChecklistEntregaLocal(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxPedidosVendaChecklistEntregaArmazenamento()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxPedidosVendaChecklistEntregaArmazenamento);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxPedidosVendaChecklistEntregaArmazenamento, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaArmazenamento dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxPedidosVendaChecklistEntregaArmazenamento"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxPedidosVendaChecklistEntregaArmazenamento", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxPedidosVendaChecklistEntregaArmazenamento (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPedidosVendaChecklistEntregaArmazenamento (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPedidosVendaChecklistEntregaArmazenamento (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaArmazenamento
                                    {
                                    id_checklist_entrega_armazenamento = row.GetValueOrDefault("id_checklist_entrega_armazenamento"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_checklist_entrega_armazenamento; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaArmazenamento>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_checklist_entrega_armazenamento).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxPedidosVendaChecklistEntregaArmazenamentoValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxPedidosVendaChecklistEntregaArmazenamento {dto.id_checklist_entrega_armazenamento} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_checklist_entrega_armazenamento))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaArmazenamento(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_checklist_entrega_armazenamento}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixOrdersRepository.BulkInsertIntoLinxPedidosVendaChecklistEntregaArmazenamento(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxPedidosVendaChecklistEntregaDificuldade()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxPedidosVendaChecklistEntregaDificuldade);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxPedidosVendaChecklistEntregaDificuldade, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaDificuldade dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxPedidosVendaChecklistEntregaDificuldade"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxPedidosVendaChecklistEntregaDificuldade", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxPedidosVendaChecklistEntregaDificuldade (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPedidosVendaChecklistEntregaDificuldade (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPedidosVendaChecklistEntregaDificuldade (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaDificuldade
                                    {
                                    id_checklist_entrega_dificuldade = row.GetValueOrDefault("id_checklist_entrega_dificuldade"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_checklist_entrega_dificuldade; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaDificuldade>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_checklist_entrega_dificuldade).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxPedidosVendaChecklistEntregaDificuldadeValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxPedidosVendaChecklistEntregaDificuldade {dto.id_checklist_entrega_dificuldade} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_checklist_entrega_dificuldade))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaDificuldade(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_checklist_entrega_dificuldade}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixOrdersRepository.BulkInsertIntoLinxPedidosVendaChecklistEntregaDificuldade(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxOrcamentoPosicao()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxOrcamentoPosicao);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxOrcamentoPosicao, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxOrcamentoPosicao dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxOrcamentoPosicao"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxOrcamentoPosicao", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxOrcamentoPosicao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxOrcamentoPosicao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxOrcamentoPosicao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxOrcamentoPosicao
                                    {
                                    id_posicao = row.GetValueOrDefault("id_posicao"),
                                    descricao_posicao = row.GetValueOrDefault("descricao_posicao"),
                                    ativo_posicao = row.GetValueOrDefault("ativo_posicao"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_posicao; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxOrcamentoPosicao>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_posicao)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxOrcamentoPosicaoValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxOrcamentoPosicao {dto.portal} {dto.id_posicao}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_posicao))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxOrcamentoPosicao(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_posicao}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixOrdersRepository.BulkInsertIntoLinxOrcamentoPosicao(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxB2CPedidos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxB2CPedidos);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxB2CPedidos, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidos dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxB2CPedidos"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxB2CPedidos", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxB2CPedidos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxB2CPedidos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxB2CPedidos (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidos
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
                                    marketplace_loja = row.GetValueOrDefault("marketplace_loja"),
                                    cnpj_origem_venda = row.GetValueOrDefault("cnpj_origem_venda"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_pedido; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxB2CPedidos>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_pedido).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxB2CPedidosValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxB2CPedidos {dto.id_pedido} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_pedido))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxB2CPedidos(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_pedido}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixOrdersRepository.BulkInsertIntoLinxB2CPedidos(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxB2CPedidosItens()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxB2CPedidosItens);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxB2CPedidosItens, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidosItens dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxB2CPedidosItens"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxB2CPedidosItens", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxB2CPedidosItens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxB2CPedidosItens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxB2CPedidosItens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidosItens
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_pedido_item; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxB2CPedidosItens>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_pedido_item)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxB2CPedidosItensValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxB2CPedidosItens {dto.portal} {dto.id_pedido_item}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_pedido_item))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxB2CPedidosItens(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_pedido_item}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixOrdersRepository.BulkInsertIntoLinxB2CPedidosItens(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxB2CPedidosStatus()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxB2CPedidosStatus);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxB2CPedidosStatus, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidosStatus dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxB2CPedidosStatus"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxB2CPedidosStatus", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxB2CPedidosStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxB2CPedidosStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxB2CPedidosStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CPedidosStatus
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxB2CPedidosStatus>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxB2CPedidosStatusValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxB2CPedidosStatus {dto.portal} {dto.id}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxB2CPedidosStatus(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixOrdersRepository.BulkInsertIntoLinxB2CPedidosStatus(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxB2CStatus()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxB2CStatus);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxB2CStatus, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CStatus dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxB2CStatus"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxB2CStatus", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxB2CStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxB2CStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxB2CStatus (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxB2CStatus
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

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxB2CStatus>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_status)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxB2CStatusValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxB2CStatus {dto.portal} {dto.id_status}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_status))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxB2CStatus(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_status}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixOrdersRepository.BulkInsertIntoLinxB2CStatus(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxPedidosVendaByCodPedido(string user_name, string doc_company, string cnpj_emp, string cod_pedido)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxPedidosVendaByCodPedido);

            // rota registrada no log de requisições; o corpo enviado vai sem a chave
            const string route = "LinxPedidosVenda";
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

                // a loja precisa ser do cliente (gravada pelo LinxGrupoLojas): o CNPJ da rota não vira chamada para outra empresa
                if (!(await _linxMicrovixService.GetStores(company, EnumApi.Erp)).Contains(cnpj_emp))
                {
                    _logger.AddMessage($"Loja {cnpj_emp} não está entre as lojas do ERP da empresa {doc_company}.");
                    return null;
                }

                var endDate = $"{_linxMicrovixService.GetBatchTime():yyyy-MM-dd}";

                // timestamp 0: a busca pela chave traz o registro inteiro, em qualquer versão
                long timestamp = 0;
                var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVenda dto, string xml)>();
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
                                    new XElement("Name", "LinxPedidosVenda"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                        new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                        new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                        new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                        new XElement("Parameter", new XAttribute("id", "data_fim"), endDate),
                                        new XElement("Parameter", new XAttribute("id", "data_alt_inicial"), "2000-01-01"),
                                        new XElement("Parameter", new XAttribute("id", "data_alt_fim"), endDate),
                                        new XElement("Parameter", new XAttribute("id", "filtra_somente_timestamp"), "0"   /* com 1 a API ignora também o filtro da busca (testado no LinxMovimento); as datas cobrem o histórico */),
                                        new XElement("Parameter", new XAttribute("id", "doc_origem"), cod_pedido)
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
                            _logger.AddMessage($"Erro da API Linx Microvix em {route} (empresa {doc_company}, {cod_pedido}): {response?.StatusCode} {body}");

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
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {cod_pedido}): {message}");

                            failed = true;
                            break;
                        }

                        var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                        var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                        // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                        if (columns.Count == 0 && pageRows.Count > 0)
                        {
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {cod_pedido}): {pageRows[0].Value}");

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
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVenda
                                {
                                portal = row.GetValueOrDefault("portal"),
                                cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                cod_pedido = row.GetValueOrDefault("cod_pedido"),
                                data_lancamento = row.GetValueOrDefault("data_lancamento"),
                                hora_lancamento = row.GetValueOrDefault("hora_lancamento"),
                                transacao = row.GetValueOrDefault("transacao"),
                                usuario = row.GetValueOrDefault("usuario"),
                                codigo_cliente = row.GetValueOrDefault("codigo_cliente"),
                                cod_produto = row.GetValueOrDefault("cod_produto"),
                                quantidade = row.GetValueOrDefault("quantidade"),
                                valor_unitario = row.GetValueOrDefault("valor_unitario"),
                                cod_vendedor = row.GetValueOrDefault("cod_vendedor"),
                                valor_frete = row.GetValueOrDefault("valor_frete"),
                                valor_total = row.GetValueOrDefault("valor_total"),
                                desconto_item = row.GetValueOrDefault("desconto_item"),
                                cod_plano_pagamento = row.GetValueOrDefault("cod_plano_pagamento"),
                                plano_pagamento = row.GetValueOrDefault("plano_pagamento"),
                                obs = row.GetValueOrDefault("obs"),
                                aprovado = row.GetValueOrDefault("aprovado"),
                                cancelado = row.GetValueOrDefault("cancelado"),
                                data_aprovacao = row.GetValueOrDefault("data_aprovacao"),
                                data_alteracao = row.GetValueOrDefault("data_alteracao"),
                                tipo_frete = row.GetValueOrDefault("tipo_frete"),
                                natureza_operacao = row.GetValueOrDefault("natureza_operacao"),
                                tabela_preco = row.GetValueOrDefault("tabela_preco"),
                                nome_tabela_preco = row.GetValueOrDefault("nome_tabela_preco"),
                                previsao_entrega = row.GetValueOrDefault("previsao_entrega"),
                                realizado_por = row.GetValueOrDefault("realizado_por"),
                                pontuacao_ser = row.GetValueOrDefault("pontuacao_ser"),
                                venda_externa = row.GetValueOrDefault("venda_externa"),
                                nf_gerada = row.GetValueOrDefault("nf_gerada"),
                                status = row.GetValueOrDefault("status"),
                                numero_projeto_officina = row.GetValueOrDefault("numero_projeto_officina"),
                                cod_natureza_operacao = row.GetValueOrDefault("cod_natureza_operacao"),
                                margem_contribuicao = row.GetValueOrDefault("margem_contribuicao"),
                                doc_origem = row.GetValueOrDefault("doc_origem"),
                                posicao_item = row.GetValueOrDefault("posicao_item"),
                                orcamento_origem = row.GetValueOrDefault("orcamento_origem"),
                                transacao_origem = row.GetValueOrDefault("transacao_origem"),
                                timestamp = row.GetValueOrDefault("timestamp"),
                                desconto = row.GetValueOrDefault("desconto"),
                                transacao_ws = row.GetValueOrDefault("transacao_ws"),
                                empresa = row.GetValueOrDefault("empresa"),
                                transportador = row.GetValueOrDefault("transportador"),
                                deposito = row.GetValueOrDefault("deposito"),
                                texto_agrupamento = row.GetValueOrDefault("texto_agrupamento"),
                                posicao = row.GetValueOrDefault("posicao"),
                                encerrado = row.GetValueOrDefault("encerrado")
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
                    _linxMicrovixService.SaveRequest(EnumJob.LinxPedidosVendaByCodPedido, user_name, "POST", route, doc_company, $"{cnpj_emp}|{cod_pedido}", $"Exceção ao chamar a API: {ex.Message}", false, logBody);
                    throw;
                }

                if (failed)
                    result = null;
                else if (rows.Count == 0)
                {
                    // a busca responde sem linhas quando a chave não existe
                    _logger.AddMessage($"LinxPedidosVenda {cod_pedido} não encontrado no Linx Microvix (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    await SaveLinxPedidosVenda(doc_company, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);

                    _logger.AddMessage($"Concluída com sucesso: LinxPedidosVenda {cod_pedido} inserido ou atualizado ({rows.Count} linha(s))!");
                    result = true;
                }

                _linxMicrovixService.SaveRequest(EnumJob.LinxPedidosVendaByCodPedido, user_name, "POST", route, doc_company, $"{cnpj_emp}|{cod_pedido}", lastResponse, result == true, logBody);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + transacao; fica a linha de maior timestamp
        /// </summary>
        private async Task<int> SaveLinxPedidosVenda(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders.LinxPedidosVenda dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPedidosVenda>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.transacao).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxPedidosVendaValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxPedidosVenda {dto.transacao} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.transacao))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders.LinxPedidosVenda(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.transacao}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixOrdersRepository.BulkInsertIntoLinxPedidosVenda(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }
    }
}

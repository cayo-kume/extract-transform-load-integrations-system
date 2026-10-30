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
    /// Movimento do Web Service de Saída Padrão (ERP). Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixMovementsService : ILinxMicrovixMovementsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixMovementsRepository _linxMicrovixMovementsRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimento> _linxMovimentoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoPrincipal> _linxMovimentoPrincipalValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoExtensao> _linxMovimentoExtensaoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoCamposAdicionais> _linxMovimentoCamposAdicionaisValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoObservacoes> _linxMovimentoObservacoesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoObservacaoCF> _linxMovimentoObservacaoCFValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoIndicacoes> _linxMovimentoIndicacoesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoSerial> _linxMovimentoSerialValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxSerialVenda> _linxSerialVendaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoVendaConjugada> _linxMovimentoVendaConjugadaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxPerguntaVenda> _linxPerguntaVendaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxRespostaVenda> _linxRespostaVendaValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixMovementsService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixMovementsRepository linxMicrovixMovementsRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimento> linxMovimentoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoPrincipal> linxMovimentoPrincipalValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoExtensao> linxMovimentoExtensaoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoCamposAdicionais> linxMovimentoCamposAdicionaisValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoObservacoes> linxMovimentoObservacoesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoObservacaoCF> linxMovimentoObservacaoCFValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoIndicacoes> linxMovimentoIndicacoesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoSerial> linxMovimentoSerialValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxSerialVenda> linxSerialVendaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoVendaConjugada> linxMovimentoVendaConjugadaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxPerguntaVenda> linxPerguntaVendaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxRespostaVenda> linxRespostaVendaValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixMovementsRepository, _linxMovimentoValidator, _linxMovimentoPrincipalValidator, _linxMovimentoExtensaoValidator, _linxMovimentoCamposAdicionaisValidator, _linxMovimentoObservacoesValidator, _linxMovimentoObservacaoCFValidator, _linxMovimentoIndicacoesValidator, _linxMovimentoSerialValidator, _linxSerialVendaValidator, _linxMovimentoVendaConjugadaValidator, _linxPerguntaVendaValidator, _linxRespostaVendaValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixMovementsRepository, linxMovimentoValidator, linxMovimentoPrincipalValidator, linxMovimentoExtensaoValidator, linxMovimentoCamposAdicionaisValidator, linxMovimentoObservacoesValidator, linxMovimentoObservacaoCFValidator, linxMovimentoIndicacoesValidator, linxMovimentoSerialValidator, linxSerialVendaValidator, linxMovimentoVendaConjugadaValidator, linxPerguntaVendaValidator, linxRespostaVendaValidator);

        public async Task<bool?> LinxMovimento()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimento);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimento, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimento dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimento"),
                                        new XElement("Parameters",
                                            new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                            new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                            new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                            new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "data_fim"), endDate),
                                            new XElement("Parameter", new XAttribute("id", "dt_update_inicial"), "2000-01-01"),
                                            new XElement("Parameter", new XAttribute("id", "dt_update_fim"), endDate),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimento", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimento (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimento (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimento (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimento
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                    transacao = row.GetValueOrDefault("transacao"),
                                    usuario = row.GetValueOrDefault("usuario"),
                                    documento = row.GetValueOrDefault("documento"),
                                    chave_nf = row.GetValueOrDefault("chave_nf"),
                                    ecf = row.GetValueOrDefault("ecf"),
                                    numero_serie_ecf = row.GetValueOrDefault("numero_serie_ecf"),
                                    modelo_nf = row.GetValueOrDefault("modelo_nf"),
                                    data_documento = row.GetValueOrDefault("data_documento"),
                                    data_lancamento = row.GetValueOrDefault("data_lancamento"),
                                    codigo_cliente = row.GetValueOrDefault("codigo_cliente"),
                                    serie = row.GetValueOrDefault("serie"),
                                    desc_cfop = row.GetValueOrDefault("desc_cfop"),
                                    id_cfop = row.GetValueOrDefault("id_cfop"),
                                    cod_vendedor = row.GetValueOrDefault("cod_vendedor"),
                                    quantidade = row.GetValueOrDefault("quantidade"),
                                    preco_custo = row.GetValueOrDefault("preco_custo"),
                                    valor_liquido = row.GetValueOrDefault("valor_liquido"),
                                    desconto = row.GetValueOrDefault("desconto"),
                                    cst_icms = row.GetValueOrDefault("cst_icms"),
                                    cst_pis = row.GetValueOrDefault("cst_pis"),
                                    cst_cofins = row.GetValueOrDefault("cst_cofins"),
                                    cst_ipi = row.GetValueOrDefault("cst_ipi"),
                                    valor_icms = row.GetValueOrDefault("valor_icms"),
                                    aliquota_icms = row.GetValueOrDefault("aliquota_icms"),
                                    base_icms = row.GetValueOrDefault("base_icms"),
                                    valor_pis = row.GetValueOrDefault("valor_pis"),
                                    aliquota_pis = row.GetValueOrDefault("aliquota_pis"),
                                    base_pis = row.GetValueOrDefault("base_pis"),
                                    valor_cofins = row.GetValueOrDefault("valor_cofins"),
                                    aliquota_cofins = row.GetValueOrDefault("aliquota_cofins"),
                                    base_cofins = row.GetValueOrDefault("base_cofins"),
                                    valor_icms_st = row.GetValueOrDefault("valor_icms_st"),
                                    aliquota_icms_st = row.GetValueOrDefault("aliquota_icms_st"),
                                    base_icms_st = row.GetValueOrDefault("base_icms_st"),
                                    valor_ipi = row.GetValueOrDefault("valor_ipi"),
                                    aliquota_ipi = row.GetValueOrDefault("aliquota_ipi"),
                                    base_ipi = row.GetValueOrDefault("base_ipi"),
                                    valor_total = row.GetValueOrDefault("valor_total"),
                                    forma_dinheiro = row.GetValueOrDefault("forma_dinheiro"),
                                    total_dinheiro = row.GetValueOrDefault("total_dinheiro"),
                                    forma_cheque = row.GetValueOrDefault("forma_cheque"),
                                    total_cheque = row.GetValueOrDefault("total_cheque"),
                                    forma_cartao = row.GetValueOrDefault("forma_cartao"),
                                    total_cartao = row.GetValueOrDefault("total_cartao"),
                                    forma_crediario = row.GetValueOrDefault("forma_crediario"),
                                    total_crediario = row.GetValueOrDefault("total_crediario"),
                                    forma_convenio = row.GetValueOrDefault("forma_convenio"),
                                    total_convenio = row.GetValueOrDefault("total_convenio"),
                                    frete = row.GetValueOrDefault("frete"),
                                    operacao = row.GetValueOrDefault("operacao"),
                                    tipo_transacao = row.GetValueOrDefault("tipo_transacao"),
                                    cod_produto = row.GetValueOrDefault("cod_produto"),
                                    cod_barra = row.GetValueOrDefault("cod_barra"),
                                    cancelado = row.GetValueOrDefault("cancelado"),
                                    excluido = row.GetValueOrDefault("excluido"),
                                    soma_relatorio = row.GetValueOrDefault("soma_relatorio"),
                                    identificador = row.GetValueOrDefault("identificador"),
                                    deposito = row.GetValueOrDefault("deposito"),
                                    obs = row.GetValueOrDefault("obs"),
                                    preco_unitario = row.GetValueOrDefault("preco_unitario"),
                                    hora_lancamento = row.GetValueOrDefault("hora_lancamento"),
                                    natureza_operacao = row.GetValueOrDefault("natureza_operacao"),
                                    tabela_preco = row.GetValueOrDefault("tabela_preco"),
                                    nome_tabela_preco = row.GetValueOrDefault("nome_tabela_preco"),
                                    cod_sefaz_situacao = row.GetValueOrDefault("cod_sefaz_situacao"),
                                    desc_sefaz_situacao = row.GetValueOrDefault("desc_sefaz_situacao"),
                                    protocolo_aut_nfe = row.GetValueOrDefault("protocolo_aut_nfe"),
                                    dt_update = row.GetValueOrDefault("dt_update"),
                                    forma_cheque_prazo = row.GetValueOrDefault("forma_cheque_prazo"),
                                    total_cheque_prazo = row.GetValueOrDefault("total_cheque_prazo"),
                                    cod_natureza_operacao = row.GetValueOrDefault("cod_natureza_operacao"),
                                    preco_tabela_epoca = row.GetValueOrDefault("preco_tabela_epoca"),
                                    desconto_total_item = row.GetValueOrDefault("desconto_total_item"),
                                    conferido = row.GetValueOrDefault("conferido"),
                                    transacao_pedido_venda = row.GetValueOrDefault("transacao_pedido_venda"),
                                    codigo_modelo_nf = row.GetValueOrDefault("codigo_modelo_nf"),
                                    acrescimo = row.GetValueOrDefault("acrescimo"),
                                    mob_checkout = row.GetValueOrDefault("mob_checkout"),
                                    aliquota_iss = row.GetValueOrDefault("aliquota_iss"),
                                    base_iss = row.GetValueOrDefault("base_iss"),
                                    ordem = row.GetValueOrDefault("ordem"),
                                    codigo_rotina_origem = row.GetValueOrDefault("codigo_rotina_origem"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    troco = row.GetValueOrDefault("troco"),
                                    transportador = row.GetValueOrDefault("transportador"),
                                    icms_aliquota_desonerado = row.GetValueOrDefault("icms_aliquota_desonerado"),
                                    icms_valor_desonerado_item = row.GetValueOrDefault("icms_valor_desonerado_item"),
                                    empresa = row.GetValueOrDefault("empresa"),
                                    desconto_item = row.GetValueOrDefault("desconto_item"),
                                    aliq_iss = row.GetValueOrDefault("aliq_iss"),
                                    iss_base_item = row.GetValueOrDefault("iss_base_item"),
                                    despesas = row.GetValueOrDefault("despesas"),
                                    seguro_total_item = row.GetValueOrDefault("seguro_total_item"),
                                    acrescimo_total_item = row.GetValueOrDefault("acrescimo_total_item"),
                                    despesas_total_item = row.GetValueOrDefault("despesas_total_item"),
                                    forma_pix = row.GetValueOrDefault("forma_pix"),
                                    total_pix = row.GetValueOrDefault("total_pix"),
                                    forma_deposito_bancario = row.GetValueOrDefault("forma_deposito_bancario"),
                                    total_deposito_bancario = row.GetValueOrDefault("total_deposito_bancario"),
                                    id_venda_produto_b2c = row.GetValueOrDefault("id_venda_produto_b2c"),
                                    item_promocional = row.GetValueOrDefault("item_promocional"),
                                    acrescimo_item = row.GetValueOrDefault("acrescimo_item"),
                                    icms_st_antecipado_aliquota = row.GetValueOrDefault("icms_st_antecipado_aliquota"),
                                    icms_st_antecipado_margem = row.GetValueOrDefault("icms_st_antecipado_margem"),
                                    icms_st_antecipado_percentual_reducao = row.GetValueOrDefault("icms_st_antecipado_percentual_reducao"),
                                    icms_st_antecipado_valor_item = row.GetValueOrDefault("icms_st_antecipado_valor_item"),
                                    icms_base_desonerado_item = row.GetValueOrDefault("icms_base_desonerado_item"),
                                    codigo_status_nfe = row.GetValueOrDefault("codigo_status_nfe"),
                                    preco_produto_epoca = row.GetValueOrDefault("preco_produto_epoca"),
                                    ficha = row.GetValueOrDefault("ficha"),
                                    numero_sequencial_pos = row.GetValueOrDefault("numero_sequencial_pos"),
                                    dt_insert = row.GetValueOrDefault("dt_insert"),
                                    custo_medio_epoca = row.GetValueOrDefault("custo_medio_epoca"),
                                    conhecimento = row.GetValueOrDefault("conhecimento")
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

                        totalInseridos += await SaveLinxMovimento(company.doc_company!, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);
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

        public async Task<bool?> LinxMovimentoPrincipal()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoPrincipal);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimentoPrincipal, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoPrincipal dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimentoPrincipal"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimentoPrincipal", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimentoPrincipal (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoPrincipal (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoPrincipal (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoPrincipal
                                    {
                                    id_movimento_principal = row.GetValueOrDefault("id_movimento_principal"),
                                    identificador = row.GetValueOrDefault("identificador"),
                                    codigoproduto_manutencao = row.GetValueOrDefault("codigoproduto_manutencao"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    id_pergunta_venda = row.GetValueOrDefault("id_pergunta_venda"),
                                    id_resposta_venda = row.GetValueOrDefault("id_resposta_venda"),
                                    total_fidelidade_cashback = row.GetValueOrDefault("total_fidelidade_cashback"),
                                    plano_fidelidade_cashback = row.GetValueOrDefault("plano_fidelidade_cashback"),
                                    remessa_pedido_compra = row.GetValueOrDefault("remessa_pedido_compra"),
                                    id_motivo_desconto = row.GetValueOrDefault("id_motivo_desconto"),
                                    valor_nota = row.GetValueOrDefault("valor_nota"),
                                    portal = row.GetValueOrDefault("portal"),
                                    estornada = row.GetValueOrDefault("estornada"),
                                    motivo_desconto_observacao = row.GetValueOrDefault("motivo_desconto_observacao"),
                                    nota_aguardou_sefaz = row.GetValueOrDefault("nota_aguardou_sefaz")
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_movimento_principal; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoPrincipal>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_movimento_principal).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxMovimentoPrincipalValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxMovimentoPrincipal {dto.id_movimento_principal} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_movimento_principal))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoPrincipal(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_movimento_principal}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxMovimentoPrincipal(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxMovimentoExtensao()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoExtensao);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimentoExtensao, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoExtensao dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimentoExtensao"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimentoExtensao", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimentoExtensao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoExtensao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoExtensao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoExtensao
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                    transacao = row.GetValueOrDefault("transacao"),
                                    identificador = row.GetValueOrDefault("identificador"),
                                    base_fcp_st = row.GetValueOrDefault("base_fcp_st"),
                                    valor_fcp_st = row.GetValueOrDefault("valor_fcp_st"),
                                    aliq_fcp_st = row.GetValueOrDefault("aliq_fcp_st"),
                                    base_icms_fcp_st = row.GetValueOrDefault("base_icms_fcp_st"),
                                    valor_icms_fcp_st = row.GetValueOrDefault("valor_icms_fcp_st"),
                                    base_icms_fcp_st_retido = row.GetValueOrDefault("base_icms_fcp_st_retido"),
                                    valor_icms_fcp_st_retido = row.GetValueOrDefault("valor_icms_fcp_st_retido"),
                                    base_icms_fcp_st_antecipado = row.GetValueOrDefault("base_icms_fcp_st_antecipado"),
                                    valor_icms_fcp_st_antecipado = row.GetValueOrDefault("valor_icms_fcp_st_antecipado"),
                                    aliquota_icms_fcp_st_antecipado = row.GetValueOrDefault("aliquota_icms_fcp_st_antecipado"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    valor_iss = row.GetValueOrDefault("valor_iss"),
                                    tipo_tributacao_iss = row.GetValueOrDefault("tipo_tributacao_iss"),
                                    icms_fcp_aliquota = row.GetValueOrDefault("icms_fcp_aliquota"),
                                    icms_fcp_base_item = row.GetValueOrDefault("icms_fcp_base_item"),
                                    icms_fcp_valor_item = row.GetValueOrDefault("icms_fcp_valor_item"),
                                    icms_base_partilha = row.GetValueOrDefault("icms_base_partilha"),
                                    aliq_difal_interna_uf_destinatario = row.GetValueOrDefault("aliq_difal_interna_uf_destinatario"),
                                    aliq_difal_interestadual_uf_envolvidas = row.GetValueOrDefault("aliq_difal_interestadual_uf_envolvidas"),
                                    icms_item_perc_partilha_destino = row.GetValueOrDefault("icms_item_perc_partilha_destino"),
                                    icms_item_perc_partilha_origem = row.GetValueOrDefault("icms_item_perc_partilha_origem"),
                                    codigo_pacote = row.GetValueOrDefault("codigo_pacote"),
                                    codigo_itens_associados = row.GetValueOrDefault("codigo_itens_associados"),
                                    codigo_kit = row.GetValueOrDefault("codigo_kit"),
                                    id_motivo_desconto = row.GetValueOrDefault("id_motivo_desconto"),
                                    icms_st_antecipado_base_item = row.GetValueOrDefault("icms_st_antecipado_base_item"),
                                    icms_suportado_valor_item = row.GetValueOrDefault("icms_suportado_valor_item"),
                                    icms_suportado_valor_unitario = row.GetValueOrDefault("icms_suportado_valor_unitario"),
                                    icms_st_pago_base = row.GetValueOrDefault("icms_st_pago_base"),
                                    icms_st_pago_valor = row.GetValueOrDefault("icms_st_pago_valor"),
                                    icms_st_pago_aliq = row.GetValueOrDefault("icms_st_pago_aliq"),
                                    icms_para_st_pago_valor = row.GetValueOrDefault("icms_para_st_pago_valor"),
                                    cod_cliente_indicador = row.GetValueOrDefault("cod_cliente_indicador"),
                                    motivo_desconto_observacao = row.GetValueOrDefault("motivo_desconto_observacao"),
                                    codigo_classificacao_tributaria = row.GetValueOrDefault("codigo_classificacao_tributaria"),
                                    codigo_cst_cbs_ibs = row.GetValueOrDefault("codigo_cst_cbs_ibs"),
                                    base_cbs_ibs_item = row.GetValueOrDefault("base_cbs_ibs_item"),
                                    aliquota_cbs = row.GetValueOrDefault("aliquota_cbs"),
                                    valor_cbs_item = row.GetValueOrDefault("valor_cbs_item"),
                                    percentual_reducao_cbs = row.GetValueOrDefault("percentual_reducao_cbs"),
                                    aliquota_efetiva_cbs = row.GetValueOrDefault("aliquota_efetiva_cbs"),
                                    aliquota_ibs_uf = row.GetValueOrDefault("aliquota_ibs_uf"),
                                    valor_ibs_uf_item = row.GetValueOrDefault("valor_ibs_uf_item"),
                                    percentual_reducao_ibs_uf = row.GetValueOrDefault("percentual_reducao_ibs_uf"),
                                    aliquota_efetiva_ibs_uf = row.GetValueOrDefault("aliquota_efetiva_ibs_uf"),
                                    aliquota_ibs_municipio = row.GetValueOrDefault("aliquota_ibs_municipio"),
                                    valor_ibs_municipio_item = row.GetValueOrDefault("valor_ibs_municipio_item"),
                                    percentual_reducao_ibs_municipio = row.GetValueOrDefault("percentual_reducao_ibs_municipio"),
                                    aliquota_efetiva_ibs_municipio = row.GetValueOrDefault("aliquota_efetiva_ibs_municipio"),
                                    valor_ibs_total_item = row.GetValueOrDefault("valor_ibs_total_item"),
                                    custo_aquisicao_item = row.GetValueOrDefault("custo_aquisicao_item")
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + transacao; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoExtensao>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.transacao).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxMovimentoExtensaoValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxMovimentoExtensao {dto.transacao} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.transacao))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoExtensao(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.transacao}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxMovimentoExtensao(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxMovimentoCamposAdicionais()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoCamposAdicionais);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimentoCamposAdicionais, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoCamposAdicionais dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimentoCamposAdicionais"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimentoCamposAdicionais", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimentoCamposAdicionais (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoCamposAdicionais (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoCamposAdicionais (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoCamposAdicionais
                                    {
                                    transacao = row.GetValueOrDefault("transacao"),
                                    id_ordservprod = row.GetValueOrDefault("id_ordservprod"),
                                    paciente = row.GetValueOrDefault("paciente"),
                                    codigo_gerencial = row.GetValueOrDefault("codigo_gerencial"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    portal = row.GetValueOrDefault("portal"),
                                    lng_esferico_od = row.GetValueOrDefault("lng_esferico_od"),
                                    lng_cilindrico_od = row.GetValueOrDefault("lng_cilindrico_od"),
                                    lng_eixo_od = row.GetValueOrDefault("lng_eixo_od"),
                                    lng_dpdpn_od = row.GetValueOrDefault("lng_dpdpn_od"),
                                    lng_altura_od = row.GetValueOrDefault("lng_altura_od"),
                                    prt_esferico_od = row.GetValueOrDefault("prt_esferico_od"),
                                    prt_cilindrico_od = row.GetValueOrDefault("prt_cilindrico_od"),
                                    prt_eixo_od = row.GetValueOrDefault("prt_eixo_od"),
                                    prt_dpdpn_od = row.GetValueOrDefault("prt_dpdpn_od"),
                                    prt_altura_od = row.GetValueOrDefault("prt_altura_od"),
                                    lng_esferico_oe = row.GetValueOrDefault("lng_esferico_oe"),
                                    lng_cilindrico_oe = row.GetValueOrDefault("lng_cilindrico_oe"),
                                    lng_eixo_oe = row.GetValueOrDefault("lng_eixo_oe"),
                                    lng_dpdpn_oe = row.GetValueOrDefault("lng_dpdpn_oe"),
                                    lng_altura_oe = row.GetValueOrDefault("lng_altura_oe"),
                                    prt_esferico_oe = row.GetValueOrDefault("prt_esferico_oe"),
                                    prt_cilindrico_oe = row.GetValueOrDefault("prt_cilindrico_oe"),
                                    prt_eixo_oe = row.GetValueOrDefault("prt_eixo_oe"),
                                    prt_dpdpn_oe = row.GetValueOrDefault("prt_dpdpn_oe"),
                                    prt_altura_oe = row.GetValueOrDefault("prt_altura_oe"),
                                    lng_adicao_od = row.GetValueOrDefault("lng_adicao_od"),
                                    lng_adicao_oe = row.GetValueOrDefault("lng_adicao_oe"),
                                    id_ficha = row.GetValueOrDefault("id_ficha"),
                                    od_oe = row.GetValueOrDefault("od_oe"),
                                    diametro_od = row.GetValueOrDefault("diametro_od"),
                                    diametro_oe = row.GetValueOrDefault("diametro_oe"),
                                    ponte = row.GetValueOrDefault("ponte"),
                                    altura = row.GetValueOrDefault("altura"),
                                    aro = row.GetValueOrDefault("aro"),
                                    diag_maior = row.GetValueOrDefault("diag_maior"),
                                    tipo_armacao = row.GetValueOrDefault("tipo_armacao"),
                                    observacao = row.GetValueOrDefault("observacao"),
                                    grupo_servico = row.GetValueOrDefault("grupo_servico"),
                                    cmpl_add_od = row.GetValueOrDefault("cmpl_add_od"),
                                    cmpl_add_oe = row.GetValueOrDefault("cmpl_add_oe"),
                                    base_od = row.GetValueOrDefault("base_od"),
                                    base_oe = row.GetValueOrDefault("base_oe"),
                                    dist_haste = row.GetValueOrDefault("dist_haste"),
                                    dist_front = row.GetValueOrDefault("dist_front"),
                                    curvatura = row.GetValueOrDefault("curvatura"),
                                    lng_pris1_od = row.GetValueOrDefault("lng_pris1_od"),
                                    lng_pris1_od_desc = row.GetValueOrDefault("lng_pris1_od_desc"),
                                    lng_pris1_oe = row.GetValueOrDefault("lng_pris1_oe"),
                                    lng_pris1_oe_desc = row.GetValueOrDefault("lng_pris1_oe_desc"),
                                    prt_pris1_od = row.GetValueOrDefault("prt_pris1_od"),
                                    prt_pris1_od_desc = row.GetValueOrDefault("prt_pris1_od_desc"),
                                    prt_pris1_oe = row.GetValueOrDefault("prt_pris1_oe"),
                                    prt_pris1_oe_desc = row.GetValueOrDefault("prt_pris1_oe_desc"),
                                    lng_pris2_od = row.GetValueOrDefault("lng_pris2_od"),
                                    lng_pris2_od_desc = row.GetValueOrDefault("lng_pris2_od_desc"),
                                    lng_pris2_oe = row.GetValueOrDefault("lng_pris2_oe"),
                                    lng_pris2_oe_desc = row.GetValueOrDefault("lng_pris2_oe_desc"),
                                    prt_pris2_od = row.GetValueOrDefault("prt_pris2_od"),
                                    prt_pris2_od_desc = row.GetValueOrDefault("prt_pris2_od_desc"),
                                    prt_pris2_oe = row.GetValueOrDefault("prt_pris2_oe"),
                                    prt_pris2_oe_desc = row.GetValueOrDefault("prt_pris2_oe_desc"),
                                    dist_vertice = row.GetValueOrDefault("dist_vertice"),
                                    inc_planto = row.GetValueOrDefault("inc_planto"),
                                    ativ_princ = row.GetValueOrDefault("ativ_princ"),
                                    area_priv = row.GetValueOrDefault("area_priv"),
                                    dist_visao = row.GetValueOrDefault("dist_visao"),
                                    personalizada = row.GetValueOrDefault("personalizada"),
                                    pris_od = row.GetValueOrDefault("pris_od"),
                                    pris_oe = row.GetValueOrDefault("pris_oe"),
                                    eixo_od = row.GetValueOrDefault("eixo_od"),
                                    eixo_oe = row.GetValueOrDefault("eixo_oe"),
                                    borda_od = row.GetValueOrDefault("borda_od"),
                                    borda_oe = row.GetValueOrDefault("borda_oe"),
                                    centro_od = row.GetValueOrDefault("centro_od"),
                                    centro_oe = row.GetValueOrDefault("centro_oe"),
                                    furo_od = row.GetValueOrDefault("furo_od"),
                                    furo_oe = row.GetValueOrDefault("furo_oe"),
                                    format_corte = row.GetValueOrDefault("format_corte"),
                                    dist_leitura = row.GetValueOrDefault("dist_leitura"),
                                    tipo_lente = row.GetValueOrDefault("tipo_lente"),
                                    calibre = row.GetValueOrDefault("calibre"),
                                    endereco_paciente = row.GetValueOrDefault("endereco_paciente"),
                                    medico_especialista = row.GetValueOrDefault("medico_especialista"),
                                    endereco_medico_especialista = row.GetValueOrDefault("endereco_medico_especialista"),
                                    refracionista_optico = row.GetValueOrDefault("refracionista_optico"),
                                    data_receita = row.GetValueOrDefault("data_receita"),
                                    valida_sao = row.GetValueOrDefault("valida_sao"),
                                    identificador_sao = row.GetValueOrDefault("identificador_sao"),
                                    status_valida_sao = row.GetValueOrDefault("status_valida_sao"),
                                    mensagem_sao = row.GetValueOrDefault("mensagem_sao"),
                                    id_produtos_opticos_tipo_aro = row.GetValueOrDefault("id_produtos_opticos_tipo_aro"),
                                    dp_montagem = row.GetValueOrDefault("dp_montagem"),
                                    id_produtos_opticos_formato_aro = row.GetValueOrDefault("id_produtos_opticos_formato_aro"),
                                    id_movimento_campos_adicionais = row.GetValueOrDefault("id_movimento_campos_adicionais"),
                                    pronta_entrega = row.GetValueOrDefault("pronta_entrega"),
                                    cod_cliente_medico_especialista = row.GetValueOrDefault("cod_cliente_medico_especialista"),
                                    cod_cliente_clinica = row.GetValueOrDefault("cod_cliente_clinica")
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + transacao; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoCamposAdicionais>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.transacao).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxMovimentoCamposAdicionaisValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxMovimentoCamposAdicionais {dto.transacao} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.transacao))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoCamposAdicionais(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.transacao}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxMovimentoCamposAdicionais(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxMovimentoObservacoes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoObservacoes);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimentoObservacoes, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoObservacoes dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimentoObservacoes"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimentoObservacoes", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimentoObservacoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoObservacoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoObservacoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoObservacoes
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_obs = row.GetValueOrDefault("id_obs"),
                                    titulo_obs = row.GetValueOrDefault("titulo_obs"),
                                    descricao_obs = row.GetValueOrDefault("descricao_obs"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_obs; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoObservacoes>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_obs)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxMovimentoObservacoesValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxMovimentoObservacoes {dto.portal} {dto.id_obs}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_obs))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoObservacoes(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_obs}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxMovimentoObservacoes(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxMovimentoObservacaoCF()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoObservacaoCF);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimentoObservacaoCF, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoObservacaoCF dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimentoObservacaoCF"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimentoObservacaoCF", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimentoObservacaoCF (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoObservacaoCF (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoObservacaoCF (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoObservacaoCF
                                    {
                                    identificador = row.GetValueOrDefault("identificador"),
                                    documento_cliente = row.GetValueOrDefault("documento_cliente"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + identificador; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoObservacaoCF>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.identificador)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxMovimentoObservacaoCFValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxMovimentoObservacaoCF {dto.portal} {dto.identificador}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.identificador))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoObservacaoCF(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.identificador}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxMovimentoObservacaoCF(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxMovimentoIndicacoes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoIndicacoes);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimentoIndicacoes, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoIndicacoes dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimentoIndicacoes"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimentoIndicacoes", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimentoIndicacoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoIndicacoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoIndicacoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoIndicacoes
                                    {
                                    identificador = row.GetValueOrDefault("identificador"),
                                    cod_cliente = row.GetValueOrDefault("cod_cliente"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + identificador + cod_cliente; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoIndicacoes>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.identificador, x.dto.cod_cliente)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxMovimentoIndicacoesValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxMovimentoIndicacoes {dto.identificador} {dto.cod_cliente} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.identificador) && String.IsNullOrEmpty(dto.cod_cliente))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoIndicacoes(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.identificador}|{entity.cod_cliente}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxMovimentoIndicacoes(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxMovimentoSerial()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoSerial);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimentoSerial, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoSerial dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimentoSerial"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimentoSerial", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimentoSerial (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoSerial (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoSerial (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoSerial
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                    identificador = row.GetValueOrDefault("identificador"),
                                    transacao = row.GetValueOrDefault("transacao"),
                                    serial = row.GetValueOrDefault("serial"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + identificador + transacao + serial; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoSerial>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.identificador, x.dto.transacao, x.dto.serial)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxMovimentoSerialValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxMovimentoSerial {dto.identificador} {dto.transacao} {dto.serial} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.identificador) && String.IsNullOrEmpty(dto.transacao) && String.IsNullOrEmpty(dto.serial))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoSerial(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.identificador}|{entity.transacao}|{entity.serial}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxMovimentoSerial(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxSerialVenda()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxSerialVenda);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxSerialVenda, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxSerialVenda dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxSerialVenda"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxSerialVenda", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxSerialVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxSerialVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxSerialVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxSerialVenda
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_serial_venda = row.GetValueOrDefault("id_serial_venda"),
                                    transacao = row.GetValueOrDefault("transacao"),
                                    codigoproduto = row.GetValueOrDefault("codigoproduto"),
                                    serial = row.GetValueOrDefault("serial"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_serial_venda; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxSerialVenda>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_serial_venda).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxSerialVendaValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxSerialVenda {dto.id_serial_venda} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_serial_venda))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxSerialVenda(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_serial_venda}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxSerialVenda(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxMovimentoVendaConjugada()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoVendaConjugada);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxMovimentoVendaConjugada, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoVendaConjugada dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxMovimentoVendaConjugada"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMovimentoVendaConjugada", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxMovimentoVendaConjugada (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoVendaConjugada (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMovimentoVendaConjugada (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimentoVendaConjugada
                                    {
                                    identificador_produto = row.GetValueOrDefault("identificador_produto"),
                                    identificador_servico = row.GetValueOrDefault("identificador_servico"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + identificador_produto + identificador_servico; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoVendaConjugada>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.identificador_produto, x.dto.identificador_servico)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxMovimentoVendaConjugadaValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxMovimentoVendaConjugada {dto.identificador_produto} {dto.identificador_servico} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.identificador_produto) && String.IsNullOrEmpty(dto.identificador_servico))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimentoVendaConjugada(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.identificador_produto}|{entity.identificador_servico}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxMovimentoVendaConjugada(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxPerguntaVenda()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxPerguntaVenda);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxPerguntaVenda, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxPerguntaVenda dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxPerguntaVenda"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxPerguntaVenda", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxPerguntaVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPerguntaVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxPerguntaVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxPerguntaVenda
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_pergunta_venda = row.GetValueOrDefault("id_pergunta_venda"),
                                    descricao_pergunta = row.GetValueOrDefault("descricao_pergunta"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_pergunta_venda; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxPerguntaVenda>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_pergunta_venda).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxPerguntaVendaValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxPerguntaVenda {dto.id_pergunta_venda} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_pergunta_venda))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxPerguntaVenda(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_pergunta_venda}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxPerguntaVenda(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxRespostaVenda()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxRespostaVenda);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxRespostaVenda, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxRespostaVenda dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxRespostaVenda"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxRespostaVenda", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxRespostaVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxRespostaVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxRespostaVenda (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxRespostaVenda
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_resposta_venda = row.GetValueOrDefault("id_resposta_venda"),
                                    descricao_resposta = row.GetValueOrDefault("descricao_resposta"),
                                    id_pergunta_venda = row.GetValueOrDefault("id_pergunta_venda"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_resposta_venda; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxRespostaVenda>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_resposta_venda).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _linxRespostaVendaValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"LinxRespostaVenda {dto.id_resposta_venda} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_resposta_venda))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxRespostaVenda(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_resposta_venda}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixMovementsRepository.BulkInsertIntoLinxRespostaVenda(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> LinxMovimentoByDocumento(string user_name, string doc_company, string cnpj_emp, string documento)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMovimentoByDocumento);

            // rota registrada no log de requisições; o corpo enviado vai sem a chave
            const string route = "LinxMovimento";
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
                var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimento dto, string xml)>();
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
                                    new XElement("Name", "LinxMovimento"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
                                        new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                        new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                        new XElement("Parameter", new XAttribute("id", "data_inicial"), "2000-01-01"),
                                        new XElement("Parameter", new XAttribute("id", "data_fim"), endDate),
                                        new XElement("Parameter", new XAttribute("id", "dt_update_inicial"), "2000-01-01"),
                                        new XElement("Parameter", new XAttribute("id", "dt_update_fim"), endDate),
                                        new XElement("Parameter", new XAttribute("id", "filtra_somente_timestamp"), "0"   /* com 1 a API ignora também o filtro da busca (testado no LinxMovimento); as datas cobrem o histórico */),
                                        new XElement("Parameter", new XAttribute("id", "documento"), documento)
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
                            _logger.AddMessage($"Erro da API Linx Microvix em {route} (empresa {doc_company}, {documento}): {response?.StatusCode} {body}");

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
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {documento}): {message}");

                            failed = true;
                            break;
                        }

                        var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                        var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                        // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                        if (columns.Count == 0 && pageRows.Count > 0)
                        {
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em {route} (empresa {doc_company}, {documento}): {pageRows[0].Value}");

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
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimento
                                {
                                portal = row.GetValueOrDefault("portal"),
                                cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                transacao = row.GetValueOrDefault("transacao"),
                                usuario = row.GetValueOrDefault("usuario"),
                                documento = row.GetValueOrDefault("documento"),
                                chave_nf = row.GetValueOrDefault("chave_nf"),
                                ecf = row.GetValueOrDefault("ecf"),
                                numero_serie_ecf = row.GetValueOrDefault("numero_serie_ecf"),
                                modelo_nf = row.GetValueOrDefault("modelo_nf"),
                                data_documento = row.GetValueOrDefault("data_documento"),
                                data_lancamento = row.GetValueOrDefault("data_lancamento"),
                                codigo_cliente = row.GetValueOrDefault("codigo_cliente"),
                                serie = row.GetValueOrDefault("serie"),
                                desc_cfop = row.GetValueOrDefault("desc_cfop"),
                                id_cfop = row.GetValueOrDefault("id_cfop"),
                                cod_vendedor = row.GetValueOrDefault("cod_vendedor"),
                                quantidade = row.GetValueOrDefault("quantidade"),
                                preco_custo = row.GetValueOrDefault("preco_custo"),
                                valor_liquido = row.GetValueOrDefault("valor_liquido"),
                                desconto = row.GetValueOrDefault("desconto"),
                                cst_icms = row.GetValueOrDefault("cst_icms"),
                                cst_pis = row.GetValueOrDefault("cst_pis"),
                                cst_cofins = row.GetValueOrDefault("cst_cofins"),
                                cst_ipi = row.GetValueOrDefault("cst_ipi"),
                                valor_icms = row.GetValueOrDefault("valor_icms"),
                                aliquota_icms = row.GetValueOrDefault("aliquota_icms"),
                                base_icms = row.GetValueOrDefault("base_icms"),
                                valor_pis = row.GetValueOrDefault("valor_pis"),
                                aliquota_pis = row.GetValueOrDefault("aliquota_pis"),
                                base_pis = row.GetValueOrDefault("base_pis"),
                                valor_cofins = row.GetValueOrDefault("valor_cofins"),
                                aliquota_cofins = row.GetValueOrDefault("aliquota_cofins"),
                                base_cofins = row.GetValueOrDefault("base_cofins"),
                                valor_icms_st = row.GetValueOrDefault("valor_icms_st"),
                                aliquota_icms_st = row.GetValueOrDefault("aliquota_icms_st"),
                                base_icms_st = row.GetValueOrDefault("base_icms_st"),
                                valor_ipi = row.GetValueOrDefault("valor_ipi"),
                                aliquota_ipi = row.GetValueOrDefault("aliquota_ipi"),
                                base_ipi = row.GetValueOrDefault("base_ipi"),
                                valor_total = row.GetValueOrDefault("valor_total"),
                                forma_dinheiro = row.GetValueOrDefault("forma_dinheiro"),
                                total_dinheiro = row.GetValueOrDefault("total_dinheiro"),
                                forma_cheque = row.GetValueOrDefault("forma_cheque"),
                                total_cheque = row.GetValueOrDefault("total_cheque"),
                                forma_cartao = row.GetValueOrDefault("forma_cartao"),
                                total_cartao = row.GetValueOrDefault("total_cartao"),
                                forma_crediario = row.GetValueOrDefault("forma_crediario"),
                                total_crediario = row.GetValueOrDefault("total_crediario"),
                                forma_convenio = row.GetValueOrDefault("forma_convenio"),
                                total_convenio = row.GetValueOrDefault("total_convenio"),
                                frete = row.GetValueOrDefault("frete"),
                                operacao = row.GetValueOrDefault("operacao"),
                                tipo_transacao = row.GetValueOrDefault("tipo_transacao"),
                                cod_produto = row.GetValueOrDefault("cod_produto"),
                                cod_barra = row.GetValueOrDefault("cod_barra"),
                                cancelado = row.GetValueOrDefault("cancelado"),
                                excluido = row.GetValueOrDefault("excluido"),
                                soma_relatorio = row.GetValueOrDefault("soma_relatorio"),
                                identificador = row.GetValueOrDefault("identificador"),
                                deposito = row.GetValueOrDefault("deposito"),
                                obs = row.GetValueOrDefault("obs"),
                                preco_unitario = row.GetValueOrDefault("preco_unitario"),
                                hora_lancamento = row.GetValueOrDefault("hora_lancamento"),
                                natureza_operacao = row.GetValueOrDefault("natureza_operacao"),
                                tabela_preco = row.GetValueOrDefault("tabela_preco"),
                                nome_tabela_preco = row.GetValueOrDefault("nome_tabela_preco"),
                                cod_sefaz_situacao = row.GetValueOrDefault("cod_sefaz_situacao"),
                                desc_sefaz_situacao = row.GetValueOrDefault("desc_sefaz_situacao"),
                                protocolo_aut_nfe = row.GetValueOrDefault("protocolo_aut_nfe"),
                                dt_update = row.GetValueOrDefault("dt_update"),
                                forma_cheque_prazo = row.GetValueOrDefault("forma_cheque_prazo"),
                                total_cheque_prazo = row.GetValueOrDefault("total_cheque_prazo"),
                                cod_natureza_operacao = row.GetValueOrDefault("cod_natureza_operacao"),
                                preco_tabela_epoca = row.GetValueOrDefault("preco_tabela_epoca"),
                                desconto_total_item = row.GetValueOrDefault("desconto_total_item"),
                                conferido = row.GetValueOrDefault("conferido"),
                                transacao_pedido_venda = row.GetValueOrDefault("transacao_pedido_venda"),
                                codigo_modelo_nf = row.GetValueOrDefault("codigo_modelo_nf"),
                                acrescimo = row.GetValueOrDefault("acrescimo"),
                                mob_checkout = row.GetValueOrDefault("mob_checkout"),
                                aliquota_iss = row.GetValueOrDefault("aliquota_iss"),
                                base_iss = row.GetValueOrDefault("base_iss"),
                                ordem = row.GetValueOrDefault("ordem"),
                                codigo_rotina_origem = row.GetValueOrDefault("codigo_rotina_origem"),
                                timestamp = row.GetValueOrDefault("timestamp"),
                                troco = row.GetValueOrDefault("troco"),
                                transportador = row.GetValueOrDefault("transportador"),
                                icms_aliquota_desonerado = row.GetValueOrDefault("icms_aliquota_desonerado"),
                                icms_valor_desonerado_item = row.GetValueOrDefault("icms_valor_desonerado_item"),
                                empresa = row.GetValueOrDefault("empresa"),
                                desconto_item = row.GetValueOrDefault("desconto_item"),
                                aliq_iss = row.GetValueOrDefault("aliq_iss"),
                                iss_base_item = row.GetValueOrDefault("iss_base_item"),
                                despesas = row.GetValueOrDefault("despesas"),
                                seguro_total_item = row.GetValueOrDefault("seguro_total_item"),
                                acrescimo_total_item = row.GetValueOrDefault("acrescimo_total_item"),
                                despesas_total_item = row.GetValueOrDefault("despesas_total_item"),
                                forma_pix = row.GetValueOrDefault("forma_pix"),
                                total_pix = row.GetValueOrDefault("total_pix"),
                                forma_deposito_bancario = row.GetValueOrDefault("forma_deposito_bancario"),
                                total_deposito_bancario = row.GetValueOrDefault("total_deposito_bancario"),
                                id_venda_produto_b2c = row.GetValueOrDefault("id_venda_produto_b2c"),
                                item_promocional = row.GetValueOrDefault("item_promocional"),
                                acrescimo_item = row.GetValueOrDefault("acrescimo_item"),
                                icms_st_antecipado_aliquota = row.GetValueOrDefault("icms_st_antecipado_aliquota"),
                                icms_st_antecipado_margem = row.GetValueOrDefault("icms_st_antecipado_margem"),
                                icms_st_antecipado_percentual_reducao = row.GetValueOrDefault("icms_st_antecipado_percentual_reducao"),
                                icms_st_antecipado_valor_item = row.GetValueOrDefault("icms_st_antecipado_valor_item"),
                                icms_base_desonerado_item = row.GetValueOrDefault("icms_base_desonerado_item"),
                                codigo_status_nfe = row.GetValueOrDefault("codigo_status_nfe"),
                                preco_produto_epoca = row.GetValueOrDefault("preco_produto_epoca"),
                                ficha = row.GetValueOrDefault("ficha"),
                                numero_sequencial_pos = row.GetValueOrDefault("numero_sequencial_pos"),
                                dt_insert = row.GetValueOrDefault("dt_insert"),
                                custo_medio_epoca = row.GetValueOrDefault("custo_medio_epoca"),
                                conhecimento = row.GetValueOrDefault("conhecimento")
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
                    _linxMicrovixService.SaveRequest(EnumJob.LinxMovimentoByDocumento, user_name, "POST", route, doc_company, $"{cnpj_emp}|{documento}", $"Exceção ao chamar a API: {ex.Message}", false, logBody);
                    throw;
                }

                if (failed)
                    result = null;
                else if (rows.Count == 0)
                {
                    // a busca responde sem linhas quando a chave não existe
                    _logger.AddMessage($"LinxMovimento {documento} não encontrado no Linx Microvix (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    await SaveLinxMovimento(doc_company, cnpj_emp, _linxMicrovixService.GetBatchTime(), rows);

                    _logger.AddMessage($"Concluída com sucesso: LinxMovimento {documento} inserido ou atualizado ({rows.Count} linha(s))!");
                    result = true;
                }

                _linxMicrovixService.SaveRequest(EnumJob.LinxMovimentoByDocumento, user_name, "POST", route, doc_company, $"{cnpj_emp}|{documento}", lastResponse, result == true, logBody);

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
        private async Task<int> SaveLinxMovimento(string doc_company, string cnpj_emp, DateTime batchTime, List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements.LinxMovimento dto, string xml)> rows)
        {
            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimento>();
            var records = new Dictionary<string, string>();

            foreach (var (dto, xml) in rows.GroupBy(x => x.dto.transacao).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
            {
                var validations = _linxMovimentoValidator.Validate(dto);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"LinxMovimento {dto.transacao} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                if (String.IsNullOrEmpty(dto.transacao))
                    continue;

                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements.LinxMovimento(doc_company, cnpj_emp, batchTime, dto);

                _listSomenteNovos.Add(entity);
                records[$"{entity.cnpj_emp}|{entity.transacao}"] = xml;
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxMicrovixMovementsRepository.BulkInsertIntoLinxMovimento(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in records)
                _logger.AddRecord(record.Key, record.Value);

            return _listSomenteNovos.Count;
        }
    }
}

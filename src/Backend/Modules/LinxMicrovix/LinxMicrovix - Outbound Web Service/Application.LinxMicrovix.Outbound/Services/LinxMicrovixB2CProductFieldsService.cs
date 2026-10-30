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
    /// Campos adicionais de produto do B2C do WS B2C. Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixB2CProductFieldsService : ILinxMicrovixB2CProductFieldsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixB2CProductFieldsRepository _linxMicrovixB2CProductFieldsRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisDetalhes> _b2CConsultaProdutosCamposAdicionaisDetalhesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisNomes> _b2CConsultaProdutosCamposAdicionaisNomesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisValores> _b2CConsultaProdutosCamposAdicionaisValoresValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionais> _b2CConsultaB2CProdutosCamposAdicionaisValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisDados> _b2CConsultaB2CProdutosCamposAdicionaisDadosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMedia> _b2CConsultaB2CProdutosCamposAdicionaisMediaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMediaDados> _b2CConsultaB2CProdutosCamposAdicionaisMediaDadosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisOpcoes> _b2CConsultaB2CProdutosCamposAdicionaisOpcoesValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixB2CProductFieldsService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixB2CProductFieldsRepository linxMicrovixB2CProductFieldsRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisDetalhes> b2CConsultaProdutosCamposAdicionaisDetalhesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisNomes> b2CConsultaProdutosCamposAdicionaisNomesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisValores> b2CConsultaProdutosCamposAdicionaisValoresValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionais> b2CConsultaB2CProdutosCamposAdicionaisValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisDados> b2CConsultaB2CProdutosCamposAdicionaisDadosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMedia> b2CConsultaB2CProdutosCamposAdicionaisMediaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMediaDados> b2CConsultaB2CProdutosCamposAdicionaisMediaDadosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisOpcoes> b2CConsultaB2CProdutosCamposAdicionaisOpcoesValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixB2CProductFieldsRepository, _b2CConsultaProdutosCamposAdicionaisDetalhesValidator, _b2CConsultaProdutosCamposAdicionaisNomesValidator, _b2CConsultaProdutosCamposAdicionaisValoresValidator, _b2CConsultaB2CProdutosCamposAdicionaisValidator, _b2CConsultaB2CProdutosCamposAdicionaisDadosValidator, _b2CConsultaB2CProdutosCamposAdicionaisMediaValidator, _b2CConsultaB2CProdutosCamposAdicionaisMediaDadosValidator, _b2CConsultaB2CProdutosCamposAdicionaisOpcoesValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixB2CProductFieldsRepository, b2CConsultaProdutosCamposAdicionaisDetalhesValidator, b2CConsultaProdutosCamposAdicionaisNomesValidator, b2CConsultaProdutosCamposAdicionaisValoresValidator, b2CConsultaB2CProdutosCamposAdicionaisValidator, b2CConsultaB2CProdutosCamposAdicionaisDadosValidator, b2CConsultaB2CProdutosCamposAdicionaisMediaValidator, b2CConsultaB2CProdutosCamposAdicionaisMediaDadosValidator, b2CConsultaB2CProdutosCamposAdicionaisOpcoesValidator);

        public async Task<bool?> B2CConsultaProdutosCamposAdicionaisDetalhes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaProdutosCamposAdicionaisDetalhes);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaProdutosCamposAdicionaisDetalhes, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisDetalhes dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaProdutosCamposAdicionaisDetalhes"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaProdutosCamposAdicionaisDetalhes", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaProdutosCamposAdicionaisDetalhes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosCamposAdicionaisDetalhes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosCamposAdicionaisDetalhes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisDetalhes
                                    {
                                    id_campo_detalhe = row.GetValueOrDefault("id_campo_detalhe"),
                                    ordem = row.GetValueOrDefault("ordem"),
                                    descricao = row.GetValueOrDefault("descricao"),
                                    id_campo = row.GetValueOrDefault("id_campo"),
                                    ativo = row.GetValueOrDefault("ativo"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_campo_detalhe; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisDetalhes>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_campo_detalhe)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaProdutosCamposAdicionaisDetalhesValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaProdutosCamposAdicionaisDetalhes {dto.portal} {dto.id_campo_detalhe}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_campo_detalhe))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisDetalhes(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_campo_detalhe}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductFieldsRepository.BulkInsertIntoB2CConsultaProdutosCamposAdicionaisDetalhes(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaProdutosCamposAdicionaisNomes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaProdutosCamposAdicionaisNomes);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaProdutosCamposAdicionaisNomes, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisNomes dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaProdutosCamposAdicionaisNomes"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaProdutosCamposAdicionaisNomes", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaProdutosCamposAdicionaisNomes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosCamposAdicionaisNomes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosCamposAdicionaisNomes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisNomes
                                    {
                                    id_campo = row.GetValueOrDefault("id_campo"),
                                    ordem = row.GetValueOrDefault("ordem"),
                                    legenda = row.GetValueOrDefault("legenda"),
                                    tipo = row.GetValueOrDefault("tipo"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_campo; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisNomes>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_campo)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaProdutosCamposAdicionaisNomesValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaProdutosCamposAdicionaisNomes {dto.portal} {dto.id_campo}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_campo))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisNomes(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_campo}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductFieldsRepository.BulkInsertIntoB2CConsultaProdutosCamposAdicionaisNomes(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaProdutosCamposAdicionaisValores()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaProdutosCamposAdicionaisValores);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaProdutosCamposAdicionaisValores, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisValores dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaProdutosCamposAdicionaisValores"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaProdutosCamposAdicionaisValores", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaProdutosCamposAdicionaisValores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosCamposAdicionaisValores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosCamposAdicionaisValores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisValores
                                    {
                                    id_campo_valor = row.GetValueOrDefault("id_campo_valor"),
                                    codigoproduto = row.GetValueOrDefault("codigoproduto"),
                                    id_campo_detalhe = row.GetValueOrDefault("id_campo_detalhe"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_campo_detalhe + codigoproduto; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisValores>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_campo_detalhe, x.dto.codigoproduto)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaProdutosCamposAdicionaisValoresValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaProdutosCamposAdicionaisValores {dto.portal} {dto.id_campo_detalhe} {dto.codigoproduto}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_campo_detalhe) && String.IsNullOrEmpty(dto.codigoproduto))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaProdutosCamposAdicionaisValores(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_campo_detalhe}|{entity.codigoproduto}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductFieldsRepository.BulkInsertIntoB2CConsultaProdutosCamposAdicionaisValores(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaB2CProdutosCamposAdicionais()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaB2CProdutosCamposAdicionais);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaB2CProdutosCamposAdicionais, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionais dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaB2CProdutosCamposAdicionais"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaB2CProdutosCamposAdicionais", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionais (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionais (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionais (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionais
                                    {
                                    id_b2c_produtos_campos_adicionais = row.GetValueOrDefault("id_b2c_produtos_campos_adicionais"),
                                    nome_campo = row.GetValueOrDefault("nome_campo"),
                                    tipo = row.GetValueOrDefault("tipo"),
                                    decimais = row.GetValueOrDefault("decimais"),
                                    multipla_escolha = row.GetValueOrDefault("multipla_escolha"),
                                    ordem = row.GetValueOrDefault("ordem"),
                                    ativo = row.GetValueOrDefault("ativo"),
                                    nivel_id = row.GetValueOrDefault("nivel_id"),
                                    nivel_descricao = row.GetValueOrDefault("nivel_descricao"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_b2c_produtos_campos_adicionais; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionais>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_b2c_produtos_campos_adicionais).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaB2CProdutosCamposAdicionaisValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaB2CProdutosCamposAdicionais {dto.id_b2c_produtos_campos_adicionais} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_b2c_produtos_campos_adicionais))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionais(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_b2c_produtos_campos_adicionais}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductFieldsRepository.BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionais(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaB2CProdutosCamposAdicionaisDados()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaB2CProdutosCamposAdicionaisDados);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaB2CProdutosCamposAdicionaisDados, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisDados dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaB2CProdutosCamposAdicionaisDados"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaB2CProdutosCamposAdicionaisDados", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisDados (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisDados (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisDados (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisDados
                                    {
                                    id_b2c_produtos_campos_adicionais_dados = row.GetValueOrDefault("id_b2c_produtos_campos_adicionais_dados"),
                                    id_b2c_produtos_campos_adicionais_opcoes = row.GetValueOrDefault("id_b2c_produtos_campos_adicionais_opcoes"),
                                    id_b2c_produtos_campos_adicionais = row.GetValueOrDefault("id_b2c_produtos_campos_adicionais"),
                                    codigoproduto = row.GetValueOrDefault("codigoproduto"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_b2c_produtos_campos_adicionais_dados; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisDados>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_b2c_produtos_campos_adicionais_dados).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaB2CProdutosCamposAdicionaisDadosValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaB2CProdutosCamposAdicionaisDados {dto.id_b2c_produtos_campos_adicionais_dados} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_b2c_produtos_campos_adicionais_dados))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisDados(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_b2c_produtos_campos_adicionais_dados}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductFieldsRepository.BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisDados(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaB2CProdutosCamposAdicionaisMedia()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaB2CProdutosCamposAdicionaisMedia);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaB2CProdutosCamposAdicionaisMedia, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMedia dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaB2CProdutosCamposAdicionaisMedia"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaB2CProdutosCamposAdicionaisMedia", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisMedia (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisMedia (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisMedia (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMedia
                                    {
                                    id_b2c_produtos_campos_adicionais_media = row.GetValueOrDefault("id_b2c_produtos_campos_adicionais_media"),
                                    descricao = row.GetValueOrDefault("descricao"),
                                    url = row.GetValueOrDefault("url"),
                                    tipo_media_descricao = row.GetValueOrDefault("tipo_media_descricao"),
                                    portal = row.GetValueOrDefault("portal"),
                                    media_principal = row.GetValueOrDefault("media_principal"),
                                    tipo_media_id = row.GetValueOrDefault("tipo_media_id"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_b2c_produtos_campos_adicionais_media; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMedia>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_b2c_produtos_campos_adicionais_media).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaB2CProdutosCamposAdicionaisMediaValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaB2CProdutosCamposAdicionaisMedia {dto.id_b2c_produtos_campos_adicionais_media} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_b2c_produtos_campos_adicionais_media))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMedia(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_b2c_produtos_campos_adicionais_media}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductFieldsRepository.BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisMedia(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaB2CProdutosCamposAdicionaisMediaDados()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaB2CProdutosCamposAdicionaisMediaDados);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaB2CProdutosCamposAdicionaisMediaDados, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMediaDados dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaB2CProdutosCamposAdicionaisMediaDados"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaB2CProdutosCamposAdicionaisMediaDados", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisMediaDados (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisMediaDados (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisMediaDados (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMediaDados
                                    {
                                    id_b2c_produtos_campos_adicionais_media_dados = row.GetValueOrDefault("id_b2c_produtos_campos_adicionais_media_dados"),
                                    id_b2c_produtos_campos_adicionais_media = row.GetValueOrDefault("id_b2c_produtos_campos_adicionais_media"),
                                    codigoproduto = row.GetValueOrDefault("codigoproduto"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_b2c_produtos_campos_adicionais_media_dados; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMediaDados>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_b2c_produtos_campos_adicionais_media_dados).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaB2CProdutosCamposAdicionaisMediaDadosValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaB2CProdutosCamposAdicionaisMediaDados {dto.id_b2c_produtos_campos_adicionais_media_dados} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_b2c_produtos_campos_adicionais_media_dados))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisMediaDados(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_b2c_produtos_campos_adicionais_media_dados}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductFieldsRepository.BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisMediaDados(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaB2CProdutosCamposAdicionaisOpcoes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaB2CProdutosCamposAdicionaisOpcoes);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaB2CProdutosCamposAdicionaisOpcoes, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisOpcoes dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaB2CProdutosCamposAdicionaisOpcoes"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaB2CProdutosCamposAdicionaisOpcoes", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisOpcoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisOpcoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaB2CProdutosCamposAdicionaisOpcoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisOpcoes
                                    {
                                    id_b2c_produtos_campos_adicionais_opcoes = row.GetValueOrDefault("id_b2c_produtos_campos_adicionais_opcoes"),
                                    id_b2c_produtos_campos_adicionais = row.GetValueOrDefault("id_b2c_produtos_campos_adicionais"),
                                    valor = row.GetValueOrDefault("valor"),
                                    ordem = row.GetValueOrDefault("ordem"),
                                    ativo = row.GetValueOrDefault("ativo"),
                                    categoria_nivel = row.GetValueOrDefault("categoria_nivel"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_b2c_produtos_campos_adicionais_opcoes; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisOpcoes>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_b2c_produtos_campos_adicionais_opcoes).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaB2CProdutosCamposAdicionaisOpcoesValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaB2CProdutosCamposAdicionaisOpcoes {dto.id_b2c_produtos_campos_adicionais_opcoes} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_b2c_produtos_campos_adicionais_opcoes))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields.B2CConsultaB2CProdutosCamposAdicionaisOpcoes(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_b2c_produtos_campos_adicionais_opcoes}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductFieldsRepository.BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisOpcoes(_listSomenteNovos, _logger.GetExecutionGuid());

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

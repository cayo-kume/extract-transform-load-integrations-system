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
    /// Atributos de produto do B2C do WS B2C. Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixB2CProductAttributesService : ILinxMicrovixB2CProductAttributesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixB2CProductAttributesRepository _linxMicrovixB2CProductAttributesRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaClassificacao> _b2CConsultaClassificacaoValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaColecoes> _b2CConsultaColecoesValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaEspessuras> _b2CConsultaEspessurasValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade1> _b2CConsultaGrade1Validator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade2> _b2CConsultaGrade2Validator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaLinhas> _b2CConsultaLinhasValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaMarcas> _b2CConsultaMarcasValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaSetores> _b2CConsultaSetoresValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaUnidade> _b2CConsultaUnidadeValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaFornecedores> _b2CConsultaFornecedoresValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaFlags> _b2CConsultaFlagsValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaTags> _b2CConsultaTagsValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaPalavrasChavePesquisa> _b2CConsultaPalavrasChavePesquisaValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaLegendasCadastrosAuxiliares> _b2CConsultaLegendasCadastrosAuxiliaresValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixB2CProductAttributesService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixB2CProductAttributesRepository linxMicrovixB2CProductAttributesRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaClassificacao> b2CConsultaClassificacaoValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaColecoes> b2CConsultaColecoesValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaEspessuras> b2CConsultaEspessurasValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade1> b2CConsultaGrade1Validator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade2> b2CConsultaGrade2Validator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaLinhas> b2CConsultaLinhasValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaMarcas> b2CConsultaMarcasValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaSetores> b2CConsultaSetoresValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaUnidade> b2CConsultaUnidadeValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaFornecedores> b2CConsultaFornecedoresValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaFlags> b2CConsultaFlagsValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaTags> b2CConsultaTagsValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaPalavrasChavePesquisa> b2CConsultaPalavrasChavePesquisaValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaLegendasCadastrosAuxiliares> b2CConsultaLegendasCadastrosAuxiliaresValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixB2CProductAttributesRepository, _b2CConsultaClassificacaoValidator, _b2CConsultaColecoesValidator, _b2CConsultaEspessurasValidator, _b2CConsultaGrade1Validator, _b2CConsultaGrade2Validator, _b2CConsultaLinhasValidator, _b2CConsultaMarcasValidator, _b2CConsultaSetoresValidator, _b2CConsultaUnidadeValidator, _b2CConsultaFornecedoresValidator, _b2CConsultaFlagsValidator, _b2CConsultaTagsValidator, _b2CConsultaPalavrasChavePesquisaValidator, _b2CConsultaLegendasCadastrosAuxiliaresValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixB2CProductAttributesRepository, b2CConsultaClassificacaoValidator, b2CConsultaColecoesValidator, b2CConsultaEspessurasValidator, b2CConsultaGrade1Validator, b2CConsultaGrade2Validator, b2CConsultaLinhasValidator, b2CConsultaMarcasValidator, b2CConsultaSetoresValidator, b2CConsultaUnidadeValidator, b2CConsultaFornecedoresValidator, b2CConsultaFlagsValidator, b2CConsultaTagsValidator, b2CConsultaPalavrasChavePesquisaValidator, b2CConsultaLegendasCadastrosAuxiliaresValidator);

        public async Task<bool?> B2CConsultaClassificacao()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaClassificacao);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaClassificacao, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaClassificacao dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaClassificacao"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaClassificacao", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaClassificacao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClassificacao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaClassificacao (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaClassificacao
                                    {
                                    codigo_classificacao = row.GetValueOrDefault("codigo_classificacao"),
                                    nome_classificacao = row.GetValueOrDefault("nome_classificacao"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + codigo_classificacao; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaClassificacao>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.codigo_classificacao)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaClassificacaoValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaClassificacao {dto.portal} {dto.codigo_classificacao}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.codigo_classificacao))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaClassificacao(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.codigo_classificacao}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaClassificacao(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaColecoes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaColecoes);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaColecoes, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaColecoes dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaColecoes"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaColecoes", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaColecoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaColecoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaColecoes (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaColecoes
                                    {
                                    codigo_colecao = row.GetValueOrDefault("codigo_colecao"),
                                    nome_colecao = row.GetValueOrDefault("nome_colecao"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    marcas = row.GetValueOrDefault("marcas"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + codigo_colecao; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaColecoes>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.codigo_colecao)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaColecoesValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaColecoes {dto.portal} {dto.codigo_colecao}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.codigo_colecao))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaColecoes(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.codigo_colecao}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaColecoes(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaEspessuras()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaEspessuras);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaEspessuras, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaEspessuras dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaEspessuras"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaEspessuras", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaEspessuras (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaEspessuras (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaEspessuras (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaEspessuras
                                    {
                                    codigo_espessura = row.GetValueOrDefault("codigo_espessura"),
                                    nome_espessura = row.GetValueOrDefault("nome_espessura"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + codigo_espessura; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaEspessuras>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.codigo_espessura)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaEspessurasValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaEspessuras {dto.portal} {dto.codigo_espessura}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.codigo_espessura))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaEspessuras(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.codigo_espessura}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaEspessuras(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaGrade1()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaGrade1);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaGrade1, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade1 dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaGrade1"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaGrade1", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaGrade1 (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaGrade1 (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaGrade1 (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade1
                                    {
                                    codigo_grade1 = row.GetValueOrDefault("codigo_grade1"),
                                    nome_grade1 = row.GetValueOrDefault("nome_grade1"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + codigo_grade1; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaGrade1>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.codigo_grade1)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaGrade1Validator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaGrade1 {dto.portal} {dto.codigo_grade1}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.codigo_grade1))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaGrade1(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.codigo_grade1}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaGrade1(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaGrade2()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaGrade2);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaGrade2, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade2 dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaGrade2"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaGrade2", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaGrade2 (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaGrade2 (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaGrade2 (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaGrade2
                                    {
                                    codigo_grade2 = row.GetValueOrDefault("codigo_grade2"),
                                    nome_grade2 = row.GetValueOrDefault("nome_grade2"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + codigo_grade2; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaGrade2>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.codigo_grade2)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaGrade2Validator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaGrade2 {dto.portal} {dto.codigo_grade2}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.codigo_grade2))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaGrade2(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.codigo_grade2}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaGrade2(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaLinhas()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaLinhas);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaLinhas, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaLinhas dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaLinhas"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaLinhas", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaLinhas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaLinhas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaLinhas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaLinhas
                                    {
                                    codigo_linha = row.GetValueOrDefault("codigo_linha"),
                                    nome_linha = row.GetValueOrDefault("nome_linha"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    setores = row.GetValueOrDefault("setores"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + codigo_linha; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaLinhas>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.codigo_linha)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaLinhasValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaLinhas {dto.portal} {dto.codigo_linha}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.codigo_linha))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaLinhas(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.codigo_linha}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaLinhas(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaMarcas()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaMarcas);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaMarcas, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaMarcas dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaMarcas"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaMarcas", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaMarcas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaMarcas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaMarcas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaMarcas
                                    {
                                    codigo_marca = row.GetValueOrDefault("codigo_marca"),
                                    nome_marca = row.GetValueOrDefault("nome_marca"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    linhas = row.GetValueOrDefault("linhas"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + codigo_marca; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaMarcas>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.codigo_marca)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaMarcasValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaMarcas {dto.portal} {dto.codigo_marca}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.codigo_marca))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaMarcas(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.codigo_marca}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaMarcas(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaSetores()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaSetores);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaSetores, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaSetores dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaSetores"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaSetores", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaSetores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaSetores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaSetores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaSetores
                                    {
                                    codigo_setor = row.GetValueOrDefault("codigo_setor"),
                                    nome_setor = row.GetValueOrDefault("nome_setor"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + codigo_setor; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaSetores>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.codigo_setor)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaSetoresValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaSetores {dto.portal} {dto.codigo_setor}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.codigo_setor))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaSetores(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.codigo_setor}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaSetores(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaUnidade()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaUnidade);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaUnidade, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaUnidade dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaUnidade"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaUnidade", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaUnidade (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaUnidade (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaUnidade (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaUnidade
                                    {
                                    unidade = row.GetValueOrDefault("unidade"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + unidade; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaUnidade>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.unidade)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaUnidadeValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaUnidade {dto.portal} {dto.unidade}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.unidade))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaUnidade(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.unidade}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaUnidade(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaFornecedores()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaFornecedores);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaFornecedores, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaFornecedores dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaFornecedores"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaFornecedores", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaFornecedores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaFornecedores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaFornecedores (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaFornecedores
                                    {
                                    cod_fornecedor = row.GetValueOrDefault("cod_fornecedor"),
                                    nome = row.GetValueOrDefault("nome"),
                                    nome_fantasia = row.GetValueOrDefault("nome_fantasia"),
                                    tipo_pessoa = row.GetValueOrDefault("tipo_pessoa"),
                                    tipo_fornecedor = row.GetValueOrDefault("tipo_fornecedor"),
                                    endereco = row.GetValueOrDefault("endereco"),
                                    numero_rua = row.GetValueOrDefault("numero_rua"),
                                    bairro = row.GetValueOrDefault("bairro"),
                                    cep = row.GetValueOrDefault("cep"),
                                    cidade = row.GetValueOrDefault("cidade"),
                                    uf = row.GetValueOrDefault("uf"),
                                    documento = row.GetValueOrDefault("documento"),
                                    fone = row.GetValueOrDefault("fone"),
                                    email = row.GetValueOrDefault("email"),
                                    pais = row.GetValueOrDefault("pais"),
                                    obs = row.GetValueOrDefault("obs"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + cod_fornecedor; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaFornecedores>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.cod_fornecedor)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaFornecedoresValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaFornecedores {dto.portal} {dto.cod_fornecedor}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.cod_fornecedor))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaFornecedores(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.cod_fornecedor}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaFornecedores(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaFlags()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaFlags);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaFlags, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaFlags dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaFlags"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaFlags", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaFlags (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaFlags (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaFlags (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaFlags
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_b2c_flags = row.GetValueOrDefault("id_b2c_flags"),
                                    descricao = row.GetValueOrDefault("descricao"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_b2c_flags; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaFlags>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_b2c_flags).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaFlagsValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaFlags {dto.id_b2c_flags} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_b2c_flags))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaFlags(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_b2c_flags}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaFlags(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaTags()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaTags);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaTags, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaTags dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaTags"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaTags", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaTags (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaTags (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaTags (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaTags
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_b2c_tags = row.GetValueOrDefault("id_b2c_tags"),
                                    descricao = row.GetValueOrDefault("descricao"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_b2c_tags; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaTags>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_b2c_tags).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaTagsValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaTags {dto.id_b2c_tags} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_b2c_tags))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaTags(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_b2c_tags}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaTags(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaPalavrasChavePesquisa()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaPalavrasChavePesquisa);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaPalavrasChavePesquisa, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaPalavrasChavePesquisa dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaPalavrasChavePesquisa"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaPalavrasChavePesquisa", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaPalavrasChavePesquisa (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPalavrasChavePesquisa (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaPalavrasChavePesquisa (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaPalavrasChavePesquisa
                                    {
                                    portal = row.GetValueOrDefault("portal"),
                                    id_b2c_palavras_chave_pesquisa = row.GetValueOrDefault("id_b2c_palavras_chave_pesquisa"),
                                    descricao = row.GetValueOrDefault("descricao"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_b2c_palavras_chave_pesquisa; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaPalavrasChavePesquisa>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_b2c_palavras_chave_pesquisa).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaPalavrasChavePesquisaValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaPalavrasChavePesquisa {dto.id_b2c_palavras_chave_pesquisa} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_b2c_palavras_chave_pesquisa))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaPalavrasChavePesquisa(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_b2c_palavras_chave_pesquisa}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaPalavrasChavePesquisa(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaLegendasCadastrosAuxiliares()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaLegendasCadastrosAuxiliares);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaLegendasCadastrosAuxiliares, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaLegendasCadastrosAuxiliares dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaLegendasCadastrosAuxiliares"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaLegendasCadastrosAuxiliares", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaLegendasCadastrosAuxiliares (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaLegendasCadastrosAuxiliares (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaLegendasCadastrosAuxiliares (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes.B2CConsultaLegendasCadastrosAuxiliares
                                    {
                                    empresa = row.GetValueOrDefault("empresa"),
                                    legenda_setor = row.GetValueOrDefault("legenda_setor"),
                                    legenda_linha = row.GetValueOrDefault("legenda_linha"),
                                    legenda_marca = row.GetValueOrDefault("legenda_marca"),
                                    legenda_colecao = row.GetValueOrDefault("legenda_colecao"),
                                    legenda_grade1 = row.GetValueOrDefault("legenda_grade1"),
                                    legenda_grade2 = row.GetValueOrDefault("legenda_grade2"),
                                    legenda_espessura = row.GetValueOrDefault("legenda_espessura"),
                                    legenda_classificacao = row.GetValueOrDefault("legenda_classificacao"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + empresa; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaLegendasCadastrosAuxiliares>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.empresa).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaLegendasCadastrosAuxiliaresValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaLegendasCadastrosAuxiliares {dto.empresa} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.empresa))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes.B2CConsultaLegendasCadastrosAuxiliares(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.empresa}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductAttributesRepository.BulkInsertIntoB2CConsultaLegendasCadastrosAuxiliares(_listSomenteNovos, _logger.GetExecutionGuid());

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

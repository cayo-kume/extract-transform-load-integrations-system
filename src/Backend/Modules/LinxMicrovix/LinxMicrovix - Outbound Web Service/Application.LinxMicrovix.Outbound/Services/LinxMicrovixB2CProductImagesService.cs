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
    /// Imagens de produto do B2C do WS B2C. Cada método faz as próprias chamadas à API (envelope XML, chamada, repetição pelo
    /// timestamp e leitura da resposta), como no Totvs (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixB2CProductImagesService : ILinxMicrovixB2CProductImagesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixB2CProductImagesRepository _linxMicrovixB2CProductImagesRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagens> _b2CConsultaProdutosImagensValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagensURL> _b2CConsultaProdutosImagensURLValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaImagens> _b2CConsultaImagensValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaImagensHD> _b2CConsultaImagensHDValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixB2CProductImagesService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixB2CProductImagesRepository linxMicrovixB2CProductImagesRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagens> b2CConsultaProdutosImagensValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagensURL> b2CConsultaProdutosImagensURLValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaImagens> b2CConsultaImagensValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaImagensHD> b2CConsultaImagensHDValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixB2CProductImagesRepository, _b2CConsultaProdutosImagensValidator, _b2CConsultaProdutosImagensURLValidator, _b2CConsultaImagensValidator, _b2CConsultaImagensHDValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixB2CProductImagesRepository, b2CConsultaProdutosImagensValidator, b2CConsultaProdutosImagensURLValidator, b2CConsultaImagensValidator, b2CConsultaImagensHDValidator);

        public async Task<bool?> B2CConsultaProdutosImagens()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaProdutosImagens);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaProdutosImagens, null);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagens dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaProdutosImagens"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaProdutosImagens", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaProdutosImagens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosImagens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosImagens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagens
                                    {
                                    id_imagem_produto = row.GetValueOrDefault("id_imagem_produto"),
                                    id_imagem = row.GetValueOrDefault("id_imagem"),
                                    codigoproduto = row.GetValueOrDefault("codigoproduto"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    portal = row.GetValueOrDefault("portal"),
                                    principal = row.GetValueOrDefault("principal")
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + id_imagem_produto; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages.B2CConsultaProdutosImagens>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_imagem_produto)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaProdutosImagensValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaProdutosImagens {dto.portal} {dto.id_imagem_produto}: {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.portal) && String.IsNullOrEmpty(dto.id_imagem_produto))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages.B2CConsultaProdutosImagens(company.doc_company!, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.portal}|{entity.id_imagem_produto}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductImagesRepository.BulkInsertIntoB2CConsultaProdutosImagens(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaProdutosImagensURL()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaProdutosImagensURL);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaProdutosImagensURL, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagensURL dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaProdutosImagensURL"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaProdutosImagensURL", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaProdutosImagensURL (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosImagensURL (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaProdutosImagensURL (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaProdutosImagensURL
                                    {
                                    codigoproduto = row.GetValueOrDefault("codigoproduto"),
                                    url_imagem_blob = row.GetValueOrDefault("url_imagem_blob"),
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + codigoproduto + url_imagem_blob; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages.B2CConsultaProdutosImagensURL>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.codigoproduto, x.dto.url_imagem_blob)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaProdutosImagensURLValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaProdutosImagensURL {dto.codigoproduto} {dto.url_imagem_blob} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.codigoproduto) && String.IsNullOrEmpty(dto.url_imagem_blob))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages.B2CConsultaProdutosImagensURL(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.codigoproduto}|{entity.url_imagem_blob}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductImagesRepository.BulkInsertIntoB2CConsultaProdutosImagensURL(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaImagens()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaImagens);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaImagens, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaImagens dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "B2CConsultaImagens"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaImagens", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaImagens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

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
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaImagens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaImagens (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

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
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaImagens
                                    {
                                    id_imagem = row.GetValueOrDefault("id_imagem"),
                                    imagem = row.GetValueOrDefault("imagem"),
                                    md5 = row.GetValueOrDefault("md5"),
                                    timestamp = row.GetValueOrDefault("timestamp"),
                                    portal = row.GetValueOrDefault("portal"),
                                    url_imagem_blob = row.GetValueOrDefault("url_imagem_blob")
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

                        // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id_imagem; fica a linha de maior timestamp
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages.B2CConsultaImagens>();
                        var records = new Dictionary<string, string>();

                        foreach (var (dto, xml) in rows.GroupBy(x => x.dto.id_imagem).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                        {
                            var validations = _b2CConsultaImagensValidator.Validate(dto);

                            if (validations.Errors.Count > 0)
                            {
                                var message = String.Empty;
                                validations.Errors.ForEach(s => message += $"B2CConsultaImagens {dto.id_imagem} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                _logger.AddMessage(message);
                            }

                            // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                            if (String.IsNullOrEmpty(dto.id_imagem))
                                continue;

                            var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages.B2CConsultaImagens(company.doc_company!, cnpj_emp, batchTime, dto);

                            _listSomenteNovos.Add(entity);
                            records[$"{entity.cnpj_emp}|{entity.id_imagem}"] = xml;
                        }

                        if (_listSomenteNovos.Count == 0)
                            continue;

                        await _linxMicrovixB2CProductImagesRepository.BulkInsertIntoB2CConsultaImagens(_listSomenteNovos, _logger.GetExecutionGuid());

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

        public async Task<bool?> B2CConsultaImagensHD()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.B2CConsultaImagensHD);

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
                        // o método exige um codigoproduto por chamada: os valores vêm do B2CConsultaProdutos já gravado (só os gravados depois da última leitura deste método)
                        var sourceValues = await _linxMicrovixService.GetSourceValues(company, EnumJob.B2CConsultaProdutos, "codigoproduto", null, EnumJob.B2CConsultaImagensHD);

                        foreach (var codigoproduto in sourceValues)
                        {
                            // timestamp exclusivo, por codigoproduto: um valor novo começa do 0 sem pular os outros
                            var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.B2CConsultaImagensHD, cnpj_emp, "codigoproduto", codigoproduto);
                            var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaImagensHD dto, string xml)>();
                            var failed = false;

                            for (var page = 1; page <= MaxPages; page++)
                            {
                                var requestBody = new XDocument(
                                    new XDeclaration("1.0", "utf-8", null),
                                    new XElement("LinxMicrovix",
                                        new XElement("Authentication", new XAttribute("user", company.b2c_user_name ?? String.Empty), new XAttribute("password", company.b2c_password ?? String.Empty)),
                                        new XElement("ResponseFormat", "xml"),
                                        new XElement("Command",
                                            new XElement("Name", "B2CConsultaImagensHD"),
                                            new XElement("Parameters",
                                                new XElement("Parameter", new XAttribute("id", "chave"), company.b2c_chave),
                                                new XElement("Parameter", new XAttribute("id", "cnpjEmp"), cnpj_emp),
                                                new XElement("Parameter", new XAttribute("id", "timestamp"), timestamp),
                                                new XElement("Parameter", new XAttribute("id", "codigoproduto"), codigoproduto)
                                            )
                                        )
                                    )
                                );

                                // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                                ApiResponse? response = null;

                                for (var attempt = 1; attempt <= 2; attempt++)
                                {
                                    using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                                    response = await _apiClient.SendAsync(HttpMethod.Post, "B2CConsultaImagensHD", content);

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
                                    _logger.AddMessage($"Erro da API Linx Microvix em B2CConsultaImagensHD (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}, codigoproduto {codigoproduto}): {response?.StatusCode} {body}");

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
                                    _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaImagensHD (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}, codigoproduto {codigoproduto}): {message}");

                                    failed = true;
                                    break;
                                }

                                var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                                var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                                // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                                if (columns.Count == 0 && pageRows.Count > 0)
                                {
                                    _logger.AddMessage($"Resposta de erro da API Linx Microvix em B2CConsultaImagensHD (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}, codigoproduto {codigoproduto}): {pageRows[0].Value}");

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
                                        new Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages.B2CConsultaImagensHD
                                        {
                                        portal = row.GetValueOrDefault("portal"),
                                        identificador_imagem = row.GetValueOrDefault("identificador_imagem"),
                                        codigoproduto = row.GetValueOrDefault("codigoproduto"),
                                        imagem = row.GetValueOrDefault("imagem"),
                                        timestamp = row.GetValueOrDefault("timestamp"),
                                        url_imagem_blob = row.GetValueOrDefault("url_imagem_blob")
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

                            // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + codigoproduto + identificador_imagem; fica a linha de maior timestamp
                            var batchTime = _linxMicrovixService.GetBatchTime();

                            var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages.B2CConsultaImagensHD>();
                            var records = new Dictionary<string, string>();

                            foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.codigoproduto, x.dto.identificador_imagem)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                            {
                                var validations = _b2CConsultaImagensHDValidator.Validate(dto);

                                if (validations.Errors.Count > 0)
                                {
                                    var message = String.Empty;
                                    validations.Errors.ForEach(s => message += $"B2CConsultaImagensHD {dto.codigoproduto} {dto.identificador_imagem} (loja {cnpj_emp}): {s.ErrorMessage}\n");
                                    _logger.AddMessage(message);
                                }

                                // sem nenhuma parte da chave o registro não é gravado; parte vazia vira o valor padrão (0, texto vazio ou 1900-01-01)
                                if (String.IsNullOrEmpty(dto.codigoproduto) && String.IsNullOrEmpty(dto.identificador_imagem))
                                    continue;

                                var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages.B2CConsultaImagensHD(company.doc_company!, cnpj_emp, batchTime, dto);

                                _listSomenteNovos.Add(entity);
                                records[$"{entity.cnpj_emp}|{entity.codigoproduto}|{entity.identificador_imagem}"] = xml;
                            }

                            if (_listSomenteNovos.Count == 0)
                                continue;

                            await _linxMicrovixB2CProductImagesRepository.BulkInsertIntoB2CConsultaImagensHD(_listSomenteNovos, _logger.GetExecutionGuid());

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
    }
}

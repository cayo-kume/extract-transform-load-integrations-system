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
    /// lojas, usuários e catálogo do Web Service de Saída Padrão (ERP). Cada método faz as próprias chamadas à API
    /// (envelope XML, chamada, repetição pelo timestamp e leitura da resposta), como no Totvs
    /// (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §4.1 e §4.2)
    /// </summary>
    public class LinxMicrovixStoresService : ILinxMicrovixStoresService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixService;
        private readonly ILinxMicrovixStoresRepository _linxMicrovixStoresRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxGrupoLojas> _linxGrupoLojasValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxLojas> _linxLojasValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxLojasParametros> _linxLojasParametrosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxUsuarios> _linxUsuariosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxMetodos> _linxMetodosValidator;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxEntrantes> _linxEntrantesValidator;

        // a API devolve no máximo N registros por chamada, e N muda por método (5000 no LinxMovimento, 100 no LinxUsuarios,
        // testado): a leitura repete pelo maior timestamp até a resposta vir vazia. O teto só protege de um laço sem fim
        private const int MaxPages = 10000;

        public LinxMicrovixStoresService(
            [FromKeyedServices("LinxMicrovixAPI")] IApiClient apiClient,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixService,
            ILinxMicrovixStoresRepository linxMicrovixStoresRepository,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxGrupoLojas> linxGrupoLojasValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxLojas> linxLojasValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxLojasParametros> linxLojasParametrosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxUsuarios> linxUsuariosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxMetodos> linxMetodosValidator,
            IValidator<Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxEntrantes> linxEntrantesValidator
        ) =>
            (_apiClient, _logger, _linxMicrovixService, _linxMicrovixStoresRepository, _linxGrupoLojasValidator, _linxLojasValidator, _linxLojasParametrosValidator, _linxUsuariosValidator, _linxMetodosValidator, _linxEntrantesValidator) =
            (apiClient, logger, linxMicrovixService, linxMicrovixStoresRepository, linxGrupoLojasValidator, linxLojasValidator, linxLojasParametrosValidator, linxUsuariosValidator, linxMetodosValidator, linxEntrantesValidator);

        public async Task<bool?> LinxGrupoLojas()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxGrupoLojas);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    // o método não tem timestamp: o grupo vem inteiro numa chamada. O grupo é opcional (respondeu sem ele, testado)
                    var parameters = new XElement("Parameters",
                        new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave)
                    );

                    if (!String.IsNullOrWhiteSpace(company.grupo))
                        parameters.Add(new XElement("Parameter", new XAttribute("id", "grupo"), company.grupo));

                    var requestBody = new XDocument(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("LinxMicrovix",
                            new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                            new XElement("ResponseFormat", "xml"),
                            new XElement("Command", new XElement("Name", "LinxGrupoLojas"), parameters)
                        )
                    );

                    // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                    ApiResponse? response = null;

                    for (var attempt = 1; attempt <= 2; attempt++)
                    {
                        using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                        response = await _apiClient.SendAsync(HttpMethod.Post, "LinxGrupoLojas", content);

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
                        _logger.AddMessage($"Erro da API Linx Microvix em LinxGrupoLojas (empresa {company.doc_company}): {response?.StatusCode} {body}");
                        continue;
                    }

                    XElement? root = null;

                    try
                    {
                        root = XDocument.Parse(response.Content?.TrimStart('﻿') ?? String.Empty).Root;
                    }
                    catch (XmlException)
                    {
                        // root continua null: tratado abaixo como resposta inválida
                    }

                    if (root is null || !String.Equals(root.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                    {
                        var message = root?.Descendants("Message").FirstOrDefault()?.Value ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content);
                        _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxGrupoLojas (empresa {company.doc_company}): {message}");
                        continue;
                    }

                    var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                    var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxGrupoLojas dto, string xml)>();

                    // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                    if (columns.Count == 0 && root.Element("ResponseData")?.Element("R") is XElement messageRow)
                    {
                        _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxGrupoLojas (empresa {company.doc_company}): {messageRow.Value}");
                        continue;
                    }

                    foreach (var r in root.Element("ResponseData")?.Elements("R") ?? [])
                    {
                        // <D /> é nulo; a linha é registrada em auditing.Records com o nome de cada coluna
                        var values = r.Elements("D").Select(d => d.IsEmpty ? null : d.Value).ToList();
                        var row = columns.Select((c, i) => (c, v: i < values.Count ? values[i] : null)).ToDictionary(x => x.c, x => x.v);

                        rows.Add((
                            new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxGrupoLojas
                            {
                                CNPJ = row.GetValueOrDefault("CNPJ"),
                                nome_empresa = row.GetValueOrDefault("nome_empresa"),
                                id_empresas_rede = row.GetValueOrDefault("id_empresas_rede"),
                                rede = row.GetValueOrDefault("rede"),
                                portal = row.GetValueOrDefault("portal"),
                                nome_portal = row.GetValueOrDefault("nome_portal"),
                                empresa = row.GetValueOrDefault("empresa"),
                                lojas_proprias = row.GetValueOrDefault("lojas_proprias"),
                                classificacao_portal = row.GetValueOrDefault("classificacao_portal"),
                                emp_mod_linx_promo = row.GetValueOrDefault("emp_mod_linx_promo")
                            },
                            new XElement("R", row.Select(x => new XElement(x.Key, x.Value))).ToString(SaveOptions.DisableFormatting)
                        ));
                    }

                    if (rows.Count == 0)
                        continue;

                    // valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: portal + empresa; sem ela o registro sai
                    var batchTime = _linxMicrovixService.GetBatchTime();

                    var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxGrupoLojas>();
                    var records = new Dictionary<string, string>();

                    foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.empresa)).Select(g => g.Last()))
                    {
                        var validations = _linxGrupoLojasValidator.Validate(dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"loja {dto.portal}/{dto.empresa} ({dto.CNPJ}): {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxGrupoLojas(company.doc_company!, batchTime, dto);

                        if (entity.portal == 0 || entity.empresa == 0)
                            continue;

                        _listSomenteNovos.Add(entity);
                        records[$"{entity.portal}|{entity.empresa}"] = xml;
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _linxMicrovixStoresRepository.BulkInsertIntoLinxGrupoLojas(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in records)
                        _logger.AddRecord(record.Key, record.Value);

                    totalInseridos += _listSomenteNovos.Count;
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

        public async Task<bool?> LinxLojas()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxLojas);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxLojas, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxLojas dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxLojas"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxLojas", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxLojas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

                                failed = true;
                                break;
                            }

                            XElement? root = null;

                            try
                            {
                                root = XDocument.Parse(response.Content?.TrimStart('﻿') ?? String.Empty).Root;
                            }
                            catch (XmlException)
                            {
                                // root continua null: tratado abaixo como resposta inválida
                            }

                            if (root is null || !String.Equals(root.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                            {
                                var message = root?.Descendants("Message").FirstOrDefault()?.Value ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content);
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxLojas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxLojas (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

                                failed = true;
                                break;
                            }

                            // resposta vazia: não há mais registros com timestamp maior que o informado
                            if (pageRows.Count == 0)
                                break;

                            foreach (var r in pageRows)
                            {
                                var values = r.Elements("D").Select(d => d.IsEmpty ? null : d.Value).ToList();
                                var row = columns.Select((c, i) => (c, v: i < values.Count ? values[i] : null)).ToDictionary(x => x.c, x => x.v);

                                rows.Add((
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxLojas
                                    {
                                        portal = row.GetValueOrDefault("portal"),
                                        empresa = row.GetValueOrDefault("empresa"),
                                        nome_emp = row.GetValueOrDefault("nome_emp"),
                                        razao_emp = row.GetValueOrDefault("razao_emp"),
                                        cnpj_emp = row.GetValueOrDefault("cnpj_emp"),
                                        inscricao_emp = row.GetValueOrDefault("inscricao_emp"),
                                        endereco_emp = row.GetValueOrDefault("endereco_emp"),
                                        num_emp = row.GetValueOrDefault("num_emp"),
                                        complement_emp = row.GetValueOrDefault("complement_emp"),
                                        bairro_emp = row.GetValueOrDefault("bairro_emp"),
                                        cep_emp = row.GetValueOrDefault("cep_emp"),
                                        cidade_emp = row.GetValueOrDefault("cidade_emp"),
                                        estado_emp = row.GetValueOrDefault("estado_emp"),
                                        fone_emp = row.GetValueOrDefault("fone_emp"),
                                        email_emp = row.GetValueOrDefault("email_emp"),
                                        cod_ibge_municipio = row.GetValueOrDefault("cod_ibge_municipio"),
                                        data_criacao_emp = row.GetValueOrDefault("data_criacao_emp"),
                                        data_criacao_portal = row.GetValueOrDefault("data_criacao_portal"),
                                        sistema_tributacao = row.GetValueOrDefault("sistema_tributacao"),
                                        regime_tributario = row.GetValueOrDefault("regime_tributario"),
                                        area_empresa = row.GetValueOrDefault("area_empresa"),
                                        timestamp = row.GetValueOrDefault("timestamp"),
                                        sigla_empresa = row.GetValueOrDefault("sigla_empresa"),
                                        id_classe_fiscal = row.GetValueOrDefault("id_classe_fiscal"),
                                        centro_distribuicao = row.GetValueOrDefault("centro_distribuicao"),
                                        inscricao_municipal_emp = row.GetValueOrDefault("inscricao_municipal_emp"),
                                        cnae_emp = row.GetValueOrDefault("cnae_emp"),
                                        cod_cliente_linx = row.GetValueOrDefault("cod_cliente_linx"),
                                        tabela_preco_preferencial = row.GetValueOrDefault("tabela_preco_preferencial"),
                                        subrede = row.GetValueOrDefault("subrede"),
                                        produto_disponivel_empresa = row.GetValueOrDefault("produto_disponivel_empresa"),
                                        id_grupo = row.GetValueOrDefault("id_grupo")
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

                        // valida, monta as entidades e grava. Chave: cnpj_emp, a loja consultada (fica a linha de maior timestamp)
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var (dto, xml) = rows.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last();
                        var validations = _linxLojasValidator.Validate(dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"loja {cnpj_emp}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxLojas>
                        {
                            new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxLojas(company.doc_company!, cnpj_emp, batchTime, dto)
                        };

                        await _linxMicrovixStoresRepository.BulkInsertIntoLinxLojas(_listSomenteNovos, _logger.GetExecutionGuid());

                        _logger.AddRecord(cnpj_emp, xml);

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

        public async Task<bool?> LinxLojasParametros()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxLojasParametros);

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
                        var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxLojasParametros, cnpj_emp);
                        var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxLojasParametros dto, string xml)>();
                        var failed = false;

                        for (var page = 1; page <= MaxPages; page++)
                        {
                            var requestBody = new XDocument(
                                new XDeclaration("1.0", "utf-8", null),
                                new XElement("LinxMicrovix",
                                    new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                    new XElement("ResponseFormat", "xml"),
                                    new XElement("Command",
                                        new XElement("Name", "LinxLojasParametros"),
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

                                response = await _apiClient.SendAsync(HttpMethod.Post, "LinxLojasParametros", content);

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
                                _logger.AddMessage($"Erro da API Linx Microvix em LinxLojasParametros (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {response?.StatusCode} {body}");

                                failed = true;
                                break;
                            }

                            XElement? root = null;

                            try
                            {
                                root = XDocument.Parse(response.Content?.TrimStart('﻿') ?? String.Empty).Root;
                            }
                            catch (XmlException)
                            {
                                // root continua null: tratado abaixo como resposta inválida
                            }

                            if (root is null || !String.Equals(root.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                            {
                                var message = root?.Descendants("Message").FirstOrDefault()?.Value ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content);
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxLojasParametros (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {message}");

                                failed = true;
                                break;
                            }

                            var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                            var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                            // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                            if (columns.Count == 0 && pageRows.Count > 0)
                            {
                                _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxLojasParametros (empresa {company.doc_company}, loja {cnpj_emp}, timestamp {timestamp}): {pageRows[0].Value}");

                                failed = true;
                                break;
                            }

                            // resposta vazia: não há mais registros com timestamp maior que o informado
                            if (pageRows.Count == 0)
                                break;

                            foreach (var r in pageRows)
                            {
                                var values = r.Elements("D").Select(d => d.IsEmpty ? null : d.Value).ToList();
                                var row = columns.Select((c, i) => (c, v: i < values.Count ? values[i] : null)).ToDictionary(x => x.c, x => x.v);

                                rows.Add((
                                    new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxLojasParametros
                                    {
                                        portal = row.GetValueOrDefault("portal"),
                                        empresa = row.GetValueOrDefault("empresa"),
                                        percentual_minimo_antecipacao = row.GetValueOrDefault("percentual_minimo_antecipacao"),
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

                        // valida, monta as entidades e grava. A resposta não traz o CNPJ: chave cnpj_emp, a loja consultada
                        var batchTime = _linxMicrovixService.GetBatchTime();

                        var (dto, xml) = rows.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last();
                        var validations = _linxLojasParametrosValidator.Validate(dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"parâmetros da loja {cnpj_emp}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxLojasParametros>
                        {
                            new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxLojasParametros(company.doc_company!, cnpj_emp, batchTime, dto)
                        };

                        await _linxMicrovixStoresRepository.BulkInsertIntoLinxLojasParametros(_listSomenteNovos, _logger.GetExecutionGuid());

                        _logger.AddRecord(cnpj_emp, xml);

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

        public async Task<bool?> LinxUsuarios()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxUsuarios);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    // método de portal (sem cnpjEmp): uma leitura por cliente. Devolve no máximo 100 usuários por chamada (testado)
                    var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxUsuarios, null);
                    var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxUsuarios dto, string xml)>();
                    var failed = false;

                    for (var page = 1; page <= MaxPages; page++)
                    {
                        var requestBody = new XDocument(
                            new XDeclaration("1.0", "utf-8", null),
                            new XElement("LinxMicrovix",
                                new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                new XElement("ResponseFormat", "xml"),
                                new XElement("Command",
                                    new XElement("Name", "LinxUsuarios"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
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

                            response = await _apiClient.SendAsync(HttpMethod.Post, "LinxUsuarios", content);

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
                            _logger.AddMessage($"Erro da API Linx Microvix em LinxUsuarios (empresa {company.doc_company}, timestamp {timestamp}): {response?.StatusCode} {body}");

                            failed = true;
                            break;
                        }

                        XElement? root = null;

                        try
                        {
                            root = XDocument.Parse(response.Content?.TrimStart('﻿') ?? String.Empty).Root;
                        }
                        catch (XmlException)
                        {
                            // root continua null: tratado abaixo como resposta inválida
                        }

                        if (root is null || !String.Equals(root.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            var message = root?.Descendants("Message").FirstOrDefault()?.Value ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content);
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxUsuarios (empresa {company.doc_company}, timestamp {timestamp}): {message}");

                            failed = true;
                            break;
                        }

                        var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                        var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                        // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                        if (columns.Count == 0 && pageRows.Count > 0)
                        {
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxUsuarios (empresa {company.doc_company}, timestamp {timestamp}): {pageRows[0].Value}");

                            failed = true;
                            break;
                        }

                        // resposta vazia: não há mais registros com timestamp maior que o informado
                        if (pageRows.Count == 0)
                            break;

                        foreach (var r in pageRows)
                        {
                            var values = r.Elements("D").Select(d => d.IsEmpty ? null : d.Value).ToList();
                            var row = columns.Select((c, i) => (c, v: i < values.Count ? values[i] : null)).ToDictionary(x => x.c, x => x.v);

                            rows.Add((
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxUsuarios
                                {
                                    usuario_id = row.GetValueOrDefault("usuario_id"),
                                    usuario_login = row.GetValueOrDefault("usuario_login"),
                                    usuario_nome = row.GetValueOrDefault("usuario_nome"),
                                    usuario_email = row.GetValueOrDefault("usuario_email"),
                                    portal = row.GetValueOrDefault("portal"),
                                    usuario_grupo_id = row.GetValueOrDefault("usuario_grupo_id"),
                                    grupo_usuarios = row.GetValueOrDefault("grupo_usuarios"),
                                    usuario_supervisor = row.GetValueOrDefault("usuario_supervisor"),
                                    usuario_doc = row.GetValueOrDefault("usuario_doc"),
                                    vendedor = row.GetValueOrDefault("vendedor"),
                                    data_criacao = row.GetValueOrDefault("data_criacao"),
                                    desativado = row.GetValueOrDefault("desativado"),
                                    empresas = row.GetValueOrDefault("empresas"),
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

                    // valida, monta as entidades e grava. Chave: portal + usuario_id (fica a linha de maior timestamp)
                    var batchTime = _linxMicrovixService.GetBatchTime();

                    var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxUsuarios>();
                    var records = new Dictionary<string, string>();

                    foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.usuario_id)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                    {
                        var validations = _linxUsuariosValidator.Validate(dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"usuário {dto.usuario_id}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxUsuarios(company.doc_company!, batchTime, dto);

                        if (entity.portal == 0 || entity.usuario_id == 0)
                            continue;

                        _listSomenteNovos.Add(entity);
                        records[$"{entity.portal}|{entity.usuario_id}"] = xml;
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _linxMicrovixStoresRepository.BulkInsertIntoLinxUsuarios(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in records)
                        _logger.AddRecord(record.Key, record.Value);

                    totalInseridos += _listSomenteNovos.Count;
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

        public async Task<bool?> LinxMetodos()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxMetodos);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    // sem parâmetro nenhum, nem a chave: com chave ou cnpjEmp a API responde "O parametro é inválido." (testado).
                    // O catálogo é o mesmo para qualquer chave e vem inteiro numa chamada
                    var requestBody = new XDocument(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("LinxMicrovix",
                            new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                            new XElement("ResponseFormat", "xml"),
                            new XElement("Command", new XElement("Name", "LinxMetodos"), new XElement("Parameters"))
                        )
                    );

                    // uma nova tentativa: 502/503/504 espera 10 s. Os erros da API chegam com HTTP 200 (testado)
                    ApiResponse? response = null;

                    for (var attempt = 1; attempt <= 2; attempt++)
                    {
                        using var content = new StringContent($"{requestBody.Declaration}{requestBody}", Encoding.UTF8, "application/xml");

                        response = await _apiClient.SendAsync(HttpMethod.Post, "LinxMetodos", content);

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
                        _logger.AddMessage($"Erro da API Linx Microvix em LinxMetodos (empresa {company.doc_company}): {response?.StatusCode} {body}");
                        continue;
                    }

                    XElement? root = null;

                    try
                    {
                        root = XDocument.Parse(response.Content?.TrimStart('﻿') ?? String.Empty).Root;
                    }
                    catch (XmlException)
                    {
                        // root continua null: tratado abaixo como resposta inválida
                    }

                    if (root is null || !String.Equals(root.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                    {
                        var message = root?.Descendants("Message").FirstOrDefault()?.Value ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content);
                        _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxMetodos (empresa {company.doc_company}): {message}");
                        continue;
                    }

                    // a resposta não tem <C>: cada <R> tem um <D> só com "Nome - descrição (parâmetros)"
                    var rows = (root.Element("ResponseData")?.Elements("R") ?? [])
                        .Select(r => (
                            dto: new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxMetodos { retorno = r.Element("D")?.Value },
                            xml: new XElement("R", new XElement("retorno", r.Element("D")?.Value)).ToString(SaveOptions.DisableFormatting)
                        ))
                        .ToList();

                    if (rows.Count == 0)
                        continue;

                    // valida, monta as entidades e grava. Chave: metodo
                    var batchTime = _linxMicrovixService.GetBatchTime();

                    var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxMetodos>();
                    var records = new Dictionary<string, string>();

                    foreach (var (dto, xml) in rows)
                    {
                        var validations = _linxMetodosValidator.Validate(dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"método {dto.retorno}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxMetodos(company.doc_company!, batchTime, dto);

                        if (String.IsNullOrEmpty(entity.metodo))
                            continue;

                        // o mesmo método duas vezes no catálogo violaria a PK: fica o último
                        _listSomenteNovos.RemoveAll(x => x.metodo == entity.metodo);
                        _listSomenteNovos.Add(entity);
                        records[entity.metodo] = xml;
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _linxMicrovixStoresRepository.BulkInsertIntoLinxMetodos(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in records)
                        _logger.AddRecord(record.Key, record.Value);

                    totalInseridos += _listSomenteNovos.Count;
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

        public async Task<bool?> LinxEntrantes()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxEntrantes);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxMicrovixService.GetCompanys())
                {
                    if (company.erp_chave is null)
                        continue;   // cliente sem o WS de saída padrão

                    // método de portal (sem cnpjEmp): uma leitura por cliente; a API só devolve os últimos 2 anos (v274)
                    var timestamp = await _linxMicrovixService.GetLastTimestamp(company, EnumJob.LinxEntrantes, null);
                    var rows = new List<(Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxEntrantes dto, string xml)>();
                    var failed = false;

                    for (var page = 1; page <= MaxPages; page++)
                    {
                        var requestBody = new XDocument(
                            new XDeclaration("1.0", "utf-8", null),
                            new XElement("LinxMicrovix",
                                new XElement("Authentication", new XAttribute("user", company.erp_user_name ?? String.Empty), new XAttribute("password", company.erp_password ?? String.Empty)),
                                new XElement("ResponseFormat", "xml"),
                                new XElement("Command",
                                    new XElement("Name", "LinxEntrantes"),
                                    new XElement("Parameters",
                                        new XElement("Parameter", new XAttribute("id", "chave"), company.erp_chave),
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

                            response = await _apiClient.SendAsync(HttpMethod.Post, "LinxEntrantes", content);

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
                            _logger.AddMessage($"Erro da API Linx Microvix em LinxEntrantes (empresa {company.doc_company}, timestamp {timestamp}): {response?.StatusCode} {body}");

                            failed = true;
                            break;
                        }

                        XElement? root = null;

                        try
                        {
                            root = XDocument.Parse(response.Content?.TrimStart('﻿') ?? String.Empty).Root;
                        }
                        catch (XmlException)
                        {
                            // root continua null: tratado abaixo como resposta inválida
                        }

                        if (root is null || !String.Equals(root.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            var message = root?.Descendants("Message").FirstOrDefault()?.Value ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content);
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxEntrantes (empresa {company.doc_company}, timestamp {timestamp}): {message}");

                            failed = true;
                            break;
                        }

                        var columns = root.Element("ResponseData")?.Element("C")?.Elements("D").Select(d => d.Value).ToList() ?? [];
                        var pageRows = root.Element("ResponseData")?.Elements("R").ToList() ?? [];

                        // erro de parâmetro também vem com ResponseSuccess = True: <C /> vazio e a mensagem numa linha (testado no LinxFaturas)
                        if (columns.Count == 0 && pageRows.Count > 0)
                        {
                            _logger.AddMessage($"Resposta de erro da API Linx Microvix em LinxEntrantes (empresa {company.doc_company}, timestamp {timestamp}): {pageRows[0].Value}");

                            failed = true;
                            break;
                        }

                        // resposta vazia: não há mais registros com timestamp maior que o informado
                        if (pageRows.Count == 0)
                            break;

                        foreach (var r in pageRows)
                        {
                            var values = r.Elements("D").Select(d => d.IsEmpty ? null : d.Value).ToList();
                            var row = columns.Select((c, i) => (c, v: i < values.Count ? values[i] : null)).ToDictionary(x => x.c, x => x.v);

                            rows.Add((
                                new Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores.LinxEntrantes
                                {
                                    id_entrantes = row.GetValueOrDefault("id_entrantes"),
                                    data_inicio = row.GetValueOrDefault("data_inicio"),
                                    data_fim = row.GetValueOrDefault("data_fim"),
                                    cnpj_empresa = row.GetValueOrDefault("cnpj_empresa"),
                                    entrantes = row.GetValueOrDefault("entrantes"),
                                    portal = row.GetValueOrDefault("portal"),
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

                    // valida, monta as entidades e grava. Chave: portal + id_entrantes (fica a linha de maior timestamp)
                    var batchTime = _linxMicrovixService.GetBatchTime();

                    var _listSomenteNovos = new List<Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxEntrantes>();
                    var records = new Dictionary<string, string>();

                    foreach (var (dto, xml) in rows.GroupBy(x => (x.dto.portal, x.dto.id_entrantes)).Select(g => g.OrderBy(x => MicrovixConverters.ToInt64(x.dto.timestamp)).Last()))
                    {
                        var validations = _linxEntrantesValidator.Validate(dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"entrantes {dto.id_entrantes}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        var entity = new Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores.LinxEntrantes(company.doc_company!, batchTime, dto);

                        if (entity.portal == 0 || entity.id_entrantes == 0)
                            continue;

                        _listSomenteNovos.Add(entity);
                        records[$"{entity.portal}|{entity.id_entrantes}"] = xml;
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _linxMicrovixStoresRepository.BulkInsertIntoLinxEntrantes(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in records)
                        _logger.AddRecord(record.Key, record.Value);

                    totalInseridos += _listSomenteNovos.Count;
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

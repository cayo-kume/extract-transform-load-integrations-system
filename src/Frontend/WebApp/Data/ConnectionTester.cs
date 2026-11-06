using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Dapper;
using WebApp.Data.Mocks;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Os valores gravados da linha do módulo da empresa, segredos incluídos, só para o teste de conexão
    /// (.docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.6.1). Nada daqui volta para a tela: a leitura do formulário continua no
    /// IModuleParametersRepository, que devolve só se o segredo está gravado.
    /// </summary>
    public interface IModuleCredentialReader
    {
        Task<IReadOnlyDictionary<string, string?>?> GetValuesAsync(int idParent, string company, CancellationToken cancellationToken = default);
    }

    public class SqlModuleCredentialReader(WebApp.Data.Sql.MainDatabase database) : IModuleCredentialReader
    {
        public async Task<IReadOnlyDictionary<string, string?>?> GetValuesAsync(int idParent, string company, CancellationToken cancellationToken = default)
        {
            var definition = ModuleDefinitions.Get(idParent);
            await using var connection = await database.OpenAsync(cancellationToken);

            var row = (IDictionary<string, object?>?)await connection.QueryFirstOrDefaultAsync(new CommandDefinition(
                $"SELECT {string.Join(", ", definition.Fields.Select(x => $"[{x.Column}]"))} FROM {definition.Table} WHERE doc_company = @company",
                new { company = ModuleDefinitions.Digits(company) }, cancellationToken: cancellationToken));

            return row?.ToDictionary(x => x.Key, x => x.Value is null or DBNull ? null : x.Value.ToString());
        }
    }

    public class MockModuleCredentialReader(MockCompanyStore store) : IModuleCredentialReader
    {
        public Task<IReadOnlyDictionary<string, string?>?> GetValuesAsync(int idParent, string company, CancellationToken cancellationToken = default)
        {
            lock (store.Gate)
            {
                return Task.FromResult(store.Parameters.TryGetValue((idParent, ModuleDefinitions.Digits(company)), out var row)
                    ? (IReadOnlyDictionary<string, string?>?)new Dictionary<string, string?>(row.Values)
                    : null);
            }
        }
    }

    /// <summary>
    /// Resultado do teste numa URL: Base URL ou Homolog URL.
    /// </summary>
    public sealed record ConnectionTestResult(string Label, string Url, bool Success, string Message);

    /// <summary>
    /// Testar conexão da aba Credenciais (.docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.6.1): com a credencial gravada, só a chamada
    /// mínima de leitura em cada URL gravada da empresa (Base URL e, se houver, Homolog URL). Pedido de token na Totvs; GET do
    /// usuário no AfterSale; um serviço no Movidesk; LinxGrupoLojas (ou B2CConsultaEmpresas) no LinxMicrovix. Nunca escreve no
    /// provedor. Tempo limite de 10 s; os segredos não vão para o log nem para a mensagem.
    /// </summary>
    public class ConnectionTester(IHttpClientFactory httpClientFactory, IModuleCredentialReader credentialReader, ILogger<ConnectionTester> logger)
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

        public static bool Supports(int idParent) => idParent is 30 or 50 or 80 or 130;

        public async Task<IReadOnlyList<ConnectionTestResult>> TestAsync(int idParent, string company, CancellationToken cancellationToken = default)
        {
            var definition = ModuleDefinitions.Get(idParent);
            var values = await credentialReader.GetValuesAsync(idParent, company, cancellationToken);

            if (values is null)
                return [new ConnectionTestResult("Produção", "", false, "Credencial não gravada.")];

            var secrets = definition.Fields.Where(x => x.Secret).Select(x => values.GetValueOrDefault(x.Column)).Where(x => !string.IsNullOrEmpty(x)).Cast<string>().ToList();
            var results = new List<ConnectionTestResult>();
            var usesHomolog = ModuleDefinitions.UsesHomolog(values);

            // cada ambiente com a própria URL e credencial (a de homologação nas colunas hml_); o que os jobs usam vem primeiro
            var environments = new[]
            {
                (homolog: false, label: "Produção", url: values.GetValueOrDefault(ModuleDefinitions.BaseUrl)),
                (homolog: true, label: "Homologação", url: values.GetValueOrDefault(ModuleDefinitions.HomologUrl))
            }.OrderBy(x => x.homolog != usesHomolog);

            foreach (var (homolog, name, url) in environments)
            {
                if (string.IsNullOrWhiteSpace(url))
                    continue;

                var label = homolog == usesHomolog ? $"{name} (em uso)" : name;
                var credential = homolog
                    ? definition.Fields.Where(x => !ModuleDefinitions.IsHomolog(x)).ToDictionary(x => x.Column, x => values.GetValueOrDefault($"hml_{x.Column}") ?? (x.Column == "grupo" ? values.GetValueOrDefault(x.Column) : null))
                    : values.ToDictionary(x => x.Key, x => x.Value);

                if (!Complete(definition, credential))
                {
                    results.Add(new ConnectionTestResult(label, url, false, "credencial incompleta neste ambiente"));
                    continue;
                }

                var started = Stopwatch.GetTimestamp();

                try
                {
                    using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    timeout.CancelAfter(Timeout);

                    var error = await CallAsync(idParent, url.TrimEnd('/'), credential, timeout.Token);
                    var elapsed = $"{Stopwatch.GetElapsedTime(started).TotalMilliseconds:0} ms";

                    results.Add(error is null
                        ? new ConnectionTestResult(label, url, true, $"conectado em {elapsed}")
                        : new ConnectionTestResult(label, url, false, Hide(error, secrets)));
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    results.Add(new ConnectionTestResult(label, url, false, "sem resposta em 10 s"));
                }
                catch (Exception exception)
                {
                    logger.LogWarning("Testar conexão do módulo {IdParent} em {Url}: {Error}", idParent, url, Hide(exception.Message, secrets));
                    results.Add(new ConnectionTestResult(label, url, false, Hide(exception.Message, secrets)));
                }
            }

            return results;
        }

        // devolve null com sucesso ou a mensagem do provedor
        private async Task<string?> CallAsync(int idParent, string url, IReadOnlyDictionary<string, string?> values, CancellationToken cancellationToken)
        {
            var client = httpClientFactory.CreateClient();
            string Value(string column) => values.GetValueOrDefault(column) ?? "";

            switch (idParent)
            {
                case 130:
                {
                    // o mesmo pedido de token do TotvsService.GetToken
                    using var content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "grant_type", "password" },
                        { "client_id", Value("client_id") },
                        { "client_secret", Value("client_secret") },
                        { "username", Value("username") },
                        { "password", Value("password") }
                    });
                    using var response = await client.PostAsync($"{url}/authorization/v2/token", content, cancellationToken);
                    var body = await response.Content.ReadAsStringAsync(cancellationToken);

                    return response.IsSuccessStatusCode && body.Contains("access_token") ? null : Error(response, body);
                }
                case 50:
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, $"{url}/v3/api/me");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Value("token"));
                    using var response = await client.SendAsync(request, cancellationToken);

                    return response.IsSuccessStatusCode ? null : Error(response, await response.Content.ReadAsStringAsync(cancellationToken));
                }
                case 30:
                {
                    using var response = await client.GetAsync($"{url}/services?token={Uri.EscapeDataString(Value("token"))}&$select=id&$top=1", cancellationToken);

                    return response.IsSuccessStatusCode ? null : Error(response, await response.Content.ReadAsStringAsync(cancellationToken));
                }
                case 80:
                {
                    // ERP quando houver a chave do WS de saída; senão o B2C. Os erros da API chegam com HTTP 200 (ResponseSuccess)
                    var erp = !string.IsNullOrEmpty(Value("erp_chave"));
                    var command = erp ? "LinxGrupoLojas" : "B2CConsultaEmpresas";
                    var parameters = new XElement("Parameters", new XElement("Parameter", new XAttribute("id", "chave"), Value(erp ? "erp_chave" : "b2c_chave")));

                    if (erp && !string.IsNullOrWhiteSpace(Value("grupo")))
                        parameters.Add(new XElement("Parameter", new XAttribute("id", "grupo"), Value("grupo")));

                    var document = new XDocument(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("LinxMicrovix",
                            new XElement("Authentication", new XAttribute("user", Value(erp ? "erp_user_name" : "b2c_user_name")), new XAttribute("password", Value(erp ? "erp_password" : "b2c_password"))),
                            new XElement("ResponseFormat", "xml"),
                            new XElement("Command", new XElement("Name", command), parameters)));

                    using var content = new StringContent($"{document.Declaration}{document}", Encoding.UTF8, "application/xml");
                    using var response = await client.PostAsync($"{url}/{command}", content, cancellationToken);
                    var body = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (!response.IsSuccessStatusCode)
                        return Error(response, body);

                    try
                    {
                        var root = XDocument.Parse(body.TrimStart('﻿')).Root;
                        return string.Equals(root?.Element("ResponseResult")?.Element("ResponseSuccess")?.Value, "true", StringComparison.OrdinalIgnoreCase)
                            ? null
                            : root?.Descendants("Message").FirstOrDefault()?.Value ?? "resposta sem sucesso";
                    }
                    catch (XmlException)
                    {
                        return "resposta que não é XML";
                    }
                }
                default:
                    return "módulo sem teste de conexão";
            }
        }

        // os obrigatórios de produção preenchidos e, no LinxMicrovix, a chave do ERP ou a do B2C
        private static bool Complete(ModuleDefinition definition, IReadOnlyDictionary<string, string?> credential) =>
            definition.Fields.Where(x => x.Required && !ModuleDefinitions.IsApiUrl(x)).All(x => !string.IsNullOrWhiteSpace(credential.GetValueOrDefault(x.Column)))
            && (definition.IdParent != 80 || !string.IsNullOrWhiteSpace(credential.GetValueOrDefault("erp_chave")) || !string.IsNullOrWhiteSpace(credential.GetValueOrDefault("b2c_chave")));

        private static string Error(HttpResponseMessage response, string body)
        {
            body = body.Trim();
            return $"{(int)response.StatusCode}{(body.Length == 0 ? "" : $": {(body.Length > 200 ? body[..200] : body)}")}";
        }

        // um segredo que o provedor devolva na mensagem não chega à tela
        private static string Hide(string message, IEnumerable<string> secrets) =>
            secrets.Aggregate(message, (text, secret) => text.Replace(secret, "••••", StringComparison.Ordinal));
    }
}

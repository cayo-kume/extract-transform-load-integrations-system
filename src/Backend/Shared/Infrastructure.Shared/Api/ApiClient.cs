using Domain.Shared.Abstractions.Api;
using Domain.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace Infrastructure.Shared.Api
{
    public class ApiClient : IApiClient
    {
        // erro de rede na leitura da resposta: uma nova tentativa, depois desta espera (docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 2.2)
        private static readonly TimeSpan NetworkRetryDelay = TimeSpan.FromSeconds(10);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ApiClient> _logger;
        private readonly string _clientName;

        // ApiEnvironment do host: Homologation usa a homolog_url das empresas (UseBaseAddress); sem a chave, Production
        private readonly bool _homologation;

        // endereço por empresa (UseBaseAddress), por nome de cliente: no AsyncLocal, cada job vê só o que ele mesmo informou
        private static readonly AsyncLocal<ImmutableDictionary<string, string>?> _baseAddresses = new();

        public ApiClient(IHttpClientFactory httpClientFactory, ILogger<ApiClient> logger, string clientName, IConfiguration? configuration = null)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _clientName = clientName;
            _homologation = string.Equals(configuration?["ApiEnvironment"], "Homologation", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string?> GetAsync(string route, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(BaseAddress(client), route, queryParams);

            _logger.LogDebug("[ApiClient] GET {Url}", url);

            var (response, content) = await SendWithRetry(client, () => new HttpRequestMessage(HttpMethod.Get, url), route, retryOnNetworkError: true);

            using (response)
                _logger.LogDebug("[ApiClient] GET {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> PostAsync(string route, object? body = null, Dictionary<string, string>? headers = null, string? contentType = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(BaseAddress(client), route);
            var json = body != null ? Newtonsoft.Json.JsonConvert.SerializeObject(body) : string.Empty;
            var httpContent = new StringContent(json, Encoding.UTF8, contentType ?? "application/json");

            _logger.LogDebug("[ApiClient] POST {Url} Body: {Body}", url, json);

            var started = Stopwatch.GetTimestamp();
            var response = await client.PostAsync(url, httpContent);
            JobProgress.ApiCall("POST", route, (int)response.StatusCode, Stopwatch.GetElapsedTime(started));
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] POST {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> PutAsync(string route, object? body = null, Dictionary<string, string>? headers = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(BaseAddress(client), route);
            var json = body != null ? Newtonsoft.Json.JsonConvert.SerializeObject(body) : string.Empty;
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("[ApiClient] PUT {Url} Body: {Body}", url, json);

            var started = Stopwatch.GetTimestamp();
            var response = await client.PutAsync(url, httpContent);
            JobProgress.ApiCall("PUT", route, (int)response.StatusCode, Stopwatch.GetElapsedTime(started));
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] PUT {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> DeleteAsync(string route, object? body = null, Dictionary<string, string>? headers = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(BaseAddress(client), route);

            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            if (body != null)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            _logger.LogDebug("[ApiClient] DELETE {Url}", url);

            var started = Stopwatch.GetTimestamp();
            var response = await client.SendAsync(request);
            JobProgress.ApiCall("DELETE", route, (int)response.StatusCode, Stopwatch.GetElapsedTime(started));
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] DELETE {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> PostFormAsync(string route, Dictionary<string, string> formData, Dictionary<string, string>? headers = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(BaseAddress(client), route);
            var httpContent = new FormUrlEncodedContent(formData);

            _logger.LogDebug("[ApiClient] POST-FORM {Url}", url);

            var started = Stopwatch.GetTimestamp();
            var response = await client.PostAsync(url, httpContent);
            JobProgress.ApiCall("POST", route, (int)response.StatusCode, Stopwatch.GetElapsedTime(started));
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] POST-FORM {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> PostMultipartAsync(string route, Dictionary<string, string> fields, string fileField, byte[] fileContent, string fileName, string fileContentType, Dictionary<string, string>? headers = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(BaseAddress(client), route);

            using var httpContent = new MultipartFormDataContent();

            foreach (var field in fields)
                httpContent.Add(new StringContent(field.Value), field.Key);

            var file = new ByteArrayContent(fileContent);
            file.Headers.ContentType = new MediaTypeHeaderValue(fileContentType);
            httpContent.Add(file, fileField, fileName);

            _logger.LogDebug("[ApiClient] POST-MULTIPART {Url} File: {FileName}", url, fileName);

            var started = Stopwatch.GetTimestamp();
            var response = await client.PostAsync(url, httpContent);
            JobProgress.ApiCall("POST", route, (int)response.StatusCode, Stopwatch.GetElapsedTime(started));
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] POST-MULTIPART {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<ApiResponse> SendAsync(HttpMethod method, string route, HttpContent? content = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, bool retryOnNetworkError = false)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(BaseAddress(client), route, queryParams);

            _logger.LogDebug("[ApiClient] {Method} {Route}", method, route);

            // uma HttpRequestMessage só pode ser enviada uma vez: cada tentativa monta a sua, com o mesmo conteúdo
            var (sent, body) = await SendWithRetry(client, () => new HttpRequestMessage(method, url) { Content = content }, route,
                retryOnNetworkError: retryOnNetworkError || method == HttpMethod.Get || method == HttpMethod.Head);

            using var response = sent;

            // cabeçalhos da resposta e do conteúdo juntos (retry-after vem na resposta, content-type no conteúdo)
            var responseHeaders = response.Headers
                .Concat(response.Content.Headers)
                .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => String.Join(",", g.SelectMany(x => x.Value)), StringComparer.OrdinalIgnoreCase);

            // a rota, não a url: a query string pode levar o token
            _logger.LogDebug("[ApiClient] {Method} {Route} → {StatusCode}", method, route, response.StatusCode);

            return new ApiResponse((int)response.StatusCode, body, responseHeaders);
        }

        /// <summary>
        /// envia e lê o corpo. O HttpClient lê o corpo inteiro dentro do SendAsync, então a resposta cortada no meio
        /// (HttpIOException "The response ended prematurely", Totvs no teste C1) sai daqui como exceção. Com retryOnNetworkError, esse
        /// erro repete uma vez depois de NetworkRetryDelay. Tempo esgotado (TaskCanceledException) não repete: a origem está lenta,
        /// e repetir dobraria o tempo; recusa de conexão também não (a API está fora, não cortou a resposta)
        /// </summary>
        private async Task<(HttpResponseMessage Response, string Body)> SendWithRetry(HttpClient client, Func<HttpRequestMessage> request, string route, bool retryOnNetworkError)
        {
            for (var attempt = 1; ; attempt++)
            {
                // sem using: o Dispose da mensagem descartaria o conteúdo, que a nova tentativa envia de novo (quem monta o
                // conteúdo o descarta)
                var message = request();
                var started = Stopwatch.GetTimestamp();

                try
                {
                    var response = await client.SendAsync(message);
                    JobProgress.ApiCall(message.Method.Method, route, (int)response.StatusCode, Stopwatch.GetElapsedTime(started));

                    try
                    {
                        return (response, await response.Content.ReadAsStringAsync());
                    }
                    catch
                    {
                        response.Dispose();
                        throw;
                    }
                }
                catch (Exception ex) when (retryOnNetworkError && attempt == 1 && IsResponseCut(ex))
                {
                    // a rota, não a url: a query string pode levar o token
                    _logger.LogWarning(ex, "[ApiClient] {Method} {Route}: a resposta foi interrompida ({Message}); nova tentativa em {Delay} s.",
                        message.Method, route, ex.Message, NetworkRetryDelay.TotalSeconds);

                    await Task.Delay(NetworkRetryDelay);
                }
            }
        }

        /// <summary>
        /// a conexão caiu ou a resposta acabou antes do fim, depois de a requisição chegar à origem
        /// </summary>
        private static bool IsResponseCut(Exception? ex) =>
            ex switch
            {
                null => false,
                OperationCanceledException => false,
                HttpIOException => true,
                HttpRequestException { HttpRequestError: HttpRequestError.ResponseEnded } => true,
                HttpRequestException { HttpRequestError: HttpRequestError.ConnectionError or HttpRequestError.NameResolutionError or HttpRequestError.SecureConnectionError } => false,
                HttpRequestException { StatusCode: not null } => false,
                IOException => true,
                _ => IsResponseCut(ex.InnerException)
            };

        public void UseBaseAddress(string? baseUrl, string? homologUrl)
        {
            if (_homologation)
                baseUrl = homologUrl;

            var current = _baseAddresses.Value ?? ImmutableDictionary<string, string>.Empty;

            _baseAddresses.Value = string.IsNullOrWhiteSpace(baseUrl)
                ? current.Remove(_clientName)
                : current.SetItem(_clientName, baseUrl.Trim());
        }

        private HttpClient CreateClient(Dictionary<string, string>? headers)
        {
            var client = _httpClientFactory.CreateClient(_clientName);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = header.Value.Split(' ', 2);
                        if (parts.Length == 2)
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(parts[0], parts[1]);
                        else
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", header.Value);
                    }
                    else
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            return client;
        }

        private string? BaseAddress(HttpClient client) =>
            _baseAddresses.Value?.GetValueOrDefault(_clientName) ?? client.BaseAddress?.ToString();

        private static string BuildUrl(string? baseAddress, string route, Dictionary<string, string>? queryParams = null)
        {
            var url = route.StartsWith("http", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(baseAddress)
                ? route
                : $"{baseAddress.TrimEnd('/')}/{route.TrimStart('/')}";

            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                url = url.Contains('?') ? $"{url}&{queryString}" : $"{url}?{queryString}";
            }

            return url;
        }
    }
}

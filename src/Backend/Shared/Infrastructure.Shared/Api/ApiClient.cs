using Domain.Shared.Abstractions.Api;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace Infrastructure.Shared.Api
{
    public class ApiClient : IApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ApiClient> _logger;
        private readonly string _clientName;

        public ApiClient(IHttpClientFactory httpClientFactory, ILogger<ApiClient> logger, string clientName)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _clientName = clientName;
        }

        public async Task<string?> GetAsync(string route, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(client.BaseAddress?.ToString(), route, queryParams);

            _logger.LogDebug("[ApiClient] GET {Url}", url);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] GET {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> PostAsync(string route, object? body = null, Dictionary<string, string>? headers = null, string? contentType = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(client.BaseAddress?.ToString(), route);
            var json = body != null ? Newtonsoft.Json.JsonConvert.SerializeObject(body) : string.Empty;
            var httpContent = new StringContent(json, Encoding.UTF8, contentType ?? "application/json");

            _logger.LogDebug("[ApiClient] POST {Url} Body: {Body}", url, json);

            var response = await client.PostAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] POST {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> PutAsync(string route, object? body = null, Dictionary<string, string>? headers = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(client.BaseAddress?.ToString(), route);
            var json = body != null ? Newtonsoft.Json.JsonConvert.SerializeObject(body) : string.Empty;
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("[ApiClient] PUT {Url} Body: {Body}", url, json);

            var response = await client.PutAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] PUT {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> DeleteAsync(string route, object? body = null, Dictionary<string, string>? headers = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(client.BaseAddress?.ToString(), route);

            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            if (body != null)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            _logger.LogDebug("[ApiClient] DELETE {Url}", url);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] DELETE {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> PostFormAsync(string route, Dictionary<string, string> formData, Dictionary<string, string>? headers = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(client.BaseAddress?.ToString(), route);
            var httpContent = new FormUrlEncodedContent(formData);

            _logger.LogDebug("[ApiClient] POST-FORM {Url}", url);

            var response = await client.PostAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] POST-FORM {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<string?> PostMultipartAsync(string route, Dictionary<string, string> fields, string fileField, byte[] fileContent, string fileName, string fileContentType, Dictionary<string, string>? headers = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(client.BaseAddress?.ToString(), route);

            using var httpContent = new MultipartFormDataContent();

            foreach (var field in fields)
                httpContent.Add(new StringContent(field.Value), field.Key);

            var file = new ByteArrayContent(fileContent);
            file.Headers.ContentType = new MediaTypeHeaderValue(fileContentType);
            httpContent.Add(file, fileField, fileName);

            _logger.LogDebug("[ApiClient] POST-MULTIPART {Url} File: {FileName}", url, fileName);

            var response = await client.PostAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[ApiClient] POST-MULTIPART {Url} → {StatusCode}", url, response.StatusCode);

            return content;
        }

        public async Task<ApiResponse> SendAsync(HttpMethod method, string route, HttpContent? content = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null)
        {
            var client = CreateClient(headers);

            var url = BuildUrl(client.BaseAddress?.ToString(), route, queryParams);

            using var request = new HttpRequestMessage(method, url) { Content = content };

            _logger.LogDebug("[ApiClient] {Method} {Route}", method, route);

            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // cabeçalhos da resposta e do conteúdo juntos (retry-after vem na resposta, content-type no conteúdo)
            var responseHeaders = response.Headers
                .Concat(response.Content.Headers)
                .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => String.Join(",", g.SelectMany(x => x.Value)), StringComparer.OrdinalIgnoreCase);

            // a rota, não a url: a query string pode levar o token
            _logger.LogDebug("[ApiClient] {Method} {Route} → {StatusCode}", method, route, response.StatusCode);

            return new ApiResponse((int)response.StatusCode, body, responseHeaders);
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

using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WebJob.Recovery
{
    public interface IAzureWebJobsClient
    {
        /// <summary>
        /// WebJobs contínuos do App Service da seção Recovery, com o status (Running, Stopped, Initializing...)
        /// </summary>
        public Task<IReadOnlyList<AzureWebJob>> GetContinuousWebJobs(CancellationToken cancellationToken = default);

        /// <summary>
        /// para o WebJob, espera o Stopped e inicia de novo; devolve o status final
        /// </summary>
        public Task<string> Restart(string webJobName, CancellationToken cancellationToken = default);
    }

    public record AzureWebJob(string Name, string Status)
    {
        public bool IsRunning => Status.Equals("Running", StringComparison.OrdinalIgnoreCase);
        public bool IsStopped => Status.Equals("Stopped", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// API do Azure Resource Manager para os WebJobs contínuos (como o AzureWebJobClient do sistema antigo), autenticada pelo
    /// DefaultAzureCredential com o TenantId da seção Recovery: na Azure, a identidade gerenciada do App Service; no local, o az
    /// login (PLANO-HOST-WEBJOB.md §7.3). A identidade precisa de permissão para listar, parar e iniciar WebJobs no App Service
    /// </summary>
    public class AzureWebJobsClient : IAzureWebJobsClient
    {
        private const string ApiVersion = "2023-12-01";
        private static readonly string[] Scope = ["https://management.azure.com/.default"];
        private static readonly TimeSpan StatusTimeout = TimeSpan.FromMinutes(2);

        private readonly HttpClient _httpClient;
        private readonly RecoverySettings _settings;
        private readonly TokenCredential _credential;

        public AzureWebJobsClient(HttpClient httpClient, IOptions<RecoverySettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            var options = new DefaultAzureCredentialOptions();

            // sem o tenant explícito, o token pode sair do tenant padrão da credencial local e ser recusado
            if (!string.IsNullOrWhiteSpace(_settings.TenantId))
                options.TenantId = _settings.TenantId;

            _credential = new DefaultAzureCredential(options);
        }

        private string SiteUrl =>
            $"https://management.azure.com/subscriptions/{_settings.SubscriptionId}/resourceGroups/{_settings.ResourceGroup}" +
            $"/providers/Microsoft.Web/sites/{_settings.AppServiceName}/continuouswebjobs";

        public async Task<IReadOnlyList<AzureWebJob>> GetContinuousWebJobs(CancellationToken cancellationToken = default)
        {
            using var document = await Send(HttpMethod.Get, $"{SiteUrl}?api-version={ApiVersion}", cancellationToken);

            // a Azure devolve o nome como "<app service>/<webjob>"
            return document!.RootElement.GetProperty("value").EnumerateArray()
                .Select(item => new AzureWebJob(
                    item.GetProperty("name").GetString()!.Split('/', 2)[^1],
                    item.TryGetProperty("properties", out var properties) && properties.TryGetProperty("status", out var status)
                        ? status.GetString() ?? string.Empty
                        : string.Empty))
                .ToList();
        }

        public async Task<string> Restart(string webJobName, CancellationToken cancellationToken = default)
        {
            var url = $"{SiteUrl}/{Uri.EscapeDataString(webJobName)}";

            (await Send(HttpMethod.Post, $"{url}/stop?api-version={ApiVersion}", cancellationToken))?.Dispose();
            await WaitFor(webJobName, "Stopped", cancellationToken);

            (await Send(HttpMethod.Post, $"{url}/start?api-version={ApiVersion}", cancellationToken))?.Dispose();
            return await WaitFor(webJobName, "Running", cancellationToken);
        }

        private async Task<string> WaitFor(string webJobName, string expected, CancellationToken cancellationToken)
        {
            var limit = DateTime.UtcNow + StatusTimeout;
            var status = string.Empty;

            while (DateTime.UtcNow < limit)
            {
                status = (await GetContinuousWebJobs(cancellationToken)).FirstOrDefault(w => w.Name == webJobName)?.Status ?? string.Empty;

                if (status.Equals(expected, StringComparison.OrdinalIgnoreCase))
                    break;

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }

            return status;
        }

        private async Task<JsonDocument?> Send(HttpMethod method, string url, CancellationToken cancellationToken)
        {
            var token = await _credential.GetTokenAsync(new TokenRequestContext(Scope, tenantId: _settings.TenantId), cancellationToken);

            using var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Azure Resource Manager devolveu {(int)response.StatusCode} em {method} {url.Split('?')[0]}: {content}");

            return string.IsNullOrWhiteSpace(content) ? null : JsonDocument.Parse(content);
        }
    }
}

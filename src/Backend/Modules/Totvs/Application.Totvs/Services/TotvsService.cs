using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Dtos.Reads.Authorization;
using Domain.Totvs.Entities.Parameters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Application.Totvs.Services
{
    /// <summary>
    /// métodos em comum usados pelos services do módulo. O ILoggerService é Scoped: dentro de uma requisição
    /// é a mesma instância do service que chamou, então as mensagens entram na execução aberta por ele.
    /// </summary>
    public class TotvsService : ITotvsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ITotvsRepository _totvsRepository;
        private readonly IConfiguration _configuration;

        private const string ParametersKey = "Totvs:Parameters";
        private const string TokenKey = "Totvs:Token";

        public TotvsService([FromKeyedServices("TotvsAPI")] IApiClient apiClient, ILoggerService logger, IMemoryCache cache, ITotvsRepository totvsRepository, IConfiguration configuration) =>
            (_apiClient, _logger, _cache, _totvsRepository, _configuration) = (apiClient, logger, cache, totvsRepository, configuration);

        public DateTime GetBatchTime() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        public async Task<IEnumerable<Company>> GetCompanys()
        {
            if (_cache.TryGetValue(ParametersKey, out List<Company>? cached) && cached is not null)
                return cached;

            var companys = (await _totvsRepository.GetCompanys())
                .Where(x => !String.IsNullOrWhiteSpace(x?.doc_company))
                .Select(x => x!)
                .ToList();

            // tabela vazia não entra no cache: um insert em TotvsParameters vale já na próxima execução
            if (companys.Count > 0)
                _cache.Set(ParametersKey, companys, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });

            return companys;
        }

        public async Task<Company?> GetCompany(string doc_company) =>
            (await GetCompanys()).FirstOrDefault(x => x.doc_company == doc_company);

        public async Task<List<int>> GetBranchCodes(Company company) =>
            (await _totvsRepository.GetBranchCodes(company.doc_company!)).ToList();

        public async Task<List<int>> GetBrandBranchCodes(Company company, int baseBranchCode) =>
            (await _totvsRepository.GetBrandBranchCodes(company.doc_company!, baseBranchCode)).ToList();

        public async Task<int?> GetBranchCodeByCnpj(Company company, string cnpj) =>
            await _totvsRepository.GetBranchCodeByCnpj(company.doc_company!, cnpj);

        // sem o pacote de binder: a chave ausente ou inválida conta como desligada
        public bool IsWriteEnabled(string group) =>
            bool.TryParse(_configuration[$"Totvs:Writes:{group}:Enabled"], out var enabled) && enabled;

        public async Task<string?> GetToken(Company company)
        {
            var key = $"{TokenKey}:{company.doc_company}";

            if (_cache.TryGetValue(key, out string? cached) && !String.IsNullOrEmpty(cached))
                return cached;

            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", company.client_id },
                { "client_secret", company.client_secret },
                { "username", company.username },
                { "password", company.password }
            });

            var response = await _apiClient.SendAsync(HttpMethod.Post, "authorization/v2/token", content);

            if (!response.IsSuccess)
            {
                // a resposta de erro não traz credenciais ({ code, message, detailedMessage })
                var body = response.Content?.Length > 800 ? response.Content[..800] : response.Content;
                _logger.AddMessage($"Falha na autenticação da Totvs (empresa {company.doc_company}): {response.StatusCode} {body}");

                return null;
            }

            Token? token = null;

            try
            {
                token = String.IsNullOrWhiteSpace(response.Content) ? null : JsonConvert.DeserializeObject<Token>(response.Content);
            }
            catch (JsonException)
            {
                // token continua null: tratado abaixo como resposta inválida
            }

            if (String.IsNullOrWhiteSpace(token?.access_token))
            {
                // o corpo não é registrado: uma resposta de sucesso pode trazer o token
                _logger.AddMessage($"Resposta inválida do login da Totvs (empresa {company.doc_company}): access_token ausente.");

                return null;
            }

            // 5 minutos de folga antes do expires_in (3600 s no teste), para o token não vencer no meio de uma paginação
            _cache.Set(key, token.access_token, TimeSpan.FromSeconds(Math.Max(token.expires_in - 300, 60)));

            return token.access_token;
        }

        public void RemoveToken(Company company) =>
            _cache.Remove($"{TokenKey}:{company.doc_company}");

        public void SaveRequest(Domain.Totvs.Enums.EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess, string? requestBody = null) =>
            _totvsRepository.InsertIntoTotvsRequests(
                new Domain.Totvs.Entities.Writes.Requests.Request(
                    execution: _logger.GetExecutionGuid() ?? Guid.Empty,
                    job: job,
                    user_name: user_name,
                    http_method: httpMethod,
                    route: route,
                    doc_company: doc_company,
                    field_key_value: field_key_value,
                    request_body: requestBody,
                    response_body: responseBody,
                    is_success: isSuccess
                )
            );

        public async Task<Domain.Totvs.Dtos.Writes.Result> ReadOnDemand(Company company, Domain.Totvs.Enums.EnumJob job, string user_name, string key, HttpMethod method, string route, string? requestBody = null, string? emptyListProperty = null)
        {
            var routeName = route.Split('?')[0];
            ApiResponse? response = null;

            try
            {
                // uma nova tentativa: são consultas, idempotentes, inclusive os POST
                for (var attempt = 1; attempt <= 2; attempt++)
                {
                    var token = await GetToken(company);

                    if (token is null)
                        break;   // a mensagem do login já foi para o log

                    using var content = requestBody is null ? null : new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

                    response = await _apiClient.SendAsync(
                        method,
                        route,
                        content,
                        headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token}" } }
                    );

                    if (attempt == 1 && response.StatusCode == 401)
                    {
                        RemoveToken(company);
                        continue;
                    }

                    if (attempt == 1 && response.StatusCode == 429)
                    {
                        var seconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var value) ? value : 5;
                        await Task.Delay(TimeSpan.FromSeconds(Math.Clamp(seconds, 1, 60)));
                        continue;
                    }

                    if (attempt == 1 && response.StatusCode is 502 or 503 or 504)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        continue;
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                throw;
            }

            if (response is null)
            {
                SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, null, false, requestBody);
                return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 0, apiCalled = false, message = "Falha no login na Totvs: veja o log da execução." };
            }

            if (!response.IsSuccess)
            {
                SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, response.Content, false, requestBody);

                var message = OnDemandMessage(response);
                _logger.AddMessage($"Erro da API Totvs em {routeName} ({key}): {response.StatusCode} {message}");

                return new Domain.Totvs.Dtos.Writes.Result
                {
                    success = false,
                    key = key,
                    statusCode = IsNotFound(response) ? 404 : response.StatusCode,
                    apiCalled = true,
                    message = message,
                    response = Truncate(response.Content),
                    retryAfterSeconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var seconds) ? seconds : null
                };
            }

            System.Text.Json.JsonElement data;

            try
            {
                data = System.Text.Json.JsonDocument.Parse(response.Content ?? String.Empty).RootElement.Clone();
            }
            catch (System.Text.Json.JsonException)
            {
                SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, Truncate(response.Content), false, requestBody);
                _logger.AddMessage($"Resposta inválida da API Totvs em {routeName} ({key}): {Truncate(response.Content)}");

                // JSON inválido: 502 (§8.1)
                return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 502, apiCalled = true, message = "Resposta inválida da Totvs.", response = Truncate(response.Content) };
            }

            var empty = emptyListProperty is not null && data.ValueKind == System.Text.Json.JsonValueKind.Object && data.TryGetProperty(emptyListProperty, out var list)
                && list.ValueKind == System.Text.Json.JsonValueKind.Array && list.GetArrayLength() == 0;

            SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, response.Content, !empty, requestBody);

            if (empty)
            {
                _logger.AddMessage($"Sem registros em {routeName} ({key}).");
                return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 404, apiCalled = true, message = "Nenhum registro encontrado.", data = data };
            }

            _logger.AddMessage($"Concluída com sucesso: {routeName} ({key}) devolvido.");

            return new Domain.Totvs.Dtos.Writes.Result { success = true, key = key, statusCode = 200, apiCalled = true, data = data };
        }

        public Domain.Totvs.Dtos.Writes.Result CompanyNotFound(string doc_company, string key)
        {
            // nenhuma chamada à API: não há o que registrar em TotvsRequests (igual ao Movidesk)
            _logger.AddMessage($"Empresa {doc_company} não cadastrada em totvs.TotvsParameters.");

            return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 0, apiCalled = false, message = $"Empresa {doc_company} não cadastrada." };
        }

        public Domain.Totvs.Dtos.Writes.Result BranchNotFound(string doc_company, string key, IEnumerable<string> branches)
        {
            // nenhuma chamada à API: não há o que registrar em TotvsRequests
            var message = $"Loja(s) {String.Join(", ", branches)} fora de totvs.TotvsBranches da empresa {doc_company}.";
            _logger.AddMessage(message);

            return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 0, apiCalled = false, message = message };
        }

        // o 400 de chave inexistente da Totvs (KeyNotFound no xml-contents e nas buscas por chave, NotFound no list-balance-bonus) é um 404
        private static bool IsNotFound(ApiResponse response)
        {
            if (response.StatusCode == 404)
                return true;

            if (response.StatusCode != 400 || String.IsNullOrWhiteSpace(response.Content))
                return false;

            try
            {
                return Newtonsoft.Json.Linq.JToken.Parse(response.Content) is Newtonsoft.Json.Linq.JArray messages
                    && messages.OfType<Newtonsoft.Json.Linq.JObject>().Any(m => (string?)m["code"] is "KeyNotFound" or "NotFound" or "PathParamNotFound");
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        // as mensagens da Totvs (lista de DomainNotificationMessage), como no TotvsAccountsPayableService
        private static string OnDemandMessage(ApiResponse response)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(response.Content) && Newtonsoft.Json.Linq.JToken.Parse(response.Content) is Newtonsoft.Json.Linq.JArray messages)
                {
                    var text = String.Join(" | ", messages.OfType<Newtonsoft.Json.Linq.JObject>().Select(m => $"{m["code"]}: {m["message"]}").Where(m => m != ": "));

                    if (!String.IsNullOrEmpty(text))
                        return text;
                }
            }
            catch (JsonReaderException)
            {
                // corpo em texto: usa o corpo como veio
            }

            return String.IsNullOrWhiteSpace(response.Content) ? $"A Totvs devolveu {response.StatusCode} sem corpo." : Truncate(response.Content)!;
        }

        private static string? Truncate(string? content) => content?.Length > 800 ? content[..800] : content;
    }
}

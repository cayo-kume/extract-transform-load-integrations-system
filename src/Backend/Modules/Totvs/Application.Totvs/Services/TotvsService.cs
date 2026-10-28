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

        // janela da trava de repetição das escritas não idempotentes (T5)
        private const int RepeatLockMinutes = 10;

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

        public string? GetWriteSetting(string group, string name) =>
            _configuration[$"Totvs:Writes:{group}:{name}"];

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

        public void SaveRequest(Domain.Totvs.Enums.EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess, string? requestBody = null, int? statusCode = null) =>
            _totvsRepository.InsertIntoTotvsRequests(
                new Domain.Totvs.Entities.Writes.Requests.Request(
                    execution: _logger.GetExecutionGuid() ?? Guid.Empty,
                    job: job,
                    user_name: user_name,
                    http_method: httpMethod,
                    route: route,
                    doc_company: doc_company,
                    field_key_value: field_key_value,
                    // T6: documentos, conta e PIX mascarados nos corpos; a chave fica inteira, é por ela que o registro é procurado (e a trava T5
                    // compara)
                    request_body: TotvsMasking.Body(requestBody),
                    response_body: TotvsMasking.Body(responseBody),
                    is_success: isSuccess,
                    status_code: statusCode
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
                SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, response.Content, false, requestBody, response.StatusCode);

                var message = TotvsMessage(response);
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

            SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, response.Content, !empty, requestBody, response.StatusCode);

            if (empty)
            {
                _logger.AddMessage($"Sem registros em {routeName} ({key}).");
                return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 404, apiCalled = true, message = "Nenhum registro encontrado.", data = data };
            }

            _logger.AddMessage($"Concluída com sucesso: {routeName} ({key}) devolvido.");

            return new Domain.Totvs.Dtos.Writes.Result { success = true, key = key, statusCode = 200, apiCalled = true, data = data };
        }

        public async Task<Domain.Totvs.Dtos.Writes.Result> Write(Company company, Domain.Totvs.Enums.EnumJob job, string user_name, string key, HttpMethod method, string route, object? body, string checkHint, bool repeatLock = false, object? loggedBody = null)
        {
            var routeName = route.Split('?')[0];

            // nulos não vão (os campos dos DTOs de escrita são anuláveis); enums como número, salvo os que declaram outro formato (T3)
            var requestBody = body is null ? null : JsonConvert.SerializeObject(body, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // o que vai para totvs.TotvsRequests: o próprio corpo ou, com loggedBody, uma versão sem o que não deve ser gravado (o XML da nota na
            // manifestação, F1). A trava compara a mesma versão
            var storedBody = loggedBody is null ? requestBody : JsonConvert.SerializeObject(loggedBody, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (repeatLock && await RepeatedWrite(company, key, route, storedBody, checkHint) is { } repeated)
                return repeated;

            ApiResponse? response = null;

            try
            {
                // uma nova tentativa só em 401 (renova o token) e 429 (espera o Retry-After): nos dois a Totvs não processou o pedido. Em
                // 502/503/504 não há nova tentativa: a escrita pode ter sido feita antes de o gateway cair
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

                    break;
                }
            }
            catch (Exception ex)
            {
                // sem resposta (timeout, rede): a escrita pode ter sido feita. O status 0 marca a dúvida para a trava de repetição
                var failure = $"Exceção ao chamar a API: {ex.Message.TrimEnd('.')}";
                SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, failure, false, storedBody, 0);
                _logger.AddMessage($"{routeName} ({key}, empresa {company.doc_company}): {failure}. {checkHint}");

                return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, apiCalled = true, message = $"{failure}. {checkHint}" };
            }

            if (response is null)
            {
                // login falhou: nenhuma chamada à rota
                SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, null, false, storedBody);

                return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = $"Falha na autenticação da Totvs (empresa {company.doc_company}). Veja o log da execução." };
            }

            SaveRequest(job, user_name, method.Method, route, company.doc_company!, key, response.Content, response.IsSuccess, storedBody, response.StatusCode);

            if (!response.IsSuccess)
            {
                var message = TotvsMessage(response);
                _logger.AddMessage($"{routeName} ({key}, empresa {company.doc_company}): {response.StatusCode} {message}");

                return new Domain.Totvs.Dtos.Writes.Result
                {
                    success = false,
                    key = key,
                    statusCode = response.StatusCode,
                    apiCalled = true,
                    message = response.StatusCode >= 500 ? $"{message.TrimEnd('.')}. {checkHint}" : message,
                    response = response.Content,
                    retryAfterSeconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var seconds) ? seconds : null
                };
            }

            // o banco local não é atualizado aqui: o registro volta pela leitura recorrente (T7)
            _logger.AddMessage($"Concluída com sucesso: {routeName} ({key}, empresa {company.doc_company}).");

            return new Domain.Totvs.Dtos.Writes.Result { success = true, key = key, statusCode = response.StatusCode, apiCalled = true, response = response.Content };
        }

        public Domain.Totvs.Dtos.Writes.Result WriteDisabled(string group, string key)
        {
            // nenhuma chamada à API: não há o que registrar em TotvsRequests
            var message = $"Escritas do grupo {group} desligadas (Totvs:Writes:{group}:Enabled).";
            _logger.AddMessage(message);

            return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, message = message };
        }

        // trava de repetição (T5): a mesma rota + chave + corpo nos últimos 10 minutos, aceita pela Totvs ou sem saber se ela gravou (5xx ou
        // exceção), volta 409 sem chamar a API. A recusa da Totvs (4xx) e o login que falhou não travam: nada foi gravado
        private async Task<Domain.Totvs.Dtos.Writes.Result?> RepeatedWrite(Company company, string key, string route, string? requestBody, string checkHint)
        {
            // request_date é gravado no horário de Brasília (Request)
            var since = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")).AddMinutes(-RepeatLockMinutes);

            // o banco guarda o corpo mascarado (T6): a comparação é com a mesma máscara, e a chave inteira separa quem a máscara igualaria
            var maskedBody = TotvsMasking.Body(requestBody);

            var previous = (await _totvsRepository.GetRecentRequests(company.doc_company!, route, key, since))
                .FirstOrDefault(x => x is not null && x.request_body == maskedBody && (x.is_success || x.status_code == 0 || x.status_code >= 500));

            if (previous is null)
                return null;

            var message = previous.is_success
                ? $"A mesma escrita foi aceita pela Totvs às {previous.request_date:HH:mm:ss}: repetir em menos de {RepeatLockMinutes} minutos é recusado."
                : $"A mesma escrita ficou sem resposta certa às {previous.request_date:HH:mm:ss} e pode ter sido feita. {checkHint}";

            _logger.AddMessage($"{route.Split('?')[0]} ({key}, empresa {company.doc_company}): repetição recusada. {message}");

            return new Domain.Totvs.Dtos.Writes.Result { success = false, key = key, statusCode = 409, message = message };
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
        private static string TotvsMessage(ApiResponse response)
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

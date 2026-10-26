using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Dtos.Reads.Authorization;
using Domain.Totvs.Entities.Parameters;
using Microsoft.Extensions.Caching.Memory;
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

        private const string ParametersKey = "Totvs:Parameters";
        private const string TokenKey = "Totvs:Token";

        public TotvsService([FromKeyedServices("TotvsAPI")] IApiClient apiClient, ILoggerService logger, IMemoryCache cache, ITotvsRepository totvsRepository) =>
            (_apiClient, _logger, _cache, _totvsRepository) = (apiClient, logger, cache, totvsRepository);

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
    }
}

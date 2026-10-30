using Application.LinxCommerce.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Dtos.Reads.Base;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Enums;
using Domain.Shared.Abstractions.Api;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.LinxCommerce.Services
{
    /// <summary>
    /// métodos em comum usados pelos services do módulo. O ILoggerService é Scoped: dentro de uma requisição
    /// é a mesma instância do service que chamou, então as mensagens entram na execução aberta por ele.
    /// Nenhuma resposta real da API foi conferida (sem credenciais): paginação, envelope e erros seguem o código antigo
    /// (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.2)
    /// </summary>
    public partial class LinxCommerceService : ILinxCommerceService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ILinxCommerceRepository _linxCommerceRepository;

        private const string ParametersKey = "LinxCommerce:Parameters";

        // o código antigo usava 500 no SearchOrders; as outras buscas liam uma página de 1000 ou 10000 sem paginar
        private const int SearchPageSize = 500;

        // não são lidos nem gravados, nem no JSON de auditing.Records (3.2)
        private static readonly HashSet<string> SensitiveFields = new(StringComparer.OrdinalIgnoreCase)
        {
            "Password", "CustomerHash", "Holder", "NumberHint", "SecurityCodeHint", "PixQRCode", "PixKey"
        };

        // datas como texto (a entidade converte, com o fuso) e decimais sem passar por double
        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            DateParseHandling = DateParseHandling.None,
            FloatParseHandling = FloatParseHandling.Decimal
        };

        [GeneratedRegex(@"^\d{14}$")]
        private static partial Regex Cnpj();

        public LinxCommerceService([FromKeyedServices("LinxCommerceAPI")] IApiClient apiClient, ILoggerService logger, IMemoryCache cache, ILinxCommerceRepository linxCommerceRepository) =>
            (_apiClient, _logger, _cache, _linxCommerceRepository) = (apiClient, logger, cache, linxCommerceRepository);

        public DateTime GetBatchTime() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        public async Task<IEnumerable<Company>> GetCompanys()
        {
            if (_cache.TryGetValue(ParametersKey, out List<Company>? cached) && cached is not null)
                return cached;

            var companys = new List<Company>();

            foreach (var company in (await _linxCommerceRepository.GetCompanys()).Where(x => x is not null).Select(x => x!))
            {
                // o doc_company entra nas queries: só CNPJ de 14 dígitos
                if (company.doc_company is null || !Cnpj().IsMatch(company.doc_company))
                {
                    _logger.AddMessage($"Cliente '{company.doc_company}' de linxcommerce.LinxCommerceParameters ignorado: doc_company não é um CNPJ de 14 dígitos.");
                    continue;
                }

                // https, salvo http://localhost (API simulada nos testes locais, sem credencial do Linx Commerce)
                if (!Uri.TryCreate(company.base_url, UriKind.Absolute, out var uri) || !(uri.Scheme == Uri.UriSchemeHttps || (uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback)) || String.IsNullOrWhiteSpace(company.user_name) || String.IsNullOrWhiteSpace(company.api_key))
                {
                    _logger.AddMessage($"Cliente {company.doc_company} de linxcommerce.LinxCommerceParameters ignorado: base_url (https), user_name ou api_key vazios.");
                    continue;
                }

                companys.Add(company);
            }

            if (companys.Count == 0)
                _logger.AddMessage("Nenhum cliente em linxcommerce.LinxCommerceParameters: nada a ler.");

            // tabela vazia não entra no cache: um insert em LinxCommerceParameters vale já na próxima execução
            if (companys.Count > 0)
                _cache.Set(ParametersKey, companys, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });

            return companys;
        }

        public string ModifiedDateWindow(DateTime since, DateTime until) =>
            $"(ModifiedDate>=\"{since.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)}\" && ModifiedDate<=\"{until.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)}\")";

        public async Task<List<(T dto, string json)>?> Search<T>(Company company, string route, string where, string orderBy)
        {
            var items = new List<(T dto, string json)>();
            var pageIndex = 0;
            var pageCount = 1;

            while (pageIndex < pageCount)
            {
                var body = new
                {
                    Page = new { PageIndex = pageIndex, PageSize = SearchPageSize },
                    Where = where,
                    WhereMetadata = "",
                    OrderBy = orderBy
                };

                var response = await Post(company, route, body, $"empresa {company.doc_company}, página {pageIndex}");

                if (response is null)
                    return null;

                SearchResult<T>? result = null;
                JArray json = [];

                try
                {
                    result = JsonConvert.DeserializeObject<SearchResult<T>>(response.Content ?? String.Empty, JsonSettings);
                    json = (Parse(response.Content!)["Result"] as JArray) ?? [];
                }
                catch (JsonException)
                {
                    // result continua null: tratado abaixo como resposta inválida
                }

                if (result?.Result is null || json.Count != result.Result.Count || !String.IsNullOrWhiteSpace(result.Page?.ErrorMessage))
                {
                    _logger.AddMessage($"Resposta inválida do Linx Commerce em {route} (empresa {company.doc_company}, página {pageIndex}): {Truncate(response.Content)}");

                    return null;
                }

                // cada item pareado com o seu JSON pela posição
                for (var i = 0; i < json.Count; i++)
                    items.Add((result.Result[i], json[i] is JObject item ? RecordJson(item) : json[i].ToString(Formatting.None)));

                if (result.Result.Count == 0)
                    break;

                // PageCount é o total de páginas, com PageIndex a partir de 0 (o código antigo fazia uma chamada a mais); sem Page,
                // uma página só. A conferir com uma resposta real
                pageCount = result.Page?.PageCount ?? 0;
                pageIndex++;
            }

            return items;
        }

        public async Task<(T dto, string json)?> Get<T>(Company company, string route, object body, string key) where T : class
        {
            var response = await Post(company, route, body, $"{key}, empresa {company.doc_company}");

            if (response is null)
                return null;

            try
            {
                var dto = JsonConvert.DeserializeObject<T>(response.Content ?? String.Empty, JsonSettings);

                if (dto is not null)
                    return (dto, RecordJson(Parse(response.Content!)));
            }
            catch (JsonException)
            {
                // tratado abaixo como resposta inválida
            }

            _logger.AddMessage($"Resposta inválida do Linx Commerce em {route} ({key}, empresa {company.doc_company}): {Truncate(response.Content)}");

            return null;
        }

        public async Task<Company?> GetCompany(string doc_company) =>
            (await GetCompanys()).FirstOrDefault(x => x.doc_company == doc_company);

        public async Task<OnDemandResponse<T>> ReadOnDemand<T>(Company company, EnumJob job, string user_name, string route, object body, string key) where T : class
        {
            var requestBody = JsonConvert.SerializeObject(body);
            var (response, failure) = await Send(company, route, requestBody);

            if (response is null)
            {
                // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                _logger.AddMessage($"Falha ao chamar o Linx Commerce em {route} ({key}, empresa {company.doc_company}): {failure}");
                SaveRequest(job, user_name, route, company.doc_company!, key, requestBody, $"Exceção ao chamar a API: {failure}", false, 0);

                return new OnDemandResponse<T>();
            }

            T? dto = null;
            string? json = null;

            if (response.IsSuccess)
            {
                try
                {
                    dto = JsonConvert.DeserializeObject<T>(response.Content ?? String.Empty, JsonSettings);
                    json = dto is null ? null : RecordJson(Parse(response.Content!));
                }
                catch (JsonException)
                {
                    dto = null;   // tratado abaixo como resposta inválida
                }
            }

            if (!response.IsSuccess)
                _logger.AddMessage($"Erro do Linx Commerce em {route} ({key}, empresa {company.doc_company}): {response.StatusCode} {Truncate(response.Content)}");
            else if (dto is null)
                _logger.AddMessage($"Resposta inválida do Linx Commerce em {route} ({key}, empresa {company.doc_company}): {Truncate(response.Content)}");

            // o corpo da resposta vai sem os campos sensíveis; resposta que não é JSON, cortada
            SaveRequest(job, user_name, route, company.doc_company!, key, requestBody, json ?? Truncate(response.Content), dto is not null, response.StatusCode);

            return new OnDemandResponse<T> { StatusCode = response.StatusCode, Dto = dto, Json = json };

            void SaveRequest(EnumJob job, string user_name, string route, string doc_company, string key, string requestBody, string? responseBody, bool isSuccess, int? statusCode) =>
                _linxCommerceRepository.InsertIntoLinxCommerceRequests(
                    new Domain.LinxCommerce.Entities.Requests.Request(
                        execution: _logger.GetExecutionGuid() ?? Guid.Empty,
                        job: job,
                        user_name: user_name,
                        http_method: "POST",
                        // a rota, sem o endereço do cliente nem o cabeçalho de autenticação
                        route: $"v1/{route}",
                        doc_company: doc_company,
                        field_key_value: key,
                        request_body: requestBody,
                        response_body: responseBody,
                        is_success: isSuccess,
                        status_code: statusCode
                    )
                );

        }

        // ------------------------------------------------------------------------------------------------------

        /// <summary>
        /// POST v1/{route} das leituras recorrentes: erro HTTP ou exceção viram mensagem no log e devolvem null
        /// </summary>
        private async Task<ApiResponse?> Post(Company company, string route, object body, string context)
        {
            var (response, failure) = await Send(company, route, JsonConvert.SerializeObject(body));

            if (response is null)
            {
                _logger.AddMessage($"Falha ao chamar o Linx Commerce em {route} ({context}): {failure}");

                return null;
            }

            if (!response.IsSuccess)
            {
                _logger.AddMessage($"Erro do Linx Commerce em {route} ({context}): {response.StatusCode} {Truncate(response.Content)}");

                return null;
            }

            return response;
        }

        /// <summary>
        /// POST v1/{route} no ambiente do cliente com o Basic usuário:chave. Uma nova tentativa: 429 espera o Retry-After e
        /// 502/503/504 espera 10 s (são leituras). Exceção de rede ou timeout devolve a mensagem dela, sem resposta
        /// </summary>
        private async Task<(ApiResponse? response, string? failure)> Send(Company company, string route, string requestBody)
        {
            var url = $"{company.base_url.TrimEnd('/')}/v1/{route}";
            var authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{company.user_name}:{company.api_key}"));

            ApiResponse? response = null;

            for (var attempt = 1; attempt <= 2; attempt++)
            {
                try
                {
                    using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    response = await _apiClient.SendAsync(
                        HttpMethod.Post,
                        url,
                        content,
                        headers: new Dictionary<string, string> { { "Authorization", $"Basic {authorization}" } }
                    );
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
                {
                    // a mensagem da exceção não traz o cabeçalho de autenticação
                    return (null, ex.Message);
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

            return (response, null);
        }

        // DateParseHandling.None: o JSON gravado fica com as datas como a API mandou
        private static JObject Parse(string content)
        {
            using var reader = new JsonTextReader(new StringReader(content)) { DateParseHandling = DateParseHandling.None, FloatParseHandling = FloatParseHandling.Decimal };

            return JObject.Load(reader);
        }

        private static string RecordJson(JObject json)
        {
            foreach (var property in json.DescendantsAndSelf().OfType<JObject>().SelectMany(x => x.Properties()).Where(x => SensitiveFields.Contains(x.Name)).ToList())
                property.Remove();

            return json.ToString(Formatting.None);
        }

        private static string? Truncate(string? content) => content?.Length > 800 ? content[..800] : content;
    }
}

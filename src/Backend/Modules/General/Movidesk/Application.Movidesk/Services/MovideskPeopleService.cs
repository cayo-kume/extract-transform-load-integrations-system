using Application.Movidesk.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Enums;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Extensions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Movidesk.Services
{
    /// <summary>
    /// pessoas da Movidesk: leituras (GET /persons) e escrita (POST, PATCH, relacionamentos e DELETE)
    /// </summary>
    public class MovideskPeopleService : IMovideskPeopleService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IMovideskService _movideskService;
        private readonly IMovideskRepository _movideskRepository;
        private readonly IMovideskPeopleRepository _movideskPeopleRepository;
        private readonly IValidator<Domain.Movidesk.Dtos.Reads.People.Person> _personValidator;

        private const string PeopleKey = "Movidesk:People";

        // limite da API: 10 requisições por minuto -> 1 requisição a cada 6 s
        private const int RequestIntervalInMilliseconds = 6 * 1000;
        private const int PageSize = 100;

        // escrita: campos nulos não são enviados (no PATCH, campo ausente = campo que não muda)
        private static readonly JsonSerializerSettings WriteSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss"
        };

        public MovideskPeopleService(
            [FromKeyedServices("MovideskAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMovideskService movideskService,
            IMovideskRepository movideskRepository,
            IMovideskPeopleRepository movideskPeopleRepository,
            IValidator<Domain.Movidesk.Dtos.Reads.People.Person> personValidator,
            IMemoryCache cache
        ) =>
            (_apiClient, _logger, _movideskService, _movideskRepository, _movideskPeopleRepository, _personValidator, _cache) =
            (apiClient, logger, movideskService, movideskRepository, movideskPeopleRepository, personValidator, cache);

        /// <summary>
        /// busca uma pessoa pelo Cód. Ref., grava nas tabelas de pessoas (sempre, sem consultar o cache)
        /// e registra a chamada em movidesk.MovideskRequests
        /// </summary>
        public async Task<bool?> GetPersonById(string user_name, string doc_company, string id)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.MovideskGetPersonById);

            // rota registrada no log de requisições: sem o token
            var route = $"persons?id={id}";
            bool? result;

            try
            {
                var company = await _movideskRepository.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao GetTicketById)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em movidesk.MovideskParameters.");
                    return null;
                }

                // mesmo horário de lote para a pessoa e as filhas (condição de exclusão do merge)
                var batchTime = _movideskService.GetBatchTime();

                string? response;

                try
                {
                    response = await _apiClient.GetAsync(
                        route: "persons",
                        queryParams: new Dictionary<string, string>
                        {
                            { "token", company.Token.ToString() },
                            { "id", id }
                        }
                    );
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _movideskService.SaveRequest(EnumJob.MovideskGetPersonById, user_name, "GET", route, doc_company, id, $"Exceção ao chamar a API: {ex.Message}", false);
                    throw;
                }

                JObject? json = null;

                try
                {
                    json = String.IsNullOrWhiteSpace(response) ? null : JToken.Parse(response) as JObject;
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    // json continua null: tratado abaixo como resposta inválida
                }

                if (String.IsNullOrWhiteSpace(response) || (json?["status"]?.Value<int?>() == 404 && json["id"] is null))
                {
                    // a API responde 404 com um JSON de erro (application/problem+json) quando a pessoa não existe
                    _logger.AddMessage($"Pessoa {id} não encontrada na Movidesk (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    var person = json?.ToObject<Domain.Movidesk.Dtos.Reads.People.Person>();

                    if (person is null || !String.Equals(person.id, id, StringComparison.Ordinal))
                    {
                        _logger.AddMessage($"Resposta inválida da API Movidesk para a pessoa {id} (empresa {doc_company}): {response}");
                        result = null;
                    }
                    else
                    {
                        // atualiza o cache do GetPeople, se ele estiver carregado, para a próxima execução não regravar a pessoa
                        _cache.TryGetValue($"{PeopleKey}:{doc_company}", out Dictionary<string, Domain.Movidesk.Entities.Reads.People.Person>? peopleCache);

                        await SavePeople(doc_company, batchTime, [person], new Dictionary<string, string> { { id, response! } }, peopleCache);

                        _logger.AddMessage($"Concluída com sucesso: pessoa {id} inserida ou atualizada!");
                        result = true;
                    }
                }

                _movideskService.SaveRequest(EnumJob.MovideskGetPersonById, user_name, "GET", route, doc_company, id, response, result == true);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// sincroniza as pessoas criadas ou alteradas desde a última execução (agentes e clientes), para cada empresa
        /// </summary>
        public async Task<bool?> GetPeople()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.MovideskGetPeople);

            var totalInseridos = 0;
            var companys = await _movideskRepository.GetCompanys();

            foreach (var company in companys)
            {
                if (company?.doc_company is null)
                    continue;

                // um horário por lote: pessoa, filhas e netas gravam o mesmo valor (condição de exclusão do merge)
                var batchTime = _movideskService.GetBatchTime();

                // changedDate é nulo para quem nunca foi alterado: a janela olha as duas datas.
                // banco vazio: primeira carga com as pessoas criadas ou alteradas nos últimos 12 meses
                var lastChange = await _movideskPeopleRepository.GetPeopleLastChange(company.doc_company);
                var since = lastChange.HasValue ? lastChange.Value.AddHours(-1) : DateTime.UtcNow.AddMonths(-12);
                var filter = $"changedDate ge {since:yyyy-MM-ddTHH:mm:ss}.00z or createdDate ge {since:yyyy-MM-ddTHH:mm:ss}.00z";

                var people = new List<Domain.Movidesk.Dtos.Reads.People.Person>();
                var peopleJson = new Dictionary<string, string>();
                var skip = 0;

                while (true)
                {
                    var response = await _apiClient.GetAsync(
                        route: "persons",
                        queryParams: new Dictionary<string, string>
                        {
                            { "token", company.Token.ToString() },
                            { "$filter", filter },
                            { "$orderby", "id asc" },
                            { "$top", PageSize.ToString() },
                            { "$skip", skip.ToString() }
                        }
                    );

                    List<Domain.Movidesk.Dtos.Reads.People.Person>? page;

                    try
                    {
                        page = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Reads.People.Person>>(response ?? String.Empty);
                    }
                    catch (Newtonsoft.Json.JsonException)
                    {
                        // objeto de erro (ou 429) em vez da lista: 3 erros seguidos bloqueiam a API, então a empresa é interrompida
                        _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}, skip {skip}): {response}");
                        break;
                    }

                    if (page is null || page.Count == 0)
                        break;

                    people.AddRange(page);

                    foreach (var item in JArray.Parse(response!))
                    {
                        var id = (string?)item["id"];

                        if (!String.IsNullOrEmpty(id))
                            peopleJson[id] = item.ToString();
                    }

                    if (page.Count < PageSize)
                        break;

                    skip += PageSize;
                    Thread.Sleep(RequestIntervalInMilliseconds);
                }

                if (people.Count == 0)
                    continue;

                // cache de estado: id -> pessoa gravada (com as datas), por empresa
                var peopleCache = await _cache.GetOrCreateAsync($"{PeopleKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _movideskPeopleRepository.GetPeopleByIds(
                        company.doc_company,
                        people.Where(x => !String.IsNullOrEmpty(x.id)).Select(x => x.id).Distinct().ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!);
                });

                // mantém só as novas ou com última mudança mais recente que a gravada
                people.RemoveAll(x =>
                    !string.IsNullOrEmpty(x.id) &&
                    peopleCache!.TryGetValue(x.id, out var y) &&
                    (NullableConverters.ToNullableDateTime(x.changedDate) ?? NullableConverters.ToNullableDateTime(x.createdDate))
                        <= (y.changedDate ?? y.createdDate)
                );

                totalInseridos += await SavePeople(company.doc_company, batchTime, people, peopleJson, peopleCache);
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// cria uma pessoa. Com returnAllProperties=true, se a API devolver a pessoa inteira ela é gravada sem releitura;
        /// se devolver só o Cód. Ref., a pessoa chega pela próxima execução do GetPeople
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostPerson(string user_name, string doc_company, Domain.Movidesk.Dtos.Writes.People.Person person) =>
            SendPerson(EnumJob.MovideskPostPerson, user_name, doc_company, person.id, HttpMethod.Post, "persons",
                query: new Dictionary<string, string> { { "returnAllProperties", "true" } },
                body: person,
                readResponse: response =>
                {
                    // o formato da resposta não está na documentação (a conferir no teste real):
                    // a pessoa inteira, o Cód. Ref. puro ou { "id": ... }
                    JToken? json = null;

                    try
                    {
                        json = String.IsNullOrWhiteSpace(response) ? null : JToken.Parse(response);
                    }
                    catch (JsonReaderException)
                    {
                        // texto puro: o próprio Cód. Ref.
                        return (String.IsNullOrWhiteSpace(response) ? person.id : response.Trim(), null);
                    }

                    if (json is JObject full && (full.ContainsKey("businessName") || full.ContainsKey("personType")))
                    {
                        // as datas da resposta do POST de tickets vêm em UTC com "Z", que faria a conversão passar para o
                        // horário local; o GET devolve UTC sem sufixo. Tirar o "Z" deixa a pessoa igual ao que o GetPeople grava
                        var created = JsonConvert.DeserializeObject<Domain.Movidesk.Dtos.Reads.People.Person>(
                            Regex.Replace(response!, @"(""\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?)Z""", "$1\"")
                        );

                        return (String.IsNullOrWhiteSpace(created?.id) ? person.id : created.id, created);
                    }

                    var createdId = json is JObject idOnly ? (string?)idOnly["id"] : json?.ToString();

                    return (String.IsNullOrWhiteSpace(createdId) ? person.id : createdId, null);
                });

        /// <summary>
        /// altera uma pessoa: só os campos enviados mudam, e listas enviadas substituem a lista inteira na Movidesk.
        /// A alteração chega ao banco pela próxima execução do GetPeople
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PatchPersonById(string user_name, string doc_company, string id, Domain.Movidesk.Dtos.Writes.People.Person person) =>
            SendPerson(EnumJob.MovideskPatchPersonById, user_name, doc_company, id, HttpMethod.Patch, "persons",
                query: new Dictionary<string, string> { { "id", id } },
                body: person,
                readResponse: _ => (id, null));

        /// <summary>
        /// inclui organizações numa pessoa sem mexer nas que ela já tem (no PATCH, relationships substitui a lista inteira)
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostPersonRelationshipsById(string user_name, string doc_company, string id, Domain.Movidesk.Dtos.Writes.People.Relationships relationships) =>
            SendPerson(EnumJob.MovideskPostPersonRelationshipsById, user_name, doc_company, id, HttpMethod.Post, "persons/relationships",
                query: new Dictionary<string, string> { { "id", id } },
                body: relationships,
                readResponse: _ => (id, null));

        /// <summary>
        /// exclui a pessoa na Movidesk (irreversível) e, com sucesso, apaga a pessoa e as filhas do banco local
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> DeletePersonById(string user_name, string doc_company, string id) =>
            SendPerson(EnumJob.MovideskDeletePersonById, user_name, doc_company, id, HttpMethod.Delete, "persons",
                query: new Dictionary<string, string> { { "id", id } },
                body: null,
                readResponse: _ => (id, null));

        /// <summary>
        /// fluxo comum das rotas de escrita de pessoas (mesmo desenho do SendTicket): empresa, chamada à API, registro em
        /// movidesk.MovideskRequests (com o corpo enviado) e, depois do sucesso, gravação da pessoa devolvida pelo POST
        /// ou exclusão local da pessoa excluída pelo DELETE
        /// </summary>
        private async Task<Domain.Movidesk.Dtos.Writes.Result> SendPerson(
            EnumJob job,
            string user_name,
            string doc_company,
            string? id,
            HttpMethod method,
            string route,
            Dictionary<string, string> query,
            object? body,
            Func<string?, (string? personId, Domain.Movidesk.Dtos.Reads.People.Person? person)> readResponse)
        {
            _logger
                .Clear()
                .AddLog(job);

            // rota registrada no log de requisições: sem o token
            var logRoute = $"{route}?{String.Join("&", query.Select(x => $"{x.Key}={x.Value}"))}";

            // corpo sem nulos (no PATCH, campo ausente = campo que não muda); o DELETE não tem corpo
            var requestBody = body is null ? null : JsonConvert.SerializeObject(body, WriteSettings);

            try
            {
                var company = await _movideskRepository.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao GetPersonById)
                    var notFound = $"Empresa {doc_company} não cadastrada em movidesk.MovideskParameters.";
                    _logger.AddMessage(notFound);

                    return new Domain.Movidesk.Dtos.Writes.Result { success = false, id = id, message = notFound };
                }

                ApiResponse response;

                try
                {
                    using var httpContent = requestBody is null ? null : new StringContent(requestBody, Encoding.UTF8, "application/json");

                    response = await _apiClient.SendAsync(
                        method,
                        route,
                        httpContent,
                        queryParams: new Dictionary<string, string>(query) { { "token", company.Token.ToString() } }
                    );
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _movideskService.SaveRequest(job, user_name, method.Method, logRoute, doc_company, id ?? String.Empty, $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                    throw;
                }

                var (personId, person) = response.IsSuccess ? readResponse(response.Content) : (id, null);

                _movideskService.SaveRequest(job, user_name, method.Method, logRoute, doc_company, personId ?? String.Empty, response.Content, response.IsSuccess, requestBody);

                if (!response.IsSuccess)
                {
                    // mensagem da API quando vier em JSON ({ "message": ... }); senão, o corpo como veio
                    string? apiMessage = null;

                    try
                    {
                        apiMessage = String.IsNullOrWhiteSpace(response.Content) ? null : (JToken.Parse(response.Content) as JObject)?["message"]?.ToString();
                    }
                    catch (JsonReaderException)
                    {
                        // corpo em texto: usa o corpo como veio
                    }

                    var message = apiMessage ?? (response.Content?.Length > 800 ? response.Content[..800] : response.Content) ?? $"A Movidesk devolveu {response.StatusCode}.";
                    _logger.AddMessage($"Pessoa {personId ?? "nova"} (empresa {doc_company}): {message}");

                    return new Domain.Movidesk.Dtos.Writes.Result
                    {
                        success = false,
                        id = personId,
                        statusCode = response.StatusCode,
                        message = message,
                        response = response.Content,
                        retryAfterSeconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var seconds) ? seconds : null
                    };
                }

                string? localMessage = null;

                if (person is not null && personId is not null)
                {
                    // pessoa inteira na resposta do POST: grava agora, sem releitura. Se falhar, a escrita continua valendo e o GetPeople corrige
                    try
                    {
                        _cache.TryGetValue($"{PeopleKey}:{doc_company}", out Dictionary<string, Domain.Movidesk.Entities.Reads.People.Person>? peopleCache);

                        await SavePeople(doc_company, _movideskService.GetBatchTime(), [person], new Dictionary<string, string> { { personId, response.Content! } }, peopleCache);
                    }
                    catch (Exception ex)
                    {
                        localMessage = $"Pessoa {personId} criada, mas não gravada no banco (o GetPeople grava depois): {ex.Message}";
                        _logger.AddMessage(localMessage);
                    }
                }

                if (method == HttpMethod.Delete && personId is not null)
                {
                    // a merge de pessoas nunca apaga linhas: a pessoa excluída sai do banco local aqui. A Movidesk já excluiu,
                    // então uma falha local não desfaz o sucesso; vai na mensagem e no log
                    try
                    {
                        await _movideskPeopleRepository.DeletePerson(doc_company, personId);

                        // sem a pessoa no cache, uma recriação com o mesmo Cód. Ref. não é descartada como "já gravada"
                        if (_cache.TryGetValue($"{PeopleKey}:{doc_company}", out Dictionary<string, Domain.Movidesk.Entities.Reads.People.Person>? peopleCache))
                            peopleCache!.Remove(personId);
                    }
                    catch (Exception ex)
                    {
                        localMessage = $"Pessoa {personId} excluída na Movidesk, mas não apagada do banco local: {ex.Message}";
                        _logger.AddMessage(localMessage);
                    }
                }

                _logger.AddMessage($"Concluída com sucesso: pessoa {personId ?? "(Cód. Ref. não informado pela API)"} ({method.Method} {route}).");

                return new Domain.Movidesk.Dtos.Writes.Result
                {
                    success = true,
                    id = personId,
                    statusCode = response.StatusCode,
                    message = localMessage,
                    response = response.Content
                };
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de pessoas gravadas.
        /// </summary>
        private async Task<int> SavePeople(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Movidesk.Dtos.Reads.People.Person> people,
            IReadOnlyDictionary<string, string> peopleJson,
            Dictionary<string, Domain.Movidesk.Entities.Reads.People.Person>? peopleCache)
        {
            var _listSomenteNovos = new List<Domain.Movidesk.Entities.Reads.People.Person>();

            // a mesma pessoa pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var person in people.Where(x => !String.IsNullOrEmpty(x.id)).GroupBy(x => x.id).Select(g => g.Last()))
            {
                var validations = _personValidator.Validate(person);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"pessoa {person.id}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Movidesk.Entities.Reads.People.Person(doc_company, batchTime, person));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _movideskPeopleRepository.BulkInsertIntoMovideskPeople(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (peopleJson.TryGetValue(record.id, out var json))
                    _logger.AddRecord(record.id, json);

                if (peopleCache is not null)
                    peopleCache[record.id] = record;
            }

            return _listSomenteNovos.Count;
        }
    }
}

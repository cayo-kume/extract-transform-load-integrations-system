using Application.Movidesk.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Enums;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Extensions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Application.Movidesk.Services
{
    /// <summary>
    /// pessoas da Movidesk (GET /persons): agentes e clientes
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
        /// e registra a chamada em general.MovideskRequests
        /// </summary>
        public async Task<bool?> GetPerson(string user_name, string doc_company, string id)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.MovideskGetPerson);

            // rota registrada no log de requisições: sem o token
            var route = $"persons?id={id}";
            bool? result;

            try
            {
                var company = await _movideskRepository.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao GetTicket)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em general.MovideskParameters.");
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
                    _movideskService.SaveRequest(EnumJob.MovideskGetPerson, user_name, "GET", route, doc_company, id, $"Exceção ao chamar a API: {ex.Message}", false);
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

                _movideskService.SaveRequest(EnumJob.MovideskGetPerson, user_name, "GET", route, doc_company, id, response, result == true);

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

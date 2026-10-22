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

namespace Application.Movidesk.Services
{
    /// <summary>
    /// catálogo de serviços da Movidesk: leituras (GET /services) e escrita (POST, PATCH e DELETE)
    /// </summary>
    public class MovideskServicesService : IMovideskServicesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IMovideskService _movideskService;
        private readonly IMovideskRepository _movideskRepository;
        private readonly IMovideskServicesRepository _movideskServicesRepository;
        private readonly IValidator<Domain.Movidesk.Dtos.Reads.Services.Service> _serviceValidator;

        private const string ServicesHashKey = "Movidesk:ServicesHash";

        // escrita: campos nulos não são enviados (no PATCH, campo ausente = campo que não muda)
        private static readonly JsonSerializerSettings WriteSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public MovideskServicesService(
            [FromKeyedServices("MovideskAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMovideskService movideskService,
            IMovideskRepository movideskRepository,
            IMovideskServicesRepository movideskServicesRepository,
            IValidator<Domain.Movidesk.Dtos.Reads.Services.Service> serviceValidator,
            IMemoryCache cache
        ) =>
            (_apiClient, _logger, _movideskService, _movideskRepository, _movideskServicesRepository, _serviceValidator, _cache) =
            (apiClient, logger, movideskService, movideskRepository, movideskServicesRepository, serviceValidator, cache);

        /// <summary>
        /// busca um serviço por id, grava nas tabelas de serviços e registra a chamada em general.MovideskRequests
        /// </summary>
        public async Task<bool?> GetServiceById(string user_name, string doc_company, int id)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.MovideskGetServiceById);

            // rota registrada no log de requisições: sem o token
            var route = $"services?id={id}";
            bool? result;

            try
            {
                var company = await _movideskRepository.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao GetTicketById)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em general.MovideskParameters.");
                    return null;
                }

                string? response;

                try
                {
                    response = await _apiClient.GetAsync(
                        route: "services",
                        queryParams: new Dictionary<string, string>
                        {
                            { "token", company.Token.ToString() },
                            { "id", id.ToString() }
                        }
                    );
                }
                catch (Exception ex)
                {
                    // registra a tentativa mesmo quando a chamada não chega a ter resposta (timeout, rede)
                    _movideskService.SaveRequest(EnumJob.MovideskGetServiceById, user_name, "GET", route, doc_company, id.ToString(), $"Exceção ao chamar a API: {ex.Message}", false);
                    throw;
                }

                if (String.IsNullOrWhiteSpace(response))
                {
                    // a API responde 404 com corpo vazio quando o serviço não existe (como em tickets)
                    _logger.AddMessage($"Serviço {id} não encontrado na Movidesk (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    Domain.Movidesk.Dtos.Reads.Services.Service? service = null;

                    try
                    {
                        service = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Movidesk.Dtos.Reads.Services.Service>(response);
                    }
                    catch (Newtonsoft.Json.JsonException)
                    {
                        // service continua null: tratado abaixo como resposta inválida
                    }

                    if (service is null || ConverterValidatorsExtensions.ConvertToInt32Validation(service.id) != id)
                    {
                        _logger.AddMessage($"Resposta inválida da API Movidesk para o serviço {id} (empresa {doc_company}): {response}");
                        result = null;
                    }
                    else
                    {
                        await SaveServices(doc_company, _movideskService.GetBatchTime(), [service], new Dictionary<int, string> { { id, response } });

                        // o catálogo da empresa mudou no banco: o próximo GetServices não pode pular a resposta pelo hash
                        _cache.Remove($"{ServicesHashKey}:{doc_company}");

                        _logger.AddMessage($"Concluída com sucesso: serviço {id} inserido ou atualizado!");
                        result = true;
                    }
                }

                _movideskService.SaveRequest(EnumJob.MovideskGetServiceById, user_name, "GET", route, doc_company, id.ToString(), response, result == true);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// sincroniza o catálogo de serviços (inteiro, numa chamada) de cada empresa; só grava quando a resposta mudou
        /// </summary>
        public async Task<bool?> GetServices()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.MovideskGetServices);

            var totalInseridos = 0;
            var companys = await _movideskRepository.GetCompanys();

            foreach (var company in companys)
            {
                if (company?.doc_company is null)
                    continue;

                // o catálogo vem inteiro numa resposta, sem paginação (65 serviços na conta real)
                var response = await _apiClient.GetAsync(
                    route: "services",
                    queryParams: new Dictionary<string, string> { { "token", company.Token.ToString() } }
                );

                List<Domain.Movidesk.Dtos.Reads.Services.Service>? services;

                try
                {
                    services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Reads.Services.Service>>(response ?? String.Empty);
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}): {response}");
                    continue;
                }

                if (services is null || services.Count == 0)
                    continue;

                // cache de hash por empresa: catálogo igual ao da última execução não é regravado
                var hash = _logger.ComputeSha256Hash(response!);
                var hashKey = $"{ServicesHashKey}:{company.doc_company}";

                if (hash.Equals(_cache.Get<string>(hashKey)))
                    continue;

                var servicesJson = new Dictionary<int, string>();

                foreach (var item in JArray.Parse(response!))
                {
                    var id = ConverterValidatorsExtensions.ConvertToInt32Validation((string?)item["id"]);

                    if (id != 0)
                        servicesJson[id] = item.ToString();
                }

                totalInseridos += await SaveServices(company.doc_company, _movideskService.GetBatchTime(), services, servicesJson);

                // 3 horas: precisa durar mais que o intervalo do job (2 horas) para a próxima execução ainda encontrar o hash
                _cache.Set(hashKey, hash, TimeSpan.FromHours(3));
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// cria um serviço (sem id: a Movidesk gera). Com returnAllProperties=true, se a API devolver o serviço inteiro ele é
        /// gravado sem releitura; se devolver só o id, o serviço chega pela próxima execução do GetServices
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostService(string user_name, string doc_company, Domain.Movidesk.Dtos.Writes.Services.Service service) =>
            SendService(EnumJob.MovideskPostService, user_name, doc_company, null, HttpMethod.Post, "services",
                query: new Dictionary<string, string> { { "returnAllProperties", "true" } },
                body: service,
                readResponse: response =>
                {
                    // o formato da resposta não está na documentação (a conferir no teste real):
                    // o serviço inteiro, o id puro ou { "id": ... }
                    JToken? json = null;

                    try
                    {
                        json = String.IsNullOrWhiteSpace(response) ? null : JToken.Parse(response);
                    }
                    catch (JsonReaderException)
                    {
                        // json continua null: o id fica desconhecido e o GetServices traz o serviço depois
                    }

                    if (json is JObject full && full.ContainsKey("name"))
                    {
                        var created = JsonConvert.DeserializeObject<Domain.Movidesk.Dtos.Reads.Services.Service>(response!);
                        var createdId = ConverterValidatorsExtensions.ConvertToInt32Validation(created?.id);

                        return (createdId > 0 ? createdId : null, created);
                    }

                    var number = ConverterValidatorsExtensions.ConvertToInt32Validation(json is JObject idOnly ? (string?)idOnly["id"] : json?.ToString());

                    return (number > 0 ? number : null, null);
                });

        /// <summary>
        /// altera um serviço: só os campos enviados mudam, e categories enviada substitui a lista inteira na Movidesk.
        /// Como o PATCH responde sem corpo e o GetServices só roda a cada 2 horas, o serviço é relido e gravado na hora
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PatchServiceById(string user_name, string doc_company, int id, Domain.Movidesk.Dtos.Writes.Services.Service service) =>
            SendService(EnumJob.MovideskPatchServiceById, user_name, doc_company, id, HttpMethod.Patch, "services",
                query: new Dictionary<string, string> { { "id", id.ToString() } },
                body: service,
                readResponse: _ => (id, null));

        /// <summary>
        /// exclui um serviço do catálogo (irreversível). O serviço continua no banco local, como os removidos pelo painel,
        /// porque tickets antigos apontam para ele
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> DeleteServiceById(string user_name, string doc_company, int id) =>
            SendService(EnumJob.MovideskDeleteServiceById, user_name, doc_company, id, HttpMethod.Delete, "services",
                query: new Dictionary<string, string> { { "id", id.ToString() } },
                body: null,
                readResponse: _ => (id, null));

        /// <summary>
        /// fluxo comum das rotas de escrita de serviços (mesmo desenho do SendTicket e do SendPerson): empresa, chamada à API,
        /// registro em general.MovideskRequests (com o corpo enviado) e, depois do sucesso, hash do catálogo fora do cache,
        /// gravação do serviço devolvido pelo POST e releitura do serviço alterado pelo PATCH
        /// </summary>
        private async Task<Domain.Movidesk.Dtos.Writes.Result> SendService(
            EnumJob job,
            string user_name,
            string doc_company,
            int? id,
            HttpMethod method,
            string route,
            Dictionary<string, string> query,
            object? body,
            Func<string?, (int? serviceId, Domain.Movidesk.Dtos.Reads.Services.Service? service)> readResponse)
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
                    // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao GetServiceById)
                    var notFound = $"Empresa {doc_company} não cadastrada em general.MovideskParameters.";
                    _logger.AddMessage(notFound);

                    return new Domain.Movidesk.Dtos.Writes.Result { success = false, id = id?.ToString(), message = notFound };
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
                    _movideskService.SaveRequest(job, user_name, method.Method, logRoute, doc_company, id?.ToString() ?? String.Empty, $"Exceção ao chamar a API: {ex.Message}", false, requestBody);
                    throw;
                }

                var (serviceId, service) = response.IsSuccess ? readResponse(response.Content) : (id, null);

                _movideskService.SaveRequest(job, user_name, method.Method, logRoute, doc_company, serviceId?.ToString() ?? String.Empty, response.Content, response.IsSuccess, requestBody);

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
                    _logger.AddMessage($"Serviço {serviceId?.ToString() ?? "novo"} (empresa {doc_company}): {message}");

                    return new Domain.Movidesk.Dtos.Writes.Result
                    {
                        success = false,
                        id = serviceId?.ToString(),
                        statusCode = response.StatusCode,
                        message = message,
                        response = response.Content,
                        retryAfterSeconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var seconds) ? seconds : null
                    };
                }

                // o catálogo mudou: o próximo GetServices não pode pular a resposta pelo hash
                _cache.Remove($"{ServicesHashKey}:{doc_company}");

                string? localMessage = null;

                try
                {
                    if (service is not null && serviceId is not null)
                    {
                        // serviço inteiro na resposta do POST: grava agora, sem releitura
                        await SaveServices(doc_company, _movideskService.GetBatchTime(), [service], new Dictionary<int, string> { { serviceId.Value, response.Content! } });
                    }
                    else if (method == HttpMethod.Patch && serviceId is not null)
                    {
                        // o PATCH responde sem corpo e o GetServices só roda a cada 2 horas: relê o serviço e grava na hora
                        var reread = await _apiClient.GetAsync(
                            route: "services",
                            queryParams: new Dictionary<string, string>
                            {
                                { "token", company.Token.ToString() },
                                { "id", serviceId.Value.ToString() }
                            }
                        );

                        var changed = String.IsNullOrWhiteSpace(reread) ? null : JsonConvert.DeserializeObject<Domain.Movidesk.Dtos.Reads.Services.Service>(reread);

                        if (changed is not null && ConverterValidatorsExtensions.ConvertToInt32Validation(changed.id) == serviceId)
                            await SaveServices(doc_company, _movideskService.GetBatchTime(), [changed], new Dictionary<int, string> { { serviceId.Value, reread! } });
                        else
                            localMessage = $"Serviço {serviceId} alterado, mas a releitura não trouxe o serviço (o GetServices grava depois): {reread}";
                    }
                }
                catch (Exception ex)
                {
                    // a escrita na Movidesk continua valendo; o GetServices corrige depois (o hash já saiu do cache)
                    localMessage = $"Serviço {serviceId} gravado na Movidesk, mas não no banco (o GetServices grava depois): {ex.Message}";
                }

                if (localMessage is not null)
                    _logger.AddMessage(localMessage);

                _logger.AddMessage($"Concluída com sucesso: serviço {serviceId?.ToString() ?? "(id não informado pela API)"} ({method.Method} {route}).");

                return new Domain.Movidesk.Dtos.Writes.Result
                {
                    success = true,
                    id = serviceId?.ToString(),
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
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Devolve a quantidade de serviços gravados.
        /// </summary>
        private async Task<int> SaveServices(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Movidesk.Dtos.Reads.Services.Service> services,
            IReadOnlyDictionary<int, string> servicesJson)
        {
            var _listSomenteNovos = new List<Domain.Movidesk.Entities.Reads.Services.Service>();

            foreach (var service in services.Where(x => !String.IsNullOrEmpty(x.id)).GroupBy(x => x.id).Select(g => g.Last()))
            {
                var validations = _serviceValidator.Validate(service);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"serviço {service.id}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Movidesk.Entities.Reads.Services.Service(doc_company, batchTime, service));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _movideskServicesRepository.BulkInsertIntoMovideskServices(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (servicesJson.TryGetValue(record.id, out var json))
                    _logger.AddRecord(record.id.ToString(), json);
            }

            return _listSomenteNovos.Count;
        }
    }
}

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
    /// catálogo de serviços da Movidesk (GET /services)
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

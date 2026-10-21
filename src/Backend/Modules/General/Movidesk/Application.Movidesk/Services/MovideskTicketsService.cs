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
    public class MovideskTicketsService : IMovideskTicketsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly IMovideskService _movideskService;
        private readonly IMovideskRepository _movideskRepository;
        private readonly IMovideskTicketsRepository _movideskTicketsRepository;
        private readonly IValidator<Domain.Movidesk.Dtos.Reads.Tickets.Ticket> _ticketValidator;

        private const string TicketsKey = "Movidesk:Tickets";

        // limite da API: 10 requisições por minuto -> 1 requisição a cada 6 s
        private const int RequestIntervalInMilliseconds = 6 * 1000;
        private const int PageSize = 100;

        // $select é obrigatório na listagem: campos do ticket, sem as listas
        private const string TicketsSelect =
            "id,protocol,type,subject,category,urgency,status,baseStatus,justification,origin,createdDate," +
            "isDeleted,originEmailAccount,ownerTeam,serviceFirstLevelId,serviceFirstLevel,serviceSecondLevel," +
            "serviceThirdLevel,contactForm,tags,cc,resolvedIn,closedIn,canceledIn,actionCount,lifeTimeWorkingTime," +
            "stoppedTime,stoppedTimeWorkingTime,resolvedInFirstCall,chatWidget,chatGroup,chatTalkTime,chatWaitingTime," +
            "sequence,slaAgreement,slaAgreementRule,slaSolutionTime,slaResponseTime,slaSolutionChangedByUser," +
            "slaSolutionDate,slaSolutionDateIsPaused,jiraIssueKey,redmineIssueId,reopenedIn,lastActionDate,lastUpdate," +
            "slaResponseDate,slaRealResponseDate";

        // as listas vêm pelo $expand (o aninhamento de mais de um nível não aparece na documentação: conferir na primeira chamada)
        private const string TicketsExpand =
            "owner,createdBy,slaSolutionChangedBy," +
            "clients($expand=organization)," +
            "actions($expand=createdBy,timeAppointments($expand=createdBy,createdByTeam),expenses($expand=createdBy,createdByTeam),attachments($expand=createdBy))," +
            "parentTickets,childrenTickets," +
            "ownerHistories($expand=owner,changedBy),statusHistories($expand=changedBy)," +
            "customFieldValues($expand=items),assets";

        public MovideskTicketsService(
            [FromKeyedServices("MovideskAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMovideskService movideskService,
            IMovideskRepository movideskRepository,
            IMovideskTicketsRepository movideskTicketsRepository,
            IValidator<Domain.Movidesk.Dtos.Reads.Tickets.Ticket> ticketValidator,
            IMemoryCache cache
        ) =>
            (_apiClient, _logger, _movideskService, _movideskRepository, _movideskTicketsRepository, _ticketValidator, _cache) =
            (apiClient, logger, movideskService, movideskRepository, movideskTicketsRepository, ticketValidator, cache);

        /// <summary>
        /// busca um ticket por número, grava nas tabelas de tickets (sempre, sem consultar o cache)
        /// e registra a chamada em general.MovideskRequests
        /// </summary>
        public async Task<bool?> GetTicket(string user_name, string doc_company, long id)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.MovideskGetTicket);

            // rota registrada no log de requisições: sem o token
            var route = $"tickets?id={id}";
            bool? result;

            try
            {
                var company = await _movideskRepository.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao AfterSale)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em general.MovideskParameters.");
                    return null;
                }

                // mesmo horário de lote para o ticket e as filhas (condição de exclusão do merge)
                var batchTime = _movideskService.GetBatchTime();

                string? response;

                try
                {
                    // por id a API já devolve todas as listas e o htmlDescription: sem $select/$expand
                    response = await _apiClient.GetAsync(
                        route: "tickets",
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
                    _movideskService.SaveRequest(EnumJob.MovideskGetTicket, user_name, "GET", route, doc_company, id.ToString(), $"Exceção ao chamar a API: {ex.Message}", false);
                    throw;
                }

                if (String.IsNullOrWhiteSpace(response))
                {
                    // a API responde 404 com corpo vazio quando o ticket não existe
                    _logger.AddMessage($"Ticket {id} não encontrado na Movidesk (empresa {doc_company}).");
                    result = false;
                }
                else
                {
                    Domain.Movidesk.Dtos.Reads.Tickets.Ticket? ticket = null;

                    try
                    {
                        ticket = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>(response);
                    }
                    catch (Newtonsoft.Json.JsonException)
                    {
                        // ticket continua null: tratado abaixo como resposta inválida
                    }

                    if (ticket is null || ConverterValidatorsExtensions.ConvertToInt64Validation(ticket.id) != id)
                    {
                        _logger.AddMessage($"Resposta inválida da API Movidesk para o ticket {id} (empresa {doc_company}): {response}");
                        result = null;
                    }
                    else
                    {
                        // atualiza o cache do GetTickets, se ele estiver carregado, para a próxima execução não regravar o ticket
                        _cache.TryGetValue($"{TicketsKey}:{doc_company}", out Dictionary<long, Domain.Movidesk.Entities.Reads.Tickets.Ticket>? ticketsCache);

                        await SaveTickets(doc_company, batchTime, [ticket], new Dictionary<long, string> { { id, response } }, ticketsCache);

                        _logger.AddMessage($"Concluída com sucesso: ticket {id} inserido ou atualizado!");
                        result = true;
                    }
                }

                _movideskService.SaveRequest(EnumJob.MovideskGetTicket, user_name, "GET", route, doc_company, id.ToString(), response, result == true);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// sincroniza os tickets atualizados desde a última execução, para cada empresa
        /// </summary>
        public async Task<bool?> GetTickets()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.MovideskGetTickets);

            var totalInseridos = 0;
            var companys = await _movideskRepository.GetCompanys();

            foreach (var company in companys)
            {
                if (company?.doc_company is null)
                    continue;

                // um horário por lote: ticket, filhas e netas gravam o mesmo valor (condição de exclusão do merge)
                var batchTime = _movideskService.GetBatchTime();

                // lastUpdate é UTC na API; 1 hora de folga para o atraso de replicação. Banco vazio: /tickets só traz até 90 dias
                var lastUpdate = await _movideskTicketsRepository.GetTicketsLastUpdate(company.doc_company);
                var since = lastUpdate.HasValue ? lastUpdate.Value.AddHours(-1) : DateTime.UtcNow.AddDays(-89);

                var tickets = new List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>();
                var ticketsJson = new Dictionary<long, string>();
                var skip = 0;

                while (true)
                {
                    var response = await _apiClient.GetAsync(
                        route: "tickets",
                        queryParams: new Dictionary<string, string>
                        {
                            { "token", company.Token.ToString() },
                            { "$select", TicketsSelect },
                            { "$expand", TicketsExpand },
                            { "$filter", $"lastUpdate ge {since:yyyy-MM-ddTHH:mm:ss}.00z" },
                            { "$orderby", "id asc" },
                            { "$top", PageSize.ToString() },
                            { "$skip", skip.ToString() }
                        }
                    );

                    List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>? page;

                    try
                    {
                        page = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>>(response ?? String.Empty);
                    }
                    catch (Newtonsoft.Json.JsonException)
                    {
                        // a API devolve um objeto de erro (ou 429 Too many failed requests) em vez da lista;
                        // 3 erros seguidos bloqueiam a API, por isso a empresa é interrompida em vez de tentar de novo
                        _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}, skip {skip}): {response}");
                        break;
                    }

                    if (page is null || page.Count == 0)
                        break;

                    tickets.AddRange(page);

                    foreach (var item in JArray.Parse(response!))
                    {
                        var id = ConverterValidatorsExtensions.ConvertToInt64Validation((string?)item["id"]);

                        if (id != 0)
                            ticketsJson[id] = item.ToString();
                    }

                    if (page.Count < PageSize)
                        break;

                    skip += PageSize;
                    Thread.Sleep(RequestIntervalInMilliseconds);
                }

                if (tickets.Count == 0)
                    continue;

                // cache de estado: id -> ticket gravado (com lastUpdate), por empresa
                var ticketsCache = await _cache.GetOrCreateAsync($"{TicketsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _movideskTicketsRepository.GetTicketsByIds(
                        company.doc_company,
                        tickets.Select(x => ConverterValidatorsExtensions.ConvertToInt64Validation(x.id)).Distinct().ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!);
                });

                // mantém só os novos ou com lastUpdate mais recente que o gravado
                tickets.RemoveAll(x =>
                    !string.IsNullOrEmpty(x.id) &&
                    ticketsCache!.TryGetValue(ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), out var y) &&
                    NullableConverters.ToNullableDateTime(x.lastUpdate) <= y.lastUpdate
                );

                totalInseridos += await SaveTickets(company.doc_company, batchTime, tickets, ticketsJson, ticketsCache);
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), preserva o htmlDescription já gravado,
        /// grava e atualiza o cache da empresa. Devolve a quantidade de tickets gravados.
        /// </summary>
        private async Task<int> SaveTickets(
            string doc_company,
            DateTime batchTime,
            IEnumerable<Domain.Movidesk.Dtos.Reads.Tickets.Ticket> tickets,
            IReadOnlyDictionary<long, string> ticketsJson,
            Dictionary<long, Domain.Movidesk.Entities.Reads.Tickets.Ticket>? ticketsCache)
        {
            var _listSomenteNovos = new List<Domain.Movidesk.Entities.Reads.Tickets.Ticket>();

            // o mesmo ticket pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
            foreach (var ticket in tickets.GroupBy(x => x.id).Select(g => g.Last()))
            {
                var validations = _ticketValidator.Validate(ticket);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"ticket {ticket.id}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.Movidesk.Entities.Reads.Tickets.Ticket(doc_company, batchTime, ticket));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            // a listagem não traz htmlDescription: mantém o que já está gravado em vez de sobrescrever com null
            var stored = (await _movideskTicketsRepository.GetActionsHtmlDescriptions(doc_company, _listSomenteNovos.Select(x => x.id).ToList()))
                .Where(x => x != null)
                .ToDictionary(x => (x!.ticketId, x.id), x => x!.htmlDescription);

            foreach (var action in _listSomenteNovos.SelectMany(x => x.actions))
                if (stored.TryGetValue((action.ticketId, action.id), out var html))
                    action.KeepHtmlDescription(html);

            await _movideskTicketsRepository.BulkInsertIntoMovideskTickets(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (ticketsJson.TryGetValue(record.id, out var json))
                    _logger.AddRecord(record.id.ToString(), json);

                if (ticketsCache is not null)
                    ticketsCache[record.id] = record;
            }

            return _listSomenteNovos.Count;
        }
    }
}

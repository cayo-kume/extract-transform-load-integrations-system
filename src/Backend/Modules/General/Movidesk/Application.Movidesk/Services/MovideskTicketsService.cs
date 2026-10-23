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
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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

        // limite da API: 10 requisições por minuto -> 1 requisição a cada 6 s (contados do início da chamada anterior)
        private const int RequestIntervalInMilliseconds = 6 * 1000;
        // 100 tickets por bloco (faixa de ids): com o $expand em grupos, cada chamada responde em 2 a 8 s (medido em 21/10/2026);
        // $top com folga para tickets que entram no filtro durante a leitura
        private const int PageSize = 100;
        private const int BlockTop = 150;
        // a fase 1 lista só id e lastUpdate; a API devolve no máximo 1.000 por chamada
        private const int IdsPageSize = 1000;

        // $select é obrigatório na listagem: campos do ticket, sem as listas
        private const string TicketsSelect =
            "id,protocol,type,subject,category,urgency,status,baseStatus,justification,origin,createdDate," +
            "isDeleted,originEmailAccount,ownerTeam,serviceFirstLevelId,serviceFirstLevel,serviceSecondLevel," +
            "serviceThirdLevel,contactForm,tags,cc,resolvedIn,closedIn,canceledIn,actionCount,lifeTimeWorkingTime," +
            "stoppedTime,stoppedTimeWorkingTime,resolvedInFirstCall,chatWidget,chatGroup,chatTalkTime,chatWaitingTime," +
            "sequence,slaAgreement,slaAgreementRule,slaSolutionTime,slaResponseTime,slaSolutionChangedByUser," +
            "slaSolutionDate,slaSolutionDateIsPaused,jiraIssueKey,redmineIssueId,reopenedIn,lastActionDate,lastUpdate," +
            "slaResponseDate,slaRealResponseDate";

        // as listas vêm pelo $expand, dividido em grupos: tudo numa chamada leva ~100 s e chega a dar 504;
        // em grupos, ~24 s por bloco de 100 (medido em 21/10/2026). O grupo 1 traz os campos; os outros, só id e as listas.
        // expenses($expand=createdByTeam) faz a API devolver 500: nas despesas só createdBy é expandido
        private static readonly (string select, string expand)[] TicketsExpandGroups =
        [
            (TicketsSelect, "owner,createdBy,slaSolutionChangedBy,parentTickets,childrenTickets"),
            ("id", "actions($expand=createdBy,timeAppointments($expand=createdBy,createdByTeam),expenses($expand=createdBy),attachments($expand=createdBy))"),
            ("id", "clients($expand=organization),assets"),
            ("id", "ownerHistories($expand=owner,changedBy),statusHistories($expand=changedBy)"),
            ("id", "customFieldValues($expand=items)"),
        ];

        // escrita: campos nulos não são enviados (no PATCH, campo ausente = campo que não muda); datas no formato da Movidesk
        private static readonly JsonSerializerSettings WriteSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss"
        };

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
        /// e registra a chamada em movidesk.MovideskRequests
        /// </summary>
        public async Task<bool?> GetTicketById(string user_name, string doc_company, long id)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.MovideskGetTicketById);

            // rota registrada no log de requisições: sem o token
            var route = $"tickets?id={id}";
            bool? result;

            try
            {
                var company = await _movideskRepository.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao AfterSale)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em movidesk.MovideskParameters.");
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
                    _movideskService.SaveRequest(EnumJob.MovideskGetTicketById, user_name, "GET", route, doc_company, id.ToString(), $"Exceção ao chamar a API: {ex.Message}", false);
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

                _movideskService.SaveRequest(EnumJob.MovideskGetTicketById, user_name, "GET", route, doc_company, id.ToString(), response, result == true);

                return result;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// sincroniza os tickets atualizados desde a última execução, para cada empresa.
        /// Fase 1: lista id e lastUpdate da janela. Fase 2: busca só os alterados, em blocos por faixa de ids,
        /// com o $expand dividido em grupos, e grava bloco a bloco.
        /// </summary>
        public async Task<bool?> GetTickets()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.MovideskGetTickets);

            var totalInseridos = 0;
            var companys = await _movideskRepository.GetCompanys();

            // início da última chamada à API: os 6 s do limite contam a partir dele
            var lastRequestAt = DateTime.MinValue;

            foreach (var company in companys)
            {
                if (company?.doc_company is null)
                    continue;

                // lastUpdate é UTC na API; 1 hora de folga para o atraso de replicação. Banco vazio: /tickets só traz até 90 dias
                var lastUpdate = await _movideskTicketsRepository.GetTicketsLastUpdate(company.doc_company);
                var since = lastUpdate.HasValue ? lastUpdate.Value.AddHours(-1) : DateTime.UtcNow.AddDays(-89);

                // fase 1: id e lastUpdate dos tickets da janela, em ordem de id (1.000 por chamada, o máximo da API)
                var listed = new List<(long id, DateTime? lastUpdate)>();
                var listFailed = false;
                var skip = 0;

                while (true)
                {
                    // espera o que falta para 6 s desde o início da chamada anterior (limite de 10 requisições por minuto)
                    var wait = lastRequestAt.AddMilliseconds(RequestIntervalInMilliseconds) - DateTime.UtcNow;

                    if (wait > TimeSpan.Zero)
                        await Task.Delay(wait);

                    lastRequestAt = DateTime.UtcNow;

                    var response = await _apiClient.GetAsync(
                        route: "tickets",
                        queryParams: new Dictionary<string, string>
                        {
                            { "token", company.Token.ToString() },
                            { "$select", "id,lastUpdate" },
                            { "$filter", $"lastUpdate ge {since:yyyy-MM-ddTHH:mm:ss}.00z" },
                            { "$orderby", "id asc" },
                            { "$top", IdsPageSize.ToString() },
                            { "$skip", skip.ToString() }
                        }
                    );

                    List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>? page;

                    try
                    {
                        page = JsonConvert.DeserializeObject<List<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>>(response ?? String.Empty);
                    }
                    catch (JsonException)
                    {
                        // a API devolve um objeto de erro (ou 429 Too many failed requests) em vez da lista
                        _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}, lista de ids, skip {skip}): {response}");
                        listFailed = true;
                        break;
                    }

                    if (page is null || page.Count == 0)
                        break;

                    listed.AddRange(page
                        .Select(x => (id: ConverterValidatorsExtensions.ConvertToInt64Validation(x.id), lastUpdate: NullableConverters.ToNullableDateTime(x.lastUpdate)))
                        .Where(x => x.id != 0));

                    if (page.Count < IdsPageSize)
                        break;

                    skip += IdsPageSize;
                }

                // erro na lista de ids: nada foi buscado ainda, a empresa fica para a próxima execução
                if (listFailed || listed.Count == 0)
                    continue;

                // um ticket alterado durante a leitura pode aparecer em duas páginas
                listed = listed
                    .GroupBy(x => x.id)
                    .Select(g => g.Last())
                    .OrderBy(x => x.id)
                    .ToList();

                // cache de estado: id -> ticket gravado (com lastUpdate), por empresa
                var ticketsCache = (await _cache.GetOrCreateAsync($"{TicketsKey}:{company.doc_company}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _movideskTicketsRepository.GetTicketsByIds(company.doc_company, listed.Select(x => x.id).ToList());

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x!.id)
                        .ToDictionary(g => g.Key, g => g.First()!);
                }))!;

                // só os novos ou com lastUpdate mais recente que o gravado vão para as chamadas caras
                var changed = listed
                    .Where(x => !(ticketsCache.TryGetValue(x.id, out var y) && x.lastUpdate <= y.lastUpdate))
                    .Select(x => x.id)
                    .ToHashSet();

                // monta os blocos da fase 2 a partir de todos os ids da fase 1 (em ordem). Cada bloco começa num ticket
                // alterado, tem no máximo PageSize ids e termina no último alterado da janela; assim a faixa
                // id ge {primeiro} and id le {último} nunca tem mais de PageSize tickets do filtro.
                var ids = listed.Select(x => x.id).ToList();
                var blocks = new List<List<long>>();
                var i = 0;

                while (i < ids.Count)
                {
                    if (!changed.Contains(ids[i]))
                    {
                        i++;
                        continue;
                    }

                    var window = ids.Skip(i).Take(PageSize).ToList();
                    var lastChanged = window.FindLastIndex(changed.Contains);

                    blocks.Add(window.Take(lastChanged + 1).ToList());
                    i += lastChanged + 1;
                }

                var apiFailed = false;

                foreach (var block in blocks)
                {
                    // fase 2: busca o bloco com uma chamada por grupo do $expand e junta as respostas por id
                    var filter = $"id ge {block[0]} and id le {block[^1]} and lastUpdate ge {since:yyyy-MM-ddTHH:mm:ss}.00z";
                    var merged = new Dictionary<long, JObject>();
                    var seen = new Dictionary<long, int>();
                    var blockFull = false;

                    foreach (var (select, expand) in TicketsExpandGroups)
                    {
                        // espera o que falta para 6 s desde o início da chamada anterior (limite de 10 requisições por minuto)
                        var wait = lastRequestAt.AddMilliseconds(RequestIntervalInMilliseconds) - DateTime.UtcNow;

                        if (wait > TimeSpan.Zero)
                            await Task.Delay(wait);

                        lastRequestAt = DateTime.UtcNow;

                        var response = await _apiClient.GetAsync(
                            route: "tickets",
                            queryParams: new Dictionary<string, string>
                            {
                                { "token", company.Token.ToString() },
                                { "$select", select },
                                { "$expand", expand },
                                { "$filter", filter },
                                { "$orderby", "id asc" },
                                { "$top", BlockTop.ToString() }
                            }
                        );

                        JArray page;

                        try
                        {
                            // lê a resposta mantendo datas e decimais como vieram (o DTO é todo string); o JArray.Parse padrão
                            // converteria as datas e mudaria o formato. Lança JsonReaderException quando a resposta não é uma lista.
                            using var reader = new JsonTextReader(new StringReader(response ?? String.Empty))
                            {
                                DateParseHandling = DateParseHandling.None,
                                FloatParseHandling = FloatParseHandling.Decimal
                            };

                            page = JArray.Load(reader);
                        }
                        catch (JsonReaderException)
                        {
                            _logger.AddMessage($"Resposta inválida da API Movidesk (empresa {company.doc_company}, tickets {block[0]} a {block[^1]}, $expand {expand}): {response}");
                            apiFailed = true;
                            break;
                        }

                        // $top cheio: pode ter ficado ticket de fora; o bloco é remontado na próxima execução
                        if (page.Count >= BlockTop)
                        {
                            _logger.AddMessage($"Bloco de tickets {block[0]} a {block[^1]} (empresa {company.doc_company}) com {page.Count} registros: não gravado, será remontado na próxima execução.");
                            blockFull = true;
                            break;
                        }

                        foreach (var item in page.OfType<JObject>())
                        {
                            var id = ConverterValidatorsExtensions.ConvertToInt64Validation((string?)item["id"]);

                            // tickets que não mudaram, ou que entraram no filtro depois da fase 1 (chegam na próxima execução)
                            if (!changed.Contains(id))
                                continue;

                            if (merged.TryGetValue(id, out var ticket))
                                ticket.Merge(item, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace });
                            else
                                merged[id] = item;

                            seen[id] = seen.GetValueOrDefault(id) + 1;
                        }
                    }

                    // a API devolveu erro: interrompe a empresa em vez de tentar de novo (3 erros seguidos bloqueiam a API).
                    // Os blocos anteriores já foram gravados.
                    if (apiFailed)
                        break;

                    if (blockFull)
                        continue;

                    // ticket sem algum grupo não é gravado: o merge apagaria as filhas que faltaram. Volta na próxima execução,
                    // porque o lastUpdate gravado continua antigo
                    foreach (var id in merged.Keys.Where(id => seen[id] < TicketsExpandGroups.Length).ToList())
                    {
                        _logger.AddMessage($"Ticket {id} (empresa {company.doc_company}) não veio em todos os grupos do $expand: não gravado nesta execução.");
                        merged.Remove(id);
                    }

                    if (merged.Count == 0)
                        continue;

                    var tickets = merged.Values.Select(x => x.ToObject<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>()!).ToList();
                    var ticketsJson = merged.ToDictionary(x => x.Key, x => x.Value.ToString());

                    // um horário de lote por bloco: ticket, filhas e netas gravam o mesmo valor (condição de exclusão do merge)
                    totalInseridos += await SaveTickets(company.doc_company, _movideskService.GetBatchTime(), tickets, ticketsJson, ticketsCache);
                }
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        /// <summary>
        /// cria um ticket. Com returnAllProperties=true, se a API devolver o ticket inteiro ele é gravado sem releitura;
        /// se devolver só o número, o ticket chega pela próxima execução do GetTickets
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostTicket(string user_name, string doc_company, Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket)
        {
            var body = JsonConvert.SerializeObject(ticket, WriteSettings);

            return SendTicket(EnumJob.MovideskPostTicket, user_name, doc_company, null, HttpMethod.Post, "tickets",
                query: new Dictionary<string, string> { { "returnAllProperties", "true" } },
                requestBody: body,
                content: () => new StringContent(body, Encoding.UTF8, "application/json"),
                readResponse: response =>
                {
                    // o formato da resposta não está na documentação (a conferir no teste real):
                    // o ticket inteiro, o número puro ou { "id": ... }
                    JToken? json = null;

                    try
                    {
                        json = String.IsNullOrWhiteSpace(response) ? null : JToken.Parse(response);
                    }
                    catch (JsonReaderException)
                    {
                        // json continua null: o número fica desconhecido e o GetTickets traz o ticket depois
                    }

                    if (json is JObject full && (full.ContainsKey("subject") || full.ContainsKey("actions")))
                    {
                        // nesta resposta as datas vêm em UTC com "Z" (ex.: 2026-10-22T15:58:42.83Z), e o "Z" faria a conversão
                        // passar para o horário local; na listagem e no GET por id elas vêm em UTC sem o sufixo. Tirar o "Z"
                        // deixa o ticket igual ao que o GetTickets grava (testado em 22/10/2026, ticket 69369)
                        var created = JsonConvert.DeserializeObject<Domain.Movidesk.Dtos.Reads.Tickets.Ticket>(
                            Regex.Replace(response!, @"(""\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?)Z""", "$1\"")
                        );
                        var createdId = ConverterValidatorsExtensions.ConvertToInt64Validation(created?.id);

                        return (createdId > 0 ? createdId : null, created);
                    }

                    var number = ConverterValidatorsExtensions.ConvertToInt64Validation(json is JObject idOnly ? (string?)idOnly["id"] : json?.ToString());

                    return (number > 0 ? number : null, null);
                });
        }

        /// <summary>
        /// altera um ticket: só os campos enviados mudam, e listas enviadas substituem a lista inteira na Movidesk.
        /// A alteração chega ao banco pela próxima execução do GetTickets
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PatchTicketById(string user_name, string doc_company, long id, Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket)
        {
            var body = JsonConvert.SerializeObject(ticket, WriteSettings);

            return SendTicket(EnumJob.MovideskPatchTicketById, user_name, doc_company, id, HttpMethod.Patch, "tickets",
                query: new Dictionary<string, string> { { "id", id.ToString() } },
                requestBody: body,
                content: () => new StringContent(body, Encoding.UTF8, "application/json"),
                readResponse: _ => (id, null));
        }

        /// <summary>
        /// envia anexos para uma ação, todos numa chamada. No log vão só os metadados e o hash de cada arquivo
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostTicketFileUploadById(string user_name, string doc_company, long id, int actionId, IReadOnlyList<(string fileName, string contentType, byte[] content)> files)
        {
            // a Movidesk guarda o arquivo: no log bastam nome, tipo, tamanho e SHA-256
            var requestBody = JsonConvert.SerializeObject(files.Select(x => new
            {
                x.fileName,
                x.contentType,
                size = x.content.Length,
                sha256 = Convert.ToHexString(SHA256.HashData(x.content)).ToLowerInvariant()
            }));

            return SendTicket(EnumJob.MovideskPostTicketFileUploadById, user_name, doc_company, id, HttpMethod.Post, "ticketFileUpload",
                query: new Dictionary<string, string> { { "id", id.ToString() }, { "actionId", actionId.ToString() } },
                requestBody: requestBody,
                content: () =>
                {
                    var multipart = new MultipartFormDataContent();

                    foreach (var (fileName, contentType, content) in files)
                    {
                        var file = new ByteArrayContent(content);
                        file.Headers.ContentType = MediaTypeHeaderValue.Parse(String.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);

                        // a documentação não informa o nome do campo (a conferir no teste real)
                        multipart.Add(file, "file", fileName);
                    }

                    return multipart;
                },
                readResponse: _ => (id, null));
        }

        /// <summary>
        /// fluxo comum das rotas de escrita de tickets: empresa, chamada à API, registro em movidesk.MovideskRequests
        /// (com o corpo enviado) e, quando readResponse devolve o ticket inteiro, gravação com o SaveTickets
        /// </summary>
        private async Task<Domain.Movidesk.Dtos.Writes.Result> SendTicket(
            EnumJob job,
            string user_name,
            string doc_company,
            long? id,
            HttpMethod method,
            string route,
            Dictionary<string, string> query,
            string requestBody,
            Func<HttpContent> content,
            Func<string?, (long? ticketId, Domain.Movidesk.Dtos.Reads.Tickets.Ticket? ticket)> readResponse)
        {
            _logger
                .Clear()
                .AddLog(job);

            // rota registrada no log de requisições: sem o token
            var logRoute = $"{route}?{String.Join("&", query.Select(x => $"{x.Key}={x.Value}"))}";

            try
            {
                var company = await _movideskRepository.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em MovideskRequests (igual ao GetTicketById)
                    var notFound = $"Empresa {doc_company} não cadastrada em movidesk.MovideskParameters.";
                    _logger.AddMessage(notFound);

                    return new Domain.Movidesk.Dtos.Writes.Result { success = false, id = id?.ToString(), message = notFound };
                }

                ApiResponse response;

                try
                {
                    using var httpContent = content();

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

                var (ticketId, ticket) = response.IsSuccess ? readResponse(response.Content) : (id, null);

                _movideskService.SaveRequest(job, user_name, method.Method, logRoute, doc_company, ticketId?.ToString() ?? String.Empty, response.Content, response.IsSuccess, requestBody);

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
                    _logger.AddMessage($"Ticket {ticketId?.ToString() ?? "novo"} (empresa {doc_company}): {message}");

                    return new Domain.Movidesk.Dtos.Writes.Result
                    {
                        success = false,
                        id = ticketId?.ToString(),
                        statusCode = response.StatusCode,
                        message = message,
                        response = response.Content,
                        retryAfterSeconds = response.Headers.TryGetValue("Retry-After", out var retryAfter) && int.TryParse(retryAfter, out var seconds) ? seconds : null
                    };
                }

                if (ticket is not null && ticketId is not null)
                {
                    // ticket inteiro na resposta: grava agora, sem releitura. Se falhar, a escrita continua valendo e o GetTickets corrige
                    try
                    {
                        _cache.TryGetValue($"{TicketsKey}:{doc_company}", out Dictionary<long, Domain.Movidesk.Entities.Reads.Tickets.Ticket>? ticketsCache);

                        await SaveTickets(doc_company, _movideskService.GetBatchTime(), [ticket], new Dictionary<long, string> { { ticketId.Value, response.Content! } }, ticketsCache);
                    }
                    catch (Exception ex)
                    {
                        _logger.AddMessage($"Ticket {ticketId} criado, mas não gravado no banco (o GetTickets grava depois): {ex.Message}");
                    }
                }

                _logger.AddMessage($"Concluída com sucesso: ticket {ticketId?.ToString() ?? "(número não informado pela API)"} ({method.Method} {route}).");

                return new Domain.Movidesk.Dtos.Writes.Result
                {
                    success = true,
                    id = ticketId?.ToString(),
                    statusCode = response.StatusCode,
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

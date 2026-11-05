using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// Implementação falsa do IOnDemandRunner. O catálogo sai das integrações sob demanda do MockCatalog: o verbo e a rota
    /// estão na descrição de cada job ("(GET Tickets/{docCompany}/{id})"), e as que não trazem a rota completa estão em
    /// RouteOverrides, copiadas dos controllers do host. A chamada não sai do WebApp: a resposta é simulada e, quando o host
    /// teria rodado a integração, vira uma execução do job na MockTimeline.
    /// </summary>
    internal sealed partial class MockOnDemandRunner(MockTimeline timeline, IConfiguration configuration) : IOnDemandRunner
    {
        private const string User = "operador";
        private const int MaxCallsPerJob = 20;

        [GeneratedRegex(@"\s*\((GET|POST|PUT|PATCH|DELETE) ([^)]*)\)\s*$")]
        private static partial Regex VerbAndRoute();

        [GeneratedRegex(@"\{(\w+)\}")]
        private static partial Regex RouteParameter();

        // prefixo das rotas de cada aplicação no host ([Route] dos controllers)
        private static readonly Dictionary<int, string> RoutePrefixes = new()
        {
            [30] = "MovideskJobs/Movidesk",
            [50] = "AfterSaleJobs/AfterSale",
            [70] = "LinxCommerceJobs/LinxCommerce",
            [80] = "LinxMicrovixJobs/LinxMicrovix",
            [90] = "DataWarehouseJobs/DataWarehouse",
            [130] = "TotvsJobs/Totvs"
        };

        // jobs cuja descrição não traz o verbo ou a rota inteira; copiado dos controllers (os da Totvs com "..." são a rota
        // provável, a confirmar no swagger.json do host na fase 5)
        private static readonly Dictionary<int, (string Verb, string Route)> RouteOverrides = new()
        {
            [5005] = ("POST", "Reverses/{docCompany}/{id}/Approve"),
            [5006] = ("PUT", "Reverses/{docCompany}/{id}/ReceiveProducts"),
            [5007] = ("PUT", "Reverses/{docCompany}/{id}/ReturnedInvoice"),
            [5008] = ("POST", "Reverses/{docCompany}/{id}/Invoice"),
            [5017] = ("POST", "Refunds/{docCompany}/{id}/Approve"),
            [5018] = ("POST", "Refunds/{docCompany}/{id}/Cancel"),
            [5019] = ("POST", "Refunds/{docCompany}/{id}/Pay"),
            [5020] = ("POST", "Refunds/{docCompany}/{id}/ChangeTotalAmount"),
            [5021] = ("POST", "Reverses/{docCompany}/{id}/AdministrativeRecords"),
            [5022] = ("GET", "Reverses/{docCompany}/{id}"),
            [5023] = ("GET", "Refunds/{docCompany}/{id}"),
            [9003] = ("POST", "RunMonthlyStock"),
            [9004] = ("POST", "RunLinxLegacy"),
            [13193] = ("GET", "SalesOrders/{docCompany}/{branchCode}/{orderCode}/ChangedBatchItems"),
            [13195] = ("GET", "SalesOrders/{docCompany}/{branchCode}/{orderCode}/Invoices"),
            [13196] = ("GET", "SalesOrders/{docCompany}/{branchCode}/{orderCode}/PendingItems"),
            [13255] = ("GET", "Invoices/{docCompany}/{branchCode}/{invoiceSequence}/Items")
        };

        // POST sem corpo no host: aprovar e cancelar reembolso só levam o id
        private static readonly HashSet<int> WithoutBody = [5017, 5018];

        private static readonly Dictionary<string, (string Type, string Description)> KnownParameters = new()
        {
            ["id"] = ("integer (≥ 1)", "Identificador no sistema de origem"),
            ["code"] = ("string", "Código no sistema de origem"),
            ["branchCode"] = ("integer", "Código da loja (empresa) na Totvs"),
            ["branchId"] = ("string", "Código ou CNPJ da loja"),
            ["orderCode"] = ("integer", "Número do pedido"),
            ["invoiceSequence"] = ("integer", "Sequência da nota fiscal"),
            ["accessKey"] = ("string (44 dígitos)", "Chave de acesso da NF-e"),
            ["cnpjEmp"] = ("string (14 dígitos)", "CNPJ da loja"),
            ["orderId"] = ("uuid", "OrderID do pedido"),
            ["orderNumber"] = ("string", "Número do pedido"),
            ["actionId"] = ("integer", "Ação do ticket"),
            ["personCode"] = ("integer", "Código da pessoa"),
            ["sequence"] = ("integer", "Sequência da mensagem"),
            ["productCode"] = ("integer", "Código do produto")
        };

        private const string TicketExample = """
            {
              "type": 2,
              "subject": "Pedido 12345 sem nota fiscal",
              "category": "Pedido",
              "urgency": "Média",
              "ownerTeam": "Atendimento",
              "createdBy": { "id": "string" },
              "clients": [ { "id": "string" } ],
              "actions": [ { "type": 2, "description": "string" } ],
              "tags": [ "string" ]
            }
            """;

        private const string TicketModel = """
            Ticket {
              type          integer   1 interno, 2 público
              subject       string    assunto (obrigatório)
              category      string
              urgency       string
              status        string
              ownerTeam     string
              createdBy     PersonReference { id: string } (obrigatório)
              clients       [ PersonReference ]
              actions       [ Action { type: integer, description: string } ]
              tags          [ string ]
            }
            """;

        private const string GenericExample = """
            {
            }
            """;

        private const string GenericModel = "Nesta fase, os campos do corpo não estão no painel: na fase 5 o modelo vem do swagger.json do host.";

        private readonly Lazy<IReadOnlyList<OnDemandOperation>> _operations = new(() => Build(timeline, configuration["LoggedCompany:DocCompany"] is { Length: > 0 } doc ? doc : "44065634000106"));
        private readonly ConcurrentDictionary<int, ConcurrentQueue<OnDemandCall>> _calls = new();

        public async Task<IReadOnlyList<OnDemandOperation>> GetOperationsAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(Random.Shared.Next(150, 400), cancellationToken);
            return _operations.Value;
        }

        public async Task<OnDemandResult> ExecuteAsync(OnDemandOperation operation, IReadOnlyDictionary<string, string?> values, string? body, CancellationToken cancellationToken = default)
        {
            var started = DateTime.Now;
            await Task.Delay(Random.Shared.Next(400, 1500), cancellationToken);

            var job = timeline.Find(operation.IdJob, 2)
                ?? throw new InvalidOperationException($"Job {operation.IdJob} não existe no catálogo.");
            var duration = DateTime.Now - started;
            var parameters = string.Join(", ", operation.Parameters
                .Where(x => !x.IsLocked && !string.IsNullOrWhiteSpace(values.GetValueOrDefault(x.Name)))
                .Select(x => $"{x.Name}={values[x.Name]}"));

            OnDemandResult Respond(int status, string reason, string text, bool ran)
            {
                // o host só grava uma execução quando a integração chega a rodar (validação recusada não roda)
                var execution = ran ? timeline.AddManualRun(job, started) : (Guid?)null;
                var result = new OnDemandResult(status, reason, text, duration, started, execution);
                Record(new OnDemandCall(operation.IdJob, started, User, parameters, status, reason, execution));
                return result;
            }

            var missing = operation.Parameters.Where(x => x.Required && string.IsNullOrWhiteSpace(values.GetValueOrDefault(x.Name))).Select(x => x.Name).ToList();
            if (missing.Count > 0)
                return Respond(400, "Bad Request", JsonSerializer.Serialize(missing.Select(x => new { propertyName = x, errorMessage = "Obrigatório." })), ran: false);

            if (operation.HasBody)
            {
                try
                {
                    using var document = JsonDocument.Parse(body ?? "");
                    if (document.RootElement.ValueKind != JsonValueKind.Object || !document.RootElement.EnumerateObject().Any())
                        return Respond(400, "Bad Request", """[ { "propertyName": "", "errorMessage": "O corpo não pode ser vazio." } ]""", ran: false);
                }
                catch (JsonException exception)
                {
                    return Respond(400, "Bad Request", $"JSON inválido: {exception.Message}", ran: false);
                }
            }

            // um em cada seis GETs não acha o registro no sistema de origem, para a tela mostrar o 404
            if (operation.Verb == "GET" && (uint)parameters.GetHashCode() % 6 == 0)
                return Respond(404, "Not Found", $"{operation.Title}: {parameters} not found in {operation.AppTitle}.", ran: true);

            return operation.Writes
                ? Respond(200, "OK", $"{operation.Title}: request sent successfully to {operation.AppTitle}.", ran: true)
                : Respond(200, "OK", $"{operation.Title} ({parameters}) integrated successfully.", ran: true);
        }

        public Task<IReadOnlyList<OnDemandCall>> GetCallsAsync(int idJob, int top, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<OnDemandCall> calls = _calls.TryGetValue(idJob, out var queue)
                ? queue.Reverse().Take(top).ToList()
                : [];

            return Task.FromResult(calls);
        }

        private void Record(OnDemandCall call)
        {
            var queue = _calls.GetOrAdd(call.IdJob, _ => new ConcurrentQueue<OnDemandCall>());
            queue.Enqueue(call);
            while (queue.Count > MaxCallsPerJob)
                queue.TryDequeue(out _);
        }

        private static IReadOnlyList<OnDemandOperation> Build(MockTimeline timeline, string docCompany) => timeline.Jobs
            .Where(x => x.Catalog.IdType == 2 && x.Catalog.IntervalTimeInMinutes == 0 && x.Catalog.RunningDailyAt is null)
            .Select(x => ToOperation(x, timeline.Apps[x.IdApp].Title, docCompany))
            .OfType<OnDemandOperation>()
            .OrderBy(x => x.AppTitle)
            .ThenBy(x => x.Tag)
            .ThenBy(x => x.IdJob)
            .ToList();

        private static OnDemandOperation? ToOperation(MockJob job, string appTitle, string docCompany)
        {
            var catalog = job.Catalog;
            var match = VerbAndRoute().Match(catalog.Description ?? "");
            var description = match.Success ? catalog.Description![..match.Index] : catalog.Description;

            var (verb, route) = RouteOverrides.TryGetValue(catalog.IdJob, out var known)
                ? known
                : match.Success ? (match.Groups[1].Value, match.Groups[2].Value.Split(" e ")[0].Trim()) : ("POST", catalog.Title ?? "");

            var isWarehouse = job.IdApp == 90;
            var parameters = new List<OnDemandParameter>();

            // o DataWarehouse não leva usuário nem empresa: recarrega tudo
            if (!isWarehouse)
                parameters.Add(new OnDemandParameter("X-User-Name", ParameterLocation.Header, "string (até 100)", true, "Quem pediu; vai para o log da chamada", User));

            foreach (Match parameter in RouteParameter().Matches(route))
            {
                var name = parameter.Groups[1].Value;
                if (name == "docCompany")
                {
                    parameters.Add(new OnDemandParameter(name, ParameterLocation.Path, "string (14 dígitos)", true, "Empresa (a logada)", docCompany));
                    continue;
                }

                var (type, about) = KnownParameters.GetValueOrDefault(name, ("string", null!));
                parameters.Add(new OnDemandParameter(name, ParameterLocation.Path, type, true, about));
            }

            if (catalog.IdJob == 9003)
                parameters.Add(new OnDemandParameter("positionDate", ParameterLocation.Query, "date", false, "Fim de mês da posição; vazio recarrega o último mês fechado"));

            var pathParameters = parameters.Count(x => x.Location == ParameterLocation.Path && x.Name != "docCompany");
            var hasBody = !isWarehouse && !WithoutBody.Contains(catalog.IdJob)
                && (verb is "POST" or "PUT" or "PATCH" || (verb == "DELETE" && pathParameters == 0));

            var segment = route.Split('/')[0];
            var tag = job.IdApp switch
            {
                80 => segment.StartsWith("B2C") ? "LinxMicrovixB2C" : "LinxMicrovixERP",
                90 => "DataWarehouse",
                _ => $"{appTitle}{segment}"
            };

            var isTicket = catalog.IdJob == 3006;
            return new OnDemandOperation(
                job.IdApp,
                appTitle,
                catalog.IdJob,
                catalog.Title ?? "",
                description?.Trim(),
                verb,
                $"{RoutePrefixes.GetValueOrDefault(job.IdApp, appTitle)}/{route}",
                tag,
                parameters,
                hasBody,
                hasBody ? (isTicket ? TicketExample : GenericExample) : null,
                hasBody ? (isTicket ? TicketModel : GenericModel) : null);
        }
    }
}

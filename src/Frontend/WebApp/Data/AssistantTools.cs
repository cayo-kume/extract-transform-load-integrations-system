using System.Text.Json;
using Anthropic.Models.Beta.Messages;
using WebApp.Components.Display;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Ferramentas do assistente: só leitura, em cima do IMonitorRepository (os mesmos dados das telas, da empresa logada).
    /// Nenhuma executa, reconhece, liga ou desliga nada, nem vê credenciais e parâmetros dos módulos.
    /// </summary>
    public class AssistantTools(IMonitorRepository repository, LoggedCompany loggedCompany)
    {
        // limite de itens por resposta: a ferramenta devolve o essencial e diz quantos ficaram de fora
        private const int MaxItems = 30;

        private static readonly JsonSerializerOptions Json = new() { WriteIndented = false };

        public static IReadOnlyList<BetaToolUnion> Definitions { get; } =
        [
            Define("saude_integracoes",
                "Lista as integrações e procedures ativas por saúde (Erro, Atrasado, Rodando, Ok), com a última execução. " +
                "Use para perguntas como 'o que está com erro agora' ou 'o que está atrasado'. Falhas reconhecidas vêm marcadas.",
                new Dictionary<string, object>
                {
                    ["saude"] = new { type = "string", @enum = new[] { "erro", "atrasado", "rodando", "ok", "todas" }, description = "Estado a listar; 'todas' traz a contagem de cada estado e os com erro ou atraso." },
                    ["id_aplicacao"] = new { type = "integer", description = "Opcional: só uma aplicação (IdApp, por exemplo 70 para LinxCommerce)." }
                },
                required: ["saude"]),
            Define("resumo_periodo",
                "Resume o período por aplicação: execuções, execuções com erro, taxa de sucesso, registros, erros, avisos, " +
                "jobs falhando agora e último erro.",
                new Dictionary<string, object>
                {
                    ["periodo"] = new { type = "string", @enum = new[] { "12h", "24h", "7d" }, description = "Janela de tempo." }
                },
                required: ["periodo"]),
            Define("buscar_jobs",
                "Procura jobs pelo IdJob, título ou descrição e devolve aplicação, tipo, agenda, se está ativo e a saúde.",
                new Dictionary<string, object>
                {
                    ["texto"] = new { type = "string", description = "IdJob ou parte do título/descrição." }
                },
                required: ["texto"]),
            Define("execucoes_job",
                "Últimas execuções de um job no período: início, fim, duração, registros, erros e avisos.",
                new Dictionary<string, object>
                {
                    ["id_job"] = new { type = "integer" },
                    ["id_tipo"] = new { type = "integer", description = "2 = integração, 1 = procedure. Padrão 2." },
                    ["periodo"] = new { type = "string", @enum = new[] { "12h", "24h", "7d" } }
                },
                required: ["id_job"]),
            Define("causas_erro",
                "Causas de erro (mensagens agrupadas pelo mesmo texto) no período, de um job, de uma aplicação ou de todas, " +
                "com quantidade, primeira e última ocorrência e um exemplo da mensagem.",
                new Dictionary<string, object>
                {
                    ["periodo"] = new { type = "string", @enum = new[] { "12h", "24h", "7d" } },
                    ["id_aplicacao"] = new { type = "integer" },
                    ["id_job"] = new { type = "integer" },
                    ["id_tipo"] = new { type = "integer", description = "2 = integração, 1 = procedure. Padrão 2 quando há id_job." },
                    ["incluir_avisos"] = new { type = "boolean" }
                },
                required: ["periodo"])
        ];

        /// <summary>
        /// Texto curto para o aviso "consultou: ..." no chat.
        /// </summary>
        public static string Label(string name) => name switch
        {
            "saude_integracoes" => "saúde das integrações ativas",
            "resumo_periodo" => "métricas do período",
            "buscar_jobs" => "busca de jobs",
            "execucoes_job" => "execuções do job",
            "causas_erro" => "causas de erro",
            _ => name
        };

        public async Task<string> RunAsync(string name, IReadOnlyDictionary<string, JsonElement> input, CancellationToken cancellationToken)
        {
            object result = name switch
            {
                "saude_integracoes" => await HealthAsync(input, cancellationToken),
                "resumo_periodo" => await SummaryAsync(input, cancellationToken),
                "buscar_jobs" => await SearchAsync(input, cancellationToken),
                "execucoes_job" => await ExecutionsAsync(input, cancellationToken),
                "causas_erro" => await CausesAsync(input, cancellationToken),
                _ => throw new ArgumentException($"Ferramenta desconhecida: {name}")
            };

            return JsonSerializer.Serialize(result, Json);
        }

        private async Task<object> HealthAsync(IReadOnlyDictionary<string, JsonElement> input, CancellationToken cancellationToken)
        {
            var jobs = (await repository.GetJobsAsync(idApp: Int(input, "id_aplicacao"), idType: null, cancellationToken))
                .Where(x => x.IsActive && !x.IsOnDemand)
                .ToList();
            var apps = await AppTitlesAsync(cancellationToken);

            var health = Text(input, "saude") ?? "todas";
            var wanted = health switch
            {
                "erro" => [JobHealth.Error],
                "atrasado" => [JobHealth.Late],
                "rodando" => [JobHealth.Running],
                "ok" => [JobHealth.Ok],
                _ => new[] { JobHealth.Error, JobHealth.Late }
            };

            var matching = jobs.Where(x => x.Health is JobHealth h && wanted.Contains(h))
                .OrderBy(x => x.Health)
                .ThenBy(x => x.LastExecution?.StartDate)
                .ToList();

            return new
            {
                empresa = loggedCompany.Label,
                contagem = jobs.GroupBy(x => x.Health?.ToString() ?? "SemExecucao").ToDictionary(x => x.Key, x => x.Count()),
                total = matching.Count,
                mostrados = Math.Min(matching.Count, MaxItems),
                jobs = matching.Take(MaxItems).Select(x => new
                {
                    aplicacao = apps.GetValueOrDefault(x.IdApp),
                    x.IdApp,
                    x.IdJob,
                    tipo = x.JobType,
                    titulo = x.Title,
                    saude = x.Health?.ToString(),
                    reconhecida = x.Acknowledgement is not null,
                    agenda = MonitorDisplay.Schedule(x),
                    ultimoInicio = x.LastExecution?.StartDate,
                    ultimoFim = x.LastExecution?.EndDate,
                    erros = x.LastExecution?.Errors,
                    link = MonitorDisplay.JobLink(x.IdApp, x.IdJob, x.IdType, Period.Last24Hours)
                })
            };
        }

        private async Task<object> SummaryAsync(IReadOnlyDictionary<string, JsonElement> input, CancellationToken cancellationToken)
        {
            var period = PeriodOf(input);
            var apps = await repository.GetAppSummariesAsync(period, cancellationToken);

            return new
            {
                empresa = loggedCompany.Label,
                periodo = MonitorDisplay.PeriodQuery(period),
                aplicacoes = apps.Select(x => new
                {
                    x.IdParent,
                    aplicacao = x.Title,
                    x.Executions,
                    execucoesComErro = x.FailedExecutions,
                    taxaSucesso = x.Executions == 0 ? (double?)null : Math.Round(100.0 * (x.Executions - x.FailedExecutions) / x.Executions, 2),
                    registros = x.Records,
                    erros = x.Errors,
                    avisos = x.Warnings,
                    jobsFalhandoAgora = x.FailingJobs,
                    ultimoErro = x.LastErrorAt
                })
            };
        }

        private async Task<object> SearchAsync(IReadOnlyDictionary<string, JsonElement> input, CancellationToken cancellationToken)
        {
            var text = Text(input, "texto") ?? "";
            var jobs = await repository.GetJobsAsync(idApp: null, idType: null, cancellationToken);
            var apps = await AppTitlesAsync(cancellationToken);
            var found = jobs
                .Where(x => MessageGrouping.Contains($"{x.IdJob} {x.Title} {x.Description}", text))
                .ToList();

            return new
            {
                total = found.Count,
                jobs = found.Take(MaxItems).Select(x => new
                {
                    aplicacao = apps.GetValueOrDefault(x.IdApp),
                    x.IdApp,
                    x.IdJob,
                    x.IdType,
                    tipo = x.JobType,
                    titulo = x.Title,
                    descricao = x.Description,
                    ativo = x.IsActive,
                    sobDemanda = x.IsOnDemand,
                    agenda = MonitorDisplay.Schedule(x),
                    saude = x.Health?.ToString(),
                    link = x.IsOnDemand ? $"/on-demand?job={x.IdJob}" : MonitorDisplay.JobLink(x.IdApp, x.IdJob, x.IdType, Period.Last24Hours)
                })
            };
        }

        private async Task<object> ExecutionsAsync(IReadOnlyDictionary<string, JsonElement> input, CancellationToken cancellationToken)
        {
            var idJob = Int(input, "id_job") ?? throw new ArgumentException("id_job é obrigatório.");
            var idType = Int(input, "id_tipo") ?? 2;
            var page = await repository.GetExecutionsAsync(idJob, idType, PeriodOf(input), page: 1, pageSize: 10, cancellationToken);

            return new
            {
                idJob,
                idType,
                total = page.TotalCount,
                execucoes = page.Items.Select(x => new
                {
                    inicio = x.StartDate,
                    fim = x.EndDate,
                    duracao = x.Duration?.ToString(@"hh\:mm\:ss"),
                    registros = x.Records,
                    erros = x.Errors,
                    avisos = x.Warnings
                })
            };
        }

        private async Task<object> CausesAsync(IReadOnlyDictionary<string, JsonElement> input, CancellationToken cancellationToken)
        {
            var idJob = Int(input, "id_job");
            var query = new MessageQuery
            {
                Period = PeriodOf(input),
                MinLevel = input.TryGetValue("incluir_avisos", out var warnings) && warnings.ValueKind == JsonValueKind.True ? MessageLevel.Warning : MessageLevel.Error,
                IdApp = Int(input, "id_aplicacao"),
                IdJob = idJob,
                IdType = idJob is null ? null : Int(input, "id_tipo") ?? 2,
                // o assistente vê também os erros já reconhecidos (ele diz quais estão reconhecidos)
                ShowAcknowledged = true
            };

            // com um job, as causas dele; sem job, os jobs com mais erros e as causas de cada um dos primeiros
            if (idJob is not null)
            {
                var groups = await repository.GetMessageGroupsAsync(query, cancellationToken);
                return new { causas = groups.Take(MaxItems).Select(Cause) };
            }

            var jobs = (await repository.GetMessageJobsAsync(query, cancellationToken)).OrderByDescending(x => x.Errors).ToList();
            var top = new List<object>();
            foreach (var job in jobs.Take(5))
            {
                var groups = await repository.GetMessageGroupsAsync(query with { IdApp = job.IdApp, IdJob = job.IdJob, IdType = job.IdType }, cancellationToken);
                top.Add(new
                {
                    job.IdApp,
                    job.IdJob,
                    titulo = job.JobTitle,
                    erros = job.Errors,
                    causas = groups.Take(5).Select(Cause),
                    link = $"/errors?app={job.IdApp}&job={job.IdJob}&type={job.IdType}"
                });
            }

            return new
            {
                jobsComMensagem = jobs.Count,
                errosNoTotal = jobs.Sum(x => x.Errors),
                principais = top
            };
        }

        private static object Cause(MessageGroupModel x) => new
        {
            nivel = x.Level.ToString(),
            vezes = x.Count,
            primeira = x.FirstAt,
            ultima = x.LastAt,
            exemplo = x.Example is { Length: > 400 } example ? example[..400] + "…" : x.Example
        };

        private async Task<Dictionary<int, string?>> AppTitlesAsync(CancellationToken cancellationToken) =>
            (await repository.GetAppSummariesAsync(Period.Last24Hours, cancellationToken)).ToDictionary(x => x.IdParent, x => x.Title);

        private static Period PeriodOf(IReadOnlyDictionary<string, JsonElement> input) =>
            MonitorDisplay.ParsePeriod(Text(input, "periodo")) ?? Period.Last24Hours;

        private static string? Text(IReadOnlyDictionary<string, JsonElement> input, string key) =>
            input.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

        private static int? Int(IReadOnlyDictionary<string, JsonElement> input, string key) =>
            input.TryGetValue(key, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var number) ? number : null;

        private static BetaToolUnion Define(string name, string description, Dictionary<string, object> properties, IReadOnlyList<string> required) =>
            new BetaTool
            {
                Name = name,
                Description = description,
                InputSchema = new()
                {
                    Properties = properties.ToDictionary(x => x.Key, x => JsonSerializer.SerializeToElement(x.Value)),
                    Required = [.. required]
                }
            };
    }
}

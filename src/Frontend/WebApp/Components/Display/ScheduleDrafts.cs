using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Components.Display
{
    /// <summary>
    /// Agenda das integrações enquanto é editada (intervalo, hora diária e fila): nada vai para o repositório até
    /// "Salvar alterações", que confirma listando cada mudança. Usada pela aba Agenda do diálogo de configurações e pelo
    /// modo agenda dos grupos da tela Jobs.
    /// </summary>
    public sealed class ScheduleDrafts
    {
        public sealed record Draft(int Interval, TimeOnly? DailyAt, string Queue);

        private readonly Dictionary<(int IdJob, int IdType), Draft> _drafts = [];

        /// <summary>
        /// Filas que os jobs já usam, para sugerir no campo Fila e avisar quando a escolhida é nova.
        /// </summary>
        public IReadOnlyList<string> Queues { get; set; } = [];

        public int Count => _drafts.Count;

        public bool HasInvalidQueue => _drafts.Values.Any(x => !IsValidQueue(x.Queue));

        public bool IsChanged(JobModel job) => _drafts.ContainsKey((job.IdJob, job.IdType));

        public int CountOf(IEnumerable<JobModel> jobs) => jobs.Count(IsChanged);

        public Draft Of(JobModel job) =>
            _drafts.GetValueOrDefault((job.IdJob, job.IdType)) ?? Original(job);

        // voltar ao valor de antes tira a integração da lista de alterações
        public void Change(JobModel job, Draft draft)
        {
            if (draft == Original(job))
                _drafts.Remove((job.IdJob, job.IdType));
            else
                _drafts[(job.IdJob, job.IdType)] = draft;
        }

        public void Clear() => _drafts.Clear();

        public void Clear(IEnumerable<JobModel> jobs)
        {
            foreach (var job in jobs)
                _drafts.Remove((job.IdJob, job.IdType));
        }

        private static Draft Original(JobModel job) => new(job.IntervalTimeInMinutes, job.RunningDailyAt, job.Queue ?? "");

        /// <summary>
        /// Com horário diário, o intervalo é 1440; limpar o horário devolve o intervalo de antes (ou 60, se ele era diário).
        /// </summary>
        public static Draft WithDailyAt(JobModel job, Draft draft, string? value)
        {
            var dailyAt = TimeOnly.TryParse(value, out var time) ? time : (TimeOnly?)null;
            if (dailyAt is not null)
                return draft with { DailyAt = dailyAt, Interval = 1440 };

            var interval = job.RunningDailyAt is null ? job.IntervalTimeInMinutes : 60;
            return draft with { DailyAt = null, Interval = interval };
        }

        /// <summary>
        /// A mesma regra do host para nome de fila (Hangfire:Server:Queues).
        /// </summary>
        public static bool IsValidQueue(string? queue) =>
            !string.IsNullOrWhiteSpace(queue) && queue.All(x => x is (>= 'a' and <= 'z') or (>= '0' and <= '9') or '_' or '-');

        public Task<IEnumerable<string>> SearchQueues(string? text, CancellationToken cancellationToken) =>
            Task.FromResult(string.IsNullOrWhiteSpace(text) ? Queues.AsEnumerable() : Queues.Where(x => x.Contains(text.Trim(), StringComparison.OrdinalIgnoreCase)));

        private static string Describe(JobModel job, Draft draft)
        {
            static string Schedule(int interval, TimeOnly? dailyAt) =>
                dailyAt is TimeOnly at ? $"diário às {at:HH\\:mm}" : interval == 0 ? "sob demanda" : $"a cada {interval} min";

            var changes = new List<string>();
            var before = Schedule(job.IntervalTimeInMinutes, job.RunningDailyAt);
            var after = Schedule(draft.Interval, draft.DailyAt);
            if (before != after)
                changes.Add($"{before} → <strong>{after}</strong>");
            if (draft.Queue != job.Queue)
                changes.Add($"fila {System.Net.WebUtility.HtmlEncode(job.Queue)} → <strong>{System.Net.WebUtility.HtmlEncode(draft.Queue)}</strong>");

            return $"<li>{job.IdJob} {System.Net.WebUtility.HtmlEncode(job.Title)}: {string.Join("; ", changes)}</li>";
        }

        /// <summary>
        /// Mudança de agenda vale para a produção: confirma listando o que muda e grava as alterações dos jobs informados
        /// (um grupo da tela Jobs, ou todos os do diálogo). Devolve quantas foram salvas; 0 se cancelou.
        /// </summary>
        public async Task<int> ConfirmAndSaveAsync(IDialogService dialogService, ISnackbar snackbar, ILogger logger, IMonitorRepository repository, string companyLabel, string? title, IEnumerable<JobModel> jobs)
        {
            var changes = jobs
                .Where(IsChanged)
                .Select(x => (Job: x, Draft: Of(x)))
                .OrderBy(x => x.Job.IdJob)
                .ToList();

            if (changes.Count == 0)
                return 0;

            var newQueues = changes.Select(x => x.Draft.Queue).Distinct().Where(x => !Queues.Contains(x)).ToList();
            var warning = newQueues.Count == 0
                ? ""
                : $"<p class=\"danger-text\">Fila nova ({System.Net.WebUtility.HtmlEncode(string.Join(", ", newQueues))}): só roda se algum servidor do host a tiver em Hangfire:Server:Queues.</p>";

            var confirmed = await dialogService.ShowMessageBoxAsync(
                $"Salvar a agenda de {title}?",
                (MarkupString)$"<p>Para <strong>{System.Net.WebUtility.HtmlEncode(companyLabel)}</strong>:</p><ul>{string.Concat(changes.Select(x => Describe(x.Job, x.Draft)))}</ul>{warning}<p>O host relê a agenda no próximo ciclo (até {HealthChip.HostRecheckMinutes} min).</p>",
                yesText: "Salvar",
                cancelText: "Cancelar");

            if (confirmed != true)
                return 0;

            var saved = 0;
            foreach (var (job, draft) in changes)
            {
                if (!await snackbar.TryAsync(logger, () => repository.SetJobScheduleAsync(job.IdJob, job.IdType, draft.Interval, draft.DailyAt, draft.Queue), $"Não foi possível salvar a agenda do job {job.IdJob}."))
                    continue;

                _drafts.Remove((job.IdJob, job.IdType));
                saved++;
            }

            if (saved > 0)
                snackbar.Add($"Agenda salva: {saved} integração(ões).", Severity.Success);

            Queues = await repository.GetQueuesAsync();
            return saved;
        }
    }
}

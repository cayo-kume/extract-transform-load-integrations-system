using Application.Shared.Abstractions.Services;
using Application.Shared.Attributes;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Shared.Entities.Auditing;
using Domain.Shared.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using WebJob.Scheduling;

namespace WebJob.Recovery
{
    public interface IStuckJobsRecovery
    {
        /// <summary>
        /// lê o heartbeat dos jobs ativos e, só com algum off, reinicia na Azure os WebJobs deles
        /// </summary>
        public Task<bool> RecoverStuckJobs();
    }

    /// <summary>
    /// recuperação dos WebJobs travados (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md, passo 7), no lugar do
    /// AzureService.RecoveryTotvsJobs() do sistema antigo: vale para todos os jobs, e a API da Azure (listar e reiniciar WebJobs)
    /// só é chamada quando o heartbeat de algum job está off. É o job 14000 (StuckJobsRecovery) de auditing.Jobs, com o seu
    /// próprio WebJob; roda só neste host
    /// </summary>
    public class StuckJobsRecovery : IStuckJobsRecovery
    {
        private readonly IJobScheduleRepository _jobScheduleRepository;
        private readonly IAzureWebJobsClient _azureWebJobsClient;
        private readonly ILoggerService _jobLog;
        private readonly IMemoryCache _cache;
        private readonly RecoverySettings _settings;
        private readonly WebJobSettings _webJobSettings;

        public StuckJobsRecovery(
            IJobScheduleRepository jobScheduleRepository,
            IAzureWebJobsClient azureWebJobsClient,
            ILoggerService jobLog,
            IMemoryCache cache,
            IOptions<RecoverySettings> settings,
            IOptions<WebJobSettings> webJobSettings
        ) =>
            (_jobScheduleRepository, _azureWebJobsClient, _jobLog, _cache, _settings, _webJobSettings) =
            (jobScheduleRepository, azureWebJobsClient, jobLog, cache, settings.Value, webJobSettings.Value);

        [JobVersion("1.0.0")]
        public async Task<bool> RecoverStuckJobs()
        {
            _jobLog.AddLog(EnumJob.StuckJobsRecovery);

            // as datas de auditing.Logs estão no horário de Brasília: o "agora" é no mesmo fuso
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_webJobSettings.TimeZone);
            var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone).DateTime;

            var heartbeats = (await _jobScheduleRepository.GetActiveJobsHeartbeat()).ToList();
            var off = new List<(JobHeartbeat Job, string Reason)>();

            foreach (var job in heartbeats)
            {
                var reason = OffReason(job, now);

                if (reason is not null)
                    off.Add((job, reason));
            }

            if (off.Count == 0)
            {
                _jobLog.AddMessage($"Nenhum job com o heartbeat off: {heartbeats.Count} job(s) ativo(s) lido(s).");
                await Finish();
                return true;
            }

            foreach (var (job, reason) in off)
                _jobLog.AddMessage($"{job.IdJob} {job.Title}: heartbeat off ({reason}).");

            if (_settings.DryRun)
            {
                _jobLog.AddMessage($"Recovery:DryRun ligado: nenhum WebJob reiniciado ({off.Count} job(s) com o heartbeat off).");
                await Finish();
                return true;
            }

            // só aqui a Azure é chamada
            var webJobs = await _azureWebJobsClient.GetContinuousWebJobs();

            foreach (var (job, _) in off)
                await Recover(job, webJobs, now);

            await Finish();
            return true;
        }

        /// <summary>
        /// por que o job está off, ou null quando está em dia ou fica fora da recuperação. Intervalo: sem execução terminada há
        /// mais que o intervalo + GraceMinutes (ou o limite próprio do job). Diário: sem execução terminada desde o último
        /// horário que já passou de GraceMinutes (ou do limite próprio)
        /// </summary>
        private string? OffReason(JobHeartbeat job, DateTime now)
        {
            if (job.IdJob == (int)EnumJob.StuckJobsRecovery || _settings.IgnoredJobs.Contains(job.IdJob))
                return null;

            // sem execução registrada: um job recém-ligado ainda não rodou; fica fora até a primeira execução
            if (job.LastEndDate is not DateTime lastEnd)
                return null;

            var threshold = _settings.Thresholds.TryGetValue(job.IdJob.ToString(), out var minutes) ? minutes : (int?)null;

            if (job.RunningDailyAt is TimeSpan at)
            {
                var tolerance = TimeSpan.FromMinutes(threshold ?? _settings.GraceMinutes);
                var slot = now.Date + at;

                if (slot + tolerance > now)
                    slot = slot.AddDays(-1);

                return lastEnd < slot ? $"diário das {at:hh\\:mm} sem execução terminada desde {slot:dd/MM HH:mm}; a última terminou em {lastEnd:dd/MM HH:mm}" : null;
            }

            if (job.IntervalTimeInMinutes <= 0)
                return null;

            var limit = TimeSpan.FromMinutes(threshold ?? job.IntervalTimeInMinutes + _settings.GraceMinutes);

            return now - lastEnd > limit ? $"a última execução terminou em {lastEnd:dd/MM HH:mm}, há mais de {limit.TotalMinutes:N0} minuto(s)" : null;
        }

        /// <summary>
        /// reinicia o WebJob do job, com as proteções do sistema antigo: cooldown entre reinícios (que também dá ao job o tempo de
        /// terminar a primeira execução depois de um restart), WebJob parado à mão não é retomado e estado transitório espera o
        /// próximo ciclo
        /// </summary>
        private async Task Recover(JobHeartbeat job, IReadOnlyList<AzureWebJob> webJobs, DateTime now)
        {
            // o WebJob do job, em qualquer versão: "<Title> - V<versão>" (ou o formato Safe)
            var webJob = webJobs.Where(w => IsWebJobOf(w.Name, job.Title)).OrderByDescending(w => w.Name).FirstOrDefault();

            if (webJob is null)
            {
                _jobLog.AddMessage($"{job.IdJob} {job.Title}: nenhum WebJob do job em {_settings.AppServiceName}; nada reiniciado.");
                return;
            }

            var cacheKey = $"Recovery:LastRestart:{webJob.Name}";
            var lastRestart = _cache.TryGetValue(cacheKey, out DateTime restartedAt) ? restartedAt : (DateTime?)null;

            if (lastRestart is DateTime last && now - last < TimeSpan.FromMinutes(_settings.RestartCooldownMinutes))
            {
                _jobLog.AddMessage($"{webJob.Name}: reiniciado às {last:HH:mm}; aguardando o intervalo de {_settings.RestartCooldownMinutes} minuto(s).");
                return;
            }

            if (webJob.IsStopped && lastRestart is null)
            {
                _jobLog.AddMessage($"{webJob.Name}: Stopped sem restart recente da recuperação (parado à mão); não retomado.");
                return;
            }

            if (!webJob.IsRunning && !webJob.IsStopped)
            {
                _jobLog.AddMessage($"{webJob.Name}: em '{webJob.Status}'; aguardando o próximo ciclo.");
                return;
            }

            var status = await _azureWebJobsClient.Restart(webJob.Name);

            // 12 h: depois disso, um Stopped volta a ser tratado como parada à mão
            _cache.Set(cacheKey, now, TimeSpan.FromHours(12));
            _jobLog.AddMessage($"{webJob.Name}: reiniciado; status '{status}'.");
        }

        private bool IsWebJobOf(string webJobName, string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false;

            var escaped = Regex.Escape(title);

            return _settings.NameFormat == "Safe"
                ? Regex.IsMatch(webJobName, $"^{escaped}-V\\d+_\\d+_\\d+$")
                : Regex.IsMatch(webJobName, $"^{escaped} - V\\d+\\.\\d+\\.\\d+$");
        }

        private async Task Finish()
        {
            _jobLog.SetLogEndDate();
            await _jobLog.CommitAllChanges();
        }
    }
}

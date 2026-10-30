using Hangfire;
using Hangfire.States;
using Microsoft.Extensions.Options;

namespace WebApi.Scheduling
{
    /// <summary>
    /// limpeza da tela de Failed do dashboard (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §3.4). Com AutomaticRetry em 0, toda
    /// falha fica em Failed, que não expira; este job passa para Deleted as que falharam há mais de
    /// Hangfire:Maintenance:FailedJobsRetentionInDays, e elas saem do storage pela retenção do §3.3. O erro continua em auditing.Logs e
    /// auditing.Messages
    /// </summary>
    public class FailedJobsCleanupJob
    {
        public const string RecurringJobId = "FailedJobsCleanup";

        // a cada hora, no minuto 15: fora dos minutos cheios dos jobs de 60 e 120 minutos e longe das 23:00 dos diários
        public const string Cron = "15 * * * *";

        private const int BatchSize = 500;

        private readonly JobStorage _jobStorage;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IOptions<HangfireSettings> _settings;
        private readonly ILogger<FailedJobsCleanupJob> _logger;

        public FailedJobsCleanupJob(JobStorage jobStorage, IBackgroundJobClient backgroundJobClient, IOptions<HangfireSettings> settings, ILogger<FailedJobsCleanupJob> logger) =>
            (_jobStorage, _backgroundJobClient, _settings, _logger) = (jobStorage, backgroundJobClient, settings, logger);

        [JobDisplayName("Limpeza da tela de Failed")]
        public Task<string> Run()
        {
            var limit = DateTime.UtcNow.AddDays(-_settings.Value.Maintenance.FailedJobsRetentionInDays);
            var monitoringApi = _jobStorage.GetMonitoringApi();
            var expired = new List<string>();
            DateTime? oldestKept = null;

            // primeiro lê tudo e só depois apaga, para a paginação não pular jobs quando a lista encolhe
            var total = monitoringApi.FailedCount();

            for (var from = 0; from < total; from += BatchSize)
            {
                foreach (var job in monitoringApi.FailedJobs(from, BatchSize))
                {
                    if (job.Value.FailedAt is not DateTime failedAt)
                        continue;

                    if (failedAt < limit)
                        expired.Add(job.Key);
                    else if (oldestKept is null || failedAt < oldestKept)
                        oldestKept = failedAt;
                }
            }

            // só troca o estado se o job ainda estiver em Failed: um job reexecutado pelo dashboard nesse meio tempo fica como está
            var deleted = expired.Count(jobId => _backgroundJobClient.ChangeState(jobId, new DeletedState(), FailedState.StateName));

            var message = $"{deleted} job(s) em Failed há mais de {_settings.Value.Maintenance.FailedJobsRetentionInDays} dia(s) passaram para Deleted; " +
                (oldestKept is null ? "nenhuma falha ficou na tela." : $"a falha mais antiga que ficou é de {oldestKept:yyyy-MM-dd HH:mm:ss} (UTC).");

            _logger.LogInformation("{Message}", message);

            return Task.FromResult(message);
        }
    }
}

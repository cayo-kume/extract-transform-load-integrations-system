using Domain.Shared.Abstractions.Repositorys;
using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Options;
using WebApi.Scheduling.RecurringJobs;

namespace WebApi.Scheduling
{
    /// <summary>
    /// monta a agenda do Hangfire a partir de auditing.Jobs (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.4): roda na subida e
    /// a cada Hangfire:Schedule:RefreshIntervalInMinutes, e um UPDATE na tabela vale, no máximo, no ciclo seguinte. Entra na agenda o
    /// job de integração ativo, com método no host e agenda que vira cron; sai o que ficou inativo, sem intervalo ou saiu do mapa
    /// </summary>
    public class SchedulerRefreshJob
    {
        public const string RecurringJobId = "SchedulerRefresh";

        private readonly IJobScheduleRepository _jobScheduleRepository;
        private readonly JobStorage _jobStorage;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IOptions<HangfireSettings> _settings;
        private readonly ILogger<SchedulerRefreshJob> _logger;

        public SchedulerRefreshJob(IJobScheduleRepository jobScheduleRepository, JobStorage jobStorage, IRecurringJobManager recurringJobManager, IOptions<HangfireSettings> settings, ILogger<SchedulerRefreshJob> logger) =>
            (_jobScheduleRepository, _jobStorage, _recurringJobManager, _settings, _logger) = (jobScheduleRepository, jobStorage, recurringJobManager, settings, logger);

        [JobDisplayName("Agenda: releitura de auditing.Jobs")]
        public async Task<string> Run()
        {
            var settings = _settings.Value;
            HangfireSettingsValidator.TryFindTimeZone(settings.TimeZone, out var timeZone);

            var warnings = new List<string>();
            var desired = new Dictionary<string, (RecurringJobDefinition Definition, string Cron, string Queue)>();

            foreach (var job in await _jobScheduleRepository.GetIntegrationJobs())
            {
                if (!job.IsActive || (job.IntervalTimeInMinutes <= 0 && job.RunningDailyAt is null))
                    continue;

                if (!RecurringJobMap.Definitions.TryGetValue(job.IdJob, out var definition))
                {
                    warnings.Add($"{job.IdJob} {job.Title}: ativo em auditing.Jobs, mas sem método no host; fica fora da agenda.");
                    continue;
                }

                var cron = JobCron.From(job.IntervalTimeInMinutes, job.RunningDailyAt);

                if (cron is null)
                {
                    warnings.Add($"{job.IdJob} {job.Title}: intervalo de {job.IntervalTimeInMinutes} minutos não vira cron (precisa dividir a hora ou o dia, ou ter RunningDailyAt); fica fora da agenda.");
                    continue;
                }

                if (!settings.Server.Queues.Contains(job.Queue))
                    warnings.Add($"{job.IdJob} {job.Title}: fila '{job.Queue}' não está em Hangfire:Server:Queues deste servidor; só roda se outro servidor atender a fila.");

                desired[RecurringJobMap.RecurringJobId(job.IdJob, job.Title, definition.Version!)] = (definition, cron, job.Queue);
            }

            using var connection = _jobStorage.GetConnection();

            // só os recurring jobs da agenda (<IdJob>-<Title>); os de manutenção do host ficam como estão
            var current = connection.GetRecurringJobs()
                .Where(r => RecurringJobMap.IdJobOf(r.Id) is not null)
                .ToDictionary(r => r.Id);

            int added = 0, changed = 0, unchanged = 0;

            foreach (var (recurringJobId, job) in desired)
            {
                var exists = current.TryGetValue(recurringJobId, out var dto);

                if (exists && dto!.Cron == job.Cron && dto.Queue == job.Queue && dto.TimeZoneId == timeZone.Id)
                    unchanged++;
                else if (exists)
                    changed++;
                else
                    added++;

                // o Hangfire só grava o que mudou; o AddOrUpdate também corrige um método trocado no mapa
                job.Definition.AddOrUpdate(_recurringJobManager, recurringJobId, job.Queue, job.Cron, new RecurringJobOptions { TimeZone = timeZone });
            }

            var removed = current.Keys.Except(desired.Keys).ToList();

            foreach (var recurringJobId in removed)
                _recurringJobManager.RemoveIfExists(recurringJobId);

            foreach (var warning in warnings)
                _logger.LogWarning("{Warning}", warning);

            var summary = $"Agenda: {desired.Count} job(s) na agenda ({added} incluído(s), {changed} alterado(s), {unchanged} sem mudança), " +
                $"{removed.Count} removido(s), {warnings.Count} aviso(s).";

            _logger.LogInformation("{Summary}", summary);

            return warnings.Count == 0 ? summary : summary + Environment.NewLine + string.Join(Environment.NewLine, warnings);
        }
    }
}

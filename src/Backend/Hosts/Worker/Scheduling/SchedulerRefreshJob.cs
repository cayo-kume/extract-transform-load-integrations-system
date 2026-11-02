using Application.Shared.Abstractions.Services;
using Domain.Shared.Abstractions.Repositorys;
using Hosts.Scheduling;
using Hosts.Scheduling.RecurringJobs;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl.Matchers;

namespace Worker.Scheduling
{
    /// <summary>
    /// monta a agenda do Quartz a partir de auditing.Jobs (src/Backend/Hosts/Worker/docs/PLANO-HOST-WORKER.md §4.4), como o
    /// SchedulerRefresh do Hangfire: roda na subida e a cada Quartz:Schedule:RefreshIntervalInMinutes, e um UPDATE na tabela vale,
    /// no máximo, no ciclo seguinte. Entra o job de integração ativo, com método no mapa e agenda que vira cron; sai o que ficou
    /// inativo, sem intervalo ou saiu do mapa. Só mexe no grupo integrations. Com dois Workers, uma releitura por vez: o
    /// [DisallowConcurrentExecution] e a trava job:scheduler-refresh, como no Hangfire (§5.3, item 4)
    /// </summary>
    [DisallowConcurrentExecution]
    public class SchedulerRefreshJob : IJob
    {
        public static readonly JobKey Key = new("SchedulerRefresh", "host");
        public const string ConcurrencyKey = "job:scheduler-refresh";

        private readonly IJobScheduleRepository _jobScheduleRepository;
        private readonly IJobLock _jobLock;
        private readonly IOptions<QuartzSettings> _settings;
        private readonly ILogger<SchedulerRefreshJob> _logger;

        public SchedulerRefreshJob(IJobScheduleRepository jobScheduleRepository, IJobLock jobLock, IOptions<QuartzSettings> settings, ILogger<SchedulerRefreshJob> logger) =>
            (_jobScheduleRepository, _jobLock, _settings, _logger) = (jobScheduleRepository, jobLock, settings, logger);

        public async Task Execute(IJobExecutionContext context)
        {
            using var jobLock = _jobLock.TryAcquire(ConcurrencyKey);

            if (jobLock is null)
            {
                _logger.LogInformation("Agenda: outra releitura de auditing.Jobs está rodando; esta foi pulada.");
                return;
            }

            try
            {
                context.Result = await Refresh(context.Scheduler, context.CancellationToken);
            }
            catch (Exception ex)
            {
                // banco fora ou erro no Quartz: a agenda fica como está e a próxima releitura tenta de novo
                _logger.LogError(ex, "Agenda: a releitura de auditing.Jobs falhou; a agenda continua como estava.");
            }
        }

        private async Task<string> Refresh(IScheduler scheduler, CancellationToken cancellationToken)
        {
            QuartzSettingsValidator.TryFindTimeZone(_settings.Value.TimeZone, out var timeZone);

            var warnings = new List<string>();
            var desired = new Dictionary<string, (RecurringJobDefinition Definition, string Cron)>();

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

                desired[RecurringJobMap.RecurringJobId(job.IdJob, job.Title, definition.Version!)] = (definition, JobCron.ToQuartz(cron));
            }

            var current = (await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(QuartzJobRunner.Group), cancellationToken))
                .ToDictionary(k => k.Name);

            int added = 0, changed = 0, unchanged = 0;

            foreach (var (name, job) in desired)
            {
                var triggerKey = new TriggerKey(name, QuartzJobRunner.Group);
                var trigger = Trigger(name, job.Cron, timeZone);

                if (!current.ContainsKey(name))
                {
                    var jobDetail = JobBuilder.Create<QuartzJobRunner>()
                        .WithIdentity(name, QuartzJobRunner.Group)
                        .WithDescription(job.Definition.Target)
                        .UsingJobData(QuartzJobRunner.IdJobKey, job.Definition.IdJob.ToString())
                        .Build();

                    await scheduler.ScheduleJob(jobDetail, [trigger], replace: true, cancellationToken);
                    added++;
                }
                else if (await scheduler.GetTrigger(triggerKey, cancellationToken) is ICronTrigger existing
                    && existing.CronExpressionString == job.Cron && existing.TimeZone.Id == timeZone.Id)
                    unchanged++;
                else
                {
                    // o trigger mudou (ou sumiu): troca só ele, o job fica
                    if (await scheduler.CheckExists(triggerKey, cancellationToken))
                        await scheduler.RescheduleJob(triggerKey, trigger, cancellationToken);
                    else
                        await scheduler.ScheduleJob(trigger, cancellationToken);

                    changed++;
                }
            }

            // inativos, sem intervalo, fora do mapa ou com a versão antiga: a execução em andamento termina normalmente
            var removed = current.Keys.Except(desired.Keys).ToList();

            foreach (var name in removed)
                await scheduler.DeleteJob(current[name], cancellationToken);

            foreach (var warning in warnings)
                _logger.LogWarning("{Warning}", warning);

            var summary = $"Agenda: {desired.Count} job(s) na agenda ({added} incluído(s), {changed} alterado(s), {unchanged} sem mudança), " +
                $"{removed.Count} removido(s), {warnings.Count} aviso(s).";

            _logger.LogInformation("{Summary}", summary);

            return summary;
        }

        private static ITrigger Trigger(string name, string cron, TimeZoneInfo timeZone) =>
            TriggerBuilder.Create()
                .WithIdentity(name, QuartzJobRunner.Group)
                .ForJob(name, QuartzJobRunner.Group)
                // disparo que esperou mais que Quartz:Misfire:ThresholdInMinutes é descartado; o job volta no horário seguinte (§5.2)
                .WithCronSchedule(cron, c => c.InTimeZone(timeZone).WithMisfireHandlingInstructionDoNothing())
                .Build();
    }
}

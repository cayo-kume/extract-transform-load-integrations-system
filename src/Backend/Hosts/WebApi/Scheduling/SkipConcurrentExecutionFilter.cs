using Application.Shared.Abstractions.Services;
using Hangfire.Server;
using Hangfire.States;
using System.Collections.Concurrent;
using WebApi.Scheduling.RecurringJobs;

namespace WebApi.Scheduling
{
    /// <summary>
    /// concorrência dos jobs da agenda (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.7, combinação de A com B): antes de
    /// executar, pede a trava do IJobLock (Shared, banco das integrações) sem esperar. Ocupada, a execução não roda, vai para Deleted
    /// com o motivo do pulo e o worker fica livre na hora; o disparo seguinte do cron tenta de novo. As rotas dos controllers ficam
    /// fora da trava
    /// </summary>
    public class SkipConcurrentExecutionFilter : IServerFilter, IElectStateFilter
    {
        private const string LockItem = "WebApi.Scheduling.JobLock";
        private const string SkippedReasonParameter = "SkippedReason";

        // jobs de manutenção do host, fora de auditing.Jobs
        private static readonly Dictionary<string, string> MaintenanceKeys = new()
        {
            [SchedulerRefreshJob.RecurringJobId] = "job:scheduler-refresh",
            [FailedJobsCleanupJob.RecurringJobId] = "job:failed-jobs-cleanup"
        };

        private readonly IJobLock _jobLock;
        private readonly ILogger<SkipConcurrentExecutionFilter> _logger;
        private readonly int _maxConsecutiveSkips;

        // pulos seguidos por chave, neste processo: zera quando o job consegue rodar
        private readonly ConcurrentDictionary<string, int> _consecutiveSkips = new();

        public SkipConcurrentExecutionFilter(IJobLock jobLock, ILogger<SkipConcurrentExecutionFilter> logger, int maxConsecutiveSkips) =>
            (_jobLock, _logger, _maxConsecutiveSkips) = (jobLock, logger, maxConsecutiveSkips);

        /// <summary>
        /// chave da trava de um recurring job: a do mapa (job:&lt;IdJob&gt; ou a do grupo) ou a de manutenção. Null fora da agenda (job
        /// enfileirado à mão), que roda sem trava
        /// </summary>
        public static string? KeyFor(string? recurringJobId)
        {
            if (recurringJobId is null)
                return null;

            if (RecurringJobMap.IdJobOf(recurringJobId) is int idJob)
                return RecurringJobMap.Definitions.TryGetValue(idJob, out var definition) ? definition.ConcurrencyKey : RecurringJobDefinition.ConcurrencyKeyFor(idJob);

            return MaintenanceKeys.GetValueOrDefault(recurringJobId);
        }

        public void OnPerforming(PerformingContext context)
        {
            var key = KeyFor(context.GetJobParameter<string>("RecurringJobId"));

            if (key is null)
                return;

            var handle = _jobLock.TryAcquire(key);

            if (handle is not null)
            {
                context.Items[LockItem] = handle;
                _consecutiveSkips.TryRemove(key, out _);

                // uma execução reexecutada pelo dashboard depois de um pulo não pode herdar o motivo
                context.SetJobParameter(SkippedReasonParameter, (string?)null);
                return;
            }

            var skips = _consecutiveSkips.AddOrUpdate(key, 1, (_, count) => count + 1);

            context.SetJobParameter(SkippedReasonParameter, $"Skipped: previous execution of {key} still running");
            context.Canceled = true;

            // um job que vive sendo pulado está lento demais para o intervalo: a correção é aumentar o intervalo na tabela
            if (skips % _maxConsecutiveSkips == 0)
                _logger.LogWarning("Job {BackgroundJobId} ({Key}) pulado {Skips} vez(es) seguida(s): a execução anterior ainda está rodando.", context.BackgroundJob.Id, key, skips);
            else
                _logger.LogInformation("Job {BackgroundJobId} ({Key}) pulado: a execução anterior ainda está rodando.", context.BackgroundJob.Id, key);
        }

        public void OnPerformed(PerformedContext context)
        {
            if (context.Items.TryGetValue(LockItem, out var item) && item is IDisposable handle)
            {
                context.Items.Remove(LockItem);
                handle.Dispose();
            }
        }

        public void OnStateElection(ElectStateContext context)
        {
            // o Hangfire 1.8 já leva a execução cancelada no OnPerforming para Deleted, com o motivo genérico "Canceled by filter";
            // aqui o motivo passa a ser o do pulo
            if (context.CandidateState is not (DeletedState or SucceededState))
                return;

            var reason = context.GetJobParameter<string>(SkippedReasonParameter);

            if (!string.IsNullOrEmpty(reason))
                context.CandidateState = new DeletedState { Reason = reason };
        }
    }
}

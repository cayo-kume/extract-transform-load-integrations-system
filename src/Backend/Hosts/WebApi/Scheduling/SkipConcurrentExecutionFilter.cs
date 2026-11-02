using Application.Shared.Abstractions.Services;
using Hangfire.Server;
using Hangfire.States;
using System.Collections.Concurrent;
using Hosts.Scheduling.RecurringJobs;

namespace WebApi.Scheduling
{
    /// <summary>
    /// concorrência dos jobs da agenda (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.7, combinação de A com B): antes de
    /// executar, pede a trava do IJobLock (Shared, banco das integrações) sem esperar. Ocupada, a execução não roda, vai para Deleted
    /// com o motivo do pulo e o worker fica livre na hora; o disparo seguinte do cron tenta de novo. As rotas dos controllers ficam
    /// fora da trava.
    /// Com um recurring job por (job, empresa) (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.4) são duas travas: a da empresa (a execução
    /// anterior da mesma empresa ainda roda: pula, como antes) e a do job, que vale entre empresas porque todas escrevem nas mesmas
    /// tabelas (untreated e a sp_*_Sync). Com a do job ocupada por outra empresa, a execução não é perdida: volta para a fila daqui a
    /// RescheduleDelay, senão as empresas disparadas no mesmo minuto pulariam sempre umas às outras
    /// </summary>
    public class SkipConcurrentExecutionFilter : IServerFilter, IElectStateFilter
    {
        private const string LockItem = "WebApi.Scheduling.JobLock";
        private const string CompanyLockItem = "WebApi.Scheduling.CompanyJobLock";
        private const string SkippedReasonParameter = "SkippedReason";
        private const string RescheduledReasonParameter = "RescheduledReason";

        private static readonly TimeSpan RescheduleDelay = TimeSpan.FromMinutes(1);

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

        /// <summary>
        /// chave da trava da empresa de um recurring job da agenda (&lt;chave do job&gt;:&lt;empresa&gt;); null nos de manutenção
        /// </summary>
        public static string? CompanyKeyFor(string? recurringJobId) =>
            KeyFor(recurringJobId) is string key && RecurringJobMap.CompanyOf(recurringJobId) is string company
                ? $"{key}:{company}"
                : null;

        public void OnPerforming(PerformingContext context)
        {
            var recurringJobId = context.GetJobParameter<string>("RecurringJobId");
            var key = KeyFor(recurringJobId);

            if (key is null)
                return;

            var companyKey = CompanyKeyFor(recurringJobId);
            var skipKey = companyKey ?? key;
            IDisposable? companyHandle = null;

            if (companyKey is not null)
            {
                companyHandle = _jobLock.TryAcquire(companyKey);

                if (companyHandle is null)
                {
                    Skip(context, skipKey);
                    return;
                }
            }

            var handle = _jobLock.TryAcquire(key);

            if (handle is null)
            {
                if (companyHandle is null)
                {
                    Skip(context, skipKey);
                    return;
                }

                // outra empresa roda o mesmo job: esta volta para a fila em vez de perder o horário
                companyHandle.Dispose();
                context.SetJobParameter(SkippedReasonParameter, (string?)null);
                context.SetJobParameter(RescheduledReasonParameter, $"Rescheduled: {key} running for another company");
                context.Canceled = true;

                _logger.LogInformation("Job {BackgroundJobId} ({Key}) adiado {Delay}: o mesmo job roda para outra empresa.", context.BackgroundJob.Id, companyKey, RescheduleDelay);
                return;
            }

            context.Items[LockItem] = handle;

            if (companyHandle is not null)
                context.Items[CompanyLockItem] = companyHandle;

            _consecutiveSkips.TryRemove(skipKey, out _);

            // uma execução reexecutada pelo dashboard depois de um pulo não pode herdar o motivo
            context.SetJobParameter(SkippedReasonParameter, (string?)null);
            context.SetJobParameter(RescheduledReasonParameter, (string?)null);
        }

        public void OnPerformed(PerformedContext context)
        {
            foreach (var item in new[] { LockItem, CompanyLockItem })
            {
                if (context.Items.TryGetValue(item, out var value) && value is IDisposable handle)
                {
                    context.Items.Remove(item);
                    handle.Dispose();
                }
            }
        }

        public void OnStateElection(ElectStateContext context)
        {
            // o Hangfire 1.8 já leva a execução cancelada no OnPerforming para Deleted, com o motivo genérico "Canceled by filter";
            // aqui o motivo passa a ser o do pulo, ou a execução volta agendada quando outra empresa ocupava o job
            if (context.CandidateState is not (DeletedState or SucceededState))
                return;

            var rescheduled = context.GetJobParameter<string>(RescheduledReasonParameter);

            if (!string.IsNullOrEmpty(rescheduled))
            {
                context.CandidateState = new ScheduledState(RescheduleDelay) { Reason = rescheduled };
                return;
            }

            var reason = context.GetJobParameter<string>(SkippedReasonParameter);

            if (!string.IsNullOrEmpty(reason))
                context.CandidateState = new DeletedState { Reason = reason };
        }

        private void Skip(PerformingContext context, string key)
        {
            var skips = _consecutiveSkips.AddOrUpdate(key, 1, (_, count) => count + 1);

            context.SetJobParameter(RescheduledReasonParameter, (string?)null);
            context.SetJobParameter(SkippedReasonParameter, $"Skipped: previous execution of {key} still running");
            context.Canceled = true;

            // um job que vive sendo pulado está lento demais para o intervalo: a correção é aumentar o intervalo na tabela
            if (skips % _maxConsecutiveSkips == 0)
                _logger.LogWarning("Job {BackgroundJobId} ({Key}) pulado {Skips} vez(es) seguida(s): a execução anterior ainda está rodando.", context.BackgroundJob.Id, key, skips);
            else
                _logger.LogInformation("Job {BackgroundJobId} ({Key}) pulado: a execução anterior ainda está rodando.", context.BackgroundJob.Id, key);
        }
    }
}

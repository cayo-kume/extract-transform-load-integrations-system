using Microsoft.Extensions.Options;
using Quartz;

namespace Worker.Scheduling
{
    /// <summary>
    /// o disparo descartado pelo misfire (esperou thread mais que Quartz:Misfire:ThresholdInMinutes, ou o Worker estava fora) não
    /// chega ao job: este listener registra o descarte no log e conta no mesmo contador dos pulos
    /// (src/Backend/Hosts/Worker/.docs/PLANO-HOST-WORKER.md §5.3, item 2). Só para os triggers do grupo integrations
    /// </summary>
    public class MisfireListener : ITriggerListener
    {
        private readonly JobSkipCounter _skips;
        private readonly IOptions<QuartzSettings> _settings;

        public MisfireListener(JobSkipCounter skips, IOptions<QuartzSettings> settings) =>
            (_skips, _settings) = (skips, settings);

        public string Name => nameof(MisfireListener);

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            _skips.Skipped(trigger.JobKey.Name, $"Misfired: waited more than {_settings.Value.Misfire.ThresholdInMinutes} minute(s) to run");
            return Task.CompletedTask;
        }

        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}

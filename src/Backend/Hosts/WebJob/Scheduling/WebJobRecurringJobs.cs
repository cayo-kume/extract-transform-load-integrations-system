using Hosts.Scheduling.RecurringJobs;
using WebJob.Recovery;

namespace WebJob.Scheduling
{
    /// <summary>
    /// os jobs que este host roda: os mapas compartilhados (Hosts.Scheduling) e os jobs só do WebJob, como a recuperação dos
    /// WebJobs travados (14000), que não faz sentido no Hangfire do WebApi. Validado do mesmo jeito que o mapa compartilhado
    /// </summary>
    public static class WebJobRecurringJobs
    {
        public static IReadOnlyDictionary<int, RecurringJobDefinition> Definitions { get; } =
            RecurringJobMap.Validate(RecurringJobMap.Definitions.Values.Concat(
            [
                RecurringJobDefinition.For<IStuckJobsRecovery>(14000, recovery => recovery.RecoverStuckJobs())
            ]));
    }
}

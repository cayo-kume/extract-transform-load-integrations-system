using Hangfire.States;
using Hangfire.Storage;

namespace WebApi.Scheduling
{
    /// <summary>
    /// retenção dos jobs concluídos no storage (§3.3): Hangfire:Storage:JobExpirationTimeoutInMinutes no lugar do 1 dia padrão. Vale só
    /// para os estados finais (Succeeded e Deleted); um job em Failed não expira e sai pela limpeza do FailedJobsCleanupJob (§3.4)
    /// </summary>
    public class JobExpirationTimeoutFilter : IApplyStateFilter
    {
        private readonly TimeSpan _timeout;

        public JobExpirationTimeoutFilter(TimeSpan timeout) =>
            _timeout = timeout;

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction) =>
            context.JobExpirationTimeout = _timeout;

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
        }
    }
}

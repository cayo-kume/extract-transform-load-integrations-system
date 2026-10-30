using Hangfire;
using System.Linq.Expressions;

namespace WebApi.Scheduling.RecurringJobs
{
    /// <summary>
    /// o que um job de auditing.Jobs executa (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.2): o método sem parâmetros do
    /// service e a chave da trava de concorrência (§4.7). Quando, em que fila e se roda vem da tabela, pelo SchedulerRefreshJob
    /// </summary>
    public class RecurringJobDefinition
    {
        private readonly Action<IRecurringJobManager, string, string, string, RecurringJobOptions> _addOrUpdate;

        public int IdJob { get; }

        /// <summary>
        /// job:&lt;IdJob&gt;, ou a chave do grupo quando jobs diferentes escrevem nos mesmos dados (ex.: group:datawarehouse)
        /// </summary>
        public string ConcurrencyKey { get; }

        /// <summary>
        /// Service.Método, para o log
        /// </summary>
        public string Target { get; }

        private RecurringJobDefinition(int idJob, string concurrencyKey, string target, Action<IRecurringJobManager, string, string, string, RecurringJobOptions> addOrUpdate) =>
            (IdJob, ConcurrencyKey, Target, _addOrUpdate) = (idJob, concurrencyKey, target, addOrUpdate);

        public static RecurringJobDefinition For<TService>(int idJob, Expression<Func<TService, Task>> call, string? concurrencyGroup = null) =>
            new(
                idJob,
                concurrencyGroup is null ? ConcurrencyKeyFor(idJob) : $"group:{concurrencyGroup}",
                $"{typeof(TService).Name}.{((MethodCallExpression)call.Body).Method.Name}",
                (manager, recurringJobId, queue, cron, options) => manager.AddOrUpdate(recurringJobId, queue, call, cron, options));

        public static string ConcurrencyKeyFor(int idJob) =>
            $"job:{idJob}";

        public void AddOrUpdate(IRecurringJobManager manager, string recurringJobId, string queue, string cron, RecurringJobOptions options) =>
            _addOrUpdate(manager, recurringJobId, queue, cron, options);
    }
}

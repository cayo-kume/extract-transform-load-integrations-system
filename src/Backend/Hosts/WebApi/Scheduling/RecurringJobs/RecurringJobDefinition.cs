using Application.Shared.Attributes;
using Hangfire;
using System.Linq.Expressions;
using System.Reflection;

namespace WebApi.Scheduling.RecurringJobs
{
    /// <summary>
    /// o que um job de auditing.Jobs executa (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.2): o método sem parâmetros do
    /// service, a versão dele ([JobVersion] no método da classe do service, §4.10) e a chave da trava de concorrência (§4.7).
    /// Quando, em que fila e se roda vem da tabela, pelo SchedulerRefreshJob
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

        /// <summary>
        /// versão do [JobVersion] do método na classe do service; null quando o método não tem o atributo (o RecurringJobMap
        /// recusa o mapa)
        /// </summary>
        public string? Version { get; }

        private RecurringJobDefinition(int idJob, string concurrencyKey, string target, string? version, Action<IRecurringJobManager, string, string, string, RecurringJobOptions> addOrUpdate) =>
            (IdJob, ConcurrencyKey, Target, Version, _addOrUpdate) = (idJob, concurrencyKey, target, version, addOrUpdate);

        public static RecurringJobDefinition For<TService>(int idJob, Expression<Func<TService, Task>> call, string? concurrencyGroup = null)
        {
            var method = ((MethodCallExpression)call.Body).Method;

            return new(
                idJob,
                concurrencyGroup is null ? ConcurrencyKeyFor(idJob) : $"group:{concurrencyGroup}",
                $"{typeof(TService).Name}.{method.Name}",
                VersionOf(typeof(TService), method),
                (manager, recurringJobId, queue, cron, options) => manager.AddOrUpdate(recurringJobId, queue, call, cron, options));
        }

        public static string ConcurrencyKeyFor(int idJob) =>
            $"job:{idJob}";

        public void AddOrUpdate(IRecurringJobManager manager, string recurringJobId, string queue, string cron, RecurringJobOptions options) =>
            _addOrUpdate(manager, recurringJobId, queue, cron, options);

        /// <summary>
        /// a expressão aponta para o método da interface; a versão está no método da classe que a implementa, no mesmo assembly
        /// (Application.&lt;Módulo&gt;). Com mais de uma classe ou nenhuma, não há como saber qual: null
        /// </summary>
        private static string? VersionOf(Type serviceInterface, MethodInfo interfaceMethod)
        {
            var implementations = serviceInterface.Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && serviceInterface.IsAssignableFrom(t))
                .ToList();

            if (implementations.Count != 1)
                return null;

            var map = implementations[0].GetInterfaceMap(serviceInterface);
            var index = Array.IndexOf(map.InterfaceMethods, interfaceMethod);

            return index < 0 ? null : map.TargetMethods[index].GetCustomAttribute<JobVersionAttribute>()?.Version;
        }
    }
}

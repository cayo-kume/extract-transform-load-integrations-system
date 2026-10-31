using Application.Shared.Attributes;
using Domain.Shared.Abstractions.Repositorys;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Hosts.Scheduling.RecurringJobs
{
    /// <summary>
    /// o que um job de auditing.Jobs executa (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.2): o método sem parâmetros do
    /// service, a versão dele ([JobVersion] no método da classe do service, §4.10) e a chave da trava de concorrência (§4.7).
    /// Quando, em que fila e se roda vem da tabela. Serve aos dois hosts de agenda: quem executa é o ScheduledJobRunner
    /// </summary>
    public class RecurringJobDefinition
    {
        private readonly Func<IServiceProvider, Task> _invoke;

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

        /// <summary>
        /// nome do job no EnumJob do módulo (o mesmo Title de auditing.Jobs), sem consultar o banco: o script de publicação do
        /// WebJob monta o nome do WebJob com ele (&lt;Title&gt; - V&lt;versão&gt;). Null quando o IdJob não está no EnumJob do módulo
        /// (o RecurringJobMap recusa o mapa)
        /// </summary>
        public string? Title { get; }

        private RecurringJobDefinition(int idJob, string? title, string concurrencyKey, string target, string? version, Func<IServiceProvider, Task> invoke) =>
            (IdJob, Title, ConcurrencyKey, Target, Version, _invoke) = (idJob, title, concurrencyKey, target, version, invoke);

        public static RecurringJobDefinition For<TService>(int idJob, Expression<Func<TService, Task>> call, string? concurrencyGroup = null)
            where TService : notnull
        {
            var method = ((MethodCallExpression)call.Body).Method;
            var compiled = call.Compile();

            return new(
                idJob,
                TitleOf(typeof(TService), idJob),
                concurrencyGroup is null ? ConcurrencyKeyFor(idJob) : $"group:{concurrencyGroup}",
                $"{typeof(TService).Name}.{method.Name}",
                VersionOf(typeof(TService), method),
                services => compiled(services.GetRequiredService<TService>()));
        }

        /// <summary>
        /// job que só executa uma procedure no banco (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md §4.2), pelo repositório do
        /// Shared. Sem método de service, a versão vem aqui, e não de [JobVersion]. O log da execução é o que a própria procedure
        /// grava (auditing.sp_Log_*); uma falha vai para o log do host. O título é o Title da linha em auditing.Jobs. Ex.:
        /// ForProcedure(10001, "P_Volo_Integracao_Clientes", "[general].[P_Volo_Integracao_Clientes]", "1.0.0")
        /// </summary>
        public static RecurringJobDefinition ForProcedure(int idJob, string title, string procedure, string version, string? concurrencyGroup = null) =>
            new(
                idJob,
                title,
                concurrencyGroup is null ? ConcurrencyKeyFor(idJob) : $"group:{concurrencyGroup}",
                $"procedure {procedure}",
                version,
                async services =>
                {
                    var executed = await services.GetRequiredService<ICommandRepository>()
                        .ExecuteCommand(procedure, new Dictionary<string, object?>(), CommandType.StoredProcedure);

                    if (!executed)
                        throw new InvalidOperationException($"A procedure {procedure} não executou.");
                });

        public static string ConcurrencyKeyFor(int idJob) =>
            $"job:{idJob}";

        /// <summary>
        /// chama o método do service, resolvido no escopo da execução (o mesmo do ILoggerService que o executor usa)
        /// </summary>
        public Task Invoke(IServiceProvider services) =>
            _invoke(services);

        /// <summary>
        /// o EnumJob do módulo fica no Domain.&lt;Módulo&gt;, que o Application.&lt;Módulo&gt; do service referencia; as faixas de IdJob não
        /// se repetem entre módulos
        /// </summary>
        private static string? TitleOf(Type serviceInterface, int idJob) =>
            serviceInterface.Assembly.GetReferencedAssemblies()
                .Where(a => a.Name?.StartsWith("Domain.", StringComparison.Ordinal) == true)
                .Select(Assembly.Load)
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsEnum && t.Name == "EnumJob" && Enum.IsDefined(t, idJob))
                .Select(t => Enum.GetName(t, idJob))
                .FirstOrDefault();

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

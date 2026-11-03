using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hosts.Scheduling
{
    public static class DependencyInjection
    {
        /// <summary>
        /// executor dos jobs da agenda, um por escopo de execução (o mesmo escopo do service e do ILoggerService)
        /// </summary>
        public static IServiceCollection AddScheduledJobRunner(this IServiceCollection services) =>
            services.AddScoped<ScheduledJobRunner>();

        /// <summary>
        /// registro da instância em auditing.SchedulingHosts e a regra de um tipo de host de agenda por banco
        /// (SchedulingHostRegistry). Entra antes do agendador (AddHangfireServer, AddQuartzHostedService, o JobScheduleWorker):
        /// os hosted services sobem na ordem do registro, e o primeiro heartbeat precisa vir antes do primeiro job
        /// </summary>
        public static IServiceCollection AddSchedulingHostRegistration(this IServiceCollection services, string hostType)
        {
            services.AddSingleton(sp => new SchedulingHostRegistry(hostType, sp.GetRequiredService<IConfiguration>()));
            services.AddHostedService<SchedulingHostHeartbeat>();

            return services;
        }
    }
}

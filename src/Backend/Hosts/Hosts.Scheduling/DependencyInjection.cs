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
    }
}

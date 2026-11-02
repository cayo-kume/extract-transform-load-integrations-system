using Application.Shared.Abstractions.Handlers.Commands;
using Application.Shared.Abstractions.Services;
using Application.Shared.Handlers.Commands;
using Application.Shared.Handlers.Exceptions;
using Application.Shared.Services;
using Domain.Shared.Abstractions.Repositorys;
using Infrastructure.Shared.Abstractions;
using Infrastructure.Shared.Connections;
using Infrastructure.Shared.Locks;
using Infrastructure.Shared.Repositorys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Application.Shared.Abstractions.Handlers.Exceptions;

namespace Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedSharedServices(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddScoped<ISQLServerConnection, SQLServerConnection>();
            services.AddScoped<ISharedRepository, SharedRepository>();
            services.AddScoped<IQueryRepository>(sp => sp.GetRequiredService<ISharedRepository>());
            services.AddScoped<IBulkInsertRepository>(sp => sp.GetRequiredService<ISharedRepository>());
            services.AddScoped<ICommandRepository>(sp => sp.GetRequiredService<ISharedRepository>());
            services.AddScoped<IMergeRepository>(sp => sp.GetRequiredService<ISharedRepository>());
            services.AddScoped<IConnectionAccessor>(sp => sp.GetRequiredService<ISharedRepository>());
            services.AddScoped<ILoggerRepository, LoggerRepository>();
            services.AddScoped<IJobScheduleRepository, JobScheduleRepository>();
            services.AddScoped<IHoldingCompanyRepository, HoldingCompanyRepository>();

            // a trava abre a própria conexão a cada pedido: não guarda estado entre execuções
            services.AddSingleton<IJobLock, SqlServerJobLock>();

            // teto da carga inicial (InitialLoad:MaxMonths): lido uma vez, vale para o processo
            services.AddSingleton<IInitialLoad, InitialLoad>();

            // empresa da execução: uma por escopo (execução agendada ou requisição)
            services.AddScoped<ICompanyContext, CompanyContext>();
            services.AddScoped<ILoggerService, LoggerService>();

            services.AddScoped<IExceptionHandler, ExceptionHandler>();
            services.AddScoped<ICommandHandler, CommandHandler>();

            return services;
        }
    }
}

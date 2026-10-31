using Application.Shared.Abstractions.Services;
using Hangfire;
using Hangfire.SqlServer;
using Hosts.Scheduling;
using Infrastructure.AfterSale.DependencyInjection;
using Infrastructure.DataWarehouse.DependencyInjection;
using Infrastructure.Shared;
//using Infrastructure.Dootax;
//using Infrastructure.Jadlog.DependencyInjection;
using Infrastructure.LinxCommerce.DependencyInjection;
using Infrastructure.LinxMicrovix.Outbound.DependencyInjection;
using Infrastructure.Movidesk.DependencyInjection;
//using Infrastructure.Oto;
//using Infrastructure.Stone;
//using Infrastructure.TotalExpress.DependencyInjection;
using Infrastructure.Totvs.DependencyInjection;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using WebApi.Scheduling;

namespace WebApi.Extensions
{
    public static class ServicesExtensions
    {
        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScopedSharedServices();

            builder.Services.AddScopedLinxCommerceServices(builder.Configuration);
            builder.Services.AddScopedLinxMicrovixServices(builder.Configuration);
            //builder.Services.AddScopedFlashCourierServices(builder.Configuration);
            //builder.Services.AddScopedTotalExpressServices(builder.Configuration);
            builder.Services.AddScopedAfterSaleServices(builder.Configuration);
            //builder.Services.AddScopedJadlogServices(builder.Configuration);
            //builder.Services.AddScopedStoneServices(builder.Configuration);
            //builder.Services.AddScopedDootaxServices(builder.Configuration);
            builder.Services.AddScopedMovideskServices(builder.Configuration);
            //builder.Services.AddScopedOttoServices(builder.Configuration);
            builder.Services.AddScopedTotvsServices(builder.Configuration);
            builder.Services.AddScopedDataWarehouseServices(builder.Configuration);

            // Hangfire opcional (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §3): com Hangfire:Enabled = false o host sobe só
            // como API, sem storage, servidor, dashboard nem agenda, e o banco do Hangfire nem é acessado
            builder.Services.Configure<HangfireSettings>(builder.Configuration.GetSection(HangfireSettings.SectionName));

            if (builder.Configuration.GetValue<bool>($"{HangfireSettings.SectionName}:Enabled"))
                builder.Services.AddHangfireService(builder.Configuration);

            return builder;
        }

        public static IServiceCollection AddHangfireService(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(HangfireSettings.SectionName).Get<HangfireSettings>() ?? new();

            // a seção é validada aqui, antes de qualquer acesso ao storage: com um parâmetro faltando, o host não sobe (§3.1)
            var validation = new HangfireSettingsValidator(configuration).Validate(null, settings);

            if (validation.Failed)
                throw new OptionsValidationException(HangfireSettings.SectionName, typeof(HangfireSettings), validation.Failures);

            var connectionString = configuration.GetConnectionString(settings.Storage.ConnectionStringName);

            services
                .AddHangfire((serviceProvider, hangfire) => hangfire
                .UseFilter(new AutomaticRetryAttribute { Attempts = 0 })
                .UseFilter(new JobExpirationTimeoutFilter(TimeSpan.FromMinutes(settings.Storage.JobExpirationTimeoutInMinutes)))
                .UseFilter(new SkipConcurrentExecutionFilter(
                    serviceProvider.GetRequiredService<IJobLock>(),
                    serviceProvider.GetRequiredService<ILogger<SkipConcurrentExecutionFilter>>(),
                    settings.Concurrency.MaxConsecutiveSkips))
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    SchemaName = settings.Storage.SchemaName,
                    PrepareSchemaIfNecessary = settings.Storage.PrepareSchemaIfNecessary,
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(settings.Storage.CommandBatchMaxTimeoutInMinutes),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(settings.Storage.SlidingInvisibilityTimeoutInMinutes),
                    QueuePollInterval = TimeSpan.FromSeconds(settings.Storage.QueuePollIntervalInSeconds),
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // executor dos jobs da agenda (Hosts.Scheduling): grava o log da execução que cai no meio
            services.AddScheduledJobRunner();

            services.AddHangfireServer(options =>
            {
                // vazio, o Hangfire usa o nome da máquina
                if (!string.IsNullOrWhiteSpace(settings.Server.ServerName))
                    options.ServerName = settings.Server.ServerName;

                options.Queues = settings.Server.Queues.Distinct().ToArray();
                options.WorkerCount = settings.Server.WorkerCount;
            });

            return services;
        }
    }
}

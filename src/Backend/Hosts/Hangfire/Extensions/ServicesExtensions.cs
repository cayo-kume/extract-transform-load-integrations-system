using Hangfire.SqlServer;
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
using System.Reflection;

namespace Hangfire.Extensions
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

            //builder.Services.AddHangfireService(builder);

            return builder;
        }

        public static IServiceCollection AddHangfireService(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var serverName = builder.Configuration.GetSection("ConfigureServer").GetSection("ServerName").Value;
            var databaseName = builder.Configuration.GetSection("ConfigureServer").GetSection("GeneralDatabaseName").Value;
            var connectionstring = builder.Configuration.GetConnectionString("Connection").Replace("[catalog]", databaseName).Replace("[database]", databaseName);

            services
                .AddHangfire(configuration => configuration
                .UseFilter(new AutomaticRetryAttribute { Attempts = 0 })

                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionstring, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 50;
                options.ServerName = serverName;
                options.Queues = new[] { serverName.ToLower() };
            });

            return services;
        }
    }
}

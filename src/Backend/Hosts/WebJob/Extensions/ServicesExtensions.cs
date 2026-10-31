using Hosts.Scheduling;
using Infrastructure.AfterSale.DependencyInjection;
using Infrastructure.DataWarehouse.DependencyInjection;
using Infrastructure.LinxCommerce.DependencyInjection;
using Infrastructure.LinxMicrovix.Outbound.DependencyInjection;
using Infrastructure.Movidesk.DependencyInjection;
using Infrastructure.Shared;
using Infrastructure.Totvs.DependencyInjection;
using WebJob.Recovery;

namespace WebJob.Extensions
{
    public static class ServicesExtensions
    {
        /// <summary>
        /// os mesmos módulos do host WebApi (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md §3.2) e o executor de jobs. Um módulo
        /// novo entra aqui e no WebApi, junto com o mapa em Hosts.Scheduling
        /// </summary>
        public static HostApplicationBuilder AddServices(this HostApplicationBuilder builder)
        {
            builder.Services.AddScopedSharedServices();

            builder.Services.AddScopedLinxCommerceServices(builder.Configuration);
            builder.Services.AddScopedLinxMicrovixServices(builder.Configuration);
            builder.Services.AddScopedAfterSaleServices(builder.Configuration);
            builder.Services.AddScopedMovideskServices(builder.Configuration);
            builder.Services.AddScopedTotvsServices(builder.Configuration);
            builder.Services.AddScopedDataWarehouseServices(builder.Configuration);

            builder.Services.AddScheduledJobRunner();

            // recuperação dos WebJobs travados (job 14000, PLANO-HOST-WEBJOB.md passo 7): só a seção Recovery e o cliente da Azure;
            // o cliente só é chamado quando algum heartbeat está off
            builder.Services.Configure<RecoverySettings>(builder.Configuration.GetSection(RecoverySettings.SectionName));
            builder.Services.AddHttpClient<IAzureWebJobsClient, AzureWebJobsClient>();
            builder.Services.AddScoped<IStuckJobsRecovery, StuckJobsRecovery>();

            return builder;
        }
    }
}

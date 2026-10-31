using Hosts.Scheduling;
using Infrastructure.AfterSale.DependencyInjection;
using Infrastructure.DataWarehouse.DependencyInjection;
using Infrastructure.LinxCommerce.DependencyInjection;
using Infrastructure.LinxMicrovix.Outbound.DependencyInjection;
using Infrastructure.Movidesk.DependencyInjection;
using Infrastructure.Shared;
using Infrastructure.Totvs.DependencyInjection;

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

            return builder;
        }
    }
}

using Application.DataWarehouse.Abstractions.Services;
using Application.DataWarehouse.Services;
using Domain.DataWarehouse.Abstractions.Repositorys;
using Infrastructure.DataWarehouse.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataWarehouse.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedDataWarehouseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDataWarehouseService, DataWarehouseService>();
            services.AddScoped<IDataWarehouseRepository, DataWarehouseRepository>();

            return services;
        }
    }
}

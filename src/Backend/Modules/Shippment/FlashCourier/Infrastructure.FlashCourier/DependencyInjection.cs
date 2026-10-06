using Application.FlashCourier.Interfaces;
using Application.FlashCourier.Services;
using Domain.FlashCourier.Interfaces.Repository;
using Infrastructure.FlashCourier.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Infrastructure.FlashCourier.Data;
using Domain.Core.Abstractions.Api;
using Infrastructure.Core.Api;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FlashCourier.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedFlashCourierServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyedScoped<IApiClient>("FlashCourierAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "FlashCourierAPI"));
            services.AddHttpClient("FlashCourierAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["FlashCourier:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<IFlashCourierService, FlashCourierService>();
            services.AddScoped<IFlashCourierRepository, FlashCourierRepository>();

            return services;
        }

        public static IServiceCollection AddDbContextFlashCourierService(this IServiceCollection services, string databaseType, string connectionstring)
        {
            if (databaseType == "SQLServer")
            {
                services.AddDbContext<FlashCourierDbContext>((serviceProvider, options) =>
                {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    options.UseSqlServer(connectionstring);
                });
            }

            if (databaseType == "MySql")
            {
                services.AddDbContext<FlashCourierDbContext>((serviceProvider, options) =>
                {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    options.UseMySQL(connectionstring);
                });
            }

            if (databaseType == "Postgree")
            {
                services.AddDbContext<FlashCourierDbContext>((serviceProvider, options) =>
                {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    options.UseNpgsql(connectionstring);
                });
            }

            return services;
        }
    }
}

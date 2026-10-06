using Application.TotalExpress.Abstractions.Services;
using Application.TotalExpress.Services;
using Domain.TotalExpress.Abstractions.Api;
using Domain.TotalExpress.Abstractions.Repository;
using Infrastructure.TotalExpress.Api;
using Infrastructure.TotalExpress.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.TotalExpress.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedTotalExpressServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAPICall, APICall>();
            services.AddHttpClient("TotalExpressAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["TotalExpress:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });
            services.AddHttpClient("TotalExpressEdiAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["TotalExpress:BaseUrlEdi"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<ITotalExpressService, TotalExpressService>();
            services.AddScoped<ITotalExpressRepository, TotalExpressRepository>();

            return services;
        }
    }
}

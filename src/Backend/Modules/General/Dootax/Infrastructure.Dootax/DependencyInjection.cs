using Application.Dootax.Handlers.Commands;
using Application.Dootax.Interfaces.Handlers.Commands;
using Application.Dootax.Interfaces.Services;
using Application.Dootax.Services;
using Domain.Dootax.Interfaces.Apis;
using Domain.Dootax.Interfaces.Repositorys;
using Infrastructure.Dootax.Api;
using Infrastructure.Dootax.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Dootax
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedDootaxServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAPICall, APICall>();
            services.AddHttpClient("DootaxAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Dootax:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<IDootaxService, DootaxService>();
            services.AddScoped<IDootaxRepository, DootaxRepository>();
            services.AddScoped<IDootaxCommandHandler, DootaxCommandHandler>();

            return services;
        }
    }
}

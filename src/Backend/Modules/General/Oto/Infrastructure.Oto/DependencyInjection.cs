using Application.Oto.Factories;
using Application.Oto.Handlers.Commands;
using Application.Oto.Interfaces.Handlers.Commands;
using Application.Oto.Interfaces.Services;
using Application.Oto.Services;
using Domain.Core.Abstractions.Api;
using Domain.Oto.Interfaces.Repositorys;
using Infrastructure.Core.Api;
using Infrastructure.Oto.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Oto
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedOttoServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyedScoped<IApiClient>("OttoAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "OttoAPI"));
            services.AddHttpClient("OttoAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Oto:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });
            services.AddHttpClient("OttoAPIHmg", client =>
            {
                client.BaseAddress = new Uri(configuration["Oto:BaseUrlHmg"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<IOtoService, OtoService>();
            services.AddScoped<IOtoRepository, OtoRepository>();
            services.AddScoped<IOtoCommandHandler, OtoCommandHandler>();

            return services;
        }
    }
}

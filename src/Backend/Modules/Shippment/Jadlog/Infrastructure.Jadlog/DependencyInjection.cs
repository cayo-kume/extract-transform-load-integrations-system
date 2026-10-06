using Application.Jadlog.Abstractions.Services;
using Application.Jadlog.Services;
using Domain.Jadlog.Abstractions.Repositorys;
using Domain.Jadlog.CustomValidations;
using FluentValidation;
using Domain.Core.Abstractions.Api;
using Infrastructure.Core.Api;
using Infrastructure.Jadlog.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Jadlog.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedJadlogServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("JadlogAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Jadlog:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });
            services.AddHttpClient("JadlogTechAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Jadlog:BaseUrlTech"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddKeyedScoped<IApiClient>("JadlogAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "JadlogAPI"));
            services.AddKeyedScoped<IApiClient>("JadlogTechAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "JadlogTechAPI"));

            services.AddValidatorsFromAssemblyContaining<ConsultaValidator>();
            services.AddValidatorsFromAssemblyContaining<EventoValidator>();
            services.AddValidatorsFromAssemblyContaining<RecebedorValidator>();
            services.AddValidatorsFromAssemblyContaining<RootValidator>();
            services.AddValidatorsFromAssemblyContaining<TrackingValidator>();
            services.AddValidatorsFromAssemblyContaining<VolumeValidator>();

            services.AddScoped<IJadlogService, JadlogService>();
            services.AddScoped<IJadlogRepository, JadlogRepository>();

            return services;
        }
    }
}

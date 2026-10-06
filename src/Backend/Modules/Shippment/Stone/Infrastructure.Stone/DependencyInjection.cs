using Application.Stone.Abstractions.Handlers.Commands;
using Application.Stone.Abstractions.Services;
using Application.Stone.Handlers.Commands;
using Application.Stone.Services;
using Application.Stone.Validations;
using Domain.Core.Abstractions.Api;
using Domain.Stone.Abstractions.Repository;
using FluentValidation;
using Infrastructure.Core.Api;
using Infrastructure.Stone.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Stone
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedStoneServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyedScoped<IApiClient>("StoneAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "StoneAPI"));
            services.AddHttpClient("StoneAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Stone:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<IStoneService, StoneService>();
            services.AddScoped<IStoneRepository, StoneRepository>();
            services.AddScoped<IStoneLogisticaCommandHandler, StoneLogisticaCommandHandler>();

            services.AddValidatorsFromAssemblyContaining<StoneOrderValidator>();

            return services;
        }
    }
}

using Application.AfterSale.Abstractions.Handlers.Commands;
using Application.AfterSale.Abstractions.Services;
using Application.AfterSale.Handlers.Commands;
using Application.AfterSale.Services;
using Application.AfterSale.Validations;
using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Api;
using FluentValidation;
using Infrastructure.AfterSale.Repositorys;
using Infrastructure.Shared.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.AfterSale.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedAfterSaleServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyedScoped<IApiClient>("AfterSaleAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "AfterSaleAPI"));
            services.AddHttpClient("AfterSaleAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["AfterSale:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<IAfterSaleMeService, AfterSaleMeService>();
            services.AddScoped<IAfterSaleMeRepository, AfterSaleMeRepository>();

            services.AddScoped<IAfterSaleRefundsService, AfterSaleRefundsService>();
            services.AddScoped<IAfterSaleRefundsRepository, AfterSaleRefundsRepository>();

            services.AddScoped<IAfterSaleReversesService, AfterSaleReversesService>();
            services.AddScoped<IAfterSaleReversesRepository, AfterSaleReversesRepository>();
            
            services.AddScoped<IAfterSaleRepository, AfterSaleRepository>();

            services.AddScoped<IAfterSaleCommandHandler, AfterSaleCommandHandler>();

            services.AddValidatorsFromAssemblyContaining<EcommerceValidation>();

            return services;
        }
    }
}

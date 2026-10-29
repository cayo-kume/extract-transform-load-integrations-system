using Application.LinxCommerce.Abstractions.Handlers.Commands;
using Application.LinxCommerce.Abstractions.Services;
using Application.LinxCommerce.Handlers.Commands;
using Application.LinxCommerce.Services;
using Application.LinxCommerce.Validations.Orders;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Api;
using FluentValidation;
using Infrastructure.LinxCommerce.Repositorys;
using Infrastructure.Shared.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.LinxCommerce.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedLinxCommerceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // sem BaseAddress: cada cliente tem o seu ambiente (base_url de linxcommerce.LinxCommerceParameters) e a rota vai absoluta
            services.AddKeyedScoped<IApiClient>("LinxCommerceAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "LinxCommerceAPI"));
            services.AddHttpClient("LinxCommerceAPI", client =>
            {
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<ILinxCommerceService, LinxCommerceService>();
            services.AddScoped<ILinxCommerceRepository, LinxCommerceRepository>();

            services.AddScoped<ILinxCommerceOrdersService, LinxCommerceOrdersService>();
            services.AddScoped<ILinxCommerceOrdersRepository, LinxCommerceOrdersRepository>();

            services.AddScoped<ILinxCommerceCustomersService, LinxCommerceCustomersService>();
            services.AddScoped<ILinxCommerceCustomersRepository, LinxCommerceCustomersRepository>();

            services.AddScoped<ILinxCommerceCatalogService, LinxCommerceCatalogService>();
            services.AddScoped<ILinxCommerceCatalogRepository, LinxCommerceCatalogRepository>();

            services.AddScoped<ILinxCommerceSalesRepresentativesService, LinxCommerceSalesRepresentativesService>();
            services.AddScoped<ILinxCommerceSalesRepresentativesRepository, LinxCommerceSalesRepresentativesRepository>();

            services.AddScoped<ILinxCommerceCommandHandler, LinxCommerceCommandHandler>();

            // registra todos os validadores do assembly numa chamada só, como o Movidesk (o código antigo fazia 36 chamadas)
            services.AddValidatorsFromAssemblyContaining<OrderValidator>();

            return services;
        }
    }
}

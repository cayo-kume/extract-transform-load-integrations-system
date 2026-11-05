using Application.LinxMicrovix.Outbound.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Application.LinxMicrovix.Outbound.Handlers.Commands;
using Application.LinxMicrovix.Outbound.Services;
using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Api;
using FluentValidation;
using Infrastructure.LinxMicrovix.Outbound.Repositorys;
using Infrastructure.Shared.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.LinxMicrovix.Outbound.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedLinxMicrovixServices(this IServiceCollection services, IConfiguration configuration)
        {
            // um cliente para as duas APIs (ERP e B2C): as duas responderam na URL da v274; a rota de cada chamada é o nome do
            // método. O portal de teste (webapi-aceitacao) é só outro LinxMicrovix:BaseUrl no appsettings do ambiente
            services.AddKeyedScoped<IApiClient>("LinxMicrovixAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "LinxMicrovixAPI"));
            services.AddHttpClient("LinxMicrovixAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["LinxMicrovix:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            // os 29 services de leitura, na ordem da fase 2 (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §9)
            services.AddScoped<ILinxMicrovixService, LinxMicrovixService>();
            services.AddScoped<ILinxMicrovixRepository, LinxMicrovixRepository>();

            services.AddScoped<ILinxMicrovixStoresService, LinxMicrovixStoresService>();
            services.AddScoped<ILinxMicrovixStoresRepository, LinxMicrovixStoresRepository>();

            services.AddScoped<ILinxMicrovixB2CStoresService, LinxMicrovixB2CStoresService>();
            services.AddScoped<ILinxMicrovixB2CStoresRepository, LinxMicrovixB2CStoresRepository>();

            services.AddScoped<ILinxMicrovixProductAttributesService, LinxMicrovixProductAttributesService>();
            services.AddScoped<ILinxMicrovixProductAttributesRepository, LinxMicrovixProductAttributesRepository>();

            services.AddScoped<ILinxMicrovixTaxesService, LinxMicrovixTaxesService>();
            services.AddScoped<ILinxMicrovixTaxesRepository, LinxMicrovixTaxesRepository>();

            services.AddScoped<ILinxMicrovixTaxSettingsService, LinxMicrovixTaxSettingsService>();
            services.AddScoped<ILinxMicrovixTaxSettingsRepository, LinxMicrovixTaxSettingsRepository>();

            services.AddScoped<ILinxMicrovixProductsService, LinxMicrovixProductsService>();
            services.AddScoped<ILinxMicrovixProductsRepository, LinxMicrovixProductsRepository>();

            services.AddScoped<ILinxMicrovixStocksService, LinxMicrovixStocksService>();
            services.AddScoped<ILinxMicrovixStocksRepository, LinxMicrovixStocksRepository>();

            services.AddScoped<ILinxMicrovixPricesService, LinxMicrovixPricesService>();
            services.AddScoped<ILinxMicrovixPricesRepository, LinxMicrovixPricesRepository>();

            services.AddScoped<ILinxMicrovixCustomersService, LinxMicrovixCustomersService>();
            services.AddScoped<ILinxMicrovixCustomersRepository, LinxMicrovixCustomersRepository>();

            services.AddScoped<ILinxMicrovixSellersService, LinxMicrovixSellersService>();
            services.AddScoped<ILinxMicrovixSellersRepository, LinxMicrovixSellersRepository>();

            services.AddScoped<ILinxMicrovixPaymentsService, LinxMicrovixPaymentsService>();
            services.AddScoped<ILinxMicrovixPaymentsRepository, LinxMicrovixPaymentsRepository>();

            services.AddScoped<ILinxMicrovixLoyaltyService, LinxMicrovixLoyaltyService>();
            services.AddScoped<ILinxMicrovixLoyaltyRepository, LinxMicrovixLoyaltyRepository>();

            services.AddScoped<ILinxMicrovixMovementsService, LinxMicrovixMovementsService>();
            services.AddScoped<ILinxMicrovixMovementsRepository, LinxMicrovixMovementsRepository>();

            services.AddScoped<ILinxMicrovixPosSalesService, LinxMicrovixPosSalesService>();
            services.AddScoped<ILinxMicrovixPosSalesRepository, LinxMicrovixPosSalesRepository>();

            services.AddScoped<ILinxMicrovixReturnsService, LinxMicrovixReturnsService>();
            services.AddScoped<ILinxMicrovixReturnsRepository, LinxMicrovixReturnsRepository>();

            services.AddScoped<ILinxMicrovixConsignmentsService, LinxMicrovixConsignmentsService>();
            services.AddScoped<ILinxMicrovixConsignmentsRepository, LinxMicrovixConsignmentsRepository>();

            services.AddScoped<ILinxMicrovixOrdersService, LinxMicrovixOrdersService>();
            services.AddScoped<ILinxMicrovixOrdersRepository, LinxMicrovixOrdersRepository>();

            services.AddScoped<ILinxMicrovixPurchaseOrdersService, LinxMicrovixPurchaseOrdersService>();
            services.AddScoped<ILinxMicrovixPurchaseOrdersRepository, LinxMicrovixPurchaseOrdersRepository>();

            services.AddScoped<ILinxMicrovixFiscalDocumentsService, LinxMicrovixFiscalDocumentsService>();
            services.AddScoped<ILinxMicrovixFiscalDocumentsRepository, LinxMicrovixFiscalDocumentsRepository>();

            services.AddScoped<ILinxMicrovixFinancialService, LinxMicrovixFinancialService>();
            services.AddScoped<ILinxMicrovixFinancialRepository, LinxMicrovixFinancialRepository>();

            services.AddScoped<ILinxMicrovixServiceOrdersService, LinxMicrovixServiceOrdersService>();
            services.AddScoped<ILinxMicrovixServiceOrdersRepository, LinxMicrovixServiceOrdersRepository>();

            services.AddScoped<ILinxMicrovixOpticsService, LinxMicrovixOpticsService>();
            services.AddScoped<ILinxMicrovixOpticsRepository, LinxMicrovixOpticsRepository>();

            services.AddScoped<ILinxMicrovixB2CCustomersService, LinxMicrovixB2CCustomersService>();
            services.AddScoped<ILinxMicrovixB2CCustomersRepository, LinxMicrovixB2CCustomersRepository>();

            services.AddScoped<ILinxMicrovixB2CProductAttributesService, LinxMicrovixB2CProductAttributesService>();
            services.AddScoped<ILinxMicrovixB2CProductAttributesRepository, LinxMicrovixB2CProductAttributesRepository>();

            services.AddScoped<ILinxMicrovixB2CProductsService, LinxMicrovixB2CProductsService>();
            services.AddScoped<ILinxMicrovixB2CProductsRepository, LinxMicrovixB2CProductsRepository>();

            services.AddScoped<ILinxMicrovixB2CProductImagesService, LinxMicrovixB2CProductImagesService>();
            services.AddScoped<ILinxMicrovixB2CProductImagesRepository, LinxMicrovixB2CProductImagesRepository>();

            services.AddScoped<ILinxMicrovixB2CProductFieldsService, LinxMicrovixB2CProductFieldsService>();
            services.AddScoped<ILinxMicrovixB2CProductFieldsRepository, LinxMicrovixB2CProductFieldsRepository>();

            services.AddScoped<ILinxMicrovixB2CStocksAndPricesService, LinxMicrovixB2CStocksAndPricesService>();
            services.AddScoped<ILinxMicrovixB2CStocksAndPricesRepository, LinxMicrovixB2CStocksAndPricesRepository>();

            services.AddScoped<ILinxMicrovixB2COrdersService, LinxMicrovixB2COrdersService>();
            services.AddScoped<ILinxMicrovixB2COrdersRepository, LinxMicrovixB2COrdersRepository>();

            services.AddScoped<ILinxMicrovixCommandHandler, LinxMicrovixCommandHandler>();

            // registra todos os validadores do assembly numa chamada só, como o Movidesk e o Totvs
            services.AddValidatorsFromAssemblyContaining<LinxMicrovixCommandHandler>();

            return services;
        }
    }
}

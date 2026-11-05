using Application.Totvs.Abstractions.Handlers.Commands;
using Application.Totvs.Abstractions.Services;
using Application.Totvs.Handlers.Commands;
using Application.Totvs.Services;
using Application.Totvs.Validations.Reads.Sellers;
using Domain.Shared.Abstractions.Api;
using Domain.Totvs.Abstractions.Repositorys;
using FluentValidation;
using Infrastructure.Shared.Api;
using Infrastructure.Totvs.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Totvs.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedTotvsServices(this IServiceCollection services, IConfiguration configuration)
        {
            // homologação é só outro Totvs:BaseUrl no appsettings do ambiente
            services.AddKeyedScoped<IApiClient>("TotvsAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "TotvsAPI"));
            services.AddHttpClient("TotvsAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Totvs:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            // os services de leitura entram aqui um por vez, à medida que forem reescritos (src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md §9)
            services.AddScoped<ITotvsService, TotvsService>();
            services.AddScoped<ITotvsRepository, TotvsRepository>();

            services.AddScoped<ITotvsPeopleService, TotvsPeopleService>();
            services.AddScoped<ITotvsPeopleRepository, TotvsPeopleRepository>();
            services.AddScoped<ITotvsPeopleCommandHandler, TotvsPeopleCommandHandler>();

            services.AddScoped<ITotvsSellersService, TotvsSellersService>();
            services.AddScoped<ITotvsSellersRepository, TotvsSellersRepository>();
            services.AddScoped<ITotvsSellersCommandHandler, TotvsSellersCommandHandler>();

            services.AddScoped<ITotvsProductsService, TotvsProductsService>();
            services.AddScoped<ITotvsProductsRepository, TotvsProductsRepository>();
            services.AddScoped<ITotvsProductsCommandHandler, TotvsProductsCommandHandler>();

            services.AddScoped<ITotvsPurchaseOrdersService, TotvsPurchaseOrdersService>();
            services.AddScoped<ITotvsPurchaseOrdersRepository, TotvsPurchaseOrdersRepository>();
            services.AddScoped<ITotvsPurchaseOrdersCommandHandler, TotvsPurchaseOrdersCommandHandler>();

            services.AddScoped<ITotvsSalesOrdersService, TotvsSalesOrdersService>();
            services.AddScoped<ITotvsSalesOrdersRepository, TotvsSalesOrdersRepository>();
            services.AddScoped<ITotvsSalesOrdersCommandHandler, TotvsSalesOrdersCommandHandler>();

            services.AddScoped<ITotvsAccountsPayableService, TotvsAccountsPayableService>();
            services.AddScoped<ITotvsAccountsPayableRepository, TotvsAccountsPayableRepository>();
            services.AddScoped<ITotvsAccountsPayableCommandHandler, TotvsAccountsPayableCommandHandler>();

            services.AddScoped<ITotvsInvoicesService, TotvsInvoicesService>();
            services.AddScoped<ITotvsInvoicesRepository, TotvsInvoicesRepository>();
            services.AddScoped<ITotvsInvoicesCommandHandler, TotvsInvoicesCommandHandler>();

            services.AddScoped<ITotvsPersonCatalogsService, TotvsPersonCatalogsService>();
            services.AddScoped<ITotvsPersonCatalogsRepository, TotvsPersonCatalogsRepository>();
            services.AddScoped<ITotvsPersonCatalogsCommandHandler, TotvsPersonCatalogsCommandHandler>();

            services.AddScoped<ITotvsRepresentativesService, TotvsRepresentativesService>();
            services.AddScoped<ITotvsRepresentativesRepository, TotvsRepresentativesRepository>();
            services.AddScoped<ITotvsRepresentativesCommandHandler, TotvsRepresentativesCommandHandler>();

            services.AddScoped<ITotvsProductCatalogsService, TotvsProductCatalogsService>();
            services.AddScoped<ITotvsProductCatalogsRepository, TotvsProductCatalogsRepository>();
            services.AddScoped<ITotvsProductCatalogsCommandHandler, TotvsProductCatalogsCommandHandler>();

            services.AddScoped<ITotvsProductPricesService, TotvsProductPricesService>();
            services.AddScoped<ITotvsProductPricesRepository, TotvsProductPricesRepository>();
            services.AddScoped<ITotvsProductPricesCommandHandler, TotvsProductPricesCommandHandler>();

            services.AddScoped<ITotvsBonusService, TotvsBonusService>();

            services.AddScoped<ITotvsCommandHandler, TotvsCommandHandler>();

            // registra todos os validadores do assembly numa chamada só, como o Movidesk
            services.AddValidatorsFromAssemblyContaining<SellerValidator>();

            return services;
        }
    }
}

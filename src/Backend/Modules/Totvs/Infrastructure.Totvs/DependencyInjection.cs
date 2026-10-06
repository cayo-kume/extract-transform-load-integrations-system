using Application.Totvs.Abstractions.Handlers.Commands;
using Application.Totvs.Abstractions.Services;
using Application.Totvs.Handlers.Commands;
using Application.Totvs.Services;
using Application.Totvs.Validations.Fiscal;
using Application.Totvs.Validations.Person.Customer;
using Application.Totvs.Validations.Person.LegalEntity;
using Application.Totvs.Validations.PurchaseOrder;
using Application.Totvs.Validations.SalesOrder;
using Domain.Shared.Abstractions.Api;
using Domain.Totvs.Abstractions.Repositorys;
using FluentValidation;
using Infrastructure.Shared.Api;
using Infrastructure.Totvs.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Totvs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedTotvsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("TotvsAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Totvs:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });
            services.AddHttpClient("TotvsHmg", client =>
            {
                client.BaseAddress = new Uri(configuration["Totvs:BaseUrlHmg"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddKeyedScoped<IApiClient>("TotvsHmg", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "TotvsHmg"));

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductCommandHandler, ProductCommandHandler>();

            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IPersonCommandHandler, PersonCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<CustomerValidator>();
            services.AddValidatorsFromAssemblyContaining<LegalEntityValidator>();
            services.AddValidatorsFromAssemblyContaining<Application.Totvs.Validations.Person.Branch.BranchValidator>();

            services.AddScoped<IFiscalService, FiscalService>();
            services.AddScoped<IFiscalRepository, FiscalRepository>();
            services.AddScoped<IFiscalCommandHandler, FiscalCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<Application.Totvs.Validations.Fiscal.InvoiceValidator>();

            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IPurchaseOrderCommandHandler, PurchaseOrderCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<PurchaseOrderValidator>();

            services.AddScoped<ISalesOrderService, SalesOrderService>();
            services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
            services.AddScoped<ISalesOrderCommandHandler, SalesOrderCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<SaleOrderValidator>();

            services.AddScoped<ISellerService, SellerService>();
            services.AddScoped<ISellerRepository, SellerRepository>();
            services.AddScoped<ISellerCommandHandler, SellerCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<Application.Totvs.Validations.Seller.SellerValidator>();

            services.AddScoped<IAccountsPayableService, AccountsPayableService>();
            services.AddScoped<IAccountsPayableRepository, AccountsPayableRepository>();
            services.AddScoped<IAccountsPayableCommandHandler, AccountsPayableCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<Application.Totvs.Validations.AccountsPayable.DuplicateValidator>();
            services.AddValidatorsFromAssemblyContaining<Application.Totvs.Validations.AccountsPayable.ComissionPaidValidator>();

            return services;
        }
    }
}

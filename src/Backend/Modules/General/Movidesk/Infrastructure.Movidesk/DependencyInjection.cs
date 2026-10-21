using Application.Movidesk.Abstractions.Handlers.Commands;
using Application.Movidesk.Abstractions.Services;
using Application.Movidesk.Handlers.Commands;
using Application.Movidesk.Services;
using Application.Movidesk.Validations.Ticket;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Api;
using FluentValidation;
using Infrastructure.Movidesk.Repositorys;
using Infrastructure.Shared.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Movidesk.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedMovideskServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyedScoped<IApiClient>("MovideskAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "MovideskAPI"));
            services.AddHttpClient("MovideskAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Movidesk:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<IMovideskTicketsService, MovideskTicketsService>();
            services.AddScoped<IMovideskTicketsRepository, MovideskTicketsRepository>();

            services.AddScoped<IMovideskPeopleService, MovideskPeopleService>();
            services.AddScoped<IMovideskPeopleRepository, MovideskPeopleRepository>();

            services.AddScoped<IMovideskServicesService, MovideskServicesService>();
            services.AddScoped<IMovideskServicesRepository, MovideskServicesRepository>();

            services.AddScoped<IMovideskService, MovideskService>();
            services.AddScoped<IMovideskRepository, MovideskRepository>();

            services.AddScoped<IMovideskCommandHandler, MovideskCommandHandler>();

            // registra todos os validadores do assembly (tickets e pessoas), como o AfterSale
            services.AddValidatorsFromAssemblyContaining<TicketValidator>();

            return services;
        }
    }
}

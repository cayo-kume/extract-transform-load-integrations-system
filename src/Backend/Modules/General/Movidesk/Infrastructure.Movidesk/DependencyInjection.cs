using Application.Movidesk.Abstractions.Handlers.Commands;
using Application.Movidesk.Abstractions.Services;
using Application.Movidesk.Handlers.Commands;
using Application.Movidesk.Services;
using Application.Movidesk.Validations.Reads.Tickets;
using Application.Movidesk.Validations.Writes.Tickets;
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
            services.AddScoped<IMovideskTicketsCommandHandler, MovideskTicketsCommandHandler>();

            services.AddScoped<IMovideskPeopleService, MovideskPeopleService>();
            services.AddScoped<IMovideskPeopleRepository, MovideskPeopleRepository>();
            services.AddScoped<IMovideskPeopleCommandHandler, MovideskPeopleCommandHandler>();

            services.AddScoped<IMovideskServicesService, MovideskServicesService>();
            services.AddScoped<IMovideskServicesRepository, MovideskServicesRepository>();

            services.AddScoped<IMovideskService, MovideskService>();
            services.AddScoped<IMovideskRepository, MovideskRepository>();

            services.AddScoped<IMovideskCommandHandler, MovideskCommandHandler>();

            // registra todos os validadores do assembly (tickets e pessoas), como o AfterSale
            services.AddValidatorsFromAssemblyContaining<TicketValidator>();

            // escrita de tickets: dois validadores do mesmo DTO (criação e alteração), então o controller recebe o tipo concreto
            services.AddScoped<PostTicketValidator>();
            services.AddScoped<PatchTicketValidator>();

            // escrita de pessoas: o mesmo padrão (criação e alteração validam o mesmo DTO)
            services.AddScoped<Application.Movidesk.Validations.Writes.People.PostPersonValidator>();
            services.AddScoped<Application.Movidesk.Validations.Writes.People.PatchPersonValidator>();
            services.AddScoped<Application.Movidesk.Validations.Writes.People.RelationshipsValidator>();

            // escrita de serviços: o mesmo padrão (criação e alteração validam o mesmo DTO)
            services.AddScoped<Application.Movidesk.Validations.Writes.Services.PostServiceValidator>();
            services.AddScoped<Application.Movidesk.Validations.Writes.Services.PatchServiceValidator>();

            return services;
        }
    }
}

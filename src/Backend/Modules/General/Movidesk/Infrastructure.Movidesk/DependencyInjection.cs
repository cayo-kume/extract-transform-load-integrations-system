using Application.Movidesk.Validations.Person;
using Application.Movidesk.Handlers.Commands;
using Application.Movidesk.Interfaces.Services;
using Application.Movidesk.Services;
using Domain.Core.Abstractions.Api;
using Domain.Movidesk.Interfaces.Handlers.Commands;
using Domain.Movidesk.Interfaces.Repositorys;
using FluentValidation;
using Infrastructure.Core.Api;
using Infrastructure.Movidesk.Repositorys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Movidesk.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedMovideskServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScopedMovideskValidations();

            services.AddKeyedScoped<IApiClient>("MovideskAPI", (sp, _) =>
                new ApiClient(sp.GetRequiredService<IHttpClientFactory>(), sp.GetRequiredService<ILogger<ApiClient>>(), "MovideskAPI"));
            services.AddHttpClient("MovideskAPI", client =>
            {
                client.BaseAddress = new Uri(configuration["Movidesk:BaseUrl"]!);
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddScoped<IMovideskService, MovideskService>();
            services.AddScoped<IMovideskRepository, MovideskRepository>();
            services.AddScoped<IMovideskCommandHandler, MovideskCommandHandler>();

            return services;
        }

        private static IServiceCollection AddScopedMovideskValidations(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<PersonValidator>();
            services.AddValidatorsFromAssemblyContaining<ContactValidator>();
            services.AddValidatorsFromAssemblyContaining<EmailValidator>();
            services.AddValidatorsFromAssemblyContaining<AddressValidator>();
            services.AddValidatorsFromAssemblyContaining<RelationshipValidator>();

            return services;
        }
    }
}

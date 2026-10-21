using Application.Movidesk.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Movidesk.Services
{
    /// <summary>
    /// catálogo de serviços da Movidesk (GET /services). Os métodos voltam quando as tabelas de serviços forem modeladas.
    /// </summary>
    public class MovideskServicesService : IMovideskServicesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IMovideskRepository _movideskRepository;
        private readonly IMovideskServicesRepository _movideskServicesRepository;

        public MovideskServicesService(
            [FromKeyedServices("MovideskAPI")] IApiClient apiClient,
            ILoggerService logger,
            IMovideskRepository movideskRepository,
            IMovideskServicesRepository movideskServicesRepository
        ) =>
            (_apiClient, _logger, _movideskRepository, _movideskServicesRepository) =
            (apiClient, logger, movideskRepository, movideskServicesRepository);

        public Task<bool?> GetService() => throw new NotImplementedException();

        public Task<bool?> GetServices() => throw new NotImplementedException();
    }
}

using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskServicesRepository : IMovideskServicesRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskServicesRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);
    }
}

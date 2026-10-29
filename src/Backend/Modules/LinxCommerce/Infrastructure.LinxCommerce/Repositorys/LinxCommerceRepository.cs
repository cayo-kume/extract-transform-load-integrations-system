using Application.LinxCommerce.Abstractions.Handlers.Commands;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.LinxCommerce.Repositorys
{
    public class LinxCommerceRepository : ILinxCommerceRepository
    {
        private readonly ILinxCommerceCommandHandler _linxCommerceCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public LinxCommerceRepository(ILinxCommerceCommandHandler linxCommerceCommandHandler, ISharedRepository coreRepository) =>
            (_linxCommerceCommandHandler, _coreRepository) = (linxCommerceCommandHandler, coreRepository);

        public async Task<IEnumerable<Company?>> GetCompanys()
        {
            string sql = _linxCommerceCommandHandler.CreateGetCompanysQuery();

            return await _coreRepository.GetRecords<Company>(sql);
        }
    }
}

using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Parameters;

namespace Infrastructure.Totvs.Repositorys
{
    public class TotvsRepository : ITotvsRepository
    {
        private readonly ITotvsCommandHandler _totvsCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsRepository(ITotvsCommandHandler totvsCommandHandler, ISharedRepository coreRepository) =>
            (_totvsCommandHandler, _coreRepository) = (totvsCommandHandler, coreRepository);

        public async Task<IEnumerable<Company?>> GetCompanys()
        {
            string sql = _totvsCommandHandler.CreateGetCompanysQuery();

            return await _coreRepository.GetRecords<Company>(sql);
        }

        public async Task<IEnumerable<int>> GetBranchCodes(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetBranchCodesQuery(doc_company);

            return (await _coreRepository.GetRecords<int>(sql)).ToList();
        }

        public async Task<IEnumerable<int>> GetBrandBranchCodes(string doc_company, int baseBranchCode)
        {
            string sql = _totvsCommandHandler.CreateGetBrandBranchCodesQuery(doc_company, baseBranchCode);

            return (await _coreRepository.GetRecords<int>(sql)).ToList();
        }
    }
}

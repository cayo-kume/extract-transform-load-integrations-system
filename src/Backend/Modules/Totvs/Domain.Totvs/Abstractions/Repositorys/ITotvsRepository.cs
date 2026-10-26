using Domain.Totvs.Entities.Parameters;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsRepository
    {
        public Task<IEnumerable<Company?>> GetCompanys();
        public Task<IEnumerable<int>> GetBranchCodes(string doc_company);
        public Task<IEnumerable<int>> GetBrandBranchCodes(string doc_company, int baseBranchCode);
        public bool InsertIntoTotvsRequests(Domain.Totvs.Entities.Writes.Requests.Request request);
    }
}

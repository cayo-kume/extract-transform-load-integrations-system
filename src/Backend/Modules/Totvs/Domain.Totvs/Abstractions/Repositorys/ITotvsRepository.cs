using Domain.Totvs.Entities.Parameters;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsRepository
    {
        public Task<IEnumerable<Company?>> GetCompanys();
        public Task<IEnumerable<int>> GetBranchCodes(string doc_company);
        public Task<IEnumerable<int>> GetBrandBranchCodes(string doc_company, int baseBranchCode);
        public Task<int?> GetBranchCodeByCnpj(string doc_company, string cnpj);
        public bool InsertIntoTotvsRequests(Domain.Totvs.Entities.Writes.Requests.Request request);
        public Task<IEnumerable<Domain.Totvs.Entities.Writes.Requests.RequestCheck?>> GetRecentRequests(string doc_company, string route, string field_key_value, DateTime since);
    }
}

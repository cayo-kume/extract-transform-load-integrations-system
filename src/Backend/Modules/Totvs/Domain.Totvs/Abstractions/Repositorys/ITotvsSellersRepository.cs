using Domain.Totvs.Entities.Reads.Sellers;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsSellersRepository
    {
        public Task<DateTime?> GetSellersLastChange(string doc_company);
        public Task<IEnumerable<Seller?>> GetSellersByCodes(string doc_company, List<int> codes);
        public Task<bool> BulkInsertIntoTotvsSellers(List<Seller> data, Guid? parentExecutionGUID);
    }
}

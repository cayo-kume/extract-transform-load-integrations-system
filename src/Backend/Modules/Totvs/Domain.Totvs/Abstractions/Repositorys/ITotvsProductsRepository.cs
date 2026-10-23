using Domain.Totvs.Entities.Reads.Products;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsProductsRepository
    {
        public Task<DateTime?> GetSkusLastChange(string doc_company, int branchInfoCode);
        public Task<IEnumerable<Sku?>> GetSkusByIds(string doc_company, int branchInfoCode, List<int> referenceIds);
        public Task<bool> BulkInsertIntoTotvsSkus(List<Sku> data, Guid? parentExecutionGUID);
    }
}

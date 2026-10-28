using Domain.Totvs.Entities.Reads.Products.Values;
using Domain.Totvs.Entities.Reads.Products;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsProductsRepository
    {
        public Task<ValueState?> GetBalancesState(string doc_company, int branchCode);
        public Task<IEnumerable<ValueChange?>> GetBalancesChanges(string doc_company, int branchCode, List<int> productCodes);
        public Task<bool> BulkInsertIntoTotvsBalances(List<Balance> data, List<BalanceLocation> locations, Guid? parentExecutionGUID);
        public Task<DateTime?> GetSkusLastChange(string doc_company, int branchInfoCode);
        public Task<IEnumerable<Sku?>> GetSkusByIds(string doc_company, int branchInfoCode, List<int> referenceIds);
        public Task<bool> BulkInsertIntoTotvsSkus(List<Sku> data, Guid? parentExecutionGUID);
        public Task<IEnumerable<int>> GetSkusProductCodes(string doc_company, int branchInfoCode);
        public Task<IEnumerable<Entities.Reads.Products.Similars.ProductSimilar?>> GetSimilarHashes(string doc_company);
        public Task<DateTime?> GetSimilarLastWindowEnd(string doc_company);
        public Task<bool> BulkInsertIntoTotvsProductsSimilar(List<Entities.Reads.Products.Similars.ProductSimilar> data, Guid? parentExecutionGUID);
        public Task<IEnumerable<Entities.Reads.Products.Batches.Batch?>> GetBatchesHashes(string doc_company, int branchCode);
        public Task<bool> BulkInsertIntoTotvsBatches(List<Entities.Reads.Products.Batches.Batch> data, Guid? parentExecutionGUID);
    }
}

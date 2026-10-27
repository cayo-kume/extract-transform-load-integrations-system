using Domain.Totvs.Entities.Reads.Products.Values;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsProductPricesRepository
    {
        public Task<ValueState?> GetPricesState(string doc_company, int branchCode);
        public Task<IEnumerable<ValueChange?>> GetPricesChanges(string doc_company, int branchCode, List<int> productCodes);
        public Task<bool> BulkInsertIntoTotvsPrices(List<Price> data, Guid? parentExecutionGUID);
        public Task<ValueState?> GetCostsState(string doc_company, int branchCode);
        public Task<IEnumerable<ValueChange?>> GetCostsChanges(string doc_company, int branchCode, List<int> productCodes);
        public Task<bool> BulkInsertIntoTotvsCosts(List<Cost> data, Guid? parentExecutionGUID);
    }
}

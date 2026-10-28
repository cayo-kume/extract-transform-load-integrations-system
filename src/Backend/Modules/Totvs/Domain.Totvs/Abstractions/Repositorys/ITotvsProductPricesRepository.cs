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
        public Task<IEnumerable<Entities.Writes.Products.CurrentProductValue?>> GetCurrentValues(string doc_company, List<int> productCodes);
        public Task<IEnumerable<Entities.Writes.Products.PromotionItemState?>> GetPromotionItems(string doc_company, int branchCode, int promotionCode);
        public Task<IEnumerable<int>> GetActivePriceTableCodes(string doc_company);
        public Task<ValueState?> GetPriceTableItemsState(string doc_company, int priceTableCode);
        public Task<IEnumerable<ValueChange?>> GetPriceTableItemsChanges(string doc_company, int priceTableCode, List<int> productCodes);
        public Task<bool> BulkInsertIntoTotvsPriceTablesItems(List<Entities.Reads.Products.PriceTables.PriceTableItem> data, Guid? parentExecutionGUID);
        public Task<IEnumerable<Entities.Reads.Products.Promotions.Promotion?>> GetPromotionsHashes(string doc_company, int branchCode);
        public Task<bool> BulkInsertIntoTotvsPromotions(List<Entities.Reads.Products.Promotions.Promotion> data, Guid? parentExecutionGUID);
    }
}

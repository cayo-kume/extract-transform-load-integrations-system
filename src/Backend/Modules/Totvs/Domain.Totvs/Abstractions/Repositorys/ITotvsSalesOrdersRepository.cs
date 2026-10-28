using Domain.Totvs.Entities.Reads.SalesOrders;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsSalesOrdersRepository
    {
        public Task<DateTime?> GetSalesOrdersLastChange(string doc_company);
        public Task<IEnumerable<SaleOrder?>> GetSalesOrdersByKeys(string doc_company, List<(int branchCode, int orderCode)> keys);
        public Task<bool> BulkInsertIntoTotvsSaleOrders(List<SaleOrder> data, Guid? parentExecutionGUID);
        public Task<DateTime?> GetSaleDiscountTypesLastChange(string doc_company);
        public Task<IEnumerable<Entities.Reads.SalesOrders.Catalogs.SaleDiscountType?>> GetSaleDiscountTypes(string doc_company);
        public Task<bool> BulkInsertIntoTotvsSaleDiscountTypes(List<Entities.Reads.SalesOrders.Catalogs.SaleDiscountType> data, Guid? parentExecutionGUID);
        public Task<DateTime?> GetSaleClassificationsLastChange(string doc_company);
        public Task<IEnumerable<Entities.Reads.SalesOrders.Catalogs.SaleClassification?>> GetSaleClassifications(string doc_company);
        public Task<bool> BulkInsertIntoTotvsSaleClassifications(List<Entities.Reads.SalesOrders.Catalogs.SaleClassification> data, Guid? parentExecutionGUID);
        public Task<DateTime?> GetSaleOrderBatchesLastChange(string doc_company);
        public Task<IEnumerable<Entities.Reads.SalesOrders.Catalogs.SaleOrderBatch?>> GetSaleOrderBatches(string doc_company);
        public Task<bool> BulkInsertIntoTotvsSaleOrderBatches(List<Entities.Reads.SalesOrders.Catalogs.SaleOrderBatch> data, Guid? parentExecutionGUID);
        public Task<DateTime?> GetBillingSuggestionsLastChange(string doc_company);
        public Task<IEnumerable<Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestion?>> GetBillingSuggestionsByKeys(string doc_company, List<(int branchCode, int suggestionCode)> keys);
        public Task<bool> BulkInsertIntoTotvsBillingSuggestions(List<Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestion> data, Guid? parentExecutionGUID);
    }
}

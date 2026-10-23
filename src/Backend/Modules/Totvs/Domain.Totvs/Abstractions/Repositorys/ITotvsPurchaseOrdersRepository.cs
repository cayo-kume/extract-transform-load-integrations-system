using Domain.Totvs.Entities.Reads.PurchaseOrders;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsPurchaseOrdersRepository
    {
        public Task<DateTime?> GetPurchaseOrdersLastChange(string doc_company);
        public Task<IEnumerable<PurchaseOrder?>> GetPurchaseOrdersByKeys(string doc_company, List<(int branchCode, int orderCode)> keys);
        public Task<bool> BulkInsertIntoTotvsPurchaseOrders(List<PurchaseOrder> data, Guid? parentExecutionGUID);
    }
}

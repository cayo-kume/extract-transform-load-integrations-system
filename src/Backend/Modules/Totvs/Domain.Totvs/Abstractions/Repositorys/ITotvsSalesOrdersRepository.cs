using Domain.Totvs.Entities.Reads.SalesOrders;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsSalesOrdersRepository
    {
        public Task<DateTime?> GetSalesOrdersLastChange(string doc_company);
        public Task<IEnumerable<SaleOrder?>> GetSalesOrdersByKeys(string doc_company, List<(int branchCode, int orderCode)> keys);
        public Task<bool> BulkInsertIntoTotvsSaleOrders(List<SaleOrder> data, Guid? parentExecutionGUID);
    }
}

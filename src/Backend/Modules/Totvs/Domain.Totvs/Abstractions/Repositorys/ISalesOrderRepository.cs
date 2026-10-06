using Domain.Totvs.Entities.SalesOrder;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ISalesOrderRepository
    {
        public Task<List<Domain.Totvs.Dtos.SalesOrder.Item?>> GetExistingSalesOrders(List<string?> registros);

        public Task<bool> BulkInsertIntoTableSalesOrdersRaw(List<SaleOrder> registros, Guid? parentExecutionGUID);
    }
}

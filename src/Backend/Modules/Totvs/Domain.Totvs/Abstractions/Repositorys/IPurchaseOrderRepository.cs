using Domain.Totvs.Entities.PurchaseOrder;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface IPurchaseOrderRepository
    {
        public Task<List<Domain.Totvs.Dtos.PurchaseOrder.Item?>> GetExistingPurchaseOrders(List<string?> registros);

        public Task<bool> BulkInsertIntoTablePurchaseOrdersRaw(List<PurchaseOrder> registros, Guid? parentExecutionGUID);
    }
}

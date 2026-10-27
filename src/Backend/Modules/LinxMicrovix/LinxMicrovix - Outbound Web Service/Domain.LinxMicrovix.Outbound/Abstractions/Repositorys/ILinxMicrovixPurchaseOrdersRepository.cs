using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PurchaseOrders;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixPurchaseOrdersRepository
    {
        public Task<bool> BulkInsertIntoLinxPedidosCompra(List<LinxPedidosCompra> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPedidosCompraStatus(List<LinxPedidosCompraStatus> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPedidosCompraNumeroFornecedor(List<LinxPedidosCompraNumeroFornecedor> data, Guid? parentExecutionGUID);
    }
}

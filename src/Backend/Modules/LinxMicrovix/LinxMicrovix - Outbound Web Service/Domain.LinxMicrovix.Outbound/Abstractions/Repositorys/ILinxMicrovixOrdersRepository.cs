using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixOrdersRepository
    {
        public Task<bool> BulkInsertIntoLinxPedidosVenda(List<LinxPedidosVenda> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPlanosPedidoVenda(List<LinxPlanosPedidoVenda> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPedidosVendaChecklistEntregaLocal(List<LinxPedidosVendaChecklistEntregaLocal> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPedidosVendaChecklistEntregaArmazenamento(List<LinxPedidosVendaChecklistEntregaArmazenamento> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPedidosVendaChecklistEntregaDificuldade(List<LinxPedidosVendaChecklistEntregaDificuldade> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxOrcamentoPosicao(List<LinxOrcamentoPosicao> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxB2CPedidos(List<LinxB2CPedidos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxB2CPedidosItens(List<LinxB2CPedidosItens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxB2CPedidosStatus(List<LinxB2CPedidosStatus> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxB2CStatus(List<LinxB2CStatus> data, Guid? parentExecutionGUID);
    }
}

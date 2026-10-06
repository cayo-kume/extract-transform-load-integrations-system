using Domain.Jadlog.Entities;

namespace Domain.Jadlog.Abstractions.Repositorys
{
    public interface IJadlogRepository
    {
        public Task<bool> BulkInsertIntoTableRaw(List<Consulta> records, Guid? parentExecutionGUID);
        public Task<bool> InsertResponse(List<OrderSent>? pedidos, Guid? parentExecutionGuid);
        public Task<List<ShipmentId>> GetShipmentIds();
        public Task<Order> GetInvoicedOrder(string orderNumber);
        public Task<List<Order>> GetInvoicedOrders();
        public Task<IEnumerable<Parameters>> GetParameters();
        public Task<Parameters> GetParameters(string doc_tomador, string doc_remetente, string tipo_servico);
    }
}

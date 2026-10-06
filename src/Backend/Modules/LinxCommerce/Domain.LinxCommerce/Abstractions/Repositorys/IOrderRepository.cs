using Domain.LinxCommerce.Entities.Order;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Responses;

namespace Domain.LinxCommerce.Interfaces.Repositorys
{
    public interface IOrderRepository
    {
        public Task<bool> BulkInsertIntoTableRaw(LinxCommerceJobParameter jobParameter, List<Order.Root> registros);
        public Task<bool> BulkInsertIntoTableRaw(LinxCommerceJobParameter jobParameter, List<Order.Root> registros, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoOrdersStatusTable(LinxCommerceJobParameter jobParameter, List<SearchOrderStatus.Result> registros, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoUpdateTrackingNumberOrderTable(LinxCommerceJobParameter jobParameter, List<OrderTrackingNumber> registros, Guid? parentExecutionGUID);
        public Task<bool> InsertIntoUpdateTrackingNumberOrderTable(LinxCommerceJobParameter jobParameter, OrderTrackingNumber registro, Guid? parentExecutionGUID);
        public Task<IEnumerable<Order.Root?>> GetRegistersExists(IEnumerable<Guid?> guids);
        public Task<List<OrderTrackingNumber>> GetTrackingNumbersToUpdate();

        public Task<IEnumerable<Order.Root?>> GetLostOrdersRegisters();
    }
}

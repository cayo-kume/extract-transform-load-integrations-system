using Domain.LinxCommerce.Entities.Reads.Orders;

namespace Domain.LinxCommerce.Abstractions.Repositorys
{
    public interface ILinxCommerceOrdersRepository
    {
        public Task<DateTimeOffset?> GetOrdersLastModifiedDate(string doc_company);
        public Task<IEnumerable<Order?>> GetOrdersByIds(string doc_company, List<Guid> ids);
        public Task<bool> BulkInsertIntoLinxCommerceOrders(List<Order> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCommerceOrderStatus(List<OrderStatus> data, Guid? parentExecutionGUID);
    }
}

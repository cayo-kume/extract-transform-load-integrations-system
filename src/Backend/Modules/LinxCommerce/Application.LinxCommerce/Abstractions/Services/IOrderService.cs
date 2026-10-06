using Domain.LinxCommerce.Entities.Parameters;

namespace Application.LinxCommerce.Abstractions.Services
{
    public interface IOrderService
    {
        public Task<bool?> GetOrder(LinxCommerceJobParameter jobParameter, string? orderId);
        public Task<bool?> GetOrderByNumber(LinxCommerceJobParameter jobParameter, string? orderNumber);
        public Task<string?> GetOrderGroup();
        public Task<string?> GetOrderGroupByNumber();
        public Task<string?> GetOrderPackage();
        public Task<string?> GetOrderPayment();
        public Task<string?> GetOrderPayments();
        public Task<string?> GetOrderStatus();
        public Task<string?> GetPaymentTerm();
        public Task<string?> GetQueueOrders();

        public Task<string?> MakeOrderTransition();
        public Task<string?> RunOrderWorkflow();

        public Task<string?> SaveOrderItemSerialKey();
        public Task<string?> SaveOrderPackage();

        public Task<string?> SearchOrderPackage();
        public Task<bool?> SearchOrdersByDateInterval(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SearchOrdersByQueue(LinxCommerceJobParameter jobParameter);
        public Task<string?> SearchOrdersBySeller();
        public Task<string?> SearchOrdersCandidates();
        public Task<bool?> SearchOrderStatus(LinxCommerceJobParameter jobParameter);
        public Task<string?> SearchFinancialOrderInfo();

        public Task<string?> UpdateOrderInvoice();
        public Task<bool?> UpdateOrder();
        public Task<bool?> UpdateTrackingNumberOrder(LinxCommerceJobParameter jobParameter);
        public Task<string?> UpdateOrderPackageTrackingNumber();

        public Task<bool?> IntegrityLockOrdersRegisters(LinxCommerceJobParameter jobParameter);
    }
}

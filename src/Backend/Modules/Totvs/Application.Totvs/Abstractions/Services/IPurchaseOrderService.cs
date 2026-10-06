namespace Application.Totvs.Abstractions.Services
{
    public interface IPurchaseOrderService
    {
        public Task<bool?> PostSearchPurchaseOrders();
    }
}

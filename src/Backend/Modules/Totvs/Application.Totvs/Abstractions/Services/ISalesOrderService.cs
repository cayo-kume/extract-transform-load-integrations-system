namespace Application.Totvs.Abstractions.Services
{
    public interface ISalesOrderService
    {
        public Task<bool?> PostSearchSalesOrders();
    }
}

namespace Application.TotalExpress.Abstractions.Services
{
    public interface ITotalExpressService
    {
        public Task<string> GetSendOrder(string orderNumber);
        public Task<bool> SendOrders();
        public Task<bool> SendOrder(string? orderNumber);
        public Task<bool> SendOrderAsEtur(string? orderNumber);
        public Task<bool> InsertLogOrdersByAWBs();
        public Task<bool> InsertLogOrdersByDateInterval();
    }
}

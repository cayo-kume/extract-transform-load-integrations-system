namespace Application.Stone.Abstractions.Services
{
    public interface IStoneService
    {
        public Task<bool> GetZplLabels();
        public Task<string> GetAccessToken();
        public Task<string> GetDeliveryOptions();
        public Task<bool> CreateDeliveryOrder();
        public Task<bool> CheckDeliveryOrder(string orderNumber);
        public Task<bool> CheckDeliveryOrders();
        public Task<bool> CancelOrder(string orderNumber);
    }
}

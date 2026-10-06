namespace Application.Oto.Interfaces.Services
{
    public interface IOtoService
    {
        public Task<bool> SendCustomers();
        public Task<bool> SendCustomersCashBack();
        public Task<bool> SendProducts();
        public Task<bool> SendOrders();
        public Task<bool> SendStores();
        public Task<bool> SendSellers();
    }
}

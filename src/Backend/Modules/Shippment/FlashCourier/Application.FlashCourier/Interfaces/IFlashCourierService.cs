namespace Application.FlashCourier.Interfaces
{
    public interface IFlashCourierService
    {
        public Task<bool> SendOrders();
        public Task<bool> SendOrder(string? orderNumber);
        public Task<bool> UpdateLogOrders();
    }
}

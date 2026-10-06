namespace Application.LinxCommerce.Abstractions.Services
{
    public interface IShipmentService
    {
        public Task<string?> GetShipment();
        public Task<string?> SearchShipments();
        public Task<string?> UpdateShipment();
    }
}

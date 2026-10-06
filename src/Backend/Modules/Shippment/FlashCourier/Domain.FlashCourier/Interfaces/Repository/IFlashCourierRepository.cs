using Domain.FlashCourier.Entities;

namespace Domain.FlashCourier.Interfaces.Repository
{
    public interface IFlashCourierRepository
    {
        public Task<Order> GetInvoicedOrder(string? orderNumber);
        public Task<IEnumerable<Order>> GetInvoicedOrders();
        public Task<IEnumerable<Order>> GetShippedOrders(string? doc_company);
        public Task<Parameters> GetParameters(string? docCompany);
        public Task<IEnumerable<Company>> GetCompanys();

        public Task<bool> GenerateRequestLog(string? orderNumber, string? request);
        public Task<bool> GenerateResponseLog(string? orderNumber, string? senderID, string? _return, string? statusFlash, string? keyNFe);

        public Task<bool> UpdateCollectionDate(string? dtSla, string? cardCode);
        public Task<bool> UpdateRealDeliveryForecastDate(string? dtSla, string? cardCode);
        public Task<bool> UpdateDeliveryMadeDate(string? occurrence, string? cardCode);
        public Task<bool> UpdateLastStatusDate(string? occurrence, string? eventId, string? _event, string? cardCode);
    }
}

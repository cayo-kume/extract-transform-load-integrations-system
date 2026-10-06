namespace Application.Jadlog.Abstractions.Services
{
    public interface IJadlogService
    {
        public Task<bool?> InsertOrder();
        public Task<bool?> CancelOrder();
        public Task<bool?> SearchTrackingHistory();
        public Task<bool?> SearchShippingValue();
        public Task<bool?> SearchDACTEXml();
        public Task<bool?> SearchPickupPoints();
        public Task<bool?> SearchQRCodePickupDropoff();
        public Task<bool?> InsertTreatment();
    }
}

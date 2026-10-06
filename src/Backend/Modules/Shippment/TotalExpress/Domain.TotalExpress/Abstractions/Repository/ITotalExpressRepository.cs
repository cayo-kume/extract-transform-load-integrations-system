using Domain.TotalExpress.Entities;

namespace Domain.TotalExpress.Abstractions.Repository
{
    public interface ITotalExpressRepository
    {
        public Task<bool> GenerateResponseLog(List<Domain.TotalExpress.Models.Encomenda> encomendas, Guid? parentExecutionGuid);
        public Task<bool> GenerateRequestLog(string? orderNumber, string? request);

        public Task<Order> GetInvoicedOrder(string? orderNumber);
        public Task<List<Order>> GetInvoicedOrders();
        public Task<Order> GetSendOrder(string orderNumber);
        public Task<Parameters> GetParameters(string? docCompany, string? method);
        public Task<IEnumerable<Parameters>> GetSenderIds();
        public Task<IEnumerable<Awb>> GetSenderAwbs();

        public Task<bool> UpdateDeliveryDates(string? deliveryMadeDate, string? collectionDate, string? deliveryForecastDate, string? lastStatusDate, string? lastStatusDescription, string? orderNumber);

        public Task<bool> InsertStatus(List<Status> listStatus, List<Error> listErrors, Guid? parentExecutionGuid);
    }
}

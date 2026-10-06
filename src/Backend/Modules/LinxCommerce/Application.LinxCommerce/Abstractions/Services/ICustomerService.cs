using Domain.LinxCommerce.Entities.Parameters;

namespace Application.LinxCommerce.Abstractions.Services
{
    public interface ICustomerService
    {
        public Task<bool?> AddCustomerRelation(LinxCommerceJobParameter jobParameter);
        public Task<bool?> ApproveCustomerFile(LinxCommerceJobParameter jobParameter);

        public Task<bool?> DeleteCustomer(LinxCommerceJobParameter jobParameter);
        public Task<bool?> DeleteCustomerFile(LinxCommerceJobParameter jobParameter);
        public Task<bool?> DeleteCustomerGroup(LinxCommerceJobParameter jobParameter);
        public Task<bool?> DeleteCustomerRelation(LinxCommerceJobParameter jobParameter);

        public Task<bool?> GetCompany(LinxCommerceJobParameter jobParameter);
        public Task<bool?> GetCustomer(LinxCommerceJobParameter jobParameter, string Identifier);
        public Task<bool?> GetCustomerFile(LinxCommerceJobParameter jobParameter);
        public Task<bool?> GetCustomerGroup(LinxCommerceJobParameter jobParameter);
        public Task<bool?> GetCustomerStatus(LinxCommerceJobParameter jobParameter);
        public Task<bool?> GetCustomerStatusByIntegrationID(LinxCommerceJobParameter jobParameter);
        public Task<bool?> GetPerson(LinxCommerceJobParameter jobParameter);
        public Task<bool?> GetQueueCustomerFile(LinxCommerceJobParameter jobParameter);
        public Task<bool?> GetQueueCustomers(LinxCommerceJobParameter jobParameter);
        public Task<bool?> GetStatusModerationCustomerMarketplace(LinxCommerceJobParameter jobParameter);

        public Task<bool?> RejectCustomerFile(LinxCommerceJobParameter jobParameter);
        public Task<bool?> RunCustomerTransition(LinxCommerceJobParameter jobParameter);

        public Task<bool?> SaveCompany(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SaveCustomer(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SaveCustomerAddress(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SaveCustomerFile(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SaveCustomerGroup(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SaveCustomerStatus(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SavePerson(LinxCommerceJobParameter jobParameter);

        public Task<bool?> SearchCustomerByDateInterval(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SearchCustomerByQueue(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SearchCustomerGroup(LinxCommerceJobParameter jobParameter);

        public Task<bool?> UpdateModerationCustomerToSellerMarketplace(LinxCommerceJobParameter jobParameter);
        public Task<bool?> UpdateModerationSellerToCustomerMarketplace(LinxCommerceJobParameter jobParameter);
    }
}

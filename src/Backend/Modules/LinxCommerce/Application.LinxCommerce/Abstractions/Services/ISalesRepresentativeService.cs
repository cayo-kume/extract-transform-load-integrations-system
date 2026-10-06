using Domain.LinxCommerce.Entities.Parameters;

namespace Application.LinxCommerce.Abstractions.Services
{
    public interface ISalesRepresentativeService
    {
        public Task<bool?> DeleteSalesRepresentative(int salesRepresentativeId);
        public Task<bool?> GetSalesRepresentative(LinxCommerceJobParameter jobParameter, int? salesRepresentativeId);
        public Task<bool?> SaveSalesRepresentative(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SearchSalesRepresentative(LinxCommerceJobParameter jobParameter);
    }
}

using Domain.LinxCommerce.Entities.Parameters;

namespace Application.LinxCommerce.Abstractions.Services
{
    public interface ISKUService
    {
        public Task<bool?> GetSKU(LinxCommerceJobParameter jobParameter, int productID);
        public Task<bool?> SearchProductByDateInterval(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SearchProductByQueue(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SearchSKUByDateInterval(LinxCommerceJobParameter jobParameter);
        public Task<bool?> SearchSKUByQueue(LinxCommerceJobParameter jobParameter);
    }
}

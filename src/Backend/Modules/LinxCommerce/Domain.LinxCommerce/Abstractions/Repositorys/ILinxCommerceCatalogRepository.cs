using Domain.LinxCommerce.Entities.Reads.Catalog;

namespace Domain.LinxCommerce.Abstractions.Repositorys
{
    public interface ILinxCommerceCatalogRepository
    {
        public Task<DateTimeOffset?> GetProductsLastModifiedDate(string doc_company);
        public Task<IEnumerable<Product?>> GetProductsByIds(string doc_company, List<int> ids);
        public Task<bool> BulkInsertIntoLinxCommerceProducts(List<Product> data, Guid? parentExecutionGUID);

        public Task<DateTimeOffset?> GetSkusLastModifiedDate(string doc_company);
        public Task<IEnumerable<Sku?>> GetSkusByIds(string doc_company, List<int> ids);
        public Task<bool> BulkInsertIntoLinxCommerceSkus(List<Sku> data, Guid? parentExecutionGUID);
    }
}

using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Product;
using Domain.LinxCommerce.Entities.Sku;

namespace Domain.LinxCommerce.Interfaces.Repositorys
{
    public interface ISKURepository
    {
        public Task<bool> InsertRecord(LinxCommerceJobParameter jobParameter, Sku? record);
        public bool BulkInsertIntoTableRaw(LinxCommerceJobParameter jobParameter, List<Sku> records, Guid? parentExecutionGUID);
        public bool BulkInsertIntoTableRaw(LinxCommerceJobParameter jobParameter, List<Product> records, Guid? parentExecutionGUID);
    }
}

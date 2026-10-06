using Domain.Totvs.Entities.Product;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface IProductRepository
    {
        public Task<bool> BulkInsertIntoTableSkusRaw(List<Sku> registros, Guid? parentExecutionGUID);

        public Task<List<Domain.Totvs.Dtos.Product.Sku?>> GetExistingSkus(List<string?> registros);
    }
}

using Domain.Totvs.Entities.Seller;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ISellerRepository
    {
        public Task<List<Domain.Totvs.Dtos.Seller.Item?>> GetExistingSellers(List<string?> registros);

        public Task<bool> BulkInsertIntoTableSellersRaw(List<Seller> registros, Guid? parentExecutionGUID);
    }
}

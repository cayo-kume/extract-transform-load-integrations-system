using Domain.Oto.Entities;
using Domain.Oto.Models;

namespace Domain.Oto.Interfaces.Repositorys
{
    public interface IOtoRepository
    {
        public Task<IEnumerable<Parameters?>> GetParameters();

        public Task<List<Order?>> GetOrders();

        public Task<IEnumerable<Client?>> GetClients();
        
        public Task<IEnumerable<Client?>> GetClientsCashBack();

        public Task<IEnumerable<Company?>> GetCompanys();

        public Task<IEnumerable<Product?>> GetProducts();

        public Task<IEnumerable<SaleRepresentative?>> GetSaleRepresentatives();

        public bool BulkInsertIntoTableRaw(List<SentRecordLog> records, Guid? parentExecutionGUID);
    }
}

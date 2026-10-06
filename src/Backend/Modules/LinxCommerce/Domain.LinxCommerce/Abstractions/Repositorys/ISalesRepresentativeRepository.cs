using Domain.LinxCommerce.Entities.Parameters;

namespace Domain.LinxCommerce.Interfaces.Repositorys
{
    public interface ISalesRepresentativeRepository
    {
        public bool BulkInsertIntoTableRaw(LinxCommerceJobParameter jobParameter, List<Entities.SalesRepresentative.SalesRepresentative> registros, Guid? parentExecutionGUID);
        public Task<List<Entities.SalesRepresentative.SalesRepresentative>> GetRegistersExists(IEnumerable<int> ordersIds);
    }
}

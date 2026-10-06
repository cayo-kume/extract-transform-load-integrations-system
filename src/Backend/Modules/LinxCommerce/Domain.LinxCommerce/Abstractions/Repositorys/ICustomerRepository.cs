using Domain.LinxCommerce.Entities.Parameters;

namespace Domain.LinxCommerce.Interfaces.Repositorys
{
    public interface ICustomerRepository
    {
        public Task<bool> BulkInsertIntoTableRaw(LinxCommerceJobParameter jobParameter, List<Domain.LinxCommerce.Entities.Customer.Person> registros, Guid? parentExecutionGUID);
        public Task<bool> InsertRecord(LinxCommerceJobParameter jobParameter, Domain.LinxCommerce.Entities.Customer.Person registro, Guid? parentExecutionGUID);
    }
}

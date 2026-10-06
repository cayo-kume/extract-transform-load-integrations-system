using Domain.Totvs.Entities.Person.Branch;
using Domain.Totvs.Entities.Person.Customer;
using Domain.Totvs.Entities.Person.LegalEntity;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface IPersonRepository
    {
        public Task<bool> BulkInsertIntoTableCustomersRaw(List<Customer> registros, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoTableLegalEntitiesRaw(List<LegalEntity> registros, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoTableBranchesRaw(List<Branch> registros, Guid? parentExecutionGUID);

        public Task<List<Domain.Totvs.Dtos.Person.Customer.Item?>> GetExistingCustomers(List<string?> registros);
        public Task<List<Domain.Totvs.Dtos.Person.LegalEntity.Item?>> GetExistingLegalEntities(List<string?> registros);
        public Task<List<Domain.Totvs.Dtos.Person.Branch.Branch?>> GetExistingBranches(List<string?> registros);
    }
}

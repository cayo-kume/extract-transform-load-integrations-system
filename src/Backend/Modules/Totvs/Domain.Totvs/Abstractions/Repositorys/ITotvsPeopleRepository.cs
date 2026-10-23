using Domain.Totvs.Entities.Reads.People.Branches;
using Domain.Totvs.Entities.Reads.People.Customers;
using Domain.Totvs.Entities.Reads.People.LegalEntities;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsPeopleRepository
    {
        public Task<DateTime?> GetBranchesLastChange(string doc_company);
        public Task<IEnumerable<Branch?>> GetBranchesByCodes(string doc_company, List<int> codes);
        public Task<bool> BulkInsertIntoTotvsBranches(List<Branch> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetCustomersLastChange(string doc_company);
        public Task<IEnumerable<Customer?>> GetCustomersByCodes(string doc_company, List<int> codes);
        public Task<bool> BulkInsertIntoTotvsPeople(List<Customer> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetLegalEntitiesLastChange(string doc_company);
        public Task<IEnumerable<LegalEntity?>> GetLegalEntitiesByCodes(string doc_company, List<int> codes);
        public Task<bool> BulkInsertIntoTotvsLegalEntities(List<LegalEntity> data, Guid? parentExecutionGUID);
    }
}

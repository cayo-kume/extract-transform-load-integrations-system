using Domain.LinxCommerce.Entities.Reads.Customers;

namespace Domain.LinxCommerce.Abstractions.Repositorys
{
    public interface ILinxCommerceCustomersRepository
    {
        public Task<DateTime?> GetCustomersLastBatchTime(string doc_company);
        public Task<bool> BulkInsertIntoLinxCommerceCustomers(List<Customer> data, Guid? parentExecutionGUID);
    }
}

using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Requests;

namespace Domain.LinxCommerce.Abstractions.Repositorys
{
    public interface ILinxCommerceRepository
    {
        public Task<IEnumerable<Company?>> GetCompanys();
        public bool InsertIntoLinxCommerceRequests(Request request);
    }
}

using Domain.LinxCommerce.Entities.Parameters;

namespace Domain.LinxCommerce.Abstractions.Repositorys
{
    public interface ILinxCommerceRepository
    {
        public Task<IEnumerable<Company?>> GetCompanys();
    }
}

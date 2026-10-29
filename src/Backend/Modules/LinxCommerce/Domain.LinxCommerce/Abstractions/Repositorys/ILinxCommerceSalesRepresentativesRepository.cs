using Domain.LinxCommerce.Entities.Reads.SalesRepresentatives;

namespace Domain.LinxCommerce.Abstractions.Repositorys
{
    public interface ILinxCommerceSalesRepresentativesRepository
    {
        public Task<bool> BulkInsertIntoLinxCommerceSalesRepresentatives(List<SalesRepresentative> data, Guid? parentExecutionGUID);
    }
}

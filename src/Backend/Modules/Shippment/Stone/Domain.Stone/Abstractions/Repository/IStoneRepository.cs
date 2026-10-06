using Domain.Stone.Entities;

namespace Domain.Stone.Abstractions.Repository
{
    public interface IStoneRepository
    {
        public Task<bool> InsertZpls(List<Zpl> records);
        public Task<bool> InsertOrder(Order? record);
        public Task<bool> BulkInsertIntoTableRaw(IList<Order> records, Guid? parentExecutionGUID);
        public Task<IEnumerable<Zpl?>> GetExistingReferenceKeys();
        public Task<IEnumerable<OrdersToBeSent?>> GetRegistersExists();
        public Task<IEnumerable<Domain.Stone.Entities.Order?>> GetRegistersExists(List<Guid> registros);
        public Task<IEnumerable<Parameters?>> GetParameters();
    }
}

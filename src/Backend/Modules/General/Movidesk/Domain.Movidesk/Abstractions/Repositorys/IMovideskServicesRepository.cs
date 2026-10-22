namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskServicesRepository
    {
        public Task<bool> BulkInsertIntoMovideskServices(List<Domain.Movidesk.Entities.Reads.Services.Service> data, Guid? parentExecutionGUID);
    }
}

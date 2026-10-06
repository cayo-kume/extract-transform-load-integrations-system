using Domain.Shared.Entities.Auditing;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface ILoggerRepository
    {
        public Task<bool> LogInsert(Log log);
    }
}

using Domain.Shared.Entities.Auditing;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface ILoggerRepository
    {
        public Task<bool> LogInsert(Log log);

        /// <summary>
        /// grava em auditing.Messages e auditing.Records as linhas de uma execução que já está em auditing.Logs (ex.: o erro de um
        /// service que caiu depois de gravar o log)
        /// </summary>
        public Task<bool> LogAppend(IEnumerable<Message> messages, IEnumerable<Record> records);
    }
}

using Application.Shared.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Shared.Entities.Auditing;

namespace Infrastructure.Shared.Repositorys
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly ICommandHandler _commandHandler;
        private readonly ISharedRepository _coreRepository;

        public LoggerRepository(ICommandHandler commandHandler, ISharedRepository coreRepository) =>
            (_commandHandler, _coreRepository) = (commandHandler, coreRepository);

        public async Task<bool> LogInsert(Log log)
        {
            string sql = _commandHandler.CreateInsertRecordQuery();
            var recordsTable = _coreRepository.CreateSystemDataTable("Records", new Record());;
            var messagesTable = _coreRepository.CreateSystemDataTable("Messages", new Message());

            var parameter = new
            {
                IdJob = (Int32)(object)log.Job,
                IdType = (Int32)(object)log.IdType,
                StartDate = log.StartDate.ToString("s"),
                EndDate = log.EndDate.ToString("s"),
                Execution = log.Execution
            };

            _coreRepository.FillSystemDataTable(recordsTable, log.Records.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(messagesTable, log.Messages.Where(y => y.Msg != null).Select(x => x).ToList());

            //Adicionar aqui um rollback se qualquer uma das 3 transações falhar

            await _coreRepository.ExecuteAuditingCommand(sql, parameter);
            _coreRepository.BulkInsertIntoAuditingTableRaw(recordsTable, "auditing");
            _coreRepository.BulkInsertIntoAuditingTableRaw(messagesTable, "auditing");

            return true;
        }
    }
}

using Application.Shared.Abstractions.Services;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Shared.Entities.Auditing;
using Domain.Shared.Enums;
using System.Security.Cryptography;
using System.Text;

namespace Application.Shared.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILoggerRepository _logRepository;
        public Log? Log { get; private set; }
        private bool _committed;

        public LoggerService(ILoggerRepository logRepository) =>
            _logRepository = logRepository;

        public ILoggerService AddLog(Enum job)
        {
            Log ??= new Log(job: job);

            return this;
        }

        // mesma ordem da interface (message, sqlCommand, exceptionMessage, stackTraceMessage): as chamadas passam pela interface,
        // e com a ordem diferente os três textos chegavam trocados em auditing.Messages
        public ILoggerService AddMessage(
            string message,
            string commandSQL,
            string exceptionMessage,
            string stackTraceMessage
        )
        {
            if (Log is null) return this;

            Log.Messages.Add(
                new Message(
                    job: Log.Job,
                    jobType: Log.IdType,
                    message: message,
                    exceptionMessage: exceptionMessage,
                    stackTraceMessage: stackTraceMessage,
                    sqlCommand: commandSQL,
                    execution: Log.Execution
                )
            );

            return this;
        }

        public ILoggerService AddMessage(string message)
        {
            if (Log is null) return this;

            Log.Messages.Add(
                new Message(
                    job: Log.Job,
                    jobType: Log.IdType,
                    message,
                    Log.Execution
                )
            );

            return this;
        }

        public ILoggerService AddRecord(string key, string xml)
        {
            if (Log is null) return this;

            Log.Records.Add(
                new Record(job: Log.Job, jobType: Log.IdType, key: key, regXML: xml, execution: Log.Execution)
            );

            return this;
        }

        public ILoggerService SetLogEndDate()
        {
            if (Log != null)
                Log.SetEndDate();

            return this;
        }

        public ILoggerService Clear()
        {
            Log?.Records.Clear();

            return this;
        }

        public async Task CommitAllChanges()
        {
            if (Log is null) return;

            await _logRepository.LogInsert(Log);
            _committed = true;
        }

        public bool HasUncommittedLog => Log is not null && !_committed;

        public Guid? GetExecutionGuid()
        {
            return Log?.Execution;
        }

        public string ComputeSha256Hash(string json)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(bytes);
        }
    }
}



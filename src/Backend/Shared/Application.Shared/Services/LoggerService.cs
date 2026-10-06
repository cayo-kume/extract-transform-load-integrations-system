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

        public LoggerService(ILoggerRepository logRepository) =>
            _logRepository = logRepository;

        public ILoggerService AddLog(Enum job)
        {
            Log ??= new Log(job: job);

            return this;
        }

        public ILoggerService AddMessage(
            string message,
            string exceptionMessage,
            string stackTraceMessage,
            string commandSQL
        )
        {
            if (Log is null) return this;

            Log.Messages.Add(
                new Message(
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
                new Record(key: key, regXML: xml, execution: Log.Execution)
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
        }

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



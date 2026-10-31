using Domain.Shared.Enums;

namespace Application.Shared.Abstractions.Services
{
    public interface ILoggerService
    {
        /// <summary>
        /// Create a new log
        /// </summary>
        /// <param name="job"></param>
        /// <returns>ILoggerService</returns>
        public ILoggerService AddLog(Enum job);

        /// <summary>
        /// Build an ERROR exception message with exception details occurred
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sqlCommand"></param>
        /// <param name="exceptionMessage"></param>
        /// <param name="stackTraceMessage"></param>
        /// <returns>ILoggerService</returns>
        public ILoggerService AddMessage(
            string message,
            string sqlCommand,
            string exceptionMessage,
            string stackTraceMessage
        );

        /// <summary>
        /// Add an information message
        /// </summary>
        /// <param name="message"></param>
        /// <returns>ILoggerService</returns>
        public ILoggerService AddMessage(string message);

        /// <summary>
        /// Add to list a new record obtained from API
        /// </summary>
        /// <param name="key"></param>
        /// <param name="xml"></param>
        /// <returns>ILoggerService</returns>
        public ILoggerService AddRecord(string key, string xml);

        /// <summary>
        /// Set the end execution time
        /// </summary>
        /// <returns>ILoggerService</returns>
        public ILoggerService SetLogEndDate();

        /// <summary>
        /// Persists generated logs in the database
        /// </summary>
        /// <returns></returns>
        public Task CommitAllChanges();

        /// <summary>
        /// há um log aberto (AddLog) que ainda não foi gravado (CommitAllChanges). O executor de jobs dos hosts de agenda usa para
        /// gravar o erro de uma execução que caiu no meio sem gravar o log duas vezes (a chave de auditing.Logs não aceita)
        /// </summary>
        public bool HasUncommittedLog { get; }

        /// <summary>
        /// Clear records list and messages list
        /// </summary>
        /// <returns>ILoggerService</returns>
        public ILoggerService Clear();

        /// <summary>
        /// Return the Parent Execution GUID
        /// </summary>
        /// <returns></returns>
        public Guid? GetExecutionGuid();

        /// <summary>
        /// Returns the SHA256 hash of a JSON string for reponses that not contains any comparable field, like a timestamp or a unique identifier.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string ComputeSha256Hash(string json);
    }
}


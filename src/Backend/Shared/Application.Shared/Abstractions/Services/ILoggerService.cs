using Domain.Shared.Enums;

namespace Application.Shared.Abstractions.Services
{
    public interface ILoggerService
    {
        /// <summary>
        /// Create a new log, for the company of the execution (ICompanyContext). Without a company, throws
        /// </summary>
        /// <param name="job"></param>
        /// <returns>ILoggerService</returns>
        public ILoggerService AddLog(Enum job);

        /// <summary>
        /// Create a new log for the company (auditing.Companies) of the holder, and set both as the company of the execution
        /// (ICompanyContext)
        /// </summary>
        /// <param name="job"></param>
        /// <param name="holder"></param>
        /// <param name="company"></param>
        /// <returns>ILoggerService</returns>
        public ILoggerService AddLog(Enum job, string holder, string company);

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
        /// Add an information message (nível 1): progresso, totais e o resumo final
        /// </summary>
        /// <param name="message"></param>
        /// <returns>ILoggerService</returns>
        public ILoggerService AddMessage(string message);

        /// <summary>
        /// Add a warning message (nível 2): parte do que devia ser processado não foi, por cadastro, ordem dos jobs ou descarte
        /// de propósito (src/Backend/Shared/.docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md)
        /// </summary>
        /// <param name="message"></param>
        /// <returns>ILoggerService</returns>
        public ILoggerService AddWarning(string message);

        /// <summary>
        /// Add an error message without exception (nível 3): a fonte externa respondeu com erro ou com algo que não dá para ler,
        /// e o dado não foi carregado
        /// </summary>
        /// <param name="message"></param>
        /// <returns>ILoggerService</returns>
        public ILoggerService AddError(string message);

        /// <summary>
        /// a execução tem alguma mensagem de erro (nível 3)
        /// </summary>
        public bool HasErrors { get; }

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
        /// Persists generated logs in the database. Called again after that, persists only the messages and records added since,
        /// in the same execution (auditing.Logs is not inserted twice)
        /// </summary>
        /// <returns></returns>
        public Task CommitAllChanges();

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


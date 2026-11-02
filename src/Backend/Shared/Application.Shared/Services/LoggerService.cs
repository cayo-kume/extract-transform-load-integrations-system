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
        private readonly ICompanyContext _companyContext;
        public Log? Log { get; private set; }
        private bool _committed;

        // quantas mensagens e registros do Log já estão no banco: uma segunda gravação (ex.: o erro de um service que caiu depois
        // de gravar o log) grava só o que veio depois, na mesma execução, sem inserir auditing.Logs de novo
        private int _committedMessages;
        private int _committedRecords;

        public LoggerService(ILoggerRepository logRepository, ICompanyContext companyContext) =>
            (_logRepository, _companyContext) = (logRepository, companyContext);

        // a empresa vem do contexto da execução, que o agendador ou a rota informam (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.3)
        public ILoggerService AddLog(Enum job) =>
            AddLog(job, _companyContext.Company ?? throw new InvalidOperationException(
                $"Job {job}: execução sem empresa. Quem inicia a execução precisa informar ICompanyContext.SetCompany."));

        public ILoggerService AddLog(Enum job, string company)
        {
            _companyContext.SetCompany(company);
            Log ??= new Log(job: job, company: company);

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
                    company: Log.Company,
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
                    company: Log.Company,
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
                new Record(job: Log.Job, jobType: Log.IdType, company: Log.Company, key: key, regXML: xml, execution: Log.Execution)
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
            _committedRecords = 0;

            return this;
        }

        public async Task CommitAllChanges()
        {
            if (Log is null) return;

            if (_committed)
                await _logRepository.LogAppend(Log.Messages.Skip(_committedMessages), Log.Records.Skip(_committedRecords));
            else
                await _logRepository.LogInsert(Log);

            _committed = true;
            _committedMessages = Log.Messages.Count;
            _committedRecords = Log.Records.Count;
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



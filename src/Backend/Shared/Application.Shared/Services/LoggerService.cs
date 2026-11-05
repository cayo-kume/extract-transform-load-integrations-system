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
        private readonly IJobExecutionGuard _guard;
        public Log? Log { get; private set; }

        // a linha de auditing.Logs já está no banco (gravada no AddLog, ou no primeiro commit se o AddLog não conseguiu): os
        // commits seguintes só atualizam o fim, sem inserir de novo (o CommitError do ScheduledJobRunner batia na PK)
        private bool _started;

        // quantas mensagens e registros do Log já estão no banco: uma segunda gravação (ex.: o erro de um service que caiu depois
        // de gravar o log) grava só o que veio depois, na mesma execução
        private int _committedMessages;
        private int _committedRecords;

        public LoggerService(ILoggerRepository logRepository, ICompanyContext companyContext, IJobExecutionGuard guard) =>
            (_logRepository, _companyContext, _guard) = (logRepository, companyContext, guard);

        // a holding e a empresa vêm do contexto da execução, que o agendador ou a rota informam
        // (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md)
        public ILoggerService AddLog(Enum job) =>
            _companyContext.Company is null || _companyContext.Holder is null
                ? throw new InvalidOperationException($"Job {job}: execução sem empresa. Quem inicia a execução precisa informar ICompanyContext.SetCompany.")
                : AddLog(job, _companyContext.Holder, _companyContext.Company);

        public ILoggerService AddLog(Enum job, string holder, string company)
        {
            _companyContext.SetCompany(holder, company);

            if (Log is null)
            {
                // numa rota do WebApi, a trava do job (JobAlreadyRunningException com ela ocupada), antes de qualquer gravação
                _guard.BeforeLogStart(job, company);

                Log = new Log(job: job, holder: holder, company: company);

                // a execução aparece aberta desde o início (docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 3.1). Sem banco, a
                // exceção sobe: o job não teria onde gravar o que lê
                _logRepository.LogStart(Log);
                _started = true;
            }

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
                    holder: Log.Holder,
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

        public ILoggerService AddMessage(string message) =>
            AddMessage(message, MessageLevel.Information);

        public ILoggerService AddWarning(string message) =>
            AddMessage(message, MessageLevel.Warning);

        public ILoggerService AddError(string message) =>
            AddMessage(message, MessageLevel.Error);

        private ILoggerService AddMessage(string message, MessageLevel level)
        {
            if (Log is null) return this;

            Log.Messages.Add(
                new Message(
                    job: Log.Job,
                    jobType: Log.IdType,
                    holder: Log.Holder,
                    company: Log.Company,
                    message,
                    Log.Execution,
                    level
                )
            );

            return this;
        }

        public bool HasErrors =>
            Log?.Messages.Any(x => x.Level == MessageLevel.Error) == true;

        public ILoggerService AddRecord(string key, string xml)
        {
            if (Log is null) return this;

            Log.Records.Add(
                new Record(job: Log.Job, jobType: Log.IdType, holder: Log.Holder, company: Log.Company, key: key, regXML: xml, execution: Log.Execution)
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

            await _logRepository.LogCommit(Log, Log.Messages.Skip(_committedMessages).ToList(), Log.Records.Skip(_committedRecords).ToList(), _started);

            _started = true;
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



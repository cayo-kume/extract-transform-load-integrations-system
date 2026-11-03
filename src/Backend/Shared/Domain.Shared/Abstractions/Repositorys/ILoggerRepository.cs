using Domain.Shared.Entities.Auditing;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface ILoggerRepository
    {
        /// <summary>
        /// grava a linha da execução em auditing.Logs no início, com EndDate nulo: uma queda no meio deixa a execução aberta
        /// (auditing.vw_JobsHeartbeat, OpenSince), e a próxima execução do job a fecha como interrompida
        /// </summary>
        public void LogStart(Log log);

        /// <summary>
        /// grava, numa transação só, o fim da execução (EndDate, quando já foi informado) e as mensagens e registros novos.
        /// Com started = false (o LogStart não chegou a gravar), insere a linha de auditing.Logs na mesma transação. Deadlock e
        /// erros transitórios do SQL Server e do Azure SQL repetem a transação inteira
        /// </summary>
        public Task LogCommit(Log log, IReadOnlyList<Message> messages, IReadOnlyList<Record> records, bool started);
    }
}

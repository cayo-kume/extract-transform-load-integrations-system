using Application.Shared.Abstractions.Services;
using Hosts.Scheduling.RecurringJobs;
using Microsoft.Extensions.Logging;

namespace Hosts.Scheduling
{
    /// <summary>
    /// executor dos jobs da agenda nos dois hosts (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md §2.2), no lugar do
    /// WebJobExceptionHandlingMiddleware: chama o método do mapa e, se uma exceção escapa do service, grava a mensagem de erro no
    /// log da execução (com o comando SQL e a pilha), fecha e grava o log em auditing.Logs e auditing.Messages, sem HttpContext.
    /// É scoped: o host cria um escopo por execução (o Hangfire faz isso sozinho), e o service e o ILoggerService vêm do mesmo
    /// escopo, então o erro vai para o log que o service abriu. A exceção segue adiante, para o host marcar a falha
    /// </summary>
    public class ScheduledJobRunner
    {
        private readonly IServiceProvider _services;
        private readonly ILoggerService _jobLog;
        private readonly ILogger<ScheduledJobRunner> _logger;

        public ScheduledJobRunner(IServiceProvider services, ILoggerService jobLog, ILogger<ScheduledJobRunner> logger) =>
            (_services, _jobLog, _logger) = (services, jobLog, logger);

        public async Task Run(int idJob)
        {
            if (!RecurringJobMap.Definitions.TryGetValue(idJob, out var definition))
                throw new InvalidOperationException($"IdJob {idJob} sem método nos mapas de jobs recorrentes.");

            try
            {
                await definition.Invoke(_services);
            }
            catch (Exception ex)
            {
                await CommitError(definition, ex);
                throw;
            }
        }

        /// <summary>
        /// o mesmo que o ExceptionHandler do Shared faz nas rotas: mensagem (o trecho depois do "|" é o comando SQL, quando
        /// houver), fim do log e gravação. Só com o log aberto e ainda não gravado: um service que já gravou o log antes de
        /// relançar não ganha uma segunda linha (a chave de auditing.Logs não aceitaria), e um que caiu antes de abrir o log
        /// não tem onde gravar (fica no log do host)
        /// </summary>
        private async Task CommitError(RecurringJobDefinition definition, Exception ex)
        {
            if (!_jobLog.HasUncommittedLog)
            {
                _logger.LogError(ex, "Job {IdJob} ({Target}) falhou sem log aberto para gravar o erro em auditing.", definition.IdJob, definition.Target);
                return;
            }

            try
            {
                var messageParts = ex.Message.Split('|');

                _jobLog
                    .AddMessage(
                        message: ex.Message,
                        sqlCommand: messageParts.Length > 1 ? messageParts[1] : ex.Message,
                        exceptionMessage: ex.InnerException?.Message ?? ex.Message,
                        stackTraceMessage: ex.InnerException?.StackTrace ?? ex.StackTrace ?? string.Empty)
                    .SetLogEndDate();

                await _jobLog.CommitAllChanges();
            }
            catch (Exception logEx)
            {
                // a gravação do log também falhou (ex.: o banco caiu): fica no log do host, e a exceção original segue
                _logger.LogError(logEx, "Job {IdJob} ({Target}): não foi possível gravar o erro em auditing.", definition.IdJob, definition.Target);
            }
        }
    }
}

using Application.Shared.Abstractions.Services;
using Domain.Shared.Abstractions.Repositorys;
using Hosts.Scheduling.RecurringJobs;
using Microsoft.Extensions.Logging;

namespace Hosts.Scheduling
{
    /// <summary>
    /// executor dos jobs da agenda nos dois hosts (src/Backend/Hosts/WebJob/.docs/PLANO-HOST-WEBJOB.md §2.2), no lugar do
    /// WebJobExceptionHandlingMiddleware: chama o método do mapa e, se uma exceção escapa do service, grava a mensagem de erro no
    /// log da execução (com o comando SQL e a pilha), fecha e grava o log em auditing.Logs e auditing.Messages, sem HttpContext.
    /// É scoped: o host cria um escopo por execução (o Hangfire faz isso sozinho), e o service e o ILoggerService vêm do mesmo
    /// escopo, então o erro vai para o log que o service abriu. A exceção segue adiante, para o host marcar a falha. Cada execução
    /// é de uma empresa (.docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md): o executor informa a empresa e a holding dela ao ICompanyContext do escopo
    /// antes de chamar o service, e o log, os parâmetros e as procedures de sincronização usam essa empresa. Quem chama segura a
    /// trava da empresa; por isso, antes de executar, o executor fecha como interrompidas as execuções do job e da empresa que
    /// ficaram abertas em auditing.Logs (queda, reinício ou deploy no meio), .docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 3.1
    /// </summary>
    public class ScheduledJobRunner
    {
        private readonly IServiceProvider _services;
        private readonly ILoggerService _jobLog;
        private readonly ICompanyContext _companyContext;
        private readonly IJobScheduleRepository _jobScheduleRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<ScheduledJobRunner> _logger;

        public ScheduledJobRunner(IServiceProvider services, ILoggerService jobLog, ICompanyContext companyContext, IJobScheduleRepository jobScheduleRepository, ICompanyRepository companyRepository, ILogger<ScheduledJobRunner> logger) =>
            (_services, _jobLog, _companyContext, _jobScheduleRepository, _companyRepository, _logger) = (services, jobLog, companyContext, jobScheduleRepository, companyRepository, logger);

        public Task Run(int idJob, string company) =>
            RecurringJobMap.Definitions.TryGetValue(idJob, out var definition)
                ? Run(definition, company)
                : throw new InvalidOperationException($"IdJob {idJob} sem método nos mapas de jobs recorrentes.");

        /// <summary>
        /// a definição já resolvida, para o host que tem jobs só dele além dos mapas compartilhados (o WebJob, com a recuperação)
        /// </summary>
        public async Task Run(RecurringJobDefinition definition, string company)
        {
            // a holding sai da empresa (auditing.Companies): os agendadores guardam só a empresa no job

            var holder = await _companyRepository.GetHolder(company)

                ?? throw new InvalidOperationException($"Empresa {company} não cadastrada em auditing.Companies.");


            _companyContext.SetCompany(holder, company);

            await CloseInterruptedExecutions(definition, company);

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
        /// execuções anteriores do job e da empresa sem fim gravado: o processo parou no meio. Uma falha aqui só vai para o log do
        /// host; o job roda do mesmo jeito
        /// </summary>
        private async Task CloseInterruptedExecutions(RecurringJobDefinition definition, string company)
        {
            try
            {
                var closed = await _jobScheduleRepository.CloseInterruptedExecutions(definition.IdJob, company);

                if (closed > 0)
                    _logger.LogWarning("Job {IdJob} ({Target}), empresa {Company}: {Closed} execução(ões) anterior(es) sem fim gravado fechada(s) como interrompida(s).", definition.IdJob, definition.Target, company, closed);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Job {IdJob} ({Target}), empresa {Company}: não foi possível fechar as execuções interrompidas.", definition.IdJob, definition.Target, company);
            }
        }

        /// <summary>
        /// o mesmo que o ExceptionHandler do Shared faz nas rotas: mensagem (o trecho depois do "|" é o comando SQL, quando
        /// houver), fim do log e gravação. Um service que já gravou o log antes de relançar recebe o erro na mesma execução
        /// (o LoggerService grava só a mensagem nova); um que caiu antes de abrir o log não tem onde gravar (fica no log do host)
        /// </summary>
        private async Task CommitError(RecurringJobDefinition definition, Exception ex)
        {
            if (_jobLog.GetExecutionGuid() is null)
            {
                _logger.LogError(ex, "Job {IdJob} ({Target}), empresa {Company}: falhou sem log aberto para gravar o erro em auditing.", definition.IdJob, definition.Target, _companyContext.Company);
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
                _logger.LogError(logEx, "Job {IdJob} ({Target}), empresa {Company}: não foi possível gravar o erro em auditing.", definition.IdJob, definition.Target, _companyContext.Company);
            }
        }
    }
}

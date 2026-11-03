using Application.Shared.Abstractions.Services;
using Hosts.Scheduling;
using Hosts.Scheduling.RecurringJobs;
using Quartz;
using System.Globalization;

namespace Worker.Scheduling
{
    /// <summary>
    /// o que o Quartz executa em cada job da agenda (src/Backend/Hosts/Worker/docs/PLANO-HOST-WORKER.md §4.2 e §5.3): uma classe só para
    /// todos os jobs, com o IdJob no JobDataMap, que repassa ao ScheduledJobRunner (log do erro em auditing). O Quartz cria um
    /// escopo por execução, então o runner e os services são scoped como nos outros hosts. Nenhuma exceção sobe: sem
    /// retentativa, o trigger segue para o próximo horário (§3.4).
    ///
    /// Concorrência em duas camadas (§5.3): o [DisallowConcurrentExecution] não dispara o mesmo JobKey por cima da execução
    /// anterior (vale entre os Workers do cluster), e o IJobLock, com as chaves dos outros hosts, cobre o resto (o grupo do DW, a
    /// troca de versão, outro host agendando). Um disparo que aconteceu com a execução anterior rodando é pulado, como no
    /// Hangfire (§5.2), pelo início e o fim da última execução guardados no JobDataMap.
    /// Com um job do Quartz por (job, empresa) (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.4), a empresa também vai no JobDataMap e
    /// são duas travas, como no WebApi: a da empresa (a execução anterior da mesma empresa ainda roda, em outro host: pula) e a do
    /// job, que vale entre empresas porque todas escrevem nas mesmas tabelas (untreated e a sp_*_Sync). Com a do job ocupada por
    /// outra empresa, a execução não é perdida: um trigger avulso dispara o mesmo job daqui a RescheduleDelay, senão as empresas
    /// disparadas no mesmo minuto pulariam sempre umas às outras
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class QuartzJobRunner : IJob
    {
        public const string Group = "integrations";
        public const string IdJobKey = "IdJob";
        public const string CompanyKey = "Company";
        public const string LastStartUtcKey = "LastStartUtc";
        public const string LastEndUtcKey = "LastEndUtc";

        private static readonly TimeSpan RescheduleDelay = TimeSpan.FromMinutes(1);

        private readonly ScheduledJobRunner _runner;
        private readonly IJobLock _jobLock;
        private readonly JobSkipCounter _skips;
        private readonly SchedulingHostRegistry _hosts;
        private readonly ILogger<QuartzJobRunner> _logger;

        public QuartzJobRunner(ScheduledJobRunner runner, IJobLock jobLock, JobSkipCounter skips, SchedulingHostRegistry hosts, ILogger<QuartzJobRunner> logger) =>
            (_runner, _jobLock, _skips, _hosts, _logger) = (runner, jobLock, skips, hosts, logger);

        public async Task Execute(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var data = context.JobDetail.JobDataMap;

            // UseProperties: o JobDataMap só guarda texto (§3.3)
            if (!int.TryParse(data.GetString(IdJobKey), out var idJob)
                || !RecurringJobMap.Definitions.TryGetValue(idJob, out var definition)
                || data.GetString(CompanyKey) is not { Length: > 0 } company)
            {
                _logger.LogError("{Name}: sem IdJob ou empresa no JobDataMap, ou sem método nos mapas de jobs recorrentes; nada executado.", name);
                return;
            }

            // outro tipo de host agenda neste banco (SchedulingHostRegistry): este não executa; o heartbeat já avisou no log
            if (!_hosts.IsActive)
            {
                context.Result = _hosts.SkipReason;
                return;
            }

            // §5.2: o disparo bloqueado pelo [DisallowConcurrentExecution] é liberado quando a anterior termina. Se o horário
            // agendado dele cai entre o início e o fim da anterior, aconteceu com ela rodando: pula, sem pedir a trava. Fora disso
            // (inclusive antes do início da anterior, quando as duas só esperaram thread), roda, por mais atrasado que esteja
            if (Utc(data, LastStartUtcKey) is DateTimeOffset lastStart && Utc(data, LastEndUtcKey) is DateTimeOffset lastEnd
                && context.ScheduledFireTimeUtc is DateTimeOffset scheduled && scheduled >= lastStart && scheduled < lastEnd)
            {
                var reason = $"Skipped: previous execution of job:{idJob} still running at {scheduled.ToLocalTime():HH:mm:ss}";
                context.Result = reason;
                _skips.Skipped(name, reason);
                return;
            }

            // disparos atrasados acumulados (o Worker ficou fora, ou o job ficou bloqueado, por menos que o limiar de misfire) viram
            // uma execução só: se o horário seguinte também já passou, este é dispensado e o mais recente roda. Não conta como
            // pulo: não houve execução por cima de outra
            if (context.NextFireTimeUtc is DateTimeOffset next && next <= DateTimeOffset.UtcNow)
            {
                context.Result = $"Superseded: the fire at {next.ToLocalTime():HH:mm:ss} is also due";
                _logger.LogInformation("{Name}: disparo de {Scheduled:HH:mm:ss} dispensado; o de {Next:HH:mm:ss} também já passou e roda no lugar dele.", name, context.ScheduledFireTimeUtc?.ToLocalTime(), next.ToLocalTime());
                return;
            }

            // as mesmas travas do WebApi e do WebJob. A da empresa ocupada: a execução anterior desta empresa roda em outro host
            // ou com outro JobKey (a versão anterior do job): pula
            var companyKey = definition.CompanyConcurrencyKey(company);
            using var companyLock = _jobLock.TryAcquire(companyKey);

            if (companyLock is null)
            {
                var reason = $"Skipped: previous execution of {companyKey} still running";
                context.Result = reason;
                _skips.Skipped(name, reason);
                return;
            }

            // a do job ocupada: o mesmo job (ou o grupo do DW) roda para outra empresa; esta execução é adiada
            using var jobLock = _jobLock.TryAcquire(definition.ConcurrencyKey);

            if (jobLock is null)
            {
                context.Result = await Reschedule(context, $"Rescheduled: {definition.ConcurrencyKey} running for another company");
                return;
            }

            _skips.Ran(name);
            var started = DateTimeOffset.Now;
            data.Put(LastStartUtcKey, started.UtcDateTime.ToString("O", CultureInfo.InvariantCulture));

            try
            {
                await _runner.Run(definition, company);

                _logger.LogInformation("{Name}: execução concluída em {Duration:hh\\:mm\\:ss}.", name, DateTimeOffset.Now - started);
            }
            catch (Exception ex)
            {
                // o erro já foi para auditing pelo ScheduledJobRunner
                _logger.LogError(ex, "{Name}: execução falhou depois de {Duration:hh\\:mm\\:ss}.", name, DateTimeOffset.Now - started);
            }
            finally
            {
                // gravado no store pelo [PersistJobDataAfterExecution]: vale em qualquer Worker do cluster
                data.Put(LastEndUtcKey, DateTimeOffset.UtcNow.UtcDateTime.ToString("O", CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// dispara o mesmo job uma vez daqui a RescheduleDelay, por um trigger avulso no grupo integrations (o SchedulerRefreshJob
        /// só compara o trigger do cron, que tem o nome do job). Um adiamento por job de cada vez: se já houver um pendente, este
        /// horário vai junto com ele. O trigger avulso some ao disparar, e some com o job quando a releitura o tira da agenda
        /// </summary>
        private async Task<string> Reschedule(IJobExecutionContext context, string reason)
        {
            var name = context.JobDetail.Key.Name;
            var triggerKey = new TriggerKey($"{name} - adiado", Group);

            if (await context.Scheduler.CheckExists(triggerKey, context.CancellationToken))
            {
                _logger.LogInformation("{Name}: {Reason}; já há uma execução adiada pendente.", name, reason);
                return reason;
            }

            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .ForJob(context.JobDetail.Key)
                .StartAt(DateTimeOffset.UtcNow.Add(RescheduleDelay))
                // adiada que ainda esperou mais que o limiar de misfire é descartada, como os disparos do cron (§5.2)
                .WithSimpleSchedule(s => s.WithMisfireHandlingInstructionNextWithRemainingCount())
                .Build();

            await context.Scheduler.ScheduleJob(trigger, context.CancellationToken);
            _logger.LogInformation("{Name}: {Reason}; adiado {Delay}.", name, reason, RescheduleDelay);

            return reason;
        }

        private static DateTimeOffset? Utc(JobDataMap data, string key) =>
            data.ContainsKey(key)
            && DateTimeOffset.TryParse(data.GetString(key), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var value)
                ? value
                : null;
    }
}

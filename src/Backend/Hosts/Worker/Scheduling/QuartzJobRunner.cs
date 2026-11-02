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
    /// Hangfire (§5.2), pelo início e o fim da última execução guardados no JobDataMap
    /// </summary>
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class QuartzJobRunner : IJob
    {
        public const string Group = "integrations";
        public const string IdJobKey = "IdJob";
        public const string LastStartUtcKey = "LastStartUtc";
        public const string LastEndUtcKey = "LastEndUtc";

        private readonly ScheduledJobRunner _runner;
        private readonly IJobLock _jobLock;
        private readonly JobSkipCounter _skips;
        private readonly ILogger<QuartzJobRunner> _logger;

        public QuartzJobRunner(ScheduledJobRunner runner, IJobLock jobLock, JobSkipCounter skips, ILogger<QuartzJobRunner> logger) =>
            (_runner, _jobLock, _skips, _logger) = (runner, jobLock, skips, logger);

        public async Task Execute(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;
            var data = context.JobDetail.JobDataMap;

            // UseProperties: o JobDataMap só guarda texto (§3.3)
            if (!int.TryParse(data.GetString(IdJobKey), out var idJob)
                || !RecurringJobMap.Definitions.TryGetValue(idJob, out var definition))
            {
                _logger.LogError("{Name}: sem IdJob no JobDataMap ou sem método nos mapas de jobs recorrentes; nada executado.", name);
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

            // a mesma trava do WebApi e do WebJob: ocupada, a execução que a segura é de outro JobKey (o grupo do DW, a versão
            // anterior do job) ou de outro host
            using var jobLock = _jobLock.TryAcquire(definition.ConcurrencyKey);

            if (jobLock is null)
            {
                var reason = $"Skipped: previous execution of {definition.ConcurrencyKey} still running";
                context.Result = reason;
                _skips.Skipped(name, reason);
                return;
            }

            _skips.Ran(name);
            var started = DateTimeOffset.Now;
            data.Put(LastStartUtcKey, started.UtcDateTime.ToString("O", CultureInfo.InvariantCulture));

            try
            {
                await _runner.Run(definition);

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

        private static DateTimeOffset? Utc(JobDataMap data, string key) =>
            data.ContainsKey(key)
            && DateTimeOffset.TryParse(data.GetString(key), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var value)
                ? value
                : null;
    }
}

using Hosts.Scheduling;
using Hosts.Scheduling.RecurringJobs;
using Quartz;

namespace Worker.Scheduling
{
    /// <summary>
    /// o que o Quartz executa em cada job da agenda (src/Backend/Hosts/Worker/docs/PLANO-HOST-WORKER.md §4.2): uma classe só para
    /// todos os jobs, com o IdJob no JobDataMap, que repassa ao ScheduledJobRunner (log do erro em auditing). O Quartz cria um
    /// escopo por execução, então o runner e os services são scoped como nos outros hosts. Nenhuma exceção sobe: sem
    /// retentativa, o trigger segue para o próximo horário (§3.4)
    /// </summary>
    public class QuartzJobRunner : IJob
    {
        public const string Group = "integrations";
        public const string IdJobKey = "IdJob";

        private readonly ScheduledJobRunner _runner;
        private readonly ILogger<QuartzJobRunner> _logger;

        public QuartzJobRunner(ScheduledJobRunner runner, ILogger<QuartzJobRunner> logger) =>
            (_runner, _logger) = (runner, logger);

        public async Task Execute(IJobExecutionContext context)
        {
            var name = context.JobDetail.Key.Name;

            // UseProperties: o JobDataMap só guarda texto (§3.3)
            if (!int.TryParse(context.MergedJobDataMap.GetString(IdJobKey), out var idJob)
                || !RecurringJobMap.Definitions.TryGetValue(idJob, out var definition))
            {
                _logger.LogError("{Name}: sem IdJob no JobDataMap ou sem método nos mapas de jobs recorrentes; nada executado.", name);
                return;
            }

            var started = DateTimeOffset.Now;

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
        }
    }
}

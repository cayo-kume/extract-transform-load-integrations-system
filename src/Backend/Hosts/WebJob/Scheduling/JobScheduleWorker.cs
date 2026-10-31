using Application.Shared.Abstractions.Services;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Shared.Entities.Auditing;
using Hosts.Scheduling;
using Hosts.Scheduling.RecurringJobs;
using Microsoft.Extensions.Options;

namespace WebJob.Scheduling
{
    /// <summary>
    /// um job por processo (src/Backend/Hosts/WebJob/docs/PLANO-HOST-WEBJOB.md §4.1): lê a linha do job em auditing.Jobs, espera o
    /// próximo horário (intervalo ou RunningDailyAt, no fuso configurado), roda o método do mapa pelo ScheduledJobRunner e volta a
    /// ler a tabela. Mudar IntervalTimeInMinutes, RunningDailyAt ou IsActive vale no ciclo seguinte, sem republicar
    /// </summary>
    public class JobScheduleWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly WebJobSettings _settings;
        private readonly ILogger<JobScheduleWorker> _logger;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IJobLock _jobLock;

        // pulos seguidos deste processo: zera quando o job consegue rodar
        private int _consecutiveSkips;

        public JobScheduleWorker(IServiceScopeFactory scopeFactory, IOptions<WebJobSettings> settings, ILogger<JobScheduleWorker> logger, IHostApplicationLifetime lifetime, IJobLock jobLock) =>
            (_scopeFactory, _settings, _logger, _lifetime, _jobLock) = (scopeFactory, settings.Value, logger, lifetime, jobLock);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_settings.IdJob is not int idJob)
            {
                _logger.LogCritical("Nenhum job informado: rode com --job <IdJob> (ex.: WebJob.exe --job 8006).");
                Environment.ExitCode = 1;
                _lifetime.StopApplication();
                return;
            }

            if (!TryFindTimeZone(_settings.TimeZone, out var timeZone))
            {
                _logger.LogCritical("WebJob:TimeZone '{TimeZone}' não é um fuso conhecido.", _settings.TimeZone);
                Environment.ExitCode = 1;
                _lifetime.StopApplication();
                return;
            }

            // os mapas são montados aqui: IdJob repetido ou método sem [JobVersion] derrubam o processo já na subida
            if (!WebJobRecurringJobs.Definitions.TryGetValue(idJob, out var definition))
            {
                // na Azure, um WebJob contínuo que termina é reiniciado: o processo fica parado, avisando, em vez de cair em laço
                _logger.LogError("Job {IdJob} sem método nos mapas de jobs recorrentes (Hosts.Scheduling): o processo não roda nada.", idJob);
                await Task.Delay(Timeout.Infinite, stoppingToken).ContinueWith(_ => { });
                return;
            }

            var recheck = TimeSpan.FromMinutes(Math.Max(1, _settings.RecheckIntervalInMinutes));
            string? lastState = null;
            DateTimeOffset lastRun = DateTimeOffset.MinValue;
            DateTimeOffset? lastDue = null;

            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await ReadJob(idJob);
                var now = DateTimeOffset.Now;
                var after = lastRun > now ? lastRun : now;
                DateTimeOffset? next = job is { IsActive: true } ? JobCron.NextOccurrence(job.IntervalTimeInMinutes, job.RunningDailyAt, after, timeZone) : null;

                // o log só registra quando o estado muda, para não repetir a cada releitura
                var state = Describe(job, definition, next);

                if (state != lastState)
                {
                    _logger.LogInformation("{State}", state);
                    lastState = state;
                }

                if (next is not DateTimeOffset due)
                {
                    await Wait(recheck, stoppingToken);
                    continue;
                }

                if (due != lastDue)
                {
                    _logger.LogInformation("Job {IdJob}: próxima execução em {Due:yyyy-MM-dd HH:mm} ({TimeZone}).", idJob, TimeZoneInfo.ConvertTime(due, timeZone), timeZone.Id);
                    lastDue = due;
                }

                var wait = due - DateTimeOffset.Now;

                // horário distante: espera em pedaços e relê a tabela, para uma mudança valer antes do horário antigo
                if (wait > recheck)
                {
                    await Wait(recheck, stoppingToken);
                    continue;
                }

                await Wait(wait, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                lastRun = due;

                // relê logo antes de rodar: um job desligado (ou com a agenda trocada) durante a espera não roda neste horário
                var current = await ReadJob(idJob);

                if (current is not { IsActive: true }
                    || current.IntervalTimeInMinutes != job!.IntervalTimeInMinutes
                    || current.RunningDailyAt != job.RunningDailyAt)
                    continue;

                await Run(idJob, definition, current);
            }
        }

        private async Task Run(int idJob, RecurringJobDefinition definition, JobSchedule job)
        {
            var name = RecurringJobMap.RecurringJobId(idJob, job.Title, definition.Version!);

            // a mesma trava do WebApi (§5): um job nunca roda junto, nem entre um WebJob e o Hangfire. Ocupada, este horário é
            // pulado sem esperar; o próximo tenta de novo
            using var jobLock = _jobLock.TryAcquire(definition.ConcurrencyKey);

            if (jobLock is null)
            {
                _consecutiveSkips++;

                if (_consecutiveSkips % Math.Max(1, _settings.MaxConsecutiveSkips) == 0)
                    _logger.LogWarning("{Name} ({Key}) pulado {Skips} vez(es) seguida(s): a execução anterior ainda está rodando. Se continuar, aumente o intervalo em auditing.Jobs.", name, definition.ConcurrencyKey, _consecutiveSkips);
                else
                    _logger.LogInformation("{Name} ({Key}) pulado: a execução anterior ainda está rodando.", name, definition.ConcurrencyKey);

                return;
            }

            _consecutiveSkips = 0;
            var started = DateTimeOffset.Now;

            try
            {
                // um escopo por execução: o service e o ILoggerService do ScheduledJobRunner vêm dele
                await using var scope = _scopeFactory.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<ScheduledJobRunner>().Run(definition);

                _logger.LogInformation("{Name}: execução concluída em {Duration:hh\\:mm\\:ss}.", name, DateTimeOffset.Now - started);
            }
            catch (Exception ex)
            {
                // o erro já foi para auditing pelo ScheduledJobRunner; o processo segue para o próximo horário
                _logger.LogError(ex, "{Name}: execução falhou depois de {Duration:hh\\:mm\\:ss}.", name, DateTimeOffset.Now - started);
            }
        }

        private async Task<JobSchedule?> ReadJob(int idJob)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                return await scope.ServiceProvider.GetRequiredService<IJobScheduleRepository>().GetIntegrationJob(idJob);
            }
            catch (Exception ex)
            {
                // banco fora: tenta de novo na próxima releitura
                _logger.LogError(ex, "Job {IdJob}: não foi possível ler auditing.Jobs.", idJob);
                return null;
            }
        }

        private static string Describe(JobSchedule? job, RecurringJobDefinition definition, DateTimeOffset? next)
        {
            if (job is null)
                return $"Job {definition.IdJob} ({definition.Target}): sem linha em auditing.Jobs (ou banco fora); aguardando.";

            var name = RecurringJobMap.RecurringJobId(job.IdJob, job.Title, definition.Version!);

            if (!job.IsActive)
                return $"{name}: inativo em auditing.Jobs (IsActive = 0); aguardando.";

            if (next is null)
                return $"{name}: intervalo de {job.IntervalTimeInMinutes} minutos não vira cron (precisa dividir a hora ou o dia, ou ter RunningDailyAt); aguardando.";

            var schedule = job.RunningDailyAt is TimeSpan at ? $"todo dia às {at:hh\\:mm}" : $"a cada {job.IntervalTimeInMinutes} minuto(s)";

            return $"{name}: ativo, {schedule}, fila {job.Queue}.";
        }

        private static async Task Wait(TimeSpan delay, CancellationToken stoppingToken)
        {
            if (delay <= TimeSpan.Zero)
                return;

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // encerramento do processo
            }
        }

        private static bool TryFindTimeZone(string id, out TimeZoneInfo timeZone)
        {
            timeZone = TimeZoneInfo.Utc;

            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(id);
                return true;
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                return false;
            }
        }
    }
}

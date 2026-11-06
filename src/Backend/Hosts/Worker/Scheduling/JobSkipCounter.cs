using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Worker.Scheduling
{
    /// <summary>
    /// pulos seguidos por job, para o aviso de Quartz:Concurrency:MaxConsecutiveSkips (src/Backend/Hosts/Worker/.docs/PLANO-HOST-WORKER.md
    /// §5.3, item 3). Conta os três tipos de pulo do mesmo jeito: o disparo que aconteceu com a execução anterior rodando, a trava
    /// ocupada e o descarte pelo misfire. É do processo, como nos outros hosts: com dois Workers, cada um conta os seus
    /// </summary>
    public class JobSkipCounter
    {
        private readonly ConcurrentDictionary<string, int> _skips = new();
        private readonly IOptions<QuartzSettings> _settings;
        private readonly ILogger<JobSkipCounter> _logger;

        public JobSkipCounter(IOptions<QuartzSettings> settings, ILogger<JobSkipCounter> logger) =>
            (_settings, _logger) = (settings, logger);

        /// <summary>
        /// registra o pulo no log e, a cada MaxConsecutiveSkips seguidos, o aviso de que o job está lento demais para o intervalo
        /// </summary>
        public void Skipped(string name, string reason)
        {
            var skips = _skips.AddOrUpdate(name, 1, (_, count) => count + 1);

            if (skips % Math.Max(1, _settings.Value.Concurrency.MaxConsecutiveSkips) == 0)
                _logger.LogWarning("{Name}: {Reason}. Pulado {Skips} vez(es) seguida(s): se continuar, aumente o intervalo em auditing.Jobs.", name, reason, skips);
            else
                _logger.LogInformation("{Name}: {Reason}.", name, reason);
        }

        /// <summary>
        /// o job rodou: o contador zera
        /// </summary>
        public void Ran(string name) =>
            _skips.TryRemove(name, out _);
    }
}

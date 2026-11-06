using Domain.Shared.Entities.Auditing;
using Microsoft.Extensions.Configuration;

namespace Hosts.Scheduling
{
    /// <summary>
    /// um tipo de host de agenda por banco (.docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 4.2): cada instância de Hangfire, Quartz ou
    /// WebJob se registra em auditing.SchedulingHosts com heartbeat (SchedulingHostHeartbeat), e só o tipo que está no ar há mais
    /// tempo executa os jobs da agenda. Vários do mesmo tipo podem (o cluster do Quartz, os WebJobs, vários servidores do Hangfire).
    /// Quem não agenda continua no ar e assume quando o outro tipo sair (parada ou 2 min sem heartbeat). Com
    /// Scheduling:AllowOtherHostTypes = true, os tipos rodam juntos de propósito (uma migração controlada); a trava continua
    /// impedindo que um job rode junto nos dois
    /// </summary>
    public class SchedulingHostRegistry
    {
        public const string SectionName = "Scheduling";

        public static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan ExpireAfter = TimeSpan.FromMinutes(2);

        // sem a primeira resposta do banco (ou com a tabela ainda não criada), agenda como antes do registro existir: sem banco,
        // nenhum job roda de qualquer jeito
        private volatile bool _isActive = true;
        private volatile string? _activeHostType;

        public SchedulingHostRegistry(string hostType, IConfiguration configuration)
        {
            HostType = hostType;
            AllowOtherHostTypes = bool.TryParse(configuration[$"{SectionName}:AllowOtherHostTypes"], out var allow) && allow;

            // na Azure o WebJob tem o nome em WEBJOBS_NAME; o serviço do Windows, em Service:Name
            var detail = Environment.GetEnvironmentVariable("WEBJOBS_NAME") ?? configuration["Service:Name"];
            InstanceName = string.Join(" | ", new[] { Environment.MachineName, detail, $"pid {Environment.ProcessId}" }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        public Guid InstanceId { get; } = Guid.NewGuid();

        /// <summary>
        /// Hangfire, Quartz ou WebJob
        /// </summary>
        public string HostType { get; }

        public string InstanceName { get; }

        public bool AllowOtherHostTypes { get; }

        /// <summary>
        /// este host executa os jobs da agenda
        /// </summary>
        public bool IsActive => AllowOtherHostTypes || _isActive;

        /// <summary>
        /// motivo do pulo quando outro tipo de host agenda neste banco (para o log e o estado do Hangfire)
        /// </summary>
        public string SkipReason => $"Skipped: the {_activeHostType} scheduling host is active in this database (auditing.SchedulingHosts)";

        /// <summary>
        /// aplica o resultado de um heartbeat: o tipo mais antigo no ar agenda. Devolve true quando o estado deste host mudou
        /// </summary>
        public bool Update(IReadOnlyList<SchedulingHostType> types, out SchedulingHostType? owner)
        {
            owner = types.FirstOrDefault();

            var active = owner is null || string.Equals(owner.HostType, HostType, StringComparison.OrdinalIgnoreCase);
            var changed = active != _isActive;

            _activeHostType = owner?.HostType;
            _isActive = active;

            return changed;
        }
    }
}

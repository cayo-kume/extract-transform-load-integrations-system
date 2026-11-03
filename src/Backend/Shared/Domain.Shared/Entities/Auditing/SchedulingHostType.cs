namespace Domain.Shared.Entities.Auditing
{
    /// <summary>
    /// um tipo de host de agenda em auditing.SchedulingHosts: as instâncias no ar, desde quando o tipo está no ar (a instância
    /// mais antiga) e o último heartbeat. Só dados; a regra de quem agenda fica no Hosts.Scheduling (SchedulingHostRegistry)
    /// </summary>
    public class SchedulingHostType
    {
        public string HostType { get; set; } = string.Empty;

        public int Instances { get; set; }

        /// <summary>
        /// UTC
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// UTC
        /// </summary>
        public DateTime LastHeartbeat { get; set; }
    }
}

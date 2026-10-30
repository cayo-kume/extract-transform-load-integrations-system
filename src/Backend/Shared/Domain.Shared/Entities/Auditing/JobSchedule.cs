namespace Domain.Shared.Entities.Auditing
{
    /// <summary>
    /// agenda de um job de integração em auditing.Jobs (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.1): se roda (IsActive),
    /// quando (IntervalTimeInMinutes ou, nos diários, RunningDailyAt) e em que fila do agendador (Queue)
    /// </summary>
    public class JobSchedule
    {
        public int IdJob { get; set; }

        public string? Title { get; set; }

        /// <summary>
        /// 0 = sob demanda, fora da agenda
        /// </summary>
        public int IntervalTimeInMinutes { get; set; }

        /// <summary>
        /// preenchida, o job é diário e roda nessa hora (no fuso do agendador); nula, segue IntervalTimeInMinutes
        /// </summary>
        public TimeSpan? RunningDailyAt { get; set; }

        public string Queue { get; set; } = "default";

        public bool IsActive { get; set; }
    }
}

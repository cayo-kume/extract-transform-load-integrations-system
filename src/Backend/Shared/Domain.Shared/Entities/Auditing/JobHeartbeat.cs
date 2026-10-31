namespace Domain.Shared.Entities.Auditing
{
    /// <summary>
    /// uma linha de auditing.vw_JobsHeartbeat: a agenda do job de integração e a última execução dele. Só dados; a regra de
    /// "off" fica em quem lê. As datas estão no horário de Brasília, como em auditing.Logs
    /// </summary>
    public class JobHeartbeat
    {
        public int? IdParent { get; set; }

        public int IdJob { get; set; }

        public string? Title { get; set; }

        public int IntervalTimeInMinutes { get; set; }

        public TimeSpan? RunningDailyAt { get; set; }

        public string Queue { get; set; } = "default";

        public bool IsActive { get; set; }

        /// <summary>
        /// início da última execução terminada
        /// </summary>
        public DateTime? LastStartDate { get; set; }

        /// <summary>
        /// fim da última execução terminada
        /// </summary>
        public DateTime? LastEndDate { get; set; }

        /// <summary>
        /// início da execução aberta (sem EndDate) mais antiga, quando houver
        /// </summary>
        public DateTime? OpenSince { get; set; }
    }
}

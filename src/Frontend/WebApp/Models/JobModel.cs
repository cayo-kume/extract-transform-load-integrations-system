namespace WebApp.Models
{
    /// <summary>
    /// Job de auditing.Jobs, com a última execução e a saúde calculada a partir dela.
    /// </summary>
    public class JobModel
    {
        /// <summary>
        /// Coluna IdParent: nas integrações é a aplicação (auditing.Apps); nas procedures é o job de integração que as chama.
        /// </summary>
        public int IdParent { get; set; }

        /// <summary>
        /// Aplicação do job, já resolvida: o IdParent nas integrações e a aplicação do job pai nas procedures.
        /// </summary>
        public int IdApp { get; set; }

        public int IdJob { get; set; }
        public int IdType { get; set; }
        public string? JobType { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// 0 = sob demanda.
        /// </summary>
        public int IntervalTimeInMinutes { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Preenchida, o job é diário e roda nessa hora.
        /// </summary>
        public TimeOnly? RunningDailyAt { get; set; }

        public string? Queue { get; set; }

        public ExecutionModel? LastExecution { get; set; }

        /// <summary>
        /// Nula para job inativo ou sob demanda que ainda não rodou.
        /// </summary>
        public JobHealth? Health { get; set; }
    }
}

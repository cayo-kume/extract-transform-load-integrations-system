namespace WebApp.Models
{
    /// <summary>
    /// Linha do painel de saúde: a última execução de um job e a saúde dele.
    /// </summary>
    public class HeartbeatModel
    {
        public int IdApp { get; set; }
        public string? AppTitle { get; set; }
        /// <summary>
        /// CNPJ da empresa da linha (auditing.Jobs.Company). Na visão "Todas as empresas" da holding, diz de qual empresa é a
        /// linha e para qual empresa vai cada ação dela.
        /// </summary>
        public string Company { get; set; } = "";

        public int IdJob { get; set; }
        public int IdType { get; set; }
        public string? JobType { get; set; }
        public string? JobTitle { get; set; }
        /// <summary>
        /// Início da última execução; nulo para job atrasado que nunca rodou.
        /// </summary>
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public int Records { get; set; }
        public JobHealth Health { get; set; }

        /// <summary>
        /// Agenda do job, para o chip dizer desde quando está atrasado ou há quanto tempo roda.
        /// </summary>
        public int IntervalTimeInMinutes { get; set; }

        public TimeOnly? RunningDailyAt { get; set; }

        public AcknowledgementModel? Acknowledgement { get; set; }

        /// <summary>
        /// A última execução tem mensagem de aviso (nível 2), que não muda a saúde.
        /// </summary>
        public bool HasWarnings { get; set; }

        public TimeSpan? Duration => EndDate - StartDate;
    }
}

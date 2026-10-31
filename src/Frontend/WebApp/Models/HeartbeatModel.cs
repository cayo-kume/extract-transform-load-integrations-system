namespace WebApp.Models
{
    /// <summary>
    /// Linha do painel de saúde: a última execução de um job e a saúde dele.
    /// </summary>
    public class HeartbeatModel
    {
        public int IdApp { get; set; }
        public string? AppTitle { get; set; }
        public int IdJob { get; set; }
        public int IdType { get; set; }
        public string? JobType { get; set; }
        public string? JobTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Records { get; set; }
        public JobHealth Health { get; set; }

        /// <summary>
        /// A última execução tem mensagem de aviso (nível 2), que não muda a saúde.
        /// </summary>
        public bool HasWarnings { get; set; }

        public TimeSpan? Duration => EndDate - StartDate;
    }
}

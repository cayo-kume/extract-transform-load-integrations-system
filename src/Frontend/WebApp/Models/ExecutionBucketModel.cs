namespace WebApp.Models
{
    /// <summary>
    /// Execuções de uma aplicação numa faixa de tempo (uma hora ou um dia), para os gráficos de tendência.
    /// </summary>
    public class ExecutionBucketModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Executions { get; set; }

        /// <summary>
        /// Execuções com ao menos uma mensagem de erro.
        /// </summary>
        public int FailedExecutions { get; set; }

        public int Errors { get; set; }
        public int Records { get; set; }
    }

    /// <summary>
    /// As faixas de tempo de uma aplicação, na ordem do tempo.
    /// </summary>
    public class AppBucketsModel
    {
        public int IdApp { get; set; }
        public IReadOnlyList<ExecutionBucketModel> Buckets { get; set; } = [];
    }
}

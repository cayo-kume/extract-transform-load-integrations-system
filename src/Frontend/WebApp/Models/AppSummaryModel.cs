namespace WebApp.Models
{
    /// <summary>
    /// Resumo de uma aplicação no período, para os cards.
    /// </summary>
    public class AppSummaryModel
    {
        public int IdParent { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public int Integrations { get; set; }
        public int Procedures { get; set; }
        public int ActiveJobs { get; set; }

        public int Executions { get; set; }

        /// <summary>
        /// Execuções com ao menos uma mensagem de erro (nível 3).
        /// </summary>
        public int FailedExecutions { get; set; }

        public int Records { get; set; }
        public int Warnings { get; set; }
        public int Errors { get; set; }

        public bool IsActive => ActiveJobs > 0;
    }
}

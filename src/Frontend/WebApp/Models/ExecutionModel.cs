namespace WebApp.Models
{
    /// <summary>
    /// Execução de auditing.Logs, com os registros, avisos e erros já contados.
    /// </summary>
    public class ExecutionModel
    {
        public long IdExecution { get; set; }
        public int IdJob { get; set; }
        public int IdType { get; set; }
        public Guid Execution { get; set; }
        public Guid? ExecutionParent { get; set; }
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Nula enquanto a execução está rodando.
        /// </summary>
        public DateTime? EndDate { get; set; }

        public int Records { get; set; }
        public int Warnings { get; set; }
        public int Errors { get; set; }

        public TimeSpan? Duration => EndDate - StartDate;
    }
}

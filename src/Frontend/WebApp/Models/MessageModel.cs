namespace WebApp.Models
{
    /// <summary>
    /// Mensagem de auditing.Messages. Nas listas por nível (Logs de Erros), vem com a aplicação, o job e o início da execução.
    /// </summary>
    public class MessageModel
    {
        public long IdExecutionMessage { get; set; }
        public int IdJob { get; set; }
        public int IdType { get; set; }
        public Guid Execution { get; set; }
        public MessageLevel Level { get; set; }
        public string? Message { get; set; }
        public string? SqlCommand { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? ExceptionStackTrace { get; set; }

        public int? IdApp { get; set; }
        public string? AppTitle { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? ExecutionStartDate { get; set; }
    }
}

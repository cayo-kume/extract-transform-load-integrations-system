namespace WebApp.Models
{
    /// <summary>
    /// Registro de auditing.Records gravado por uma execução.
    /// </summary>
    public class RecordModel
    {
        public long IdRecord { get; set; }
        public int IdJob { get; set; }
        public int IdType { get; set; }
        public Guid Execution { get; set; }
        public string? FieldKeyValue { get; set; }
        public string? RecordText { get; set; }
    }
}

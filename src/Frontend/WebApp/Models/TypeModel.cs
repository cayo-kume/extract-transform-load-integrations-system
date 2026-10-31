namespace WebApp.Models
{
    /// <summary>
    /// Tipo de job de auditing.Types (1 = Procedure, 2 = Integração).
    /// </summary>
    public class TypeModel
    {
        public int IdType { get; set; }
        public string? JobType { get; set; }
    }
}

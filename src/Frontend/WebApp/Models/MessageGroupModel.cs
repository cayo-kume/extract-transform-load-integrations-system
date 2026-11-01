namespace WebApp.Models
{
    /// <summary>
    /// Mensagens de um job com o mesmo texto (uma "causa"), depois de trocar números e Guids por marcadores
    /// (ver Data/MessageGrouping.cs).
    /// </summary>
    public class MessageGroupModel
    {
        public string Key { get; set; } = "";
        public MessageLevel Level { get; set; }

        /// <summary>
        /// Texto da ocorrência mais recente, como exemplo do grupo.
        /// </summary>
        public string? Example { get; set; }

        public int Count { get; set; }
        public DateTime FirstAt { get; set; }
        public DateTime LastAt { get; set; }
    }
}

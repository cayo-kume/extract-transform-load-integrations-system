namespace WebApp.Models
{
    /// <summary>
    /// Filtro das mensagens da tela Logs de Erros: período, nível mínimo, aplicação, job, texto e causa.
    /// </summary>
    public record MessageQuery
    {
        public Period Period { get; init; } = Period.Last24Hours;
        public MessageLevel MinLevel { get; init; } = MessageLevel.Error;
        public int? IdApp { get; init; }

        /// <summary>
        /// Job das mensagens, junto com IdType; nulo mostra a aplicação inteira.
        /// </summary>
        public int? IdJob { get; init; }

        public int? IdType { get; init; }

        /// <summary>
        /// Texto procurado na mensagem e na exceção, sem diferenciar maiúsculas nem acentos.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Só as mensagens de uma causa (MessageGroupModel.Key).
        /// </summary>
        public string? GroupKey { get; init; }
    }
}

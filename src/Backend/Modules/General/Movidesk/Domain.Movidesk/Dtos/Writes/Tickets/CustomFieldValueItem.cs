namespace Domain.Movidesk.Dtos.Writes.Tickets
{
    /// <summary>
    /// item de um campo adicional do tipo lista
    /// </summary>
    public class CustomFieldValueItem
    {
        // Cód. Ref.; lista de pessoas
        public string? personId { get; init; }

        // Cód. Ref.; lista de clientes
        public string? clientId { get; init; }

        // nome da equipe; lista de agentes
        public string? team { get; init; }

        // nome do item; lista de valores e seleção múltipla
        public string? customFieldItem { get; init; }
    }
}

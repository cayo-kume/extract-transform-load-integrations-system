namespace Domain.Movidesk.Dtos.Writes.Tickets
{
    /// <summary>
    /// corpo do POST e do PATCH de tickets: só os campos que podem ser escritos. Tudo anulável e serializado sem nulos:
    /// no PATCH, null = campo não enviado (não muda); lista vazia = lista apagada; lista com itens = lista substituída
    /// </summary>
    public class Ticket
    {
        // 1 = Interno, 2 = Público
        public int? type { get; init; }

        public string? subject { get; init; }

        // nome de uma categoria cadastrada e relacionada ao tipo do ticket
        public string? category { get; init; }

        // nome de uma urgência cadastrada
        public string? urgency { get; init; }

        // vai junto com a justification
        public string? status { get; init; }

        public string? justification { get; init; }

        public int? origin { get; init; }

        // vai junto com o ownerTeam
        public PersonReference? owner { get; init; }

        // vai junto com o owner
        public string? ownerTeam { get; init; }

        public PersonReference? createdBy { get; init; }

        // serviço selecionado (o mais profundo)
        public int? serviceFirstLevelId { get; init; }

        public List<string>? tags { get; init; }

        // UTC
        public DateTime? resolvedIn { get; init; }

        // UTC
        public DateTime? closedIn { get; init; }

        public bool? resolvedInFirstCall { get; init; }

        public int? sequence { get; init; }

        // UTC
        public DateTime? slaSolutionDate { get; init; }

        public List<Client>? clients { get; init; }

        public List<Action>? actions { get; init; }

        public List<TicketReference>? parentTickets { get; init; }

        public List<TicketReference>? childrenTickets { get; init; }

        public List<CustomFieldValue>? customFieldValues { get; init; }

        public List<AssetReference>? assets { get; init; }
    }
}

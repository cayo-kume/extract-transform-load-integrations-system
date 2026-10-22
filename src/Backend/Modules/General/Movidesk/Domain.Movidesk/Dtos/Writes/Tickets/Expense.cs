namespace Domain.Movidesk.Dtos.Writes.Tickets
{
    /// <summary>
    /// despesa de uma ação
    /// </summary>
    public class Expense
    {
        public long? id { get; init; }

        // tipo de despesa cadastrado
        public string? type { get; init; }

        public PersonReference? createdBy { get; init; }

        public TeamReference? createdByTeam { get; init; }

        // UTC, menor ou igual à data atual
        public DateTime? date { get; init; }

        // obrigatório quando não houver value
        public int? quantity { get; init; }

        // obrigatório quando não houver quantity
        public decimal? value { get; init; }
    }
}

namespace Domain.Movidesk.Dtos.Writes.Tickets
{
    /// <summary>
    /// ação do ticket. Sem id (ou id = 0) a ação é inserida; com id, alterada
    /// </summary>
    public class Action
    {
        public int? id { get; init; }

        // 1 = Interna, 2 = Pública
        public int? type { get; init; }

        public int? origin { get; init; }

        // interpretada como HTML pela Movidesk
        public string? description { get; init; }

        // UTC
        public DateTime? createdDate { get; init; }

        // obrigatório quando houver apontamentos
        public PersonReference? createdBy { get; init; }

        public List<TimeAppointment>? timeAppointments { get; init; }

        public List<Expense>? expenses { get; init; }

        public List<string>? tags { get; init; }
    }
}

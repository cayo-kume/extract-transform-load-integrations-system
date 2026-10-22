namespace Domain.Movidesk.Dtos.Writes.Tickets
{
    /// <summary>
    /// apontamento de horas de uma ação. Sem id o apontamento é inserido; com id, alterado
    /// </summary>
    public class TimeAppointment
    {
        public int? id { get; init; }

        // atividade cadastrada
        public string? activity { get; init; }

        // com as horas zeradas
        public DateTime? date { get; init; }

        // ex.: 08:00:00
        public string? periodStart { get; init; }

        // ex.: 12:00:00
        public string? periodEnd { get; init; }

        // ex.: 04:00:00
        public string? workTime { get; init; }

        public string? workTypeName { get; init; }

        public PersonReference? createdBy { get; init; }

        public TeamReference? createdByTeam { get; init; }
    }
}

using static Domain.Movidesk.Dtos.Tickets.Ticket;

namespace Domain.Movidesk.Dtos.Tickets
{
    public class TimeAppointment
    {
        public string id { get; init; }
        public string activity { get; init; }
        public string date { get; init; }
        public string periodStart { get; init; }
        public string periodEnd { get; init; }
        public string workTime { get; init; }
        public string accountedTime { get; init; }
        public string workTypeName { get; init; }

        public Person createdBy { get; init; }
        public CreatedByTeam createdByTeam { get; init; }
    }
}

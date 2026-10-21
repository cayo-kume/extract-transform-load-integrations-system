namespace Domain.Movidesk.Dtos.Reads.Tickets
{
    public class Expense
    {
        public string id { get; init; }
        public string type { get; init; }
        public string serviceReport { get; init; }
        public string date { get; init; }
        public string quantity { get; init; }
        public string value { get; init; }

        public PersonWithAddress createdBy { get; init; }
        public CreatedByTeam createdByTeam { get; init; }
    }
}

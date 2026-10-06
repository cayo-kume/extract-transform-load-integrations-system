namespace Domain.Movidesk.Entities.Ticket
{
    public class Expense
    {
        public string ticketId { get; private set; }
        public string type { get; private set; }
        public string serviceReport { get; private set; }
        public string createdBy { get; private set; } //Person
        public string createdByTeam { get; private set; }
        
        public DateTime date { get; private set; }
        
        public decimal value { get; private set; }
        
        public int actionId { get; private set; }
        public int id { get; private set; }
        public int quantity { get; private set; }

    }
}

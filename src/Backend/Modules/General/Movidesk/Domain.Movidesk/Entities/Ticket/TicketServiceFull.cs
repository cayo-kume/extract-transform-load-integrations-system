namespace Domain.Movidesk.Entities.Ticket
{
    public class TicketServiceFull
    {
        public string ticketId { get; private set; }
        public int id { get; private set; }
        public string service { get; private set; }

        public TicketServiceFull() { }

        public TicketServiceFull(string ticketId, int id, string service)
        {
            this.ticketId = ticketId;
            this.id = id;
            this.service = service;
        }
    }
}

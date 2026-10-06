namespace Domain.Movidesk.Entities.Ticket
{
    public class TicketTag
    {
        public string ticketId { get; private set; }
        public int id { get; private set; }
        public string tag { get; private set; }

        public TicketTag() { }

        public TicketTag(string ticketId, int id, string tag)
        {
            this.ticketId = ticketId;
            this.id = id;
            this.tag = tag;
        }
    }
}

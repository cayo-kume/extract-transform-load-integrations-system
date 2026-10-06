namespace Domain.Movidesk.Entities.Ticket
{
    public class ActionTag
    {
        public int actionId { get; private set; }
        public int id { get; private set; }
        
        public string ticketId { get; private set; }
        public string tag { get; private set; }
    }
}

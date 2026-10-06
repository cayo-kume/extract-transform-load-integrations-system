namespace Domain.Movidesk.Entities.Ticket
{
    public class Attachment
    {
        public string ticketId { get; private set; }
        public int actionId { get; private set; }
        public int id { get; private set; }
        public string fileName { get; private set; }
        public string path { get; private set; }
        public string createdBy { get; private set; } //Person
        public DateTime createdDate { get; private set; }
        
    }
}

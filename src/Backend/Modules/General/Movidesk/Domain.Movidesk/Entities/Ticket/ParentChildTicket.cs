namespace Domain.Movidesk.Entities.Ticket
{
    public class ParentChildTicket
    {
        public string ticketId { get; private set; }
        public string id { get; private set; }
        public string subject { get; private set; }
        public string isDeleted { get; private set; }
        public string isParent { get; private set; }
        public string isChildren { get; private set; }
    }
}

namespace Domain.Movidesk.Entities.Ticket
{
    public class Action
    {
        public string ticketId { get; private set; }
        public string description { get; private set; }
        public string htmlDescription { get; private set; }
        public string status { get; private set; }
        public string justification { get; private set; }
        public string createdBy { get; private set; } //Person
        
        public int id { get; private set; }
        public int type { get; private set; }
        public int origin { get; private set; }
        
        public DateTime createdDate { get; private set; }
        
        public bool isDeleted { get; private set; }

        public List<ActionTag> tags { get; private set; }
        public List<TimeAppointment> timeAppointments { get; private set; }
        public List<Attachment> attachments { get; private set; }
        public List<Expense> expenses { get; private set; }
    }
}

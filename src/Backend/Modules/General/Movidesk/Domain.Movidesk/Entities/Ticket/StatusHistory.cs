namespace Domain.Movidesk.Entities.Ticket
{
    public class StatusHistory
    {
        public int id { get; private set; }

        public string status { get; private set; }
        public string justification { get; private set; }
        public string changedBy { get; private set; } //Person
        public string ticketdId { get; private set; }

        public DateTime changedDate { get; private set; }
        
        public decimal permanencyTimeFullTime { get; private set; }
        public decimal permanencyTimeWorkingTime { get; private set; }
    }
}

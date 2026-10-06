namespace Domain.Movidesk.Entities.Ticket
{
    public class TimeAppointment
    {
        public string ticketId { get; private set; }
        public string workTypeName { get; private set; }
        public string createdBy { get; private set; }
        public string createdByTeam { get; private set; } //Person
        public string activity { get; private set; }
        
        public int actionId { get; private set; }
        public int id { get; private set; }
        
        public decimal accountedTime { get; private set; }
        
        public DateTime date { get; private set; }
        
        public TimeOnly periodStart { get; private set; }
        public TimeOnly periodEnd { get; private set; }
        public TimeOnly workTime { get; private set; }
    }
}

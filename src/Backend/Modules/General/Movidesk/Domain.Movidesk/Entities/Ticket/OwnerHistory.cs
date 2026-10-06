namespace Domain.Movidesk.Entities.Ticket
{
    public class OwnerHistory
    {
        public string ticketId { get; private set; }
        public string ownerTeam { get; private set; }
        public string owner { get; private set; }  //Person
        public string changedBy { get; private set; }  //Person
        
        public int id { get; private set; }
        
        public decimal permanencyTimeFullTime { get; private set; }
        public decimal permanencyTimeWorkingTime { get; private set; }
        
        public DateTime changedDate { get; private set; }

    }
}

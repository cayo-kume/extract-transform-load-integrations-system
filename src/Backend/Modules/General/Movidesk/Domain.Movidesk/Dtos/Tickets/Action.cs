namespace Domain.Movidesk.Dtos.Tickets
{
    public class Action
    {
        public string id { get; init; }
        public string type { get; init; }
        public string origin { get; init; }
        public string description { get; init; }
        public string htmlDescription { get; init; }
        public string status { get; init; }
        public string justification { get; init; }
        public string createdDate { get; init; }
        public string isDeleted { get; init; }

        public Person createdBy { get; init; }

        public List<string> tags { get; init; }
        public List<TimeAppointment> timeAppointments { get; init; }
        public List<Attachment> attachments { get; init; }
        public List<Expense> expenses { get; init; }
    }
}

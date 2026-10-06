namespace Domain.Movidesk.Dtos.Tickets
{
    public class StatusHistory
    {
        public string status { get; init; }
        public string justification { get; init; }
        public string changedDate { get; init; }
        public string permanencyTimeFullTime { get; init; }
        public string permanencyTimeWorkingTime { get; init; }
        
        public Person changedBy { get; init; }
    }
}

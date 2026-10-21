namespace Domain.Movidesk.Dtos.Reads.Tickets
{
    public class OwnerHistory
    {
        public string ownerTeam { get; init; }
        public string changedDate { get; init; }
        public string permanencyTimeFullTime { get; init; }
        public string permanencyTimeWorkingTime { get; init; }

        public Person owner { get; init; }
        public Person changedBy { get; init; }
    }
}

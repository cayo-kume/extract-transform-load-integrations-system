namespace Domain.Movidesk.Dtos.Reads.Tickets
{
    public class Client : Person
    {
        public string isDeleted { get; init; }

        public Person organization { get; init; }
    }
}

namespace Domain.Movidesk.Dtos.Tickets
{
    public class Attachment
    {
        public string fileName { get; init; }
        public string path { get; init; }
        public string createdDate { get; init; }
        
        public Person createdBy { get; init; }
    }
}

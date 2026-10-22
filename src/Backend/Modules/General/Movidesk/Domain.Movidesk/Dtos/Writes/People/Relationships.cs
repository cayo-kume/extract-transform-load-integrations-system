namespace Domain.Movidesk.Dtos.Writes.People
{
    /// <summary>
    /// corpo do POST /persons/relationships: inclui organizações sem mexer nas que a pessoa já tem
    /// </summary>
    public class Relationships
    {
        public List<Relationship>? relationships { get; init; }
    }
}

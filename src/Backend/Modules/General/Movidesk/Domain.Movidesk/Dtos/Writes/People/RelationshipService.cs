namespace Domain.Movidesk.Dtos.Writes.People
{
    /// <summary>
    /// serviço liberado num relacionamento
    /// </summary>
    public class RelationshipService
    {
        public int? id { get; init; }

        public bool? copyToChildren { get; init; }
    }
}

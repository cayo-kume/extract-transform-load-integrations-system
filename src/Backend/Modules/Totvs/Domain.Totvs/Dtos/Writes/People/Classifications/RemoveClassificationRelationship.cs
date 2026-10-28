namespace Domain.Totvs.Dtos.Writes.People.Classifications
{
    /// <summary>
    /// corpo do POST person/v2/classification-relationship/remove (RemoveClassificationRelationshipCommand): tira classificações de uma pessoa
    /// (o par da lista classifications das escritas de cliente, que só inclui)
    /// </summary>
    public class RemoveClassificationRelationship
    {
        public int? personCode { get; init; }
        public List<RemoveClassification>? classifications { get; init; }
    }
}

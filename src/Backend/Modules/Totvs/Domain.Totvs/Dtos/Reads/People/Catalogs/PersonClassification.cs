namespace Domain.Totvs.Dtos.Reads.People.Catalogs
{
    /// <summary>
    /// classificação de pessoa e o seu tipo (GET person/v2/classifications, ClassificationTypeRelationshipModel)
    /// </summary>
    public class PersonClassification
    {
        public string? typeCode { get; init; }
        public string? typeDescription { get; init; }
        public string? code { get; init; }
        public string? description { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}

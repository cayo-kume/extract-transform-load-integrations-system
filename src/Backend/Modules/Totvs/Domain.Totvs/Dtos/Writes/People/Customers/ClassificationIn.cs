namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// classificação (ClassificationInDto): a lista só inclui; para tirar, PostRemoveClassificationRelationship
    /// </summary>
    public class ClassificationIn
    {
        public int? classificationTypeCode { get; init; }
        public string? classificationCode { get; init; }
    }
}

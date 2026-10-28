namespace Domain.Totvs.Dtos.Writes.People.Classifications
{
    /// <summary>
    /// classificação a tirar (RemoveClassificationModel): o tipo e o código. Se o código nulo tira todas do tipo não está confirmado, então o
    /// validador o exige
    /// </summary>
    public class RemoveClassification
    {
        public int? classificationTypeCode { get; init; }
        public string? classificationCode { get; init; }
    }
}

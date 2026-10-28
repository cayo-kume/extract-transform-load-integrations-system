namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Classificação. (ClassificationInDto)
    /// </summary>
    public class B2COrderClassification
    {
        // Codigo do tipo de classificação.
        public int? classificationTypeCode { get; init; }
        // Codigo da classificação.
        public string? classificationCode { get; init; }
    }
}

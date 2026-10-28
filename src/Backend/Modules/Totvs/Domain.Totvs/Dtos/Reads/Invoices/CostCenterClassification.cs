namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// classificação do centro de custo (CostCenterClassificationModel)
    /// </summary>
    public class CostCenterClassification
    {
        public string? typeCode { get; init; }
        public string? typeDescription { get; init; }
        public string? code { get; init; }
        public string? description { get; init; }
    }
}

namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// prazo médio da tabela de preço (AveragePeriodModel)
    /// </summary>
    public class PriceTableAveragePeriod
    {
        public string? quantity { get; init; }
        public string? variationValue { get; init; }
        public string? variationPercentage { get; init; }
    }
}

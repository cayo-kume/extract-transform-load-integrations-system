namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// item da escala de tabela de preço (PriceTableScaleItemModel)
    /// </summary>
    public class PriceTableScaleItem
    {
        public string? quantity { get; init; }
        public string? variationValue { get; init; }
        public string? variationPercentage { get; init; }
    }
}

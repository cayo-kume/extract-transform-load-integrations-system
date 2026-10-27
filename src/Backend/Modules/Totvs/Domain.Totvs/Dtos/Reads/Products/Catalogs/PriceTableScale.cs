namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// escala de tabela de preço (GET product/v2/price-table-scales, PriceTableScaleModel)
    /// </summary>
    public class PriceTableScale
    {
        public string? scaleCode { get; init; }
        public string? scaleDescription { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public List<PriceTableScaleItem>? scales { get; init; }
    }
}

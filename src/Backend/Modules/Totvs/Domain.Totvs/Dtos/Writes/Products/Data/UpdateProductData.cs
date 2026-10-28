namespace Domain.Totvs.Dtos.Writes.Products.Data
{
    /// <summary>
    /// corpo do PUT product/v2/data (UpdateProductDataCommand): dados fiscais dos produtos do filtro (NCM, unidade, CST, peso, CEST e prefixo
    /// EAN). O prefixo EAN não aceita remoção
    /// </summary>
    public class UpdateProductData
    {
        public ProductDataFilter? filter { get; init; }
        public string? ncmCode { get; init; }
        public string? measuredUnit { get; init; }
        public TaxStatusCodeType? cstCode { get; init; }
        public decimal? weight { get; init; }
        public string? cestCode { get; init; }
        public string? prefixEanGtin { get; init; }
    }
}

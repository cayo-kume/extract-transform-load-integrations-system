namespace Domain.Totvs.Dtos.Reads.Products.PriceTables
{
    /// <summary>
    /// preços de um produto numa tabela de preço (POST product/v2/price-tables/search, PriceTableModel); os dados do produto já estão
    /// em TotvsSkus e não são gravados aqui
    /// </summary>
    public class PriceTableProduct
    {
        public string? productCode { get; init; }
        public string? productName { get; init; }
        public string? productSku { get; init; }
        public string? referenceCode { get; init; }
        public string? colorCode { get; init; }
        public string? colorName { get; init; }
        public string? sizeName { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public string? referenceId { get; init; }
        public List<PriceTableProductPrice>? prices { get; init; }
    }
}

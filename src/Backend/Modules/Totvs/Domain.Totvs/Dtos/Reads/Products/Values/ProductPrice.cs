namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// preços de um produto (POST product/v2/prices/search, ProductPriceModel); os dados do produto já estão em TotvsSkus e não são gravados aqui
    /// </summary>
    public class ProductPrice
    {
        public string? productCode { get; init; }
        public string? productName { get; init; }
        public string? productSku { get; init; }
        public string? referenceCode { get; init; }
        public string? colorCode { get; init; }
        public string? colorName { get; init; }
        public string? sizeName { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public List<ProductPriceItem>? prices { get; init; }
    }
}

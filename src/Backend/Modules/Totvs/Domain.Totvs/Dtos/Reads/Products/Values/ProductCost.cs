namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// custos de um produto (POST product/v2/costs/search, ProductCostModel)
    /// </summary>
    public class ProductCost
    {
        public string? productCode { get; init; }
        public string? productName { get; init; }
        public string? productSku { get; init; }
        public string? referenceCode { get; init; }
        public string? colorCode { get; init; }
        public string? colorName { get; init; }
        public string? sizeName { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public List<ProductCostItem>? costs { get; init; }
    }
}

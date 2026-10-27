namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// custo de um produto numa loja (ProductCostItemModel)
    /// </summary>
    public class ProductCostItem
    {
        public string? branchCode { get; init; }
        public string? costCode { get; init; }
        public string? costName { get; init; }
        public string? cost { get; init; }
    }
}

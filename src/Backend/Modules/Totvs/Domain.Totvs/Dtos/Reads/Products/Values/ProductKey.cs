namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// código de um produto (POST product/v2/product-codes/search, ProductKeyModel); maxChangeFilterDate só vem com filter.change
    /// </summary>
    public class ProductKey
    {
        public string? productCode { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}

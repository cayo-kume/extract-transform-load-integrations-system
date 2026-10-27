namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// local do produto na loja (ProductLocationModel)
    /// </summary>
    public class ProductLocation
    {
        public string? branchCode { get; init; }
        public string? locationCode { get; init; }
        public string? description { get; init; }
    }
}

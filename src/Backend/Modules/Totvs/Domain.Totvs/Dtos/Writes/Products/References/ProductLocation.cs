namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// localização do SKU por empresa (InsertProductLocationModel)
    /// </summary>
    public class ProductLocation
    {
        public int? branchCode { get; init; }
        public string? locationCode { get; init; }
    }
}

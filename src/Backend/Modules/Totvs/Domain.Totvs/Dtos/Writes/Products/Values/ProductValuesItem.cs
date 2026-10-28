namespace Domain.Totvs.Dtos.Writes.Products.Values
{
    /// <summary>
    /// produto com os valores (ProductValueModel)
    /// </summary>
    public class ProductValuesItem
    {
        public int? productCode { get; init; }
        public List<ProductValue>? values { get; init; }
    }
}

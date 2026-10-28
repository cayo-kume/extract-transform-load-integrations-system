namespace Domain.Totvs.Dtos.Writes.Products.Classifications
{
    /// <summary>
    /// tipo e código de uma classificação de produto (ProductClassificationInsertModel)
    /// </summary>
    public class ProductClassificationItem
    {
        public int? typeCode { get; init; }
        public string? code { get; init; }
    }
}

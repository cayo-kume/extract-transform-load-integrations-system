namespace Domain.Totvs.Dtos.Writes.Products.Classifications
{
    /// <summary>
    /// produto (productCode) ou grupo (referenceId), um ou outro, com as classificações (ProductClassificationModel)
    /// </summary>
    public class ProductClassification
    {
        public int? productCode { get; init; }
        public int? referenceId { get; init; }
        public List<ProductClassificationItem>? classifications { get; init; }
    }
}

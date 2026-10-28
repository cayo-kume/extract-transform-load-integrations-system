namespace Domain.Totvs.Dtos.Writes.Products.Classifications
{
    /// <summary>
    /// corpo do POST product/v2/classification-relationship/create e /update (InsertProductClassificationCommand e
    /// UpdateProductClassificationCommand): classificações de produtos ou de grupos. O tipo 6000 (marca) é recusado (PR10)
    /// </summary>
    public class ProductClassifications
    {
        public List<ProductClassification>? products { get; init; }
    }
}

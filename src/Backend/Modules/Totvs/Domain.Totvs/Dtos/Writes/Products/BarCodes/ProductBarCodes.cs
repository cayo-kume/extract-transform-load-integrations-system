namespace Domain.Totvs.Dtos.Writes.Products.BarCodes
{
    /// <summary>
    /// produto com os códigos a incluir (ProductBarCodeModel)
    /// </summary>
    public class ProductBarCodes
    {
        public int? productCode { get; init; }
        public List<BarCodeInsert>? barCodes { get; init; }
    }
}

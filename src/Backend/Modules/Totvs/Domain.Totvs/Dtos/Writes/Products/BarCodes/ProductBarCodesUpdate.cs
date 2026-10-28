namespace Domain.Totvs.Dtos.Writes.Products.BarCodes
{
    /// <summary>
    /// produto com os códigos a alterar (ProductBarCodeUpdateModel)
    /// </summary>
    public class ProductBarCodesUpdate
    {
        public int? productCode { get; init; }
        public List<BarCodeUpdate>? barCodes { get; init; }
    }
}

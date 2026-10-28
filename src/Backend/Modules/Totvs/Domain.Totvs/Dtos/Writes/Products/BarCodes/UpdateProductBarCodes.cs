namespace Domain.Totvs.Dtos.Writes.Products.BarCodes
{
    /// <summary>
    /// corpo do POST product/v2/barcodes/update (UpdateProductBarCodeCommand): o barCode é a chave; muda a quantidade e o padrão
    /// </summary>
    public class UpdateProductBarCodes
    {
        public List<ProductBarCodesUpdate>? products { get; init; }
    }
}

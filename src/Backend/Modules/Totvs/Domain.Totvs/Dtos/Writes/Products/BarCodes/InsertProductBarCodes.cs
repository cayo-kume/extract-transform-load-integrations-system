namespace Domain.Totvs.Dtos.Writes.Products.BarCodes
{
    /// <summary>
    /// corpo do POST product/v2/barcodes (InsertProductBarCodeCommand): inclui códigos de barras nos produtos. O código padrão
    /// (isMainCode) é o productSku, chave das integrações de e-commerce
    /// </summary>
    public class InsertProductBarCodes
    {
        public List<ProductBarCodes>? products { get; init; }
    }
}

namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// código de barras do produto (BarCodeModel)
    /// </summary>
    public class Barcode
    {
        public string? code { get; init; }
        public string? quantity { get; init; }
        public string? isMainCode { get; init; }
    }
}

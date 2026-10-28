namespace Domain.Totvs.Dtos.Writes.Products.BarCodes
{
    /// <summary>
    /// código de barras alterado (BarCodeUpdateModel): aqui a quantidade é decimal
    /// </summary>
    public class BarCodeUpdate
    {
        public string? barCode { get; init; }
        public decimal? quantity { get; init; }
        public bool? isMainCode { get; init; }
    }
}

namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Item. (OrderGiftInDto)
    /// </summary>
    public class B2COrderGift
    {
        // Código interno do produto.
        public int? productCode { get; init; }
        // Código de identificação única do produto.
        public string? productSku { get; init; }
        // Quantidade solicitada.
        public decimal? quantity { get; init; }
        // Preço unitário.
        public decimal? price { get; init; }
    }
}

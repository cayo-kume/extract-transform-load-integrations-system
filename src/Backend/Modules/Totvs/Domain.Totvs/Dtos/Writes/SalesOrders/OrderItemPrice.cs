namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// item com o preço novo (UpdateOrderItemPriceModel): pelo productCode ou pelo productSku
    /// </summary>
    public class OrderItemPrice
    {
        public int? productCode { get; init; }
        public string? productSku { get; init; }
        public decimal? price { get; init; }
    }
}

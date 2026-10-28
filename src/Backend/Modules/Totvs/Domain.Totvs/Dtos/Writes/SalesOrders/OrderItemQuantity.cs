namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// item com a quantidade nova (UpdateOrderItemQuantityModel): pelo productCode ou pelo productSku
    /// </summary>
    public class OrderItemQuantity
    {
        public int? productCode { get; init; }
        public string? productSku { get; init; }
        public decimal? quantity { get; init; }
    }
}

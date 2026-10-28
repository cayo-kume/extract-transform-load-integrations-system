namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// item com a quantidade a cancelar e o motivo (CancelOrderItemModel)
    /// </summary>
    public class OrderItemCancel
    {
        public int? productCode { get; init; }
        public string? productSku { get; init; }
        public decimal? cancelQuantity { get; init; }
        public int? reasonCancellationCode { get; init; }
        public string? reasonCancellationDescription { get; init; }
    }
}

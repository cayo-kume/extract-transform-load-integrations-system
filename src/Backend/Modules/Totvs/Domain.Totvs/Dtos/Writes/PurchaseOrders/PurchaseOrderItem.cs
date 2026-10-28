namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// item do pedido de compra (PurchaseOrderItemModel): pelo productCode ou pelo productSku
    /// </summary>
    public class PurchaseOrderItem
    {
        public int? productCode { get; init; }
        public string? productSku { get; init; }
        public decimal? value { get; init; }
        public decimal? discountPercentage { get; init; }
        public decimal? ipiPercentage { get; init; }
        public decimal? quantity { get; init; }
        public DateTime? deliveryForecastDate { get; init; }
        public DateTime? deliveryDeadlineDate { get; init; }
        public List<PurchaseOrderItemValue>? itemsValues { get; init; }
        public List<PurchaseOrderExpense>? itemsExpenses { get; init; }
    }
}

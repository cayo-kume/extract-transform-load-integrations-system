namespace Domain.Totvs.Dtos.Reads.SalesOrders.BillingSuggestions
{
    /// <summary>
    /// item do pedido na sugestão de faturamento (BillingSuggestionOrderItemModel)
    /// </summary>
    public class BillingSuggestionOrderItem
    {
        public string? productCode { get; init; }
        public string? description { get; init; }
        public string? standardBarCode { get; init; }
        public string? sizeName { get; init; }
        public string? colorName { get; init; }
        public string? quantity { get; init; }
        public string? suggestedQuantity { get; init; }
        public string? pendingQuantity { get; init; }
        public string? price { get; init; }
    }
}

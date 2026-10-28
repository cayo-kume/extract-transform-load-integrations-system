namespace Domain.Totvs.Dtos.Reads.SalesOrders.BillingSuggestions
{
    /// <summary>
    /// pedido da sugestão de faturamento (BillingSuggestionOrderModel)
    /// </summary>
    public class BillingSuggestionOrder
    {
        public string? orderBranchCode { get; init; }
        public string? orderCode { get; init; }
        public string? quantity { get; init; }
        public string? suggestedQuantity { get; init; }
        public string? pendingQuantity { get; init; }
        public string? value { get; init; }
        public string? suggestedValue { get; init; }
        public string? pendingValue { get; init; }
        public List<BillingSuggestionOrderItem>? orderItems { get; init; }
    }
}

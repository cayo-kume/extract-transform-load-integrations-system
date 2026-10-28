namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/relationship-counts (RelationshipCountCommand): liga contagens ao pedido (só por loja + número)
    /// </summary>
    public class OrderCounts
    {
        public int? branchCode { get; init; }
        public int? orderCode { get; init; }
        public List<OrderCount>? counts { get; init; }
    }
}

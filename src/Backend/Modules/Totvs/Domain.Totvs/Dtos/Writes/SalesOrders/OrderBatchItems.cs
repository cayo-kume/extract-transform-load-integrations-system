namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/relationship-batch-items (RelationshipBatchItemsCommand): liga itens de lote aos itens do pedido (só
    /// por loja + número). Com isUpdateQuantity, a quantidade do pedido muda
    /// </summary>
    public class OrderBatchItems
    {
        public int? branchCode { get; init; }
        public int? orderCode { get; init; }
        public List<OrderBatchItemsProduct>? items { get; init; }
    }
}

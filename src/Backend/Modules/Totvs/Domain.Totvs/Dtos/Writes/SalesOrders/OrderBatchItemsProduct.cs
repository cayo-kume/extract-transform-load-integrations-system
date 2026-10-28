namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// produto do pedido com os itens de lote (ItemRelationshipBatchItemModel)
    /// </summary>
    public class OrderBatchItemsProduct
    {
        public int? code { get; init; }
        public bool? isUpdateQuantity { get; init; }
        public List<OrderBatchItem>? batchItems { get; init; }
    }
}

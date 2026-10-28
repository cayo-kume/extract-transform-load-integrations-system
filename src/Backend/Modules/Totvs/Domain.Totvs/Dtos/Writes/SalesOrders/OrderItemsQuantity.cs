namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/quantity-items (UpdateOrderItemQuantityCommand): a quantidade nova dos itens, sempre maior que a
    /// atual (reduzir é pelo cancelamento de itens)
    /// </summary>
    public class OrderItemsQuantity : OrderKey
    {
        public List<OrderItemQuantity>? items { get; init; }
    }
}

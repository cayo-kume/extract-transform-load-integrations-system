namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/cancel-items (CancelOrderItemCommand): cancela quantidades dos itens. cancelQuantity é delta: repetir
    /// cancela de novo, daí a trava (T5)
    /// </summary>
    public class OrderItemsCancel : OrderKey
    {
        public List<OrderItemCancel>? items { get; init; }
    }
}

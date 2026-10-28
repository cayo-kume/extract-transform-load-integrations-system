namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/items (AddOrderItemsCommand): inclui itens no pedido (valor e reserva). Não idempotente: trava (T5)
    /// </summary>
    public class OrderItemsAdd : OrderKey
    {
        public List<OrderItemAdd>? items { get; init; }
    }
}

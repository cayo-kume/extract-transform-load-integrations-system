namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/price-items (UpdateOrderItemPriceCommand): o preço novo dos itens. Efeito financeiro direto no valor do
    /// pedido; sem conferência local de preço
    /// </summary>
    public class OrderItemsPrice : OrderKey
    {
        public List<OrderItemPrice>? items { get; init; }
    }
}

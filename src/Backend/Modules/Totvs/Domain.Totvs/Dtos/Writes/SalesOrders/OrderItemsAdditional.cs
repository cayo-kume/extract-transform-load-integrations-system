namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/additional-items (UpdateOrderItemsCommand): medida de rolo e previsão de faturamento dos itens. Nulos
    /// não vão: o campo fica como está
    /// </summary>
    public class OrderItemsAdditional : OrderKey
    {
        public List<OrderItemAdditional>? items { get; init; }
    }
}

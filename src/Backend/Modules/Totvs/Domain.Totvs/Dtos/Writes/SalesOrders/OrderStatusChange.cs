namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/orders/change-status (UpdateOrderInDto): muda a situação do pedido, pelo nome (OrderStatusNames).
    /// BillingReleased libera para faturar
    /// </summary>
    public class OrderStatusChange : OrderKey
    {
        public string? statusOrder { get; init; }
        public int? reasonBlockingCode { get; init; }
        public string? reasonBlockingDescription { get; init; }
    }
}

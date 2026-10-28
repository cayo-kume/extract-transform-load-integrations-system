namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// parâmetros do DELETE sales-order/v2/items: o pedido e o produto (productCode ou productSku), lidos da query
    /// </summary>
    public class OrderItemDelete : OrderKey
    {
        public int? productCode { get; init; }
        public string? productSku { get; init; }
    }
}

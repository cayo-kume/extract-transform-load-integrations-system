namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// chave do pedido de venda nas escritas (§7.4): orderId (código único do e-commerce) ou loja + número, um ou outro. As escritas herdam
    /// daqui; com loja + número, a loja precisa ser da empresa em totvs.TotvsBranches
    /// </summary>
    public class OrderKey
    {
        public int? branchCode { get; init; }
        public int? orderCode { get; init; }
        public string? orderId { get; init; }
    }
}

namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// pedido de venda da nota (orderBranchCode é o branchCode do pedido; vínculo sem loja ou número é descartado)
    /// </summary>
    public class SalesOrder
    {
        public string? branchCode { get; init; }
        public string? orderCode { get; init; }
        public string? orderId { get; init; }
        public string? customerOrderCode { get; init; }
    }
}

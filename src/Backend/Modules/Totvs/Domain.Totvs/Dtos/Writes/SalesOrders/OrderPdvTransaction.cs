namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/pdv-transactions (OrderPdvTransactionInDto): liga o pedido a uma venda do TOTVS Moda PDV; o pedido passa a
    /// constar como faturado pelo PDV (hasPdvTransaction)
    /// </summary>
    public class OrderPdvTransaction : OrderKey
    {
        public string? transactionId { get; init; }
        public int? transactionBranchCode { get; init; }
        public int? transactionCode { get; init; }
        public DateTime? transactionDate { get; init; }
    }
}

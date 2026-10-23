namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// item de lote do produto do pedido (batchBranchCode é o branchCode do lote)
    /// </summary>
    public class BatchItem
    {
        public string? sequence { get; init; }
        public string? branchCode { get; init; }
        public string? code { get; init; }
        public string? quantity { get; init; }
    }
}

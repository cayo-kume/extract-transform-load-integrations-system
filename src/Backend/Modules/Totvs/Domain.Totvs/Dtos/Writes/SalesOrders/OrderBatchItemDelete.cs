namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// parâmetros do DELETE sales-order/v2/batch-items: o pedido, o produto e a sequência do item de lote, lidos da query
    /// </summary>
    public class OrderBatchItemDelete : OrderKey
    {
        public int? productCode { get; init; }
        public string? productSku { get; init; }
        public int? batchSequence { get; init; }
    }
}

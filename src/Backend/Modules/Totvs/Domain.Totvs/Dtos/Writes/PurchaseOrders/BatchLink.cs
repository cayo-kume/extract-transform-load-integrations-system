namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// corpo do POST purchase-order/v2/purchase-order/batch-link (PurchaseOrderBatchLinkInCommand): cria ou troca o vínculo de um item de lote
    /// com um pedido de compra (e a nota). A loja é a do lote
    /// </summary>
    public class BatchLink
    {
        public int? branchCode { get; init; }
        public int? batchCode { get; init; }
        public int? sequence { get; init; }
        public string? batchItemBarCode { get; init; }
        public int? orderCode { get; init; }
        public InvoiceIdentification? invoice { get; init; }
    }
}

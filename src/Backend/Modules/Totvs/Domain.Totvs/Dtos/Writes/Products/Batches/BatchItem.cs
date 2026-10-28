namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// item do lote (BatchItemModel): balance, quantityIssued e quantityCanceled são de saída e não vão
    /// </summary>
    public class BatchItem
    {
        public int? batchItemSequential { get; init; }
        public BatchItemStatusType? status { get; init; }
        public string? itemProvider { get; init; }
        public string? itemClient { get; init; }
        public decimal? quantityItem { get; init; }
        public decimal? quantityTare { get; init; }
        // peso bruto (grafia da API)
        public decimal? quantityWeigth { get; init; }
        public decimal? quantityMeters { get; init; }
        public string? nuance { get; init; }
        public decimal? quantityCone { get; init; }
        public string? locationBatch { get; init; }
        // pedido de venda relacionado: empresa e pedido juntos
        public int? branchSalesOrder { get; init; }
        public int? salesOrder { get; init; }
        public int? typeTare { get; init; }
        public List<BatchAdditionalField>? additionalFields { get; init; }
        public BatchInvoiceInspection? invoice { get; init; }
        public string? conditioningType { get; init; }
        public decimal? quantityYield { get; init; }
    }
}

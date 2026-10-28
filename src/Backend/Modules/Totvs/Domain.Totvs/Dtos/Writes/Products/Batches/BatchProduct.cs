namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// capa do lote (BatchProductModel); a loja precisa ser da empresa
    /// </summary>
    public class BatchProduct
    {
        public int? branchCode { get; init; }
        public int? productCode { get; init; }
        public int? batchCode { get; init; }
        public int? balanceCode { get; init; }
        public string? batchProvider { get; init; }
        public string? batchClient { get; init; }
        public DateTime? batchManufacturingDate { get; init; }
        public DateTime? batchExpirationDate { get; init; }
        public BatchType? batchType { get; init; }
        public List<BatchItem>? batchItem { get; init; }
    }
}

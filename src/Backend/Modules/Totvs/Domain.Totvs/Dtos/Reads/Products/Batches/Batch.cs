namespace Domain.Totvs.Dtos.Reads.Products.Batches
{
    /// <summary>
    /// lote de um produto numa loja (POST product/v2/batch/search, SimpleBatchProductModel)
    /// </summary>
    public class Batch
    {
        public string? branchCode { get; init; }
        public string? productCode { get; init; }
        public string? batchCode { get; init; }
        public string? batchProvider { get; init; }
        public string? batchClient { get; init; }
        public string? batchManufacturingDate { get; init; }
        public string? batchExpirationDate { get; init; }
        public string? batchType { get; init; }
        public List<BatchItem>? batchItem { get; init; }
    }
}

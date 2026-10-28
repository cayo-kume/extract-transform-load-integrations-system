namespace Domain.Totvs.Dtos.Reads.Products.Batches
{
    /// <summary>
    /// item do lote (SimpleBatchItemModel); quantityWeigth na grafia da API
    /// </summary>
    public class BatchItem
    {
        public string? batchItemSequential { get; init; }
        public string? balance { get; init; }
        public string? quantityItem { get; init; }
        public string? quantityIssued { get; init; }
        public string? quantityTare { get; init; }
        public string? quantityWeigth { get; init; }
        public string? quantityMeters { get; init; }
        public string? quantityCanceled { get; init; }
        public string? quantityYield { get; init; }
        public string? itemProvider { get; init; }
        public string? itemClient { get; init; }
        public string? locationBatch { get; init; }
        public string? status { get; init; }
        public string? conditioningType { get; init; }
        public string? nuance { get; init; }
        public List<BatchItemAdditionalField>? additionalFields { get; init; }
    }
}

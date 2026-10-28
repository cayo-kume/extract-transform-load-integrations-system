namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// dados alterados do item de lote (UpdateBatchInfoModel)
    /// </summary>
    public class UpdateBatchInfoData
    {
        public string? nuance { get; init; }
        public string? location { get; init; }
        public string? itemClient { get; init; }
        public string? itemProvider { get; init; }
        public int? branchSalesOrder { get; init; }
        public int? salesOrder { get; init; }
        public List<BatchAdditionalField>? additionalFields { get; init; }
        public string? conditioningType { get; init; }
        public decimal? quantityItem { get; init; }
        public decimal? quantityYield { get; init; }
        public decimal? quantityMeters { get; init; }
        public decimal? supplierWidth { get; init; }
        public decimal? realWidth { get; init; }
        public int? grammage { get; init; }
    }
}

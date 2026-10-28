namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// item a alterar (UpdateBatchItemModel): por loja + lote + sequência ou pelo lote do cliente, um ou outro
    /// </summary>
    public class UpdateBatchItem
    {
        public int? branchCode { get; init; }
        public int? batchCode { get; init; }
        public int? sequence { get; init; }
        public string? itemClient { get; init; }
        public UpdateBatchInfoData? dataChanged { get; init; }
    }
}

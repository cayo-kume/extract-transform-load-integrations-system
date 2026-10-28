namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// corpo do PUT product/v2/batch-info (UpdateBatchInfoCommand): altera dados de itens de lote
    /// </summary>
    public class UpdateBatchInfo
    {
        public List<UpdateBatchItem>? batchItems { get; init; }
    }
}

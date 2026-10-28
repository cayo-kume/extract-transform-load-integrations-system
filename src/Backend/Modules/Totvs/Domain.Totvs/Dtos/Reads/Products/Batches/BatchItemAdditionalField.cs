namespace Domain.Totvs.Dtos.Reads.Products.Batches
{
    /// <summary>
    /// campo adicional do item de lote (BatchItemAdditionalFieldModel)
    /// </summary>
    public class BatchItemAdditionalField
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? value { get; init; }
    }
}

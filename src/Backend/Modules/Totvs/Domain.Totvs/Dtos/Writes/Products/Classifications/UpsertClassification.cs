namespace Domain.Totvs.Dtos.Writes.Products.Classifications
{
    /// <summary>
    /// classificação do tipo (UpsertClassificationModel); o swagger não dá tamanho: valem 10, 40 e 100 dos outros modelos
    /// </summary>
    public class UpsertClassification
    {
        public string? code { get; init; }
        public string? description { get; init; }
        public string? auxiliaryDescription { get; init; }
    }
}

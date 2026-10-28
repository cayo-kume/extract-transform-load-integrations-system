namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// campo adicional do item de lote (BatchAdditionalFieldModel)
    /// </summary>
    public class BatchAdditionalField
    {
        public int? code { get; init; }
        // JsonElement quando vem do controller: vai à Totvs como veio (texto, número ou data DD/MM/YYYY)
        [Newtonsoft.Json.JsonConverter(typeof(RawJsonConverter))]
        public object? value { get; init; }
    }
}

namespace Domain.Totvs.Dtos.Writes.Products.Batches
{
    /// <summary>
    /// campo da inspeção (BatchInspectionFieldModel)
    /// </summary>
    public class BatchInspectionField
    {
        public int? field { get; init; }
        // JsonElement quando vem do controller: vai à Totvs como veio (texto, número ou data DD/MM/YYYY)
        [Newtonsoft.Json.JsonConverter(typeof(RawJsonConverter))]
        public object? inspectionValue { get; init; }
    }
}

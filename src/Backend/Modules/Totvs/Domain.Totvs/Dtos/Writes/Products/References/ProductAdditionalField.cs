namespace Domain.Totvs.Dtos.Writes.Products.References
{
    /// <summary>
    /// campo adicional do SKU (InsertProductAdditionalFieldModel)
    /// </summary>
    public class ProductAdditionalField
    {
        public int? code { get; init; }
        // JsonElement quando vem do controller: vai à Totvs como veio (texto, número ou data DD/MM/YYYY)
        [Newtonsoft.Json.JsonConverter(typeof(RawJsonConverter))]
        public object? value { get; init; }
    }
}

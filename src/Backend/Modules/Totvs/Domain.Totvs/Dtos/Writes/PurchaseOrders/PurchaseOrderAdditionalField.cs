namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// campo adicional do pedido de compra (PurchaseOrderAdditionalFieldModel)
    /// </summary>
    public class PurchaseOrderAdditionalField
    {
        public int? code { get; init; }
        // JsonElement quando vem do controller: vai à Totvs como veio (texto, número ou data DD/MM/YYYY)
        [Newtonsoft.Json.JsonConverter(typeof(RawJsonConverter))]
        public object? value { get; init; }
    }
}

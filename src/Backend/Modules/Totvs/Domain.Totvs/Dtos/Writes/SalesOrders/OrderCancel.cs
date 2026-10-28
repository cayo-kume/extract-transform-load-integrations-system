namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/orders/cancel (CancelOrderInDto): cancela o pedido inteiro, sem volta pela API. A descrição tem R maiúsculo no
    /// swagger, e o nome vai assim nos dois serializadores
    /// </summary>
    public class OrderCancel : OrderKey
    {
        public int? reasonCancellationCode { get; init; }
        [Newtonsoft.Json.JsonProperty("ReasonCancellationDescription")]
        [System.Text.Json.Serialization.JsonPropertyName("ReasonCancellationDescription")]
        public string? ReasonCancellationDescription { get; init; }
    }
}

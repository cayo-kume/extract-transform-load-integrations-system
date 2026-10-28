namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/additional-order (OrderAdditionalInDto): dados adicionais do pedido (rastreio, estágio do e-commerce,
    /// entrega antecipada). Nulos não vão: o campo fica como está
    /// </summary>
    public class OrderAdditional : OrderKey
    {
        public string? shippingService { get; init; }   // até 10
        public string? trackingId { get; init; }        // até 40
        public string? ecommerceStage { get; init; }    // até 40
        public int? omniSiteId { get; init; }
        public int? compensationDays { get; init; }     // até 3 dígitos
        public int? anticipationDay { get; init; }      // até 3 dígitos

        // aceita entrega antecipada: letra N ou S (EarlydeliveryType), não número (SO11)
        public string? entryType { get; init; }

        public ExperienceType? experienceType { get; init; }
    }
}

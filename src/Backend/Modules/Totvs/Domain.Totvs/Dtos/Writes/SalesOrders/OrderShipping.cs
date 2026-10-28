namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/shipping-order (ManageOrderShippingCommand): transportadora, redespacho, frete e endereço de entrega.
    /// Muda o total do pedido (frete). Nulos não vão: o campo fica como está
    /// </summary>
    public class OrderShipping : OrderKey
    {
        // transportadora: código ou CPF/CNPJ, um ou outro; com ela, freightType é obrigatório
        public int? shippingCompanyCode { get; init; }
        public string? shippingCompanyCpfCnpj { get; init; }

        // redespacho: código ou CPF/CNPJ, um ou outro; com ele, redispatchingFreightType (1 emitente, 2 destinatário) é obrigatório
        public int? redispatchingShippingCompanyCode { get; init; }
        public string? redispatchingShippingCompanyCpfCnpj { get; init; }
        public int? redispatchingFreightType { get; init; }

        public FreightType? freightType { get; init; }
        public decimal? freightPercentage { get; init; }   // 3 inteiros e 6 decimais
        public decimal? freightValue { get; init; }        // 12 inteiros e 2 decimais

        public ShippingAddress? shippingAddress { get; init; }
    }
}

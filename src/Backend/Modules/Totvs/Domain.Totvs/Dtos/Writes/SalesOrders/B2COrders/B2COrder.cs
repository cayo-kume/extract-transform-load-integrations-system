namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// corpo do POST sales-order/v2/b2c-orders (OrderInDto): inclui um pedido de venda (estoque reservado, financeiro do pagamento); a resposta traz
    /// loja, número e orderId. O orderId é o controle de duplicidade (SO5). Cliente, representante, intermediário, guia, vendedor, transportadora e
    /// redespacho vão pelo código ou pelo CPF/CNPJ, um ou outro
    /// </summary>
    public class B2COrder
    {
        // Identificador único do pedido.
        public string? orderId { get; init; }
        // Código da empresa.
        public int? branchCode { get; init; }
        // Identificador do pedido utilizado pelo cliente.
        public string? customerOrderCode { get; init; }
        // Código de integração com terceiros.
        public string? integrationCode { get; init; }
        // Data de lançamento.
        public DateTime? orderDate { get; init; }
        // Código do cliente.
        public int? customerCode { get; init; }
        // CPF ou CNPJ do cliente.
        public string? customerCpfCnpj { get; init; }
        // Código do representante.
        public int? representativeCode { get; init; }
        // CPF ou CNPJ do representante.
        public string? representativeCpfCnpj { get; init; }
        // Código do intermediário.
        public int? intermediaryCode { get; init; }
        // CNPJ do intermediário.
        public string? intermediaryCnpj { get; init; }
        // Código do guia.
        public int? guideCode { get; init; }
        // CPF ou CNPJ do guia.
        public string? guideCpfCnpj { get; init; }
        // Código do vendedor.
        public int? sellerCode { get; init; }
        // CPF ou CNPJ do vendedor.
        public string? sellerCpfCnpj { get; init; }
        // Código da operação.
        public int? operationCode { get; init; }
        // Código da condição de pagamento.
        public int? paymentConditionCode { get; init; }
        // Data base de pagamento.
        public DateTime? paymentBaseDate { get; init; }
        // Código da prioridade.
        public int? priorityCode { get; init; }
        // Data da chegada do pedido.
        public DateTime? arrivalDate { get; init; }
        // Código da transportadora.
        public int? shippingCompanyCode { get; init; }
        // CPF ou CNPJ da transportadora.
        public string? shippingCompanyCpfCnpj { get; init; }
        // Código da transportadora de redespacho.
        public int? redispatchingShippingCompanyCode { get; init; }
        // CPF ou CNPJ da transportadora de redespacho.
        public string? redispatchingShippingCompanyCpfCnpj { get; init; }
        // Tipo de frete de redespacho.
        public int? redispatchingFreightType { get; init; }
        // Data de previsão de faturamento.
        public DateTime? billingForecastDate { get; init; }
        // Previsão da data limite de entrega.
        public DateTime? billingLimitDate { get; init; }
        public FreightType? freightType { get; init; }
        // Percentual de frete.
        public decimal? freightPercentage { get; init; }
        // Valor de frete.
        public decimal? freightValue { get; init; }
        // Quantidade de volume.
        public decimal? packageNumber { get; init; }
        // Peso bruto.
        public decimal? weight { get; init; }
        public B2COrderStatus? statusOrder { get; init; }
        // Código do motivo de bloqueio.
        public int? reasonBlockingCode { get; init; }
        // Número do siteid.
        public int? omniSiteId { get; init; }
        // Identificador do tipo de serviço de transporte.
        public string? shippingService { get; init; }
        //  Enum pelo nome (Ecommerce, ShipFromStore, ClickAndCollect, PickupStore, InfiniteShelf)
        public string? experienceType { get; init; }
        public B2COrderChargeType? chargeType { get; init; }
        // Código da tabela de preço.
        public int? priceTableCode { get; init; }
        // Valor de despesa acessória.
        public decimal? additionalValue { get; init; }
        // Código do lote de pedido.
        public string? batchOrder { get; init; }
        // Valor total líquido do pedido.
        public decimal? totalAmountOrder { get; init; }
        // Responsável por receber o pedido na entrega.
        public string? receivingPerson { get; init; }
        // Medida de rolo.
        public int? rollMeasureCode { get; init; }
        // Código do cliente comprador.
        public int? buyerCustomerCode { get; init; }
        // CPF ou CNPJ do cliente comprador.
        public string? buyerCustomerCpfCnpj { get; init; }
        // Lista de descontos.
        public List<B2COrderDiscount>? discounts { get; init; }
        // Lista de itens.
        public List<B2COrderItem>? items { get; init; }
        // Lista de classificações.
        public List<B2COrderClassification>? classifications { get; init; }
        // Lista de pagamentos.
        public List<B2COrderPayment>? payments { get; init; }
        // Lista de observações manuais.
        public List<B2COrderObservation>? observations { get; init; }
        // Lista de observações de faturamento.
        public List<B2COrderInvoiceObservation>? invoiceObservations { get; init; }
        // Lista de observações de representante.
        public List<B2COrderRepresentativeObservation>? representativeObservations { get; init; }
        public B2COrderShippingAddress? shippingAddress { get; init; }
        // Lista de brinde.
        public List<B2COrderGift>? gifts { get; init; }
        // Lista de comissionado.
        public List<B2COrderCommissioned>? commissioneds { get; init; }
        // Lista de operação triangular.
        public List<B2COrderOutsourced>? outsourceds { get; init; }
        // Lista de Limitadores de percentual sugerido por data de faturamento.
        public List<B2COrderBillingPercentageLimitDate>? billingPercentageLimitDates { get; init; }
    }
}

namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// pedido de venda do POST sales-order/v2/orders/search
    /// </summary>
    public class SaleOrder
    {
        public string? branchCode { get; init; }
        public string? orderCode { get; init; }
        public string? orderId { get; init; }
        public string? customerOrderCode { get; init; }
        public string? insertDate { get; init; }
        public string? integrationCode { get; init; }
        public string? guideCode { get; init; }
        public string? guideCpfCnpj { get; init; }
        public string? sellerCode { get; init; }
        public string? sellerCpfCnpj { get; init; }
        public string? orderDate { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public string? arrivalDate { get; init; }
        public string? customerCode { get; init; }
        public string? customerCpfCnpj { get; init; }
        public string? customerName { get; init; }
        public string? representativeCode { get; init; }
        public string? representativeCpfCnpj { get; init; }
        public string? representativeName { get; init; }
        public string? operationCode { get; init; }
        public string? operationName { get; init; }
        public string? paymentConditionCode { get; init; }
        public string? paymentConditionName { get; init; }
        public string? quantity { get; init; }
        public string? grossValue { get; init; }
        public string? discountValue { get; init; }
        public string? netValue { get; init; }
        public string? freightType { get; init; }
        public string? freightValue { get; init; }
        public string? shippingCompanyCode { get; init; }
        public string? shippingCompanyCpfCnpj { get; init; }
        public string? shippingCompanyName { get; init; }
        public string? redispatchingFreightType { get; init; }
        public string? redispatchingShippingCompanyCode { get; init; }
        public string? redispatchingShippingCompanyCpfCnpj { get; init; }
        public string? redispatchingShippingCompanyName { get; init; }
        public string? totalAmountOrder { get; init; }
        public string? statusOrder { get; init; }
        public string? reasonBlockingCode { get; init; }
        public string? reasonBlockingDescription { get; init; }
        public string? reasonCancellationCode { get; init; }
        public string? reasonCancellationDescription { get; init; }
        public string? omniSiteId { get; init; }
        public string? additionalValue { get; init; }
        public string? experienceType { get; init; }
        public string? hasPdvTransaction { get; init; }
        public string? hasFinancialProcessed { get; init; }
        public string? priceTableCode { get; init; }
        public string? shippingService { get; init; }
        public string? shippingServiceName { get; init; }
        public string? cancellationLimitDate { get; init; }
        public string? billingForecastDate { get; init; }
        public string? billingLimitDate { get; init; }
        public string? lastBillingDate { get; init; }
        public string? paymentBaseDate { get; init; }
        public string? batchOrder { get; init; }
        public string? rollMeasureCode { get; init; }
        public string? rollMeasureDescription { get; init; }
        public string? ecommerceStage { get; init; }

        public ShippingAddress? shippingAddress { get; init; }
        public List<Product>? items { get; init; }
        public List<Invoice>? invoices { get; init; }
        public List<Observation>? observations { get; init; }
        public List<InvoiceObservation>? invoiceObservations { get; init; }
        public List<RepresentativeObservation>? representativeObservations { get; init; }
        public List<Classification>? classifications { get; init; }
        public List<Discount>? discounts { get; init; }
        public List<Commissioned>? commissioneds { get; init; }
        public List<Count>? counts { get; init; }
    }
}

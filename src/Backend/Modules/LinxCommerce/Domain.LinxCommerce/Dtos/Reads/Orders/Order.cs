namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// Order do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class Order
    {
        public string? OrderID { get; init; }
        public string? OrderNumber { get; init; }
        public string? MarketPlaceBrand { get; init; }
        public string? OriginalOrderID { get; init; }
        public string? WebSiteID { get; init; }
        public string? WebSiteIntegrationID { get; init; }
        public string? CustomerID { get; init; }
        public string? ShopperTicketID { get; init; }
        public string? ItemsQty { get; init; }
        public string? ItemsCount { get; init; }
        public string? TaxAmount { get; init; }
        public string? DeliveryAmount { get; init; }
        public string? DiscountAmount { get; init; }
        public string? PaymentTaxAmount { get; init; }
        public string? SubTotal { get; init; }
        public string? Total { get; init; }
        public string? TotalDue { get; init; }
        public string? TotalPaid { get; init; }
        public string? TotalRefunded { get; init; }
        public string? PaymentDate { get; init; }
        public string? PaymentStatus { get; init; }
        public string? ShipmentDate { get; init; }
        public string? ShipmentStatus { get; init; }
        public string? GlobalStatus { get; init; }
        public string? DeliveryPostalCode { get; init; }
        public string? CreatedChannel { get; init; }
        public string? TrafficSourceID { get; init; }
        public string? OrderStatusID { get; init; }
        public string? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public string? ModifiedDate { get; init; }
        public string? ModifiedBy { get; init; }
        public string? Remarks { get; init; }
        public string? SellerCommissionAmount { get; init; }
        public string? CommissionAmount { get; init; }
        public string? OrderGroupID { get; init; }
        public string? OrderGroupNumber { get; init; }
        public string? HasConflicts { get; init; }
        public string? AcquiredDate { get; init; }
        public string? HasHubOrderWithoutShipmentConflict { get; init; }
        public string? CustomerType { get; init; }
        public string? CancelledDate { get; init; }
        public string? WebSiteName { get; init; }
        public string? CustomerName { get; init; }
        public string? CustomerEmail { get; init; }
        public string? CustomerGender { get; init; }
        public string? CustomerBirthDate { get; init; }
        public string? CustomerPhone { get; init; }
        public string? CustomerCPF { get; init; }
        public string? CustomerCNPJ { get; init; }
        public string? CustomerTradingName { get; init; }
        public string? CustomerSiteTaxPayer { get; init; }

        public OrderSeller? Seller { get; init; }
        public OrderType? OrderType { get; init; }
        public OrderMultiSiteTenant? MultiSiteTenant { get; init; }
        public OrderSalesRepresentative? SalesRepresentative { get; init; }
        public List<OrderItem>? Items { get; init; }
        public List<OrderTag>? Tags { get; init; }
        public List<OrderAddress>? Addresses { get; init; }
        public List<OrderPaymentMethod>? PaymentMethods { get; init; }
        public List<OrderDeliveryMethod>? DeliveryMethods { get; init; }
        public List<OrderDiscount>? Discounts { get; init; }
        public List<OrderShipment>? Shipments { get; init; }
        public OrderInvoice? OrderInvoice { get; init; }
        public List<OrderProperty>? Properties { get; init; }
    }
}

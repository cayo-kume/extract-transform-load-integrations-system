namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderItem do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderItem
    {
        public string? OrderItemID { get; init; }
        public string? ParentItemID { get; init; }
        public string? ProductID { get; init; }
        public string? SkuID { get; init; }
        public string? SKU { get; init; }
        public string? SellerSKU { get; init; }
        public string? WebSiteID { get; init; }
        public string? CatalogID { get; init; }
        public string? PriceListID { get; init; }
        public string? WareHouseID { get; init; }
        public string? WarehouseIntegrationID { get; init; }
        public string? Qty { get; init; }
        public string? Price { get; init; }
        public string? DiscountAmount { get; init; }
        public string? TaxationAmount { get; init; }
        public string? SubtotalAdjustment { get; init; }
        public string? Subtotal { get; init; }
        public string? Total { get; init; }
        public string? IsFreeShipping { get; init; }
        public string? IsDeleted { get; init; }
        public string? Status { get; init; }
        public string? ProductIntegrationID { get; init; }
        public string? SKUIntegrationID { get; init; }
        public string? CatalogItemType { get; init; }
        public string? IsFreeOffer { get; init; }
        public string? IsGiftWrapping { get; init; }
        public string? IsService { get; init; }
        public string? SpecialType { get; init; }
        public string? BundlePriceType { get; init; }
        public string? BundleKitDiscount { get; init; }
        public string? BundleKitDiscountValue { get; init; }
        public string? InStockHandlingDays { get; init; }
        public string? OutStockHandlingDays { get; init; }
        public string? ProductName { get; init; }
        public string? SkuName { get; init; }
        public string? IsDeliverable { get; init; }
        public string? Weight { get; init; }
        public string? Depth { get; init; }
        public string? Height { get; init; }
        public string? Width { get; init; }
        public string? FulfillmentID { get; init; }
        public string? UPC { get; init; }
    }
}

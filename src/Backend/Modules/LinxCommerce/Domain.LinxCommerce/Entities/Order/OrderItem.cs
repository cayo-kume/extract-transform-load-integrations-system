namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderItem
    {
        public DateTime? lastupdateon { get; set; }
        public Int64? OrderItemID { get; set; }
        public Guid? OrderID { get; set; }
        public Int32? ParentItemID { get; set; }
        public Int64? ProductID { get; set; }
        public Int32? SkuID { get; set; }
        public string? SKU { get; set; }
        public string? SellerSKU { get; set; }
        public Int32? WebSiteID { get; set; }
        public Int32? CatalogID { get; set; }
        public Int32? PriceListID { get; set; }
        public Int32? WareHouseID { get; set; }
        public string? WarehouseIntegrationID { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TaxationAmount { get; set; }
        public decimal? SubtotalAdjustment { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Total { get; set; }
        public bool? IsFreeShipping { get; set; }
        public bool? IsDeleted { get; set; }
        public string? Status { get; set; }
        public string? ProductIntegrationID { get; set; }
        public string? SKUIntegrationID { get; set; }
        public Int32? CatalogItemType { get; set; }
        public bool? IsFreeOffer { get; set; }
        public bool? IsGiftWrapping { get; set; }
        public bool? IsService { get; set; }
        public Int32? SpecialType { get; set; }
        public string? BundlePriceType { get; set; }
        public string? BundleKitDiscount { get; set; }
        public decimal? BundleKitDiscountValue { get; set; }
        public Int32? InStockHandlingDays { get; set; }
        public Int32? OutStockHandlingDays { get; set; }
        public string? ProductName { get; set; }
        public string? SkuName { get; set; }
        public bool? IsDeliverable { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Depth { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public Guid? FulfillmentID { get; set; }
        public string? UPC { get; set; }

        public OrderItem() { }

        public OrderItem(OrderItem orderItem)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.OrderItemID = orderItem.OrderItemID;
            this.OrderID = orderItem.OrderID;
            this.ParentItemID = orderItem.ParentItemID;
            this.ProductID = orderItem.ProductID;
            this.SkuID = orderItem.SkuID;
            this.SKU = orderItem.SKU;
            this.SellerSKU = orderItem.SellerSKU;
            this.WebSiteID = orderItem.WebSiteID;
            this.CatalogID = orderItem.CatalogID;
            this.PriceListID = orderItem.PriceListID;
            this.WareHouseID = orderItem.WareHouseID;
            this.WarehouseIntegrationID = orderItem.WarehouseIntegrationID;
            this.Qty = orderItem.Qty;
            this.Price = orderItem.Price;
            this.DiscountAmount = orderItem.DiscountAmount;
            this.TaxationAmount = orderItem.TaxationAmount;
            this.SubtotalAdjustment = orderItem.SubtotalAdjustment;
            this.Subtotal = orderItem.Subtotal;
            this.Total = orderItem.Total;
            this.IsFreeShipping = orderItem.IsFreeShipping;
            this.IsDeleted = orderItem.IsDeleted;
            this.Status = orderItem.Status;
            this.ProductIntegrationID = orderItem.ProductIntegrationID;
            this.SKUIntegrationID = orderItem.SKUIntegrationID;
            this.CatalogItemType = orderItem.CatalogItemType;
            this.IsFreeOffer = orderItem.IsFreeOffer;
            this.IsGiftWrapping = orderItem.IsGiftWrapping;
            this.IsService = orderItem.IsService;
            this.SpecialType = orderItem.SpecialType;
            this.BundlePriceType = orderItem.BundlePriceType;
            this.BundleKitDiscount = orderItem.BundleKitDiscount;
            this.BundleKitDiscountValue = orderItem.BundleKitDiscountValue;
            this.InStockHandlingDays = orderItem.InStockHandlingDays;
            this.OutStockHandlingDays = orderItem.OutStockHandlingDays;
            this.ProductName = orderItem.ProductName;
            this.SkuName = orderItem.SkuName;
            this.IsDeliverable = orderItem.IsDeliverable;
            this.Weight = orderItem.Weight;
            this.Depth = orderItem.Depth;
            this.Height = orderItem.Height;
            this.Width = orderItem.Width;
            this.FulfillmentID = orderItem.FulfillmentID;
            this.UPC = orderItem.UPC;
        }
    }
}

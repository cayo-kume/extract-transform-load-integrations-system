using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// item do pedido, tabela linxcommerce.LinxCommerceOrdersItems
    /// </summary>
    public class OrderItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public long OrderItemID { get; private set; }
        public long? ParentItemID { get; private set; }
        public long? ProductID { get; private set; }
        public long? SkuID { get; private set; }
        public string SKU { get; private set; } = String.Empty;
        public string SellerSKU { get; private set; } = String.Empty;
        public int? WebSiteID { get; private set; }
        public int? CatalogID { get; private set; }
        public int? PriceListID { get; private set; }
        public int? WareHouseID { get; private set; }
        public string WarehouseIntegrationID { get; private set; } = String.Empty;
        public decimal? Qty { get; private set; }
        public decimal? Price { get; private set; }
        public decimal? DiscountAmount { get; private set; }
        public decimal? TaxationAmount { get; private set; }
        public decimal? SubtotalAdjustment { get; private set; }
        public decimal? Subtotal { get; private set; }
        public decimal? Total { get; private set; }
        public bool? IsFreeShipping { get; private set; }
        public bool? IsDeleted { get; private set; }
        public string Status { get; private set; } = String.Empty;
        public string ProductIntegrationID { get; private set; } = String.Empty;
        public string SKUIntegrationID { get; private set; } = String.Empty;
        public int? CatalogItemType { get; private set; }
        public bool? IsFreeOffer { get; private set; }
        public bool? IsGiftWrapping { get; private set; }
        public bool? IsService { get; private set; }
        public int? SpecialType { get; private set; }
        public string BundlePriceType { get; private set; } = String.Empty;
        public string BundleKitDiscount { get; private set; } = String.Empty;
        public decimal? BundleKitDiscountValue { get; private set; }
        public int? InStockHandlingDays { get; private set; }
        public int? OutStockHandlingDays { get; private set; }
        public string ProductName { get; private set; } = String.Empty;
        public string SkuName { get; private set; } = String.Empty;
        public bool? IsDeliverable { get; private set; }
        public decimal? Weight { get; private set; }
        public decimal? Depth { get; private set; }
        public decimal? Height { get; private set; }
        public decimal? Width { get; private set; }
        public Guid? FulfillmentID { get; private set; }
        public string UPC { get; private set; } = String.Empty;

        public OrderItem() { }

        public OrderItem(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Dtos.Reads.Orders.OrderItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;

            OrderItemID = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.OrderItemID);
            ParentItemID = LinxCommerceConverters.ToNullableInt64(dto.ParentItemID);
            ProductID = LinxCommerceConverters.ToNullableInt64(dto.ProductID);
            SkuID = LinxCommerceConverters.ToNullableInt64(dto.SkuID);
            SKU = ConverterValidatorsExtensions.StringLengthValidation(dto.SKU, 100);
            SellerSKU = ConverterValidatorsExtensions.StringLengthValidation(dto.SellerSKU, 100);
            WebSiteID = NullableConverters.ToNullableInt32(dto.WebSiteID);
            CatalogID = NullableConverters.ToNullableInt32(dto.CatalogID);
            PriceListID = NullableConverters.ToNullableInt32(dto.PriceListID);
            WareHouseID = NullableConverters.ToNullableInt32(dto.WareHouseID);
            WarehouseIntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.WarehouseIntegrationID, 100);
            Qty = NullableConverters.ToNullableDecimal(dto.Qty);
            Price = NullableConverters.ToNullableDecimal(dto.Price);
            DiscountAmount = NullableConverters.ToNullableDecimal(dto.DiscountAmount);
            TaxationAmount = NullableConverters.ToNullableDecimal(dto.TaxationAmount);
            SubtotalAdjustment = NullableConverters.ToNullableDecimal(dto.SubtotalAdjustment);
            Subtotal = NullableConverters.ToNullableDecimal(dto.Subtotal);
            Total = NullableConverters.ToNullableDecimal(dto.Total);
            IsFreeShipping = NullableConverters.ToNullableBoolean(dto.IsFreeShipping);
            IsDeleted = NullableConverters.ToNullableBoolean(dto.IsDeleted);
            Status = ConverterValidatorsExtensions.StringLengthValidation(dto.Status, 50);
            ProductIntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.ProductIntegrationID, 100);
            SKUIntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.SKUIntegrationID, 100);
            CatalogItemType = NullableConverters.ToNullableInt32(dto.CatalogItemType);
            IsFreeOffer = NullableConverters.ToNullableBoolean(dto.IsFreeOffer);
            IsGiftWrapping = NullableConverters.ToNullableBoolean(dto.IsGiftWrapping);
            IsService = NullableConverters.ToNullableBoolean(dto.IsService);
            SpecialType = NullableConverters.ToNullableInt32(dto.SpecialType);
            BundlePriceType = ConverterValidatorsExtensions.StringLengthValidation(dto.BundlePriceType, 50);
            BundleKitDiscount = ConverterValidatorsExtensions.StringLengthValidation(dto.BundleKitDiscount, 50);
            BundleKitDiscountValue = NullableConverters.ToNullableDecimal(dto.BundleKitDiscountValue);
            InStockHandlingDays = NullableConverters.ToNullableInt32(dto.InStockHandlingDays);
            OutStockHandlingDays = NullableConverters.ToNullableInt32(dto.OutStockHandlingDays);
            ProductName = ConverterValidatorsExtensions.StringLengthValidation(dto.ProductName, 250);
            SkuName = ConverterValidatorsExtensions.StringLengthValidation(dto.SkuName, 250);
            IsDeliverable = NullableConverters.ToNullableBoolean(dto.IsDeliverable);
            Weight = NullableConverters.ToNullableDecimal(dto.Weight);
            Depth = NullableConverters.ToNullableDecimal(dto.Depth);
            Height = NullableConverters.ToNullableDecimal(dto.Height);
            Width = NullableConverters.ToNullableDecimal(dto.Width);
            FulfillmentID = LinxCommerceConverters.ToNullableGuid(dto.FulfillmentID);
            UPC = ConverterValidatorsExtensions.StringLengthValidation(dto.UPC, 50);
        }
    }
}

using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// pedido (POST Sales/API.svc/web/GetOrder), tabela linxcommerce.LinxCommerceOrders
    /// </summary>
    public class Order
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public string OrderNumber { get; private set; } = String.Empty;
        public string MarketPlaceBrand { get; private set; } = String.Empty;
        public Guid? OriginalOrderID { get; private set; }
        public int? WebSiteID { get; private set; }
        public string WebSiteIntegrationID { get; private set; } = String.Empty;
        public string WebSiteName { get; private set; } = String.Empty;
        public int? CustomerID { get; private set; }
        public Guid? ShopperTicketID { get; private set; }
        public decimal? ItemsQty { get; private set; }
        public int? ItemsCount { get; private set; }
        public decimal? TaxAmount { get; private set; }
        public decimal? DeliveryAmount { get; private set; }
        public decimal? DiscountAmount { get; private set; }
        public decimal? PaymentTaxAmount { get; private set; }
        public decimal? SubTotal { get; private set; }
        public decimal? Total { get; private set; }
        public decimal? TotalDue { get; private set; }
        public decimal? TotalPaid { get; private set; }
        public decimal? TotalRefunded { get; private set; }
        public DateTimeOffset? PaymentDate { get; private set; }
        public string PaymentStatus { get; private set; } = String.Empty;
        public DateTimeOffset? ShipmentDate { get; private set; }
        public string ShipmentStatus { get; private set; } = String.Empty;
        public string GlobalStatus { get; private set; } = String.Empty;
        public string DeliveryPostalCode { get; private set; } = String.Empty;
        public string CreatedChannel { get; private set; } = String.Empty;
        public int? TrafficSourceID { get; private set; }
        public int? OrderStatusID { get; private set; }
        public DateTimeOffset? CreatedDate { get; private set; }
        public string CreatedBy { get; private set; } = String.Empty;
        public DateTimeOffset? ModifiedDate { get; private set; }
        public string ModifiedBy { get; private set; } = String.Empty;
        public string Remarks { get; private set; } = String.Empty;
        public decimal? SellerCommissionAmount { get; private set; }
        public decimal? CommissionAmount { get; private set; }
        public Guid? OrderGroupID { get; private set; }
        public string OrderGroupNumber { get; private set; } = String.Empty;
        public bool? HasConflicts { get; private set; }
        public DateTimeOffset? AcquiredDate { get; private set; }
        public bool? HasHubOrderWithoutShipmentConflict { get; private set; }
        public DateTimeOffset? CancelledDate { get; private set; }
        public string CustomerType { get; private set; } = String.Empty;
        public string CustomerName { get; private set; } = String.Empty;
        public string CustomerEmail { get; private set; } = String.Empty;
        public string CustomerGender { get; private set; } = String.Empty;
        public DateTimeOffset? CustomerBirthDate { get; private set; }
        public string CustomerPhone { get; private set; } = String.Empty;
        public string CustomerCPF { get; private set; } = String.Empty;
        public string CustomerCNPJ { get; private set; } = String.Empty;
        public string CustomerTradingName { get; private set; } = String.Empty;
        public string CustomerSiteTaxPayer { get; private set; } = String.Empty;
        public int? SalesRepresentativeID { get; private set; }
        public int? SellerID { get; private set; }
        public string SellerName { get; private set; } = String.Empty;
        public string SellerEMail { get; private set; } = String.Empty;
        public string SellerPhone { get; private set; } = String.Empty;
        public string SellerIntegrationID { get; private set; } = String.Empty;
        public string MultiSiteTenantBrandId { get; private set; } = String.Empty;
        public string MultiSiteTenantBrandType { get; private set; } = String.Empty;
        public string MultiSiteTenantCompanyId { get; private set; } = String.Empty;
        public string MultiSiteTenantDeviceType { get; private set; } = String.Empty;
        public int? OrderTypeID { get; private set; }
        public string OrderTypeName { get; private set; } = String.Empty;
        public string OrderTypeIntegrationID { get; private set; } = String.Empty;
        public bool? OrderTypeAllowMultiPayment { get; private set; }
        public bool? OrderTypeEmitFiscalTicket { get; private set; }
        public bool? OrderTypeRequirePayment { get; private set; }
        public bool? OrderTypeRequireInventory { get; private set; }

        [SkipProperty]
        public List<OrderItem> items { get; private set; } = [];

        [SkipProperty]
        public List<OrderTag> tags { get; private set; } = [];

        [SkipProperty]
        public List<OrderAddress> addresses { get; private set; } = [];

        [SkipProperty]
        public List<OrderPaymentMethod> paymentMethods { get; private set; } = [];

        [SkipProperty]
        public List<OrderDeliveryMethod> deliveryMethods { get; private set; } = [];

        [SkipProperty]
        public List<OrderDiscount> discounts { get; private set; } = [];

        [SkipProperty]
        public List<OrderShipment> shipments { get; private set; } = [];

        [SkipProperty]
        public List<OrderPackage> packages { get; private set; } = [];

        [SkipProperty]
        public List<OrderInvoice> invoices { get; private set; } = [];

        [SkipProperty]
        public List<OrderProperty> properties { get; private set; } = [];

        public Order() { }

        public Order(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Orders.Order dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            OrderID = ConverterValidatorsExtensions.ConvertToGuidValidation(dto.OrderID);
            OrderNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.OrderNumber, 50);
            MarketPlaceBrand = ConverterValidatorsExtensions.StringLengthValidation(dto.MarketPlaceBrand, 100);
            OriginalOrderID = LinxCommerceConverters.ToNullableGuid(dto.OriginalOrderID);
            WebSiteID = NullableConverters.ToNullableInt32(dto.WebSiteID);
            WebSiteIntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.WebSiteIntegrationID, 100);
            WebSiteName = ConverterValidatorsExtensions.StringLengthValidation(dto.WebSiteName, 250);
            CustomerID = NullableConverters.ToNullableInt32(dto.CustomerID);
            ShopperTicketID = LinxCommerceConverters.ToNullableGuid(dto.ShopperTicketID);
            ItemsQty = NullableConverters.ToNullableDecimal(dto.ItemsQty);
            ItemsCount = NullableConverters.ToNullableInt32(dto.ItemsCount);
            TaxAmount = NullableConverters.ToNullableDecimal(dto.TaxAmount);
            DeliveryAmount = NullableConverters.ToNullableDecimal(dto.DeliveryAmount);
            DiscountAmount = NullableConverters.ToNullableDecimal(dto.DiscountAmount);
            PaymentTaxAmount = NullableConverters.ToNullableDecimal(dto.PaymentTaxAmount);
            SubTotal = NullableConverters.ToNullableDecimal(dto.SubTotal);
            Total = NullableConverters.ToNullableDecimal(dto.Total);
            TotalDue = NullableConverters.ToNullableDecimal(dto.TotalDue);
            TotalPaid = NullableConverters.ToNullableDecimal(dto.TotalPaid);
            TotalRefunded = NullableConverters.ToNullableDecimal(dto.TotalRefunded);
            PaymentDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.PaymentDate);
            PaymentStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentStatus, 50);
            ShipmentDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.ShipmentDate);
            ShipmentStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.ShipmentStatus, 50);
            GlobalStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.GlobalStatus, 50);
            DeliveryPostalCode = ConverterValidatorsExtensions.StringLengthValidation(dto.DeliveryPostalCode, 20);
            CreatedChannel = ConverterValidatorsExtensions.StringLengthValidation(dto.CreatedChannel, 50);
            TrafficSourceID = NullableConverters.ToNullableInt32(dto.TrafficSourceID);
            OrderStatusID = NullableConverters.ToNullableInt32(dto.OrderStatusID);
            CreatedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.CreatedDate);
            CreatedBy = ConverterValidatorsExtensions.StringLengthValidation(dto.CreatedBy, 100);
            ModifiedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.ModifiedDate);
            ModifiedBy = ConverterValidatorsExtensions.StringLengthValidation(dto.ModifiedBy, 100);
            Remarks = dto.Remarks ?? String.Empty;
            SellerCommissionAmount = NullableConverters.ToNullableDecimal(dto.SellerCommissionAmount);
            CommissionAmount = NullableConverters.ToNullableDecimal(dto.CommissionAmount);
            OrderGroupID = LinxCommerceConverters.ToNullableGuid(dto.OrderGroupID);
            OrderGroupNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.OrderGroupNumber, 50);
            HasConflicts = NullableConverters.ToNullableBoolean(dto.HasConflicts);
            AcquiredDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.AcquiredDate);
            HasHubOrderWithoutShipmentConflict = NullableConverters.ToNullableBoolean(dto.HasHubOrderWithoutShipmentConflict);
            CancelledDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.CancelledDate);
            CustomerType = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerType, 10);
            CustomerName = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerName, 250);
            CustomerEmail = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerEmail, 250);
            CustomerGender = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerGender, 10);
            CustomerBirthDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.CustomerBirthDate);
            CustomerPhone = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerPhone, 50);
            CustomerCPF = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerCPF, 20);
            CustomerCNPJ = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerCNPJ, 20);
            CustomerTradingName = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerTradingName, 250);
            CustomerSiteTaxPayer = ConverterValidatorsExtensions.StringLengthValidation(dto.CustomerSiteTaxPayer, 50);
            SalesRepresentativeID = NullableConverters.ToNullableInt32(dto.SalesRepresentative?.SalesRepresentativeID);
            SellerID = NullableConverters.ToNullableInt32(dto.Seller?.SellerID);
            SellerName = ConverterValidatorsExtensions.StringLengthValidation(dto.Seller?.Name, 250);
            SellerEMail = ConverterValidatorsExtensions.StringLengthValidation(dto.Seller?.EMail, 250);
            SellerPhone = ConverterValidatorsExtensions.StringLengthValidation(dto.Seller?.Phone, 50);
            SellerIntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.Seller?.IntegrationID, 100);
            MultiSiteTenantBrandId = ConverterValidatorsExtensions.StringLengthValidation(dto.MultiSiteTenant?.BrandId, 100);
            MultiSiteTenantBrandType = ConverterValidatorsExtensions.StringLengthValidation(dto.MultiSiteTenant?.BrandType, 100);
            MultiSiteTenantCompanyId = ConverterValidatorsExtensions.StringLengthValidation(dto.MultiSiteTenant?.CompanyId, 100);
            MultiSiteTenantDeviceType = ConverterValidatorsExtensions.StringLengthValidation(dto.MultiSiteTenant?.DeviceType, 100);
            OrderTypeID = NullableConverters.ToNullableInt32(dto.OrderType?.OrderTypeID);
            OrderTypeName = ConverterValidatorsExtensions.StringLengthValidation(dto.OrderType?.Name, 250);
            OrderTypeIntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.OrderType?.IntegrationID, 100);
            OrderTypeAllowMultiPayment = NullableConverters.ToNullableBoolean(dto.OrderType?.AllowMultiPayment);
            OrderTypeEmitFiscalTicket = NullableConverters.ToNullableBoolean(dto.OrderType?.EmitFiscalTicket);
            OrderTypeRequirePayment = NullableConverters.ToNullableBoolean(dto.OrderType?.RequirePayment);
            OrderTypeRequireInventory = NullableConverters.ToNullableBoolean(dto.OrderType?.RequireInventory);

            // repetidos violariam a PK; sem a chave a linha também violaria
            foreach (var item in (dto.Items ?? []).Select(x => new OrderItem(doc_company, integrated_or_updated_at, OrderID, x)).Where(x => x.OrderItemID != 0).GroupBy(x => x.OrderItemID).Select(g => g.Last()))
                items.Add(item);
            foreach (var item in (dto.Tags ?? []).Select(x => new OrderTag(doc_company, integrated_or_updated_at, OrderID, x)).Where(x => x.TagID != 0).GroupBy(x => x.TagID).Select(g => g.Last()))
                tags.Add(item);
            foreach (var item in (dto.Addresses ?? []).Select(x => new OrderAddress(doc_company, integrated_or_updated_at, OrderID, x)).Where(x => x.OrderAddressID != 0).GroupBy(x => x.OrderAddressID).Select(g => g.Last()))
                addresses.Add(item);
            foreach (var item in (dto.PaymentMethods ?? []).Select(x => new OrderPaymentMethod(doc_company, integrated_or_updated_at, OrderID, x)).Where(x => x.OrderPaymentMethodID != 0).GroupBy(x => x.OrderPaymentMethodID).Select(g => g.Last()))
                paymentMethods.Add(item);
            foreach (var item in (dto.DeliveryMethods ?? []).Select(x => new OrderDeliveryMethod(doc_company, integrated_or_updated_at, OrderID, x)).Where(x => x.OrderDeliveryMethodID != 0).GroupBy(x => x.OrderDeliveryMethodID).Select(g => g.Last()))
                deliveryMethods.Add(item);
            foreach (var item in (dto.Discounts ?? []).Select(x => new OrderDiscount(doc_company, integrated_or_updated_at, OrderID, x)).Where(x => x.DiscountID != 0).GroupBy(x => x.DiscountID).Select(g => g.Last()))
                discounts.Add(item);
            foreach (var item in (dto.Shipments ?? []).Select(x => new OrderShipment(doc_company, integrated_or_updated_at, OrderID, x)).Where(x => x.OrderShipmentID != Guid.Empty).GroupBy(x => x.OrderShipmentID).Select(g => g.Last()))
                shipments.Add(item);
            foreach (var item in (dto.Properties ?? []).Select(x => new OrderProperty(doc_company, integrated_or_updated_at, OrderID, x)).Where(x => x.OrderPropertyID != 0).GroupBy(x => x.OrderPropertyID).Select(g => g.Last()))
                properties.Add(item);

            // pacotes vêm dentro das remessas; a nota é um objeto (1:1) com chave própria
            foreach (var item in (dto.Shipments ?? []).SelectMany(s => (s.Packages ?? []).Select(p => new OrderPackage(doc_company, integrated_or_updated_at, OrderID, ConverterValidatorsExtensions.ConvertToGuidValidation(s.OrderShipmentID), p))).Where(x => x.OrderShipmentID != Guid.Empty && x.OrderPackageID != Guid.Empty).GroupBy(x => (x.OrderShipmentID, x.OrderPackageID)).Select(g => g.Last()))
                packages.Add(item);

            if (dto.OrderInvoice is not null && new OrderInvoice(doc_company, integrated_or_updated_at, OrderID, dto.OrderInvoice) is { OrderInvoiceID: var invoiceId } invoice && invoiceId != Guid.Empty)
                invoices.Add(invoice);
        }
    }
}

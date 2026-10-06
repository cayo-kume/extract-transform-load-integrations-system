using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxCommerce.Entities.Order
{
    public class Order
    {
        public class Root
        {
            public DateTime? lastupdateon { get; set; }
            public Guid? OrderID { get; set; }
            public string? OrderNumber { get; set; }
            public string? MarketPlaceBrand { get; set; }
            public Guid? OriginalOrderID { get; set; }
            public int? WebSiteID { get; set; }
            public string? WebSiteIntegrationID { get; set; }
            public int? CustomerID { get; set; }
            public Guid? ShopperTicketID { get; set; }
            public decimal? ItemsQty { get; set; }
            public int? ItemsCount { get; set; }
            public decimal? TaxAmount { get; set; }
            public decimal? DeliveryAmount { get; set; }
            public decimal? DiscountAmount { get; set; }
            public decimal? PaymentTaxAmount { get; set; }
            public decimal? SubTotal { get; set; }
            public decimal? Total { get; set; }
            public decimal? TotalDue { get; set; }
            public decimal? TotalPaid { get; set; }
            public decimal? TotalRefunded { get; set; }
            public DateTimeOffset? PaymentDate { get; set; }
            public string? PaymentStatus { get; set; }
            public DateTimeOffset? ShipmentDate { get; set; }
            public string? ShipmentStatus { get; set; }
            public string? GlobalStatus { get; set; }
            public string? DeliveryPostalCode { get; set; }
            public string? CreatedChannel { get; set; }
            public int? TrafficSourceID { get; set; }
            public int? OrderStatusID { get; set; }
            public DateTimeOffset? CreatedDate { get; set; }
            public string? CreatedBy { get; set; }
            public DateTimeOffset? ModifiedDate { get; set; }
            public string? ModifiedBy { get; set; }
            public string? Remarks { get; set; }
            public decimal? SellerCommissionAmount { get; set; }
            public decimal? CommissionAmount { get; set; }
            public Guid? OrderGroupID { get; set; }
            public string? OrderGroupNumber { get; set; }
            public bool? HasConflicts { get; set; }
            public DateTimeOffset? AcquiredDate { get; set; }
            public bool? HasHubOrderWithoutShipmentConflict { get; set; }
            public string? CustomerType { get; set; }
            public DateTimeOffset? CancelledDate { get; set; }
            public string? WebSiteName { get; set; }
            public string? CustomerName { get; set; }
            public string? CustomerEmail { get; set; }
            public string? CustomerGender { get; set; }
            public DateTimeOffset? CustomerBirthDate { get; set; }
            public string? CustomerPhone { get; set; }
            public string? CustomerCPF { get; set; }
            public string? CustomerCNPJ { get; set; }
            public string? CustomerTradingName { get; set; }
            public string? CustomerSiteTaxPayer { get; set; }

            public Int32? SalesRepresentativeID { get; set; }

            public string? SellerEMail { get; set; }
            public string? SellerIntegrationID { get; set; }
            public string? SellerName { get; set; }
            public string? SellerPhone { get; set; }
            public Int32? SellerID { get; set; }

            public string? MultiSiteTenantBrandId { get; set; }
            public string? MultiSiteTenantBrandType { get; set; }
            public string? MultiSiteTenantCompanyId { get; set; }
            public string? MultiSiteTenantDeviceType { get; set; }

            public Int32? OrderTypeID { get; set; }
            public bool? OrderTypeAllowMultiPayment { get; set; }
            public string? OrderTypeIntegrationID { get; set; }
            public string? OrderTypeName { get; set; }
            public bool? OrderTypeEmitFiscalTicket { get; set; }
            public bool? OrderTypeRequirePayment { get; set; }
            public bool? OrderTypeRequireInventory { get; set; }

            [SkipProperty]
            public List<OrderItem> Items { get; set; } = new List<OrderItem>();

            [SkipProperty]
            public List<OrderTag> Tags { get; set; } = new List<OrderTag>();

            [SkipProperty]
            public List<OrderAddress> Addresses { get; set; } = new List<OrderAddress>();

            [SkipProperty]
            public List<OrderPaymentMethod> PaymentMethods { get; set; } = new List<OrderPaymentMethod>();

            [SkipProperty]
            public List<OrderDeliveryMethod> DeliveryMethods { get; set; } = new List<OrderDeliveryMethod>();

            [SkipProperty]
            public List<OrderDiscount> Discounts { get; set; } = new List<OrderDiscount>();

            [SkipProperty]
            public List<OrderShipment> Shipments { get; set; } = new List<OrderShipment>();

            [SkipProperty]
            public OrderInvoice OrderInvoice { get; set; }

            [SkipProperty]
            public OrderSeller Seller { get; set; }

            [SkipProperty]
            public OrderType OrderType { get; set; }

            [SkipProperty]
            public OrderMultiSiteTenant MultiSiteTenant { get; set; }

            [SkipProperty]
            public SalesRepresentative.SalesRepresentative SalesRepresentative { get; set; }

            [SkipProperty]
            public List<OrderProperties> Properties { get; set; } = new List<OrderProperties>();

            [SkipProperty]
            [NotMapped]
            public Dictionary<Guid?, string> Responses { get; set; } = new Dictionary<Guid?, string>();

            public Root() { }

            public Root(Order.Root order, string? getOrderResponse)
            {
                lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
                this.OrderInvoice = new OrderInvoice(order.OrderInvoice == null ? new OrderInvoice() : order.OrderInvoice, order.OrderID);
                this.Seller = new OrderSeller(order.Seller == null ? new OrderSeller() : order.Seller);
                this.OrderType = new OrderType(order.OrderType == null ? new OrderType() : order.OrderType);
                this.MultiSiteTenant = new OrderMultiSiteTenant(order.MultiSiteTenant == null ? new OrderMultiSiteTenant() : order.MultiSiteTenant);
                this.SalesRepresentative = new SalesRepresentative.SalesRepresentative(order.SalesRepresentative == null ? new SalesRepresentative.SalesRepresentative() : order.SalesRepresentative);

                this.OrderID = order.OrderID;
                this.OrderNumber = order.OrderNumber;
                this.MarketPlaceBrand = order.MarketPlaceBrand;
                this.OriginalOrderID = order.OriginalOrderID;
                this.WebSiteID = order.WebSiteID;
                this.WebSiteIntegrationID = order.WebSiteIntegrationID;
                this.CustomerID = order.CustomerID;
                this.ShopperTicketID = order.ShopperTicketID;
                this.ItemsQty = order.ItemsQty;
                this.ItemsCount = order.ItemsCount;
                this.TaxAmount = order.TaxAmount;
                this.DeliveryAmount = order.DeliveryAmount;
                this.DiscountAmount = order.DiscountAmount;
                this.PaymentTaxAmount = order.PaymentTaxAmount;
                this.SubTotal = order.SubTotal;
                this.Total = order.Total;
                this.TotalDue = order.TotalDue;
                this.TotalPaid = order.TotalPaid;
                this.TotalRefunded = order.TotalRefunded;
                this.PaymentDate = order.PaymentDate;
                this.PaymentStatus = order.PaymentStatus;
                this.ShipmentDate = order.ShipmentDate;
                this.ShipmentStatus = order.ShipmentStatus;
                this.GlobalStatus = order.GlobalStatus;
                this.DeliveryPostalCode = order.DeliveryPostalCode;
                this.CreatedChannel = order.CreatedChannel;
                this.TrafficSourceID = order.TrafficSourceID;
                this.OrderStatusID = order.OrderStatusID;
                this.CreatedDate = order.CreatedDate;
                this.CreatedBy = order.CreatedBy;
                this.ModifiedDate = order.ModifiedDate;
                this.ModifiedBy = order.ModifiedBy;
                this.Remarks = order.Remarks;
                this.SellerCommissionAmount = order.SellerCommissionAmount;
                this.CommissionAmount = order.CommissionAmount;
                this.OrderGroupID = order.OrderGroupID;
                this.OrderGroupNumber = order.OrderGroupNumber;
                this.HasConflicts = order.HasConflicts;
                this.AcquiredDate = order.AcquiredDate;
                this.HasHubOrderWithoutShipmentConflict = order.HasHubOrderWithoutShipmentConflict;
                this.CustomerType = order.CustomerType;
                this.CancelledDate = order.CancelledDate;
                this.WebSiteName = order.WebSiteName;
                this.CustomerName = order.CustomerName;
                this.CustomerEmail = order.CustomerEmail;
                this.CustomerGender = order.CustomerGender;
                this.CustomerBirthDate = order.CustomerBirthDate;
                this.CustomerPhone = order.CustomerPhone;
                this.CustomerCPF = order.CustomerCPF;
                this.CustomerCNPJ = order.CustomerCNPJ;
                this.CustomerTradingName = order.CustomerTradingName;
                this.CustomerSiteTaxPayer = order.CustomerSiteTaxPayer;

                this.SalesRepresentativeID = order.SalesRepresentative == null ? 0 : order.SalesRepresentative.SalesRepresentativeID;

                this.SellerEMail = this.Seller.EMail;
                this.SellerIntegrationID = this.Seller.IntegrationID;
                this.SellerName = this.Seller.Name;
                this.SellerPhone = this.Seller.Phone;
                this.SellerID = this.Seller.SellerID;

                this.MultiSiteTenantBrandId = this.MultiSiteTenant.BrandId;
                this.MultiSiteTenantBrandType = this.MultiSiteTenant.BrandType;
                this.MultiSiteTenantCompanyId = this.MultiSiteTenant.CompanyId;
                this.MultiSiteTenantDeviceType = this.MultiSiteTenant.DeviceType;

                this.OrderTypeID = this.OrderType.OrderTypeID;
                this.OrderTypeAllowMultiPayment = this.OrderType.AllowMultiPayment;
                this.OrderTypeIntegrationID = this.OrderType.IntegrationID;
                this.OrderTypeName = this.OrderType.Name;
                this.OrderTypeEmitFiscalTicket = this.OrderType.EmitFiscalTicket;
                this.OrderTypeRequirePayment = this.OrderType.RequirePayment;
                this.OrderTypeRequireInventory = this.OrderType.RequireInventory;

                this.Responses.Add(order.OrderID, getOrderResponse);

                foreach (OrderItem item in order.Items)
                {
                    this.Items.Add(new OrderItem(item));
                }

                foreach (OrderTag tag in order.Tags)
                {
                    this.Tags.Add(new OrderTag(tag, order.OrderID));
                }

                foreach (OrderAddress address in order.Addresses)
                {
                    this.Addresses.Add(new OrderAddress(address));
                }

                foreach (OrderPaymentMethod paymentMethod in order.PaymentMethods)
                {
                    this.PaymentMethods.Add(new OrderPaymentMethod(paymentMethod));
                }

                foreach (OrderDeliveryMethod deliveryMethod in order.DeliveryMethods)
                {
                    this.DeliveryMethods.Add(new OrderDeliveryMethod(deliveryMethod));
                }

                foreach (OrderDiscount orderDiscount in order.Discounts)
                {
                    this.Discounts.Add(new OrderDiscount(orderDiscount, order.OrderID));
                }

                foreach (OrderShipment orderShipment in order.Shipments)
                {
                    this.Shipments.Add(new OrderShipment(orderShipment));
                }

                foreach (OrderProperties orderProperties in order.Properties)
                {
                    this.Properties.Add(new OrderProperties(orderProperties, order.OrderID));
                }
            }
        }
    }
}

using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderTrackingNumber
    {
        public Guid? OrderID { get; set; }
        public Guid? OrderInvoiceID { get; set; }
        public string? OrderInvoiceCode { get; set; }
        public string? TrackingNumber { get; set; }

        [SkipProperty]
        public Dictionary<Guid?, string> Requests { get; set; } = new Dictionary<Guid?, string>();

        public OrderTrackingNumber() { }

        public OrderTrackingNumber(OrderTrackingNumber orderTrackingNumber, string updateOrderRequest)
        {
            this.OrderID = orderTrackingNumber.OrderID;
            this.OrderInvoiceID = orderTrackingNumber.OrderInvoiceID;
            this.OrderInvoiceCode = orderTrackingNumber.OrderInvoiceCode;
            this.TrackingNumber = orderTrackingNumber.TrackingNumber;

            this.Requests.Add(orderTrackingNumber.OrderID, updateOrderRequest);
        }
    }
}

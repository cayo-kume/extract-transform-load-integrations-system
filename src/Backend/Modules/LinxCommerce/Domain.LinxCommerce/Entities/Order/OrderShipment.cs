using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderShipment
    {
        public DateTime lastupdateon { get; set; }
        public Guid? OrderShipmentID { get; set; }
        public Guid? OrderID { get; set; }
        public Int32? DeliveryMethodID { get; set; }
        public string? ShipmentNumber { get; set; }
        public string? ShipmentStatus { get; set; }
        public Int32? AssignUserId { get; set; }
        public string? AssignUserName { get; set; }
        public Int32? DockID { get; set; }

        [SkipProperty]
        public List<OrderPackage> Packages { get; set; } = new List<OrderPackage>();

        public OrderShipment() { }

        public OrderShipment(OrderShipment orderShipment)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.OrderShipmentID = orderShipment.OrderShipmentID;
            this.OrderID = orderShipment.OrderID;
            this.DeliveryMethodID = orderShipment.DeliveryMethodID;
            this.ShipmentNumber = orderShipment.ShipmentNumber;
            this.ShipmentStatus = orderShipment.ShipmentStatus;
            this.AssignUserId = orderShipment.AssignUserId;
            this.AssignUserName = orderShipment.AssignUserName;
            this.DockID = orderShipment.DockID;

            foreach (OrderPackage package in orderShipment.Packages)
            {
                this.Packages.Add(new OrderPackage(package));
            }
        }
    }
}

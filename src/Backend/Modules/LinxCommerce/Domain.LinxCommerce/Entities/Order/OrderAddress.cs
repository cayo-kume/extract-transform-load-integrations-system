namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderAddress
    {
        public DateTime lastupdateon { get; set; }
        public Int32? OrderAddressID { get; set; }
        public Guid? OrderID { get; set; }
        public string? Name { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Neighbourhood { get; set; }
        public string? Number { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? AddressNotes { get; set; }
        public string? Landmark { get; set; }
        public string? ContactName { get; set; }
        public string? ContactDocumentNumber { get; set; }
        public Int32? AddressType { get; set; }
        public Int32? PointOfSaleID { get; set; }
        public string? ContactPhone { get; set; }

        public OrderAddress() { }

        public OrderAddress(OrderAddress orderAddress)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.OrderAddressID = orderAddress.OrderAddressID;
            this.OrderID = orderAddress.OrderID;
            this.Name = orderAddress.Name;
            this.AddressLine = orderAddress.AddressLine;
            this.City = orderAddress.City;
            this.Neighbourhood = orderAddress.Neighbourhood;
            this.Number = orderAddress.Number;
            this.State = orderAddress.State;
            this.PostalCode = orderAddress.PostalCode;
            this.AddressNotes = orderAddress.AddressNotes;
            this.Landmark = orderAddress.Landmark;
            this.ContactName = orderAddress.ContactName;
            this.ContactDocumentNumber = orderAddress.ContactDocumentNumber;
            this.AddressType = orderAddress.AddressType;
            this.PointOfSaleID = orderAddress.PointOfSaleID;
            this.ContactPhone = orderAddress.ContactPhone;
        }
    }
}

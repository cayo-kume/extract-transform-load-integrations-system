namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderProperties
    {
        public DateTime? lastupdateon { get; set; }
        public Guid? OrderID { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
        public Int64 OrderPropertyID { get; set; }
        public string Reference { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string XmlExtended { get; set; }

        public OrderProperties() { }

        public OrderProperties(OrderProperties orderProperties, Guid? OrderID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.OrderID = OrderID;
            Amount = orderProperties.Amount;
            Message = orderProperties.Message;
            OrderPropertyID = orderProperties.OrderPropertyID;
            Reference = orderProperties.Reference;
            Source = orderProperties.Source;
            Type = orderProperties.Type;
            XmlExtended = orderProperties.XmlExtended;
        }
    }
}

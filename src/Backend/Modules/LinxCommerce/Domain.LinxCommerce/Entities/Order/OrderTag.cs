namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderTag
    {
        public DateTime lastupdateon { get; set; }
        public Int32? TagID { get; set; }
        public string? Alias { get; set; }
        public string? Name { get; set; }
        public bool? IsSystem { get; set; }
        public bool? IsDeleted { get; set; }
        public Guid? OrderID { get; set; }

        public OrderTag() { }

        public OrderTag(OrderTag orderTag, Guid? orderID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.OrderID = orderID;
            this.IsDeleted = orderTag.IsDeleted;
            this.IsSystem = orderTag.IsSystem;
            this.Name = orderTag.Name;
            this.Alias = orderTag.Alias;
            this.TagID = orderTag.TagID;
        }
    }
}

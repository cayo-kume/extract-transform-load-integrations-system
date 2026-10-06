namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderDiscount
    {
        public DateTime lastupdateon { get; set; }
        public decimal? Amount { get; set; }
        public Int32? DiscountID { get; set; }
        public string? Message { get; set; }
        public string? Reference { get; set; }
        public string? Type { get; set; }
        public Guid? OrderID { get; set; }

        public OrderDiscount() { }

        public OrderDiscount(OrderDiscount orderDiscount, Guid? orderID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.Amount = orderDiscount.Amount;
            this.DiscountID = orderDiscount.DiscountID;
            this.Message = orderDiscount.Message;
            this.Reference = orderDiscount.Reference;
            this.Type = orderDiscount.Type;
            this.OrderID = orderID;
        }
    }
}

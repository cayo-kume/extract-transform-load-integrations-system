namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentativeMaxDiscount
    {
        public string? Type { get; set; }
        public decimal? Amount { get; set; }
        public int? SalesRepresentativeID { get; set; }

        public SalesRepresentativeMaxDiscount() { }

        public SalesRepresentativeMaxDiscount(SalesRepresentativeMaxDiscount salesRepresentativeMaxDiscount, Int32 salesRepresentativeID)
        {
            this.SalesRepresentativeID = salesRepresentativeID;
            this.Type = salesRepresentativeMaxDiscount.Type;
            this.Amount = salesRepresentativeMaxDiscount.Amount;
        }
    }
}

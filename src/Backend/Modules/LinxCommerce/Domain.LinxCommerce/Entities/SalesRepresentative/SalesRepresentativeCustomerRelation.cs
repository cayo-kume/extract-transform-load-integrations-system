namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentativeCustomerRelation
    {
        public string? CustomerID { get; set; }
        public string? Status { get; set; }
        public string? IsMaxDiscountEnabled { get; set; }
        public int? SalesRepresentativeID { get; set; }

        public SalesRepresentativeCustomerRelation() { }

        public SalesRepresentativeCustomerRelation(SalesRepresentativeCustomerRelation customer, Int32 salesRepresentativeID)
        {
            this.SalesRepresentativeID = salesRepresentativeID;
            this.CustomerID = customer.CustomerID;
            this.Status = customer.Status;
            this.IsMaxDiscountEnabled = customer.IsMaxDiscountEnabled;
        }
    }
}

namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentativeComission
    {
        public int? SalesRepresentativeID { get; set; }
        public decimal? TotalCommission { get; set; }
        public decimal? DeliveryCommission { get; set; }

        public SalesRepresentativeComission() { }

        public SalesRepresentativeComission(SalesRepresentativeComission salesRepresentativeComission, Int32 salesRepresentativeID)
        {
            this.SalesRepresentativeID = salesRepresentativeID;
            this.DeliveryCommission = salesRepresentativeComission.DeliveryCommission;
            this.TotalCommission = salesRepresentativeComission.TotalCommission;
        }
    }
}

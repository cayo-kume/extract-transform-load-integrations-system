namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentativeIdentification
    {
        public int? SalesRepresentativeID { get; set; }
        public string? Type { get; set; }
        public string? DocumentNumber { get; set; }

        public SalesRepresentativeIdentification() { }

        public SalesRepresentativeIdentification(SalesRepresentativeIdentification salesRepresentativeIdentification, Int32 salesRepresentativeID)
        {
            this.SalesRepresentativeID = salesRepresentativeID;
            this.Type = salesRepresentativeIdentification.Type;
            this.DocumentNumber = salesRepresentativeIdentification.DocumentNumber;
        }
    }
}

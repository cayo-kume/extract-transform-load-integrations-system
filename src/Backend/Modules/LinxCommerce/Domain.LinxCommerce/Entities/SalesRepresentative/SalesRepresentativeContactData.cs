namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentativeContactData
    {
        public int? SalesRepresentativeID { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CellPhone { get; set; }

        public SalesRepresentativeContactData() { }

        public SalesRepresentativeContactData(SalesRepresentativeContactData salesRepresentativeContactData, Int32 salesRepresentativeID)
        {
            this.SalesRepresentativeID = salesRepresentativeID;
            this.Email = salesRepresentativeContactData.Email;
            this.Phone = salesRepresentativeContactData.Phone;
            this.CellPhone = salesRepresentativeContactData.CellPhone;
        }
    }
}

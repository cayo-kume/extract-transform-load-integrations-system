namespace Domain.LinxCommerce.Entities.SalesRepresentative
{
    public class SalesRepresentativeAddress
    {
        public int? SalesRepresentativeID { get; set; }
        public bool? IsMainAddress { get; set; }
        public string? Name { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Neighbourhood { get; set; }
        public string? Number { get; set; }
        public string? State { get; set; }
        public string? AddressNotes { get; set; }
        public string? Landmark { get; set; }
        public string? ContactName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? PostalCode { get; set; }

        public SalesRepresentativeAddress() { }

        public SalesRepresentativeAddress(SalesRepresentativeAddress salesRepresentativeAddress)
        {
            this.SalesRepresentativeID = salesRepresentativeAddress.SalesRepresentativeID;
            this.IsMainAddress = salesRepresentativeAddress.IsMainAddress;
            this.Name = salesRepresentativeAddress.Name;
            this.AddressLine = salesRepresentativeAddress.AddressLine;
            this.City = salesRepresentativeAddress.City;
            this.Neighbourhood = salesRepresentativeAddress.Neighbourhood;
            this.Number = salesRepresentativeAddress.Number;
            this.State = salesRepresentativeAddress.State;
            this.AddressNotes = salesRepresentativeAddress.AddressNotes;
            this.Landmark = salesRepresentativeAddress.Landmark;
            this.ContactName = salesRepresentativeAddress.ContactName;
            this.Latitude = salesRepresentativeAddress.Latitude;
            this.Longitude = salesRepresentativeAddress.Longitude;
            this.PostalCode = salesRepresentativeAddress.PostalCode;
        }
    }
}

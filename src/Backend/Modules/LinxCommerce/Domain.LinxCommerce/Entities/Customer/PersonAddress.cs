namespace Domain.LinxCommerce.Entities.Customer
{
    public class PersonAddress
    {
        public DateTime lastupdateon { get; set; }
        public Int32? ID { get; set; }
        public string? Name { get; set; }
        public string? ContactName { get; set; }
        public string? PostalCode { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Neighbourhood { get; set; }
        public string? Number { get; set; }
        public string? State { get; set; }
        public string? AddressNotes { get; set; }
        public string? Landmark { get; set; }
        public bool? MainAddress { get; set; }
        public Int32? CustomerID { get; set; }

        public PersonAddress() { }

        public PersonAddress(PersonAddress address, Int32? customerID)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.ID = address.ID;
            this.Name = address.Name;
            this.ContactName = address.ContactName;
            this.PostalCode = address.PostalCode;
            this.AddressLine = address.AddressLine;
            this.City = address.City;
            this.Neighbourhood = address.Neighbourhood;
            this.Number = address.Number;
            this.State = address.State;
            this.AddressNotes = address.AddressNotes;
            this.Landmark = address.Landmark;
            this.MainAddress = address.MainAddress;
            this.CustomerID = customerID;
        }
    }
}

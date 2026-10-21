namespace Domain.Movidesk.Dtos.Reads.People
{
    public class Address
    {
        public string addressType { get; init; }
        public string country { get; init; }
        public string postalCode { get; init; }
        public string state { get; init; }
        public string city { get; init; }
        public string district { get; init; }
        public string street { get; init; }
        public string number { get; init; }
        public string complement { get; init; }
        public string reference { get; init; }
        public string isDefault { get; init; }
        public string countryId { get; init; }
    }
}

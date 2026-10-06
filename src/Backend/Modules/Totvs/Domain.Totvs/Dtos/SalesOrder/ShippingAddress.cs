namespace Domain.Totvs.Dtos.SalesOrder
{
    public class ShippingAddress
    {
        public string addressType { get; set; }
        public string publicPlace { get; set; }
        public string address { get; set; }
        public string number { get; set; }
        public string complement { get; set; }
        public string addressSequence { get; set; }
        public string neighborhood { get; set; }
        public string ibgeCityCode { get; set; }
        public string cityName { get; set; }
        public string stateAbbreviation { get; set; }
        public string cep { get; set; }
        public string bcbCountryCode { get; set; }
        public string countryName { get; set; }
        public string postOfficeBox { get; set; }
        public string reference { get; set; }
    }
}

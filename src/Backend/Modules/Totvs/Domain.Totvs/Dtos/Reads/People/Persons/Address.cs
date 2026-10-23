namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// endereço da pessoa física ou jurídica (API Person)
    /// </summary>
    public class Address
    {
        public string? addressTypeCode { get; init; }
        public string? sequenceCode { get; init; }
        public string? addressType { get; init; }
        public string? publicPlace { get; init; }
        public string? address { get; init; }
        public string? addressNumber { get; init; }
        public string? complement { get; init; }
        public string? neighborhood { get; init; }
        public string? ibgeCityCode { get; init; }
        public string? cityName { get; init; }
        public string? stateAbbreviation { get; init; }
        public string? cep { get; init; }
        public string? bcbCountryCode { get; init; }
        public string? countryName { get; init; }
        public string? postOfficeBox { get; init; }
        public string? reference { get; init; }
    }
}

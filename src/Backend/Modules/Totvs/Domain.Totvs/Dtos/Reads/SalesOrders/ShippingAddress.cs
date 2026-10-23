namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// objeto 1:1 (shippingAddress); vira colunas do pai com o prefixo shippingAddress
    /// </summary>
    public class ShippingAddress
    {
        public string? addressType { get; init; }
        public string? publicPlace { get; init; }
        public string? address { get; init; }
        public string? number { get; init; }
        public string? complement { get; init; }
        public string? addressSequence { get; init; }
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

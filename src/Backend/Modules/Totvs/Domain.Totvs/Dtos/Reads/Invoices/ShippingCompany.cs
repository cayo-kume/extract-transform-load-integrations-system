namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// objeto 1:1 (shippingCompany); vira colunas da nota com o prefixo shippingCompany
    /// </summary>
    public class ShippingCompany
    {
        public string? shippingCompanyCode { get; init; }
        public string? shippingCompanyName { get; init; }
        public string? freitghtType { get; init; }
        public string? freitghtTypeRedispatch { get; init; }
        public string? redispatchCarrierCode { get; init; }
        public string? redispatchCarrierName { get; init; }
        public string? cityCodeRedispatch { get; init; }
        public string? cityNameRedispatch { get; init; }
        public string? cityUfRedispatch { get; init; }
        public string? freightValue { get; init; }
        public string? shippingCompanyType { get; init; }
        public string? publicPlaceTypeName { get; init; }
        public string? publicPlaceName { get; init; }
        public string? publicPlaceCode { get; init; }
        public string? neighborhood { get; init; }
        public string? cityName { get; init; }
        public string? stateAbbreviation { get; init; }
        public string? cep { get; init; }
        public string? postOfficeBox { get; init; }
        public string? numberStateRegistration { get; init; }
        public string? cpfCnpj { get; init; }
        public string? plaqueCode { get; init; }
        public string? plaqueUf { get; init; }
        public string? packageNumber { get; init; }
        public string? grossWeight { get; init; }
        public string? netWeight { get; init; }
        public string? species { get; init; }
        public string? brandName { get; init; }
        public string? trackingCode { get; init; }
    }
}

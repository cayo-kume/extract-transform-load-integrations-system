namespace Domain.Totvs.Dtos.Reads.People.Persons
{
    /// <summary>
    /// objeto 1:1 da pessoa (preferences); vira colunas do pai com o prefixo preferences
    /// </summary>
    public class Preferences
    {
        public string? paymentConditionCode { get; init; }
        public string? paymentConditionDescription { get; init; }
        public string? shippingCompanyCode { get; init; }
        public string? shippingCompanyName { get; init; }
        public string? freightType { get; init; }
        public string? bridgeShippingCompanyCode { get; init; }
        public string? bridgeShippingCompanyName { get; init; }
        public string? maximumAverageTerm { get; init; }
        public string? priceTableCode { get; init; }
        public string? priceTableDescription { get; init; }
        public string? orderPriority { get; init; }
        public string? bearerCode { get; init; }
        public string? bearerName { get; init; }
    }
}

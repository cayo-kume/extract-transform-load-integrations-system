namespace Domain.Totvs.Dtos.Writes.People.Customers
{
    /// <summary>
    /// preferências comerciais da pessoa jurídica (PreferencesInDto)
    /// </summary>
    public class Preferences
    {
        public int? paymentConditionCode { get; init; }
        public int? shippingCompanyCode { get; init; }
        public string? freightType { get; init; }
        public int? bridgeShippingCompanyCode { get; init; }
        public int? maximumAverageTerm { get; init; }
        public int? priceTableCode { get; init; }
        public int? orderPriority { get; init; }
        public int? bearerCode { get; init; }
    }
}

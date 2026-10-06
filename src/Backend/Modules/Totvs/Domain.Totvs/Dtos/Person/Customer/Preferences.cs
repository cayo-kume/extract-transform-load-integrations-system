namespace Domain.Totvs.Dtos.Person.Customer
{
    public class Preferences
    {
        public string paymentConditionCode { get; set; }
        public string paymentConditionDescription { get; set; }
        public string shippingCompanyCode { get; set; }
        public string shippingCompanyName { get; set; }
        public string freightType { get; set; }
        public string bridgeShippingCompanyCode { get; set; }
        public string bridgeShippingCompanyName { get; set; }
        public string maximumAverageTerm { get; set; }
        public string priceTableCode { get; set; }
        public string priceTableDescription { get; set; }
        public string orderPriority { get; set; }
        public string bearerCode { get; set; }
        public string bearerName { get; set; }
    }
}

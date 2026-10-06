namespace Domain.Totvs.Dtos.PurchaseOrder
{
    public class Product
    {
        public string productCode { get; set; }
        public string name { get; set; }
        public string referenceCode { get; set; }
        public string referenceName { get; set; }
        public string productSku { get; set; }
        public string colorCode { get; set; }
        public string colorName { get; set; }
        public string sizeName { get; set; }
        public string measuredUnit { get; set; }
        public string quantity { get; set; }
        public string lowQuantity { get; set; }
        public string settledQuantity { get; set; }
        public string canceledQuantity { get; set; }
        public string extraQuantity { get; set; }
        public string deliveryForecastDate { get; set; }
        public string deliveryDeadlineDate { get; set; }
        public string value { get; set; }
        public string discountPercentage { get; set; }
        public string conversionUnit { get; set; }
        public string conversionQuantity { get; set; }
    }
}

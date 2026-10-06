namespace Domain.Totvs.Dtos.SalesOrder
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
        public string quantity { get; set; }
        public string toSettleQuantity { get; set; }
        public string settledQuantity { get; set; }
        public string canceledQuantity { get; set; }
        public string extraQuantity { get; set; }
        public string pendingQuantity { get; set; }
        public string originalPrice { get; set; }
        public string price { get; set; }
        public string discountPercentage { get; set; }
        public string lastChangeUserInsert { get; set; }
        public string lastChangeDate { get; set; }
        public string billingForecastDate { get; set; }
        public string insertDate { get; set; }
        public string cancellationLimitDate { get; set; }
        public string billingLimitDate { get; set; }
        public string lastBillingDate { get; set; }
        public string rollMeasureCode { get; set; }
        public string rollMeasureDescription { get; set; }
        public string customerOrderCode { get; set; }
        public string customerProductCode { get; set; }
        public List<BatchItem> batchItems { get; set; }
    }
}

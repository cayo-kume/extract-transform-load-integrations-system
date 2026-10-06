namespace Domain.Totvs.Dtos.SalesOrder
{
    public class Invoice
    {
        public string accessKey { get; set; }
        public string code { get; set; }
        public string serial { get; set; }
        public string issueDate { get; set; }
        public string status { get; set; }
        public string shippingCompanyName { get; set; }
        public string packageNumber { get; set; }
        public string grossWeight { get; set; }
        public string netWeight { get; set; }
        public string trackingCode { get; set; }
        public string discountPercentage { get; set; }
        public string quantity { get; set; }
        public string productValue { get; set; }
        public string additionalValue { get; set; }
        public string shippingValue { get; set; }
        public string InsuranceValue { get; set; }
        public string ipiValue { get; set; }
        public string totalValue { get; set; }
        public string transactionBranchCode { get; set; }
        public string transactionDate { get; set; }
        public string transactionCode { get; set; }
        public Electronic electronic { get; set; }
    }
}

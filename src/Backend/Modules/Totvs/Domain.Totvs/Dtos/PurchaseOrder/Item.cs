namespace Domain.Totvs.Dtos.PurchaseOrder
{
    public class Item
    {
        public string branchCode { get; set; }
        public string orderCode { get; set; }
        public string supplierOrderCode { get; set; }
        public string supplierCode { get; set; }
        public string supplierCpfCnpj { get; set; }
        public string supplierName { get; set; }
        public string buyerCode { get; set; }
        public string buyerCpfCnpj { get; set; }
        public string buyerName { get; set; }
        public string operationCode { get; set; }
        public string operationName { get; set; }
        public string shippingCompanyName { get; set; }
        public string paymentConditionCode { get; set; }
        public string paymentConditionName { get; set; }
        public string paymentType { get; set; }
        public string status { get; set; }
        public string freightType { get; set; }
        public string maxChangeFilterDate { get; set; }
        public string registrationDate { get; set; }
        public string deliveryForecastDate { get; set; }
        public string deliveryDeadlineDate { get; set; }
        public string basePaymentDate { get; set; }
        public string productValue { get; set; }
        public string quantity { get; set; }
        public string ipiValue { get; set; }
        public string totalAmountOrder { get; set; }
        public List<Product> items { get; set; }
    }
}

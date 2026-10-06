namespace Domain.Totvs.Dtos.Fiscal
{
    public class Item
    {
        public string branchCode { get; set; }
        public string branchCnpj { get; set; }
        public string invoiceSequence { get; set; }
        public string invoiceDate { get; set; }
        public string personCode { get; set; }
        public string personName { get; set; }
        public string invoiceCode { get; set; }
        public string serialCode { get; set; }
        public string invoiceStatus { get; set; }
        public string transactionBranchCode { get; set; }
        public string transactionDate { get; set; }
        public string transactionCode { get; set; }
        public string inclusionComponentCode { get; set; }
        public string peripheralPdvCode { get; set; }
        public string versionPdv { get; set; }
        public string mobileVersion { get; set; }
        public string idDocumentPDV { get; set; }
        public string userCode { get; set; }
        public string origin { get; set; }
        public string documentType { get; set; }
        public string operationType { get; set; }
        public string operationCode { get; set; }
        public string operatioName { get; set; }
        public string issueDate { get; set; }
        public string releaseDate { get; set; }
        public string exitTime { get; set; }
        public string lastchangeDate { get; set; }
        public string maxChangeFilterDate { get; set; }
        public string paymentConditionCode { get; set; }
        public string paymentConditionName { get; set; }
        public string discountPercentage { get; set; }
        public string quantity { get; set; }
        public string productValue { get; set; }
        public string additionalValue { get; set; }
        public string shippingValue { get; set; }
        public string insuranceValue { get; set; }
        public string ipiValue { get; set; }
        public string baseIcmsValue { get; set; }
        public string icmsValue { get; set; }
        public string icmsSubStValue { get; set; }
        public string totalValue { get; set; }
        public string terminalCode { get; set; }
        public string sellerCpf { get; set; }
        public string observationNFE { get; set; }
        public Eletronic eletronic { get; set; }
        public Ecf ecf { get; set; }
        public Sat sat { get; set; }
        public ShippingCompany shippingCompany { get; set; }
        public Person person { get; set; }
        public ProductionOrder productionOrder { get; set; }
        public List<SalesOrder> salesOrder { get; set; }
        public List<ProductItem> items { get; set; }
        public List<Payment> payments { get; set; }
        public List<ObservationNF> observationNF { get; set; }
        public List<ReferencedTax> referencedTaxInvoice { get; set; }
    }
}

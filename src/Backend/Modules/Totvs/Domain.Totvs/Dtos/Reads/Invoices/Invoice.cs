namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// nota fiscal (POST fiscal/v2/invoices/search) (InvoiceDataModel)
    /// </summary>
    public class Invoice
    {
        public string? branchCode { get; init; }
        public string? branchCnpj { get; init; }
        public string? invoiceSequence { get; init; }
        public string? invoiceDate { get; init; }
        public string? personCode { get; init; }
        public string? personName { get; init; }
        public string? invoiceCode { get; init; }
        public string? serialCode { get; init; }
        public string? invoiceStatus { get; init; }
        public string? transactionBranchCode { get; init; }
        public string? transactionDate { get; init; }
        public string? transactionCode { get; init; }
        public string? inclusionComponentCode { get; init; }
        public string? peripheralPdvCode { get; init; }
        public string? versionPdv { get; init; }
        public string? mobileVersion { get; init; }
        public string? idDocumentPDV { get; init; }
        public string? userCode { get; init; }
        public string? origin { get; init; }
        public string? documentType { get; init; }
        public string? operationType { get; init; }
        public string? operationCode { get; init; }
        public string? operatioName { get; init; }
        public string? issueDate { get; init; }
        public string? releaseDate { get; init; }
        public string? exitTime { get; init; }
        public string? lastchangeDate { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public string? paymentConditionCode { get; init; }
        public string? paymentConditionName { get; init; }
        public string? discountPercentage { get; init; }
        public string? quantity { get; init; }
        public string? productValue { get; init; }
        public string? additionalValue { get; init; }
        public string? shippingValue { get; init; }
        public string? insuranceValue { get; init; }
        public string? ipiValue { get; init; }
        public string? baseIcmsValue { get; init; }
        public string? icmsValue { get; init; }
        public string? icmsSubStValue { get; init; }
        public string? totalValue { get; init; }
        public string? terminalCode { get; init; }
        public string? sellerCpf { get; init; }
        public string? observationNFE { get; init; }

        public Electronic? eletronic { get; init; }
        public Ecf? ecf { get; init; }
        public Sat? sat { get; init; }
        public ShippingCompany? shippingCompany { get; init; }
        public Person? person { get; init; }
        public ProductionOrder? productionOrder { get; init; }
        public List<Item>? items { get; init; }
        public List<Payment>? payments { get; init; }
        public List<SalesOrder>? salesOrder { get; init; }
        public List<ObservationNF>? observationNF { get; init; }
        public List<ReferencedTaxInvoice>? referencedTaxInvoice { get; init; }
    }
}

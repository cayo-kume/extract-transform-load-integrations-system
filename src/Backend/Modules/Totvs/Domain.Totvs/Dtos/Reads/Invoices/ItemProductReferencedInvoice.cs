namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// nota referenciada pelo produto devolvido (referencedBranchCode é o branchCode da nota referenciada)
    /// </summary>
    public class ItemProductReferencedInvoice
    {
        public string? invoiceSequenceItem { get; init; }
        public string? accessKey { get; init; }
        public string? satSerialCode { get; init; }
        public string? issueDate { get; init; }
        public string? invoiceCode { get; init; }
        public string? invoiceSerialCode { get; init; }
        public string? unitNetValue { get; init; }
        public string? quantity { get; init; }
        public string? cfop { get; init; }
        public string? branchCode { get; init; }
        public string? branchCnpj { get; init; }
        public string? terminalCode { get; init; }
    }
}

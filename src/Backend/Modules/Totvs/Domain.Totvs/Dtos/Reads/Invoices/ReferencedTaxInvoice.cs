namespace Domain.Totvs.Dtos.Reads.Invoices
{
    /// <summary>
    /// nota referenciada na capa (nulos da chave viram 0 e vazio)
    /// </summary>
    public class ReferencedTaxInvoice
    {
        public string? referencedInvoiceBranchCode { get; init; }
        public string? invoiceCode { get; init; }
        public string? invoiceSerialCode { get; init; }
        public string? accessKey { get; init; }
        public string? satSerialCode { get; init; }
        public string? issueDate { get; init; }
        public string? branchCnpj { get; init; }
    }
}

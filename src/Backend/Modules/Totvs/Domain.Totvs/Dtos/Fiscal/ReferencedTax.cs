namespace Domain.Totvs.Dtos.Fiscal
{
    public class ReferencedTax
    {
        public string accessKey { get; set; }
        public string satSerialCode { get; set; }
        public string issueDate { get; set; }
        public string referencedInvoiceBranchCode { get; set; }
        public string branchCnpj { get; set; }
        public string invoiceCode { get; set; }
        public string invoiceSerialCode { get; set; }
    }
}

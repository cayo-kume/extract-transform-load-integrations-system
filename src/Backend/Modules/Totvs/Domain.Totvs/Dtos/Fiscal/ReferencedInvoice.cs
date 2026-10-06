namespace Domain.Totvs.Dtos.Fiscal
{
    public class ReferencedInvoice
    {
        public string accessKey { get; set; }
        public string satSerialCode { get; set; }
        public string issueDate { get; set; }
        public string invoiceSequenceItem { get; set; }
        public string invoiceCode { get; set; }
        public string invoiceSerialCode { get; set; }
        public string unitNetValue { get; set; }
        public string quantity { get; set; }
        public string cfop { get; set; }
        public string branchCode { get; set; }
        public string branchCnpj { get; set; }
        public string terminalCode { get; set; }
    }
}

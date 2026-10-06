using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Fiscal
{
    public class ReferencedTax
    {
        public int invoiceSequence { get; private set; }
        
        public int referencedInvoiceBranchCode { get; private set; }
        public int invoiceCode { get; private set; }

        public string accessKey { get; private set; }
        public string satSerialCode { get; private set; }
        public string branchCnpj { get; private set; }
        public string invoiceSerialCode { get; private set; }
        
        public DateTime issueDate { get; private set; }

        public ReferencedTax() { }

        public ReferencedTax(Domain.Totvs.Dtos.Fiscal.ReferencedTax referencedTaxInvoiceDto, int invoiceSequence)
        {
            this.invoiceSequence = invoiceSequence;

            referencedInvoiceBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(referencedTaxInvoiceDto.referencedInvoiceBranchCode);
            invoiceCode = ConverterValidatorsExtensions.ConvertToInt32Validation(referencedTaxInvoiceDto.invoiceCode);

            accessKey = referencedTaxInvoiceDto.accessKey?.Length > 14 ? referencedTaxInvoiceDto.accessKey?.Substring(0, 14) : referencedTaxInvoiceDto.accessKey;
            satSerialCode = referencedTaxInvoiceDto.satSerialCode?.Length > 14 ? referencedTaxInvoiceDto.satSerialCode?.Substring(0, 14) : referencedTaxInvoiceDto.satSerialCode;
            branchCnpj = referencedTaxInvoiceDto.branchCnpj?.Length > 14 ? referencedTaxInvoiceDto.branchCnpj?.Substring(0, 14) : referencedTaxInvoiceDto.branchCnpj;
            invoiceSerialCode = referencedTaxInvoiceDto.invoiceSerialCode?.Length > 14 ? referencedTaxInvoiceDto.invoiceSerialCode?.Substring(0, 14) : referencedTaxInvoiceDto.invoiceSerialCode;

            issueDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(referencedTaxInvoiceDto.issueDate);
        }
    }
}

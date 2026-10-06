using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Fiscal
{
    public class ReferencedInvoice
    {
        public int invoiceSequence { get; private set; }
        public int invoiceItemsSequence { get; private set; }

        public int productCode { get; private set; }
        public int invoiceSequenceItem { get; private set; }
        public int invoiceCode { get; private set; }
        public int branchCode { get; private set; }
        public int terminalCode { get; private set; }
        
        public DateTime issueDate { get; private set; }

        public decimal unitNetValue { get; private set; }
        public decimal quantity { get; private set; }

        public string accessKey { get; private set; }
        public string satSerialCode { get; private set; }
        public string invoiceSerialCode { get; private set; }
        public string cfop { get; private set; }
        public string branchCnpj { get; private set; }

        public ReferencedInvoice() { }

        public ReferencedInvoice(Domain.Totvs.Dtos.Fiscal.ReferencedInvoice referencedInvoiceDto, int invoiceSequence, int invoiceItemsSequence, int productCode)
        {
            this.invoiceSequence = invoiceSequence;
            this.invoiceItemsSequence = invoiceItemsSequence;
            this.productCode = productCode;

            invoiceSequenceItem = ConverterValidatorsExtensions.ConvertToInt32Validation(referencedInvoiceDto.invoiceSequenceItem);
            invoiceCode = ConverterValidatorsExtensions.ConvertToInt32Validation(referencedInvoiceDto.invoiceCode);
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(referencedInvoiceDto.branchCode);
            terminalCode = ConverterValidatorsExtensions.ConvertToInt32Validation(referencedInvoiceDto.terminalCode);

            issueDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(referencedInvoiceDto.issueDate);

            unitNetValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(referencedInvoiceDto.unitNetValue);
            quantity = ConverterValidatorsExtensions.ConvertToDecimalValidation(referencedInvoiceDto.quantity);

            accessKey = referencedInvoiceDto.accessKey?.Length > 44 ? referencedInvoiceDto.accessKey?.Substring(0, 44) : referencedInvoiceDto.accessKey;
            satSerialCode = referencedInvoiceDto.satSerialCode?.Length > 60 ? referencedInvoiceDto.satSerialCode?.Substring(0, 60) : referencedInvoiceDto.satSerialCode;
            invoiceSerialCode = referencedInvoiceDto.invoiceSerialCode?.Length > 10 ? referencedInvoiceDto.invoiceSerialCode?.Substring(0, 10) : referencedInvoiceDto.invoiceSerialCode;
            cfop = referencedInvoiceDto.cfop?.Length > 20 ? referencedInvoiceDto.cfop?.Substring(0, 20) : referencedInvoiceDto.cfop;
            branchCnpj = referencedInvoiceDto.branchCnpj?.Length > 14 ? referencedInvoiceDto.branchCnpj?.Substring(0, 14) : referencedInvoiceDto.branchCnpj;
        }
    }
}

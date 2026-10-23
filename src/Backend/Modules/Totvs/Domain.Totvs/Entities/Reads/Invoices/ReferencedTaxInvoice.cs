using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// nota referenciada na capa (nulos da chave viram 0 e vazio), tabela totvs.TotvsInvoicesReferencedTaxes
    /// </summary>
    public class ReferencedTaxInvoice
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public int referencedInvoiceBranchCode { get; private set; }
        public int invoiceCode { get; private set; }
        public string invoiceSerialCode { get; private set; } = String.Empty;
        public string accessKey { get; private set; } = String.Empty;
        public string satSerialCode { get; private set; } = String.Empty;
        public DateTime? issueDate { get; private set; }
        public string branchCnpj { get; private set; } = String.Empty;

        public ReferencedTaxInvoice() { }

        public ReferencedTaxInvoice(string doc_company, DateTime integrated_or_updated_at, int branchCode, int invoiceSequence, DateTime invoiceDate, Dtos.Reads.Invoices.ReferencedTaxInvoice dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.invoiceSequence = invoiceSequence;
            this.invoiceDate = invoiceDate;

            referencedInvoiceBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.referencedInvoiceBranchCode);
            invoiceCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.invoiceCode);
            invoiceSerialCode = ConverterValidatorsExtensions.StringLengthValidation(dto.invoiceSerialCode, 3);
            accessKey = ConverterValidatorsExtensions.StringLengthValidation(dto.accessKey, 100);
            satSerialCode = ConverterValidatorsExtensions.StringLengthValidation(dto.satSerialCode, 20);
            issueDate = NullableConverters.ToNullableDateTime(dto.issueDate);
            branchCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.branchCnpj, 14);
        }
    }
}

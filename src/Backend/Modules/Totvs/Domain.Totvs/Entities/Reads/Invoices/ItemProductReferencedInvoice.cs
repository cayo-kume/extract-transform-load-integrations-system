using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// nota referenciada pelo produto devolvido (referencedBranchCode é o branchCode da nota referenciada), tabela totvs.TotvsInvoicesItemsProductsReferenced
    /// </summary>
    public class ItemProductReferencedInvoice
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public int itemSequence { get; private set; }
        public int productCode { get; private set; }
        public int invoiceSequenceItem { get; private set; }
        public string accessKey { get; private set; } = String.Empty;
        public string satSerialCode { get; private set; } = String.Empty;
        public DateTime? issueDate { get; private set; }
        public int? invoiceCode { get; private set; }
        public string invoiceSerialCode { get; private set; } = String.Empty;
        public decimal? unitNetValue { get; private set; }
        public decimal? quantity { get; private set; }
        public int? cfop { get; private set; }
        public int? referencedBranchCode { get; private set; }
        public string branchCnpj { get; private set; } = String.Empty;
        public int? terminalCode { get; private set; }

        public ItemProductReferencedInvoice() { }

        public ItemProductReferencedInvoice(string doc_company, DateTime integrated_or_updated_at, int branchCode, int invoiceSequence, DateTime invoiceDate, int itemSequence, int productCode, Dtos.Reads.Invoices.ItemProductReferencedInvoice dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.invoiceSequence = invoiceSequence;
            this.invoiceDate = invoiceDate;
            this.itemSequence = itemSequence;
            this.productCode = productCode;

            invoiceSequenceItem = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.invoiceSequenceItem);
            accessKey = ConverterValidatorsExtensions.StringLengthValidation(dto.accessKey, 100);
            satSerialCode = ConverterValidatorsExtensions.StringLengthValidation(dto.satSerialCode, 20);
            issueDate = NullableConverters.ToNullableDateTime(dto.issueDate);
            invoiceCode = NullableConverters.ToNullableInt32(dto.invoiceCode);
            invoiceSerialCode = ConverterValidatorsExtensions.StringLengthValidation(dto.invoiceSerialCode, 3);
            unitNetValue = NullableConverters.ToNullableDecimal(dto.unitNetValue);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            cfop = NullableConverters.ToNullableInt32(dto.cfop);
            referencedBranchCode = NullableConverters.ToNullableInt32(dto.branchCode);
            branchCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.branchCnpj, 14);
            terminalCode = NullableConverters.ToNullableInt32(dto.terminalCode);
        }
    }
}

using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// faixa de numeração inutilizada, tabela totvs.TotvsInvoicesDisable. serialCode e documentType são anuláveis no swagger e
    /// entram na chave: gravados como '' e 0 quando vêm nulos (NF-e 55 e NFC-e 65 têm numeração própria)
    /// </summary>
    public class InvoiceDisable
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int documentType { get; private set; }
        public string serialCode { get; private set; } = String.Empty;
        public int initialInvoiceNumber { get; private set; }
        public int finalInvoiceNumber { get; private set; }
        public string branchCnpj { get; private set; } = String.Empty;
        public long? receipt { get; private set; }
        public DateTime? receivementDate { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        public InvoiceDisable() { }

        public InvoiceDisable(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Invoices.InvoiceDisable dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            documentType = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.documentType);
            serialCode = ConverterValidatorsExtensions.StringLengthValidation(dto.serialCode, 3);
            initialInvoiceNumber = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.initialInvoiceNumber);
            finalInvoiceNumber = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.finalInvoiceNumber);
            branchCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.branchCnpj, 14);
            receipt = String.IsNullOrWhiteSpace(dto.receipt) ? null : ConverterValidatorsExtensions.ConvertToInt64Validation(dto.receipt);
            receivementDate = NullableConverters.ToNullableDateTime(dto.receivementDate);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
        }
    }
}

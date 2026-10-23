using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// código de barras do item da nota, tabela totvs.TotvsInvoicesItemsBarCodes
    /// </summary>
    public class ItemBarcode
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public int itemSequence { get; private set; }
        public string barcode { get; private set; } = String.Empty;

        public ItemBarcode() { }

        public ItemBarcode(string doc_company, DateTime integrated_or_updated_at, int branchCode, int invoiceSequence, DateTime invoiceDate, int itemSequence, Dtos.Reads.Invoices.ItemBarcode dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.invoiceSequence = invoiceSequence;
            this.invoiceDate = invoiceDate;
            this.itemSequence = itemSequence;

            barcode = ConverterValidatorsExtensions.StringLengthValidation(dto.barcode, 60);
        }
    }
}

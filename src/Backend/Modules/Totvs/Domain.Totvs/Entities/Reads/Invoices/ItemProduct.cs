using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// produto do item da nota, tabela totvs.TotvsInvoicesItemsProducts
    /// </summary>
    public class ItemProduct
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public int itemSequence { get; private set; }
        public int productCode { get; private set; }
        public string productName { get; private set; } = String.Empty;
        public int? dealerCode { get; private set; }
        public decimal? quantity { get; private set; }
        public decimal? unitGrossValue { get; private set; }
        public decimal? unitDiscountValue { get; private set; }
        public decimal? unitNetValue { get; private set; }
        public decimal? grossValue { get; private set; }
        public decimal? discountValue { get; private set; }
        public decimal? netValue { get; private set; }

        [SkipProperty]
        public List<ItemProductReferencedInvoice> referencedInvoices { get; private set; } = [];

        public ItemProduct() { }

        public ItemProduct(string doc_company, DateTime integrated_or_updated_at, int branchCode, int invoiceSequence, DateTime invoiceDate, int itemSequence, Dtos.Reads.Invoices.ItemProduct dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.invoiceSequence = invoiceSequence;
            this.invoiceDate = invoiceDate;
            this.itemSequence = itemSequence;

            productCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.productCode);
            productName = ConverterValidatorsExtensions.StringLengthValidation(dto.productName, 100);
            dealerCode = NullableConverters.ToNullableInt32(dto.dealerCode);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            unitGrossValue = NullableConverters.ToNullableDecimal(dto.unitGrossValue);
            unitDiscountValue = NullableConverters.ToNullableDecimal(dto.unitDiscountValue);
            unitNetValue = NullableConverters.ToNullableDecimal(dto.unitNetValue);
            grossValue = NullableConverters.ToNullableDecimal(dto.grossValue);
            discountValue = NullableConverters.ToNullableDecimal(dto.discountValue);
            netValue = NullableConverters.ToNullableDecimal(dto.netValue);

            // repetidos violariam a PK (chaves conferidas numa amostra real)
            referencedInvoices.AddRange((dto.referencedInvoices ?? []).Select(x => new ItemProductReferencedInvoice(doc_company, integrated_or_updated_at, branchCode, invoiceSequence, invoiceDate, itemSequence, productCode, x)).GroupBy(x => x.invoiceSequenceItem).Select(g => g.Last()));
        }
    }
}

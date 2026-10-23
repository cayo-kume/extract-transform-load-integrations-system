using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// item da nota (sequence nulo é rejeitado na validação e vira 0), tabela totvs.TotvsInvoicesItems
    /// </summary>
    public class Item
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public int sequence { get; private set; }
        public string code { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;
        public string ncm { get; private set; } = String.Empty;
        public int? cfop { get; private set; }
        public string measureUnit { get; private set; } = String.Empty;
        public decimal? quantity { get; private set; }
        public decimal? grossValue { get; private set; }
        public decimal? discountValue { get; private set; }
        public decimal? netValue { get; private set; }
        public decimal? unitGrossValue { get; private set; }
        public decimal? unitDiscountValue { get; private set; }
        public decimal? unitNetValue { get; private set; }
        public decimal? additionalValue { get; private set; }
        public decimal? freightValue { get; private set; }
        public decimal? insuranceValue { get; private set; }
        public string cest { get; private set; } = String.Empty;
        public string? additionalItemInformation { get; private set; }

        // rateDifferential (1:1): colunas com prefixo, nulas quando o objeto não vem
        public decimal? rateDifferentialCalculationBaseValue { get; private set; }
        public decimal? rateDifferentialInternalRate { get; private set; }
        public decimal? rateDifferentialInterstateRate { get; private set; }
        public decimal? rateDifferentialFcpRate { get; private set; }
        public decimal? rateDifferentialFcpValue { get; private set; }
        public decimal? rateDifferentialRecipientRateDifferentialValue { get; private set; }
        public decimal? rateDifferentialSenderRateDifferentialValue { get; private set; }

        [SkipProperty]
        public List<ItemProduct> products { get; private set; } = [];

        [SkipProperty]
        public List<ItemTax> taxes { get; private set; } = [];

        [SkipProperty]
        public List<ItemBarcode> barCodes { get; private set; } = [];

        public Item() { }

        public Item(string doc_company, DateTime integrated_or_updated_at, int branchCode, int invoiceSequence, DateTime invoiceDate, Dtos.Reads.Invoices.Item dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.invoiceSequence = invoiceSequence;
            this.invoiceDate = invoiceDate;

            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.sequence);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 60);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 100);
            ncm = ConverterValidatorsExtensions.StringLengthValidation(dto.ncm, 10);
            cfop = NullableConverters.ToNullableInt32(dto.cfop);
            measureUnit = ConverterValidatorsExtensions.StringLengthValidation(dto.measureUnit, 10);
            quantity = NullableConverters.ToNullableDecimal(dto.quantity);
            grossValue = NullableConverters.ToNullableDecimal(dto.grossValue);
            discountValue = NullableConverters.ToNullableDecimal(dto.discountValue);
            netValue = NullableConverters.ToNullableDecimal(dto.netValue);
            unitGrossValue = NullableConverters.ToNullableDecimal(dto.unitGrossValue);
            unitDiscountValue = NullableConverters.ToNullableDecimal(dto.unitDiscountValue);
            unitNetValue = NullableConverters.ToNullableDecimal(dto.unitNetValue);
            additionalValue = NullableConverters.ToNullableDecimal(dto.additionalValue);
            freightValue = NullableConverters.ToNullableDecimal(dto.freightValue);
            insuranceValue = NullableConverters.ToNullableDecimal(dto.insuranceValue);
            cest = ConverterValidatorsExtensions.StringLengthValidation(dto.cest, 10);
            additionalItemInformation = dto.additionalItemInformation;

            rateDifferentialCalculationBaseValue = NullableConverters.ToNullableDecimal(dto.rateDifferential?.calculationBaseValue);
            rateDifferentialInternalRate = NullableConverters.ToNullableDecimal(dto.rateDifferential?.internalRate);
            rateDifferentialInterstateRate = NullableConverters.ToNullableDecimal(dto.rateDifferential?.interstateRate);
            rateDifferentialFcpRate = NullableConverters.ToNullableDecimal(dto.rateDifferential?.fcpRate);
            rateDifferentialFcpValue = NullableConverters.ToNullableDecimal(dto.rateDifferential?.fcpValue);
            rateDifferentialRecipientRateDifferentialValue = NullableConverters.ToNullableDecimal(dto.rateDifferential?.recipientRateDifferentialValue);
            rateDifferentialSenderRateDifferentialValue = NullableConverters.ToNullableDecimal(dto.rateDifferential?.senderRateDifferentialValue);

            // repetidos violariam a PK (chaves conferidas numa amostra real)
            products.AddRange((dto.products ?? []).Select(x => new ItemProduct(doc_company, integrated_or_updated_at, branchCode, invoiceSequence, invoiceDate, sequence, x)).GroupBy(x => x.productCode).Select(g => g.Last()));
            taxes.AddRange((dto.taxes ?? []).Select(x => new ItemTax(doc_company, integrated_or_updated_at, branchCode, invoiceSequence, invoiceDate, sequence, x)).GroupBy(x => x.code).Select(g => g.Last()));
            barCodes.AddRange((dto.barCodes ?? []).Select(x => new ItemBarcode(doc_company, integrated_or_updated_at, branchCode, invoiceSequence, invoiceDate, sequence, x)).GroupBy(x => x.barcode.ToUpperInvariant()).Select(g => g.Last()));
        }
    }
}

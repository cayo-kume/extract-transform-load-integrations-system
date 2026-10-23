using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// imposto do item da nota (code nulo vira 0, faz parte da chave), tabela totvs.TotvsInvoicesItemsTaxes
    /// </summary>
    public class ItemTax
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public int itemSequence { get; private set; }
        public int code { get; private set; }
        public string name { get; private set; } = String.Empty;
        public string cst { get; private set; } = String.Empty;
        public decimal? taxPercentage { get; private set; }
        public decimal? calculationBasisPercentage { get; private set; }
        public decimal? calculationBasisDiscountPercentage { get; private set; }
        public decimal? calculationBasisValue { get; private set; }
        public decimal? freeValue { get; private set; }
        public decimal? otherValue { get; private set; }
        public decimal? taxValue { get; private set; }
        public string benefitCode { get; private set; } = String.Empty;
        public decimal? unencumberedValue { get; private set; }
        public int? unencumberedReason { get; private set; }
        public decimal? deferredBaseValue { get; private set; }
        public string taxClassificationCode { get; private set; } = String.Empty;

        public ItemTax() { }

        public ItemTax(string doc_company, DateTime integrated_or_updated_at, int branchCode, int invoiceSequence, DateTime invoiceDate, int itemSequence, Dtos.Reads.Invoices.ItemTax dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.invoiceSequence = invoiceSequence;
            this.invoiceDate = invoiceDate;
            this.itemSequence = itemSequence;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 60);
            cst = ConverterValidatorsExtensions.StringLengthValidation(dto.cst, 6);
            taxPercentage = NullableConverters.ToNullableDecimal(dto.taxPercentage);
            calculationBasisPercentage = NullableConverters.ToNullableDecimal(dto.calculationBasisPercentage);
            calculationBasisDiscountPercentage = NullableConverters.ToNullableDecimal(dto.calculationBasisDiscountPercentage);
            calculationBasisValue = NullableConverters.ToNullableDecimal(dto.calculationBasisValue);
            freeValue = NullableConverters.ToNullableDecimal(dto.freeValue);
            otherValue = NullableConverters.ToNullableDecimal(dto.otherValue);
            taxValue = NullableConverters.ToNullableDecimal(dto.taxValue);
            benefitCode = ConverterValidatorsExtensions.StringLengthValidation(dto.benefitCode, 20);
            unencumberedValue = NullableConverters.ToNullableDecimal(dto.unencumberedValue);
            unencumberedReason = NullableConverters.ToNullableInt32(dto.unencumberedReason);
            deferredBaseValue = NullableConverters.ToNullableDecimal(dto.deferredBaseValue);
            taxClassificationCode = ConverterValidatorsExtensions.StringLengthValidation(dto.taxClassificationCode, 10);
        }
    }
}

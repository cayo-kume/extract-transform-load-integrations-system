using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.AccountsPayable
{
    /// <summary>
    /// despesa (rateio) da duplicata, tabela totvs.TotvsDuplicatesExpenses
    /// </summary>
    public class Expense
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int supplierCode { get; private set; }
        public long duplicateCode { get; private set; }
        public int installmentCode { get; private set; }
        public int expenseCode { get; private set; }
        public string expenseName { get; private set; } = String.Empty;
        public int costCenterCode { get; private set; }
        public decimal? proratedPercentage { get; private set; }
        public decimal? proratedValue { get; private set; }

        public Expense() { }

        public Expense(string doc_company, DateTime integrated_or_updated_at, int branchCode, int supplierCode, long duplicateCode, int installmentCode, Dtos.Reads.AccountsPayable.Expense dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.supplierCode = supplierCode;
            this.duplicateCode = duplicateCode;
            this.installmentCode = installmentCode;

            expenseCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.expenseCode);
            expenseName = ConverterValidatorsExtensions.StringLengthValidation(dto.expenseName, 100);
            costCenterCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.costCenterCode);
            proratedPercentage = NullableConverters.ToNullableDecimal(dto.proratedPercentage);
            proratedValue = NullableConverters.ToNullableDecimal(dto.proratedValue);
        }
    }
}

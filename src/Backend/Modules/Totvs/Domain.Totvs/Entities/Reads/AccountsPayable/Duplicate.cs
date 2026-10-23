using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.AccountsPayable
{
    /// <summary>
    /// duplicata (parcela) a pagar (POST accounts-payable/v2/duplicates/search), tabela totvs.TotvsDuplicates; tamanhos do swagger da API Accounts Payable
    /// </summary>
    public class Duplicate
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }
        public int branchCode { get; private set; }
        public long duplicateCode { get; private set; }
        public int supplierCode { get; private set; }
        public string supplierCpfCnpj { get; private set; } = String.Empty;
        public int installmentCode { get; private set; }
        public int? bearerCode { get; private set; }
        public DateTime? entryDate { get; private set; }
        public DateTime? issueDate { get; private set; }
        public DateTime? dueDate { get; private set; }
        public DateTime? settlementDate { get; private set; }
        public DateTime? arrivalDate { get; private set; }
        public string status { get; private set; } = String.Empty;
        public decimal? duplicateValue { get; private set; }
        public decimal? feesValue { get; private set; }
        public decimal? discountValue { get; private set; }
        public decimal? paidValue { get; private set; }
        public string inclusionType { get; private set; } = String.Empty;
        public int? userInclusionCode { get; private set; }
        public string userInclusionName { get; private set; } = String.Empty;

        [SkipProperty]
        public List<Expense> expense { get; private set; } = [];

        [SkipProperty]
        public List<DuplicateClassification> classifications { get; private set; } = [];

        public Duplicate() { }

        public Duplicate(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.AccountsPayable.Duplicate dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            duplicateCode = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.duplicateCode);
            supplierCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.supplierCode);
            supplierCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.supplierCpfCnpj, 14);
            installmentCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.installmentCode);
            bearerCode = NullableConverters.ToNullableInt32(dto.bearerCode);
            entryDate = NullableConverters.ToNullableDateTime(dto.entryDate);
            issueDate = NullableConverters.ToNullableDateTime(dto.issueDate);
            dueDate = NullableConverters.ToNullableDateTime(dto.dueDate);
            settlementDate = NullableConverters.ToNullableDateTime(dto.settlementDate);
            arrivalDate = NullableConverters.ToNullableDateTime(dto.arrivalDate);
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 20);
            duplicateValue = NullableConverters.ToNullableDecimal(dto.duplicateValue);
            feesValue = NullableConverters.ToNullableDecimal(dto.feesValue);
            discountValue = NullableConverters.ToNullableDecimal(dto.discountValue);
            paidValue = NullableConverters.ToNullableDecimal(dto.paidValue);
            inclusionType = ConverterValidatorsExtensions.StringLengthValidation(dto.inclusionType, 40);
            userInclusionCode = NullableConverters.ToNullableInt32(dto.userInclusionCode);
            userInclusionName = ConverterValidatorsExtensions.StringLengthValidation(dto.userInclusionName, 60);

            // repetidos violariam a PK de cada filha (chaves conferidas numa amostra real)
            expense.AddRange((dto.expense ?? []).Select(x => new Expense(doc_company, integrated_or_updated_at, branchCode, supplierCode, duplicateCode, installmentCode, x)).GroupBy(x => (x.expenseCode, x.costCenterCode)).Select(g => g.Last()));
            classifications.AddRange((dto.classifications ?? []).Select(x => new DuplicateClassification(doc_company, integrated_or_updated_at, branchCode, supplierCode, duplicateCode, installmentCode, x)).GroupBy(x => (x.classificationType, x.classificationCode.ToUpperInvariant())).Select(g => g.Last()));
        }
    }
}

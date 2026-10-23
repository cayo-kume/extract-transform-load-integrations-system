using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.AccountsPayable
{
    /// <summary>
    /// fechamento de comissão por comissionado (POST accounts-payable/v2/comissions-paid/search), tabela totvs.TotvsCommissionsPaid; tamanhos do swagger da API Accounts Payable
    /// </summary>
    public class CommissionPaid
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int closingBranchCode { get; private set; }
        public DateTime? closingDate { get; private set; }
        public int closingCode { get; private set; }
        public string commissionedCpfCnpj { get; private set; } = String.Empty;
        public string commissionedName { get; private set; } = String.Empty;
        public int? commissionStatus { get; private set; }
        public decimal? valueBilling { get; private set; }
        public decimal? valueBillingCommission { get; private set; }
        public decimal? valueReceiving { get; private set; }
        public decimal? valueReceivingCommission { get; private set; }
        public decimal? irrfValue { get; private set; }
        public decimal? issqnValue { get; private set; }
        public decimal? valueRebate { get; private set; }
        public decimal? valueRebateBilling { get; private set; }
        public decimal? valueRebateReceiving { get; private set; }
        public decimal? discountValue { get; private set; }
        public decimal? netValue { get; private set; }
        public decimal? accountBalanceValue { get; private set; }
        public decimal? grossValue { get; private set; }
        public decimal? returnValue { get; private set; }

        [SkipProperty]
        public List<AccountMovement> accountMovements { get; private set; } = [];

        public CommissionPaid() { }

        public CommissionPaid(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.AccountsPayable.CommissionPaid dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            closingBranchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.closingBranchCode);
            closingDate = NullableConverters.ToNullableDateTime(dto.closingDate);
            closingCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.closingCode);
            commissionedCpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.commissionedCpfCnpj, 14);
            commissionedName = ConverterValidatorsExtensions.StringLengthValidation(dto.commissionedName, 60);
            commissionStatus = NullableConverters.ToNullableInt32(dto.commissionStatus);
            valueBilling = NullableConverters.ToNullableDecimal(dto.valueBilling);
            valueBillingCommission = NullableConverters.ToNullableDecimal(dto.valueBillingCommission);
            valueReceiving = NullableConverters.ToNullableDecimal(dto.valueReceiving);
            valueReceivingCommission = NullableConverters.ToNullableDecimal(dto.valueReceivingCommission);
            irrfValue = NullableConverters.ToNullableDecimal(dto.irrfValue);
            issqnValue = NullableConverters.ToNullableDecimal(dto.issqnValue);
            valueRebate = NullableConverters.ToNullableDecimal(dto.valueRebate);
            valueRebateBilling = NullableConverters.ToNullableDecimal(dto.valueRebateBilling);
            valueRebateReceiving = NullableConverters.ToNullableDecimal(dto.valueRebateReceiving);
            discountValue = NullableConverters.ToNullableDecimal(dto.discountValue);
            netValue = NullableConverters.ToNullableDecimal(dto.netValue);
            accountBalanceValue = NullableConverters.ToNullableDecimal(dto.accountBalanceValue);
            grossValue = NullableConverters.ToNullableDecimal(dto.grossValue);
            returnValue = NullableConverters.ToNullableDecimal(dto.returnValue);

            // repetidos violariam a PK de cada filha (chaves conferidas numa amostra real)
            accountMovements.AddRange((dto.accountMovements ?? []).Select((x, i) => new AccountMovement(doc_company, integrated_or_updated_at, closingBranchCode, closingCode, commissionedCpfCnpj, i + 1, x)));
        }
    }
}

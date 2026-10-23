using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.AccountsPayable
{
    /// <summary>
    /// movimento da conta do comissionado (sem chave na API: a posição é a chave), tabela totvs.TotvsCommissionsPaidAccountMovements
    /// </summary>
    public class AccountMovement
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int closingBranchCode { get; private set; }
        public int closingCode { get; private set; }
        public string commissionedCpfCnpj { get; private set; } = String.Empty;
        public int sequence { get; private set; }
        public DateTime? movementDate { get; private set; }
        public string auxiliaryHistoryDescription { get; private set; } = String.Empty;
        public string documentDescription { get; private set; } = String.Empty;
        public int? historyCode { get; private set; }
        public string historyDescription { get; private set; } = String.Empty;
        public decimal? launchValue { get; private set; }

        public AccountMovement() { }

        public AccountMovement(string doc_company, DateTime integrated_or_updated_at, int closingBranchCode, int closingCode, string commissionedCpfCnpj, int position, Dtos.Reads.AccountsPayable.AccountMovement dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.closingBranchCode = closingBranchCode;
            this.closingCode = closingCode;
            this.commissionedCpfCnpj = commissionedCpfCnpj;

            sequence = position;
            movementDate = NullableConverters.ToNullableDateTime(dto.movementDate);
            auxiliaryHistoryDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.auxiliaryHistoryDescription, 60);
            documentDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.documentDescription, 15);
            historyCode = NullableConverters.ToNullableInt32(dto.historyCode);
            historyDescription = ConverterValidatorsExtensions.StringLengthValidation(dto.historyDescription, 40);
            launchValue = NullableConverters.ToNullableDecimal(dto.launchValue);
        }
    }
}

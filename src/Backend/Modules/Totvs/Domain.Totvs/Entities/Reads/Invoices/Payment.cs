using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// pagamento da nota (documentNumber é int64 na API), tabela totvs.TotvsInvoicesPayments
    /// </summary>
    public class Payment
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public long documentNumber { get; private set; }
        public int installment { get; private set; }
        public DateTime? expirationDate { get; private set; }
        public decimal? paymentValue { get; private set; }
        public int? documentTypeCode { get; private set; }
        public string documentType { get; private set; } = String.Empty;
        public int? bearerCode { get; private set; }
        public string bearerName { get; private set; } = String.Empty;

        // cardInformation (1:1): colunas com prefixo, nulas quando o objeto não vem
        public string cardInformationAuthorizationCode { get; private set; } = String.Empty;
        public string cardInformationNsu { get; private set; } = String.Empty;
        public string cardInformationInclusionType { get; private set; } = String.Empty;
        public int? cardInformationCardOperatorCode { get; private set; }
        public string cardInformationCardOperatorName { get; private set; } = String.Empty;
        public string cardInformationCardFlag { get; private set; } = String.Empty;
        public int? cardInformationCardFlagCode { get; private set; }

        // digitalWalletInformation (1:1): colunas com prefixo, nulas quando o objeto não vem
        public string digitalWalletInformationNsu { get; private set; } = String.Empty;
        public string digitalWalletInformationTransactionId { get; private set; } = String.Empty;
        public string digitalWalletInformationProcessorId { get; private set; } = String.Empty;

        public Payment() { }

        public Payment(string doc_company, DateTime integrated_or_updated_at, int branchCode, int invoiceSequence, DateTime invoiceDate, Dtos.Reads.Invoices.Payment dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.invoiceSequence = invoiceSequence;
            this.invoiceDate = invoiceDate;

            documentNumber = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.documentNumber);
            installment = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.installment);
            expirationDate = NullableConverters.ToNullableDateTime(dto.expirationDate);
            paymentValue = NullableConverters.ToNullableDecimal(dto.paymentValue);
            documentTypeCode = NullableConverters.ToNullableInt32(dto.documentTypeCode);
            documentType = ConverterValidatorsExtensions.StringLengthValidation(dto.documentType, 30);
            bearerCode = NullableConverters.ToNullableInt32(dto.bearerCode);
            bearerName = ConverterValidatorsExtensions.StringLengthValidation(dto.bearerName, 40);

            cardInformationAuthorizationCode = ConverterValidatorsExtensions.StringLengthValidation(dto.cardInformation?.authorizationCode, 20);
            cardInformationNsu = ConverterValidatorsExtensions.StringLengthValidation(dto.cardInformation?.nsu, 20);
            cardInformationInclusionType = ConverterValidatorsExtensions.StringLengthValidation(dto.cardInformation?.inclusionType, 10);
            cardInformationCardOperatorCode = NullableConverters.ToNullableInt32(dto.cardInformation?.cardOperatorCode);
            cardInformationCardOperatorName = ConverterValidatorsExtensions.StringLengthValidation(dto.cardInformation?.cardOperatorName, 60);
            cardInformationCardFlag = ConverterValidatorsExtensions.StringLengthValidation(dto.cardInformation?.cardFlag, 60);
            cardInformationCardFlagCode = NullableConverters.ToNullableInt32(dto.cardInformation?.cardFlagCode);

            digitalWalletInformationNsu = ConverterValidatorsExtensions.StringLengthValidation(dto.digitalWalletInformation?.nsu, 100);
            digitalWalletInformationTransactionId = ConverterValidatorsExtensions.StringLengthValidation(dto.digitalWalletInformation?.transactionId, 100);
            digitalWalletInformationProcessorId = ConverterValidatorsExtensions.StringLengthValidation(dto.digitalWalletInformation?.processorId, 100);
        }
    }
}

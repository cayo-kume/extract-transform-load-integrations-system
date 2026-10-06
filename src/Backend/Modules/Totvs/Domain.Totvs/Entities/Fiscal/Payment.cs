using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Fiscal
{
    public class Payment
    {
        public int invoiceSequence { get; private set; }

        public int documentNumber { get; private set; }
        public int documentTypeCode { get; private set; }
        public int installment { get; private set; }
        public int bearerCode { get; private set; }
        public int cardOperatorCode { get; private set; }
        public int cardFlagCode { get; private set; }
        
        public DateTime expirationDate { get; private set; }
        
        public decimal paymentValue { get; private set; }
        
        public string documentType { get; private set; }
        public string bearerName { get; private set; }
        public string authorizationCode { get; private set; }
        public string cardInformationNsu { get; private set; }
        public string inclusionType { get; private set; }
        public string cardOperatorName { get; private set; }
        public string cardFlag { get; private set; }
        public string transactionId { get; private set; }
        public string digitalWalletInformationNsu { get; private set; }
        public string processorId { get; private set; }

        public Payment() { }

        public Payment(Domain.Totvs.Dtos.Fiscal.Payment paymentDto, int invoiceSequence)
        {
            this.invoiceSequence = invoiceSequence;

            //payment
            documentNumber = ConverterValidatorsExtensions.ConvertToInt32Validation(paymentDto.documentNumber);
            documentTypeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(paymentDto.documentTypeCode);
            installment = ConverterValidatorsExtensions.ConvertToInt32Validation(paymentDto.installment);
            bearerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(paymentDto.bearerCode);
            
            expirationDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(paymentDto.expirationDate);

            paymentValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(paymentDto.paymentValue);

            documentType = paymentDto.documentType?.Length > 14 ? paymentDto.documentType?.Substring(0, 14) : paymentDto.documentType;
            bearerName = paymentDto.bearerName?.Length > 14 ? paymentDto.bearerName?.Substring(0, 14) : paymentDto.bearerName;

            //card flag
            cardOperatorCode = ConverterValidatorsExtensions.ConvertToInt32Validation(paymentDto.cardInformation?.cardOperatorCode);
            cardFlagCode = ConverterValidatorsExtensions.ConvertToInt32Validation(paymentDto.cardInformation?.cardFlagCode);

            authorizationCode = paymentDto.cardInformation?.authorizationCode?.Length > 14 ? paymentDto.cardInformation?.authorizationCode?.Substring(0, 14) : paymentDto.cardInformation?.authorizationCode;
            cardInformationNsu = paymentDto.cardInformation?.nsu?.Length > 14 ? paymentDto.cardInformation?.nsu?.Substring(0, 14) : paymentDto.cardInformation?.nsu;
            inclusionType = paymentDto.cardInformation?.inclusionType?.Length > 14 ? paymentDto.cardInformation?.inclusionType?.Substring(0, 14) : paymentDto.cardInformation?.inclusionType;
            cardOperatorName = paymentDto.cardInformation?.cardOperatorName?.Length > 14 ? paymentDto.cardInformation?.cardOperatorName?.Substring(0, 14) : paymentDto.cardInformation?.cardOperatorName;
            cardFlag = paymentDto.cardInformation?.cardFlag?.Length > 14 ? paymentDto.cardInformation?.cardFlag?.Substring(0, 14) : paymentDto.cardInformation?.cardFlag;
        
            transactionId = paymentDto.digitalWalletInformation?.transactionId?.Length > 14 ? paymentDto.digitalWalletInformation?.transactionId?.Substring(0, 14) : paymentDto.digitalWalletInformation?.transactionId;
            digitalWalletInformationNsu = paymentDto.digitalWalletInformation?.nsu?.Length > 14 ? paymentDto.digitalWalletInformation?.nsu?.Substring(0, 14) : paymentDto.digitalWalletInformation?.nsu;
            processorId = paymentDto.digitalWalletInformation?.processorId?.Length > 14 ? paymentDto.digitalWalletInformation?.processorId?.Substring(0, 14) : paymentDto.digitalWalletInformation?.processorId;
        }
    }
}

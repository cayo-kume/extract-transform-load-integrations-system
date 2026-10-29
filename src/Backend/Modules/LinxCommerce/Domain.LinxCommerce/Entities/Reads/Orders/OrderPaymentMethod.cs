using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// pagamento do pedido, com o PaymentInfo (1:1) em colunas e sem os dados do cartão e do PIX, tabela linxcommerce.LinxCommerceOrdersPaymentMethods
    /// </summary>
    public class OrderPaymentMethod
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public int OrderPaymentMethodID { get; private set; }
        public string PaymentNumber { get; private set; } = String.Empty;
        public int? PaymentMethodID { get; private set; }
        public Guid? TransactionID { get; private set; }
        public string ReconciliationNumber { get; private set; } = String.Empty;
        public string Status { get; private set; } = String.Empty;
        public string IntegrationID { get; private set; } = String.Empty;
        public decimal? Amount { get; private set; }
        public decimal? AmountNoInterest { get; private set; }
        public decimal? InterestValue { get; private set; }
        public decimal? PaidAmount { get; private set; }
        public decimal? RefundAmount { get; private set; }
        public int? Installments { get; private set; }
        public decimal? InstallmentAmount { get; private set; }
        public decimal? TaxAmount { get; private set; }
        public DateTimeOffset? PaymentDate { get; private set; }
        public DateTimeOffset? CaptureDate { get; private set; }
        public DateTimeOffset? AcquiredDate { get; private set; }
        public DateTimeOffset? PaymentCancelledDate { get; private set; }
        public string PaymentInfoIdentifier { get; private set; } = String.Empty;
        public string PaymentInfoAlias { get; private set; } = String.Empty;
        public DateTimeOffset? PaymentInfoPaymentDate { get; private set; }
        public DateTimeOffset? PaymentInfoExpirationDate { get; private set; }
        public int? PaymentInfoMonth { get; private set; }
        public int? PaymentInfoYear { get; private set; }
        public string PaymentInfoTransactionNumber { get; private set; } = String.Empty;
        public string PaymentInfoAuthorizationCode { get; private set; } = String.Empty;
        public string PaymentInfoReceiptCode { get; private set; } = String.Empty;
        public string PaymentInfoReconciliationNumber { get; private set; } = String.Empty;
        public string PaymentInfoConfirmationNumber { get; private set; } = String.Empty;
        public int? PaymentInfoPaymentType { get; private set; }
        public string PaymentInfoProvider { get; private set; } = String.Empty;
        public string PaymentInfoProviderDocumentNumber { get; private set; } = String.Empty;

        public OrderPaymentMethod() { }

        public OrderPaymentMethod(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Dtos.Reads.Orders.OrderPaymentMethod dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;

            OrderPaymentMethodID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.OrderPaymentMethodID);
            PaymentNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentNumber, 50);
            PaymentMethodID = NullableConverters.ToNullableInt32(dto.PaymentMethodID);
            TransactionID = LinxCommerceConverters.ToNullableGuid(dto.TransactionID);
            ReconciliationNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.ReconciliationNumber, 100);
            Status = ConverterValidatorsExtensions.StringLengthValidation(dto.Status, 50);
            IntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.IntegrationID, 100);
            Amount = NullableConverters.ToNullableDecimal(dto.Amount);
            AmountNoInterest = NullableConverters.ToNullableDecimal(dto.AmountNoInterest);
            InterestValue = NullableConverters.ToNullableDecimal(dto.InterestValue);
            PaidAmount = NullableConverters.ToNullableDecimal(dto.PaidAmount);
            RefundAmount = NullableConverters.ToNullableDecimal(dto.RefundAmount);
            Installments = NullableConverters.ToNullableInt32(dto.Installments);
            InstallmentAmount = NullableConverters.ToNullableDecimal(dto.InstallmentAmount);
            TaxAmount = NullableConverters.ToNullableDecimal(dto.TaxAmount);
            PaymentDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.PaymentDate);
            CaptureDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.CaptureDate);
            AcquiredDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.AcquiredDate);
            PaymentCancelledDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.PaymentCancelledDate);
            PaymentInfoIdentifier = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentInfo?.Identifier, 100);
            PaymentInfoAlias = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentInfo?.Alias, 100);
            PaymentInfoPaymentDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.PaymentInfo?.PaymentDate);
            PaymentInfoExpirationDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.PaymentInfo?.ExpirationDate);
            PaymentInfoMonth = NullableConverters.ToNullableInt32(dto.PaymentInfo?.Month);
            PaymentInfoYear = NullableConverters.ToNullableInt32(dto.PaymentInfo?.Year);
            PaymentInfoTransactionNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentInfo?.TransactionNumber, 100);
            PaymentInfoAuthorizationCode = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentInfo?.AuthorizationCode, 100);
            PaymentInfoReceiptCode = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentInfo?.ReceiptCode, 100);
            PaymentInfoReconciliationNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentInfo?.ReconciliationNumber, 100);
            PaymentInfoConfirmationNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentInfo?.ConfirmationNumber, 100);
            PaymentInfoPaymentType = NullableConverters.ToNullableInt32(dto.PaymentInfo?.PaymentType);
            PaymentInfoProvider = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentInfo?.Provider, 100);
            PaymentInfoProviderDocumentNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.PaymentInfo?.ProviderDocumentNumber, 20);
        }
    }
}

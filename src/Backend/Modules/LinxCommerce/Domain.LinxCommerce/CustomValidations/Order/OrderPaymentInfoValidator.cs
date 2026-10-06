using Domain.Core.Extensions;
using Domain.LinxCommerce.CustomValidations.Validations;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderPaymentInfoValidator : AbstractValidator<OrderPaymentInfo>
    {
        public OrderPaymentInfoValidator()
        {
            RuleFor(x => x.Identifier)
                .TruncateIfExceeds(60, (x, val) => x.Identifier = val);

            RuleFor(x => x.Alias)
                .TruncateIfExceeds(50, (x, val) => x.Alias = val);

            RuleFor(x => x.Holder)
                .TruncateIfExceeds(50, (x, val) => x.Holder = val);

            RuleFor(x => x.NumberHint)
                .TruncateIfExceeds(50, (x, val) => x.NumberHint = val);

            RuleFor(x => x.SecurityCodeHint)
                .TruncateIfExceeds(50, (x, val) => x.SecurityCodeHint = val);

            RuleFor(x => x.TransactionNumber)
                .TruncateIfExceeds(50, (x, val) => x.TransactionNumber = val);

            RuleFor(x => x.AuthorizationCode)
                .TruncateIfExceeds(50, (x, val) => x.AuthorizationCode = val);

            RuleFor(x => x.ReceiptCode)
                .TruncateIfExceeds(50, (x, val) => x.ReceiptCode = val);

            RuleFor(x => x.ReconciliationNumber)
                .TruncateIfExceeds(50, (x, val) => x.ReconciliationNumber = val);

            RuleFor(x => x.ConfirmationNumber)
                .TruncateIfExceeds(50, (x, val) => x.ConfirmationNumber = val);

            RuleFor(x => x.Provider)
                .TruncateIfExceeds(50, (x, val) => x.Provider = val);

            RuleFor(x => x.ProviderDocumentNumber)
                .TruncateIfExceeds(14, (x, val) => x.ProviderDocumentNumber = val);

            RuleFor(x => x.PaymentDate)
                .NormalizeDateIfInvalid((x, val) => x.PaymentDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: PaymentDate | Value: {x.PaymentDate}, Error when trying to convert value: {x.PaymentDate.ToString()} to DateTime");

            RuleFor(x => x.ExpirationDate)
                .NormalizeDateIfInvalid((x, val) => x.ExpirationDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: ExpirationDate | Value: {x.ExpirationDate}, Error when trying to convert value: {x.ExpirationDate.ToString()} to DateTime");
        }
    }
}

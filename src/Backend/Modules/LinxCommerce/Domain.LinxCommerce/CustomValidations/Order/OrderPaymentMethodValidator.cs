using Domain.Core.Extensions;
using Domain.LinxCommerce.CustomValidations.Validations;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderPaymentMethodValidator : AbstractValidator<OrderPaymentMethod>
    {
        public OrderPaymentMethodValidator()
        {
            RuleFor(x => x.PaymentNumber)
                .TruncateIfExceeds(10, (x, val) => x.PaymentNumber = val);

            RuleFor(x => x.ReconciliationNumber)
                .TruncateIfExceeds(50, (x, val) => x.ReconciliationNumber = val);

            RuleFor(x => x.Status)
                .TruncateIfExceeds(1, (x, val) => x.Status = val);

            RuleFor(x => x.IntegrationID)
                .TruncateIfExceeds(50, (x, val) => x.IntegrationID = val);

            RuleFor(x => x.PaymentDate)
                .NormalizeDateIfInvalid((x, val) => x.PaymentDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: PaymentDate | Value: {x.PaymentDate}, Error when trying to convert value: {x.PaymentDate.ToString()} to DateTime");

            RuleFor(x => x.CaptureDate)
                .NormalizeDateIfInvalid((x, val) => x.CaptureDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: CaptureDate | Value: {x.CaptureDate}, Error when trying to convert value: {x.CaptureDate.ToString()} to DateTime");

            RuleFor(x => x.AcquiredDate)
                .NormalizeDateIfInvalid((x, val) => x.AcquiredDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: AcquiredDate | Value: {x.AcquiredDate}, Error when trying to convert value: {x.AcquiredDate.ToString()} to DateTime");

            RuleFor(x => x.PaymentCancelledDate)
                .NormalizeDateIfInvalid((x, val) => x.PaymentCancelledDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: PaymentCancelledDate | Value: {x.PaymentCancelledDate}, Error when trying to convert value: {x.PaymentCancelledDate.ToString()} to DateTime");

            RuleFor(x => x.PaymentInfo)
                .SetValidator(new OrderPaymentInfoValidator());
        }
    }
}

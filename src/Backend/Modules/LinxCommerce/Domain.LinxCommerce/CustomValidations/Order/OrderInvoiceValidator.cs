using Domain.Core.Extensions;
using Domain.LinxCommerce.CustomValidations.Validations;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderInvoiceValidator : AbstractValidator<OrderInvoice>
    {
        public OrderInvoiceValidator()
        {
            RuleFor(x => x.Code)
                .TruncateIfExceeds(44, (x, val) => x.Code = val);

            RuleFor(x => x.Url)
                .TruncateIfExceeds(800, (x, val) => x.Url = val);

            RuleFor(x => x.Series)
                .TruncateIfExceeds(10, (x, val) => x.Series = val);

            RuleFor(x => x.Number)
                .TruncateIfExceeds(10, (x, val) => x.Number = val);

            RuleFor(x => x.CFOP)
                .TruncateIfExceeds(10, (x, val) => x.CFOP = val);

            RuleFor(x => x.InvoicePdf)
                .TruncateIfExceeds(50, (x, val) => x.InvoicePdf = val);

            RuleFor(x => x.Observation)
                .TruncateIfExceeds(120, (x, val) => x.Observation = val);

            RuleFor(x => x.Operation)
                .TruncateIfExceeds(120, (x, val) => x.Operation = val);

            RuleFor(x => x.ProcessedAt)
                .NormalizeDateIfInvalid((x, val) => x.ProcessedAt = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: ProcessedAt | Value: {x.ProcessedAt}, Error when trying to convert value: {x.ProcessedAt.ToString()} to DateTime");

            RuleFor(x => x.UpdatedAt)
                .NormalizeDateIfInvalid((x, val) => x.UpdatedAt = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: UpdatedAt | Value: {x.UpdatedAt}, Error when trying to convert value: {x.UpdatedAt.ToString()} to DateTime");

            RuleFor(x => x.IssuedAt)
                .NormalizeDateIfInvalid((x, val) => x.IssuedAt = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: IssuedAt | Value: {x.IssuedAt}, Error when trying to convert value: {x.IssuedAt.ToString()} to DateTime");

            RuleFor(x => x.CreatedAt)
                .NormalizeDateIfInvalid((x, val) => x.CreatedAt = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: CreatedAt | Value: {x.CreatedAt}, Error when trying to convert value: {x.CreatedAt.ToString()} to DateTime");
        }
    }
}

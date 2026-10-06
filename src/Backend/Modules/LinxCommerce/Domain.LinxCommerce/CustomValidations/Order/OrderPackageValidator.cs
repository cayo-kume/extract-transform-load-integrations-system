using Domain.Core.Extensions;
using Domain.LinxCommerce.CustomValidations.Validations;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderPackageValidator : AbstractValidator<OrderPackage>
    {
        public OrderPackageValidator()
        {
            RuleFor(x => x.PackageNumber)
                .TruncateIfExceeds(10, (x, val) => x.PackageNumber = val);

            RuleFor(x => x.TrackingNumber)
                .TruncateIfExceeds(50, (x, val) => x.TrackingNumber = val);

            RuleFor(x => x.TrackingNumberUrl)
                .TruncateIfExceeds(800, (x, val) => x.TrackingNumberUrl = val);

            RuleFor(x => x.ShippedBy)
                .TruncateIfExceeds(50, (x, val) => x.ShippedBy = val);

            RuleFor(x => x.PackageType)
                .TruncateIfExceeds(50, (x, val) => x.PackageType = val);

            RuleFor(x => x.Source)
                .TruncateIfExceeds(50, (x, val) => x.Source = val);

            RuleFor(x => x.ShippedDate)
                .NormalizeDateIfInvalid((x, val) => x.ShippedDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: ShippedDate | Value: {x.ShippedDate}, Error when trying to convert value: {x.ShippedDate.ToString()} to DateTime");
        }
    }
}

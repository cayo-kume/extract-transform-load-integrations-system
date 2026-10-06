using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Product;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Product
{
    public class MetadataValueValidator : AbstractValidator<MetadataValue>
    {
        public MetadataValueValidator()
        {
            RuleFor(x => x.PropertyName)
                .TruncateIfExceeds(160, (x, val) => x.PropertyName = val);

            RuleFor(x => x.PropertyGroup)
                .TruncateIfExceeds(50, (x, val) => x.PropertyGroup = val);

            RuleFor(x => x.FormattedValue)
                .TruncateIfExceeds(50, (x, val) => x.FormattedValue = val);

            RuleFor(x => x.DisplayName)
                .TruncateIfExceeds(160, (x, val) => x.DisplayName = val);
        }
    }
}

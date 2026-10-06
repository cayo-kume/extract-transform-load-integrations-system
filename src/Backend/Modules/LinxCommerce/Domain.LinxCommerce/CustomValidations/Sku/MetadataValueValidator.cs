using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Sku;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Sku
{
    public class MetadataValueValidator : AbstractValidator<MetadataValue>
    {
        public MetadataValueValidator()
        {
            RuleFor(x => x.PropertyName)
                .TruncateIfExceeds(160, (x, val) => x.PropertyName = val);

            RuleFor(x => x.PropertyGroup)
                .TruncateIfExceeds(50, (x, val) => x.PropertyGroup = val);

            RuleFor(x => x.Value)
                .TruncateIfExceeds(50, (x, val) => x.Value = val);

            RuleFor(x => x.SerializedValue)
                .TruncateIfExceeds(50, (x, val) => x.SerializedValue = val);

            RuleFor(x => x.SerializedBlobValue)
                .TruncateIfExceeds(60, (x, val) => x.SerializedBlobValue = val);

            RuleFor(x => x.IntegrationID)
                .TruncateIfExceeds(50, (x, val) => x.IntegrationID = val);

            RuleFor(x => x.FormattedValue)
                .TruncateIfExceeds(50, (x, val) => x.FormattedValue = val);

            RuleFor(x => x.DisplayName)
                .TruncateIfExceeds(160, (x, val) => x.DisplayName = val);
        }
    }
}

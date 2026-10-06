using Domain.Core.Extensions;
using Domain.LinxCommerce.CustomValidations.Validations;
using Domain.LinxCommerce.Entities.Product;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Product
{
    public class MidiaValidator : AbstractValidator<Midia>
    {
        public MidiaValidator()
        {
            RuleFor(x => x.OriginalFileName)
                .TruncateIfExceeds(60, (x, val) => x.OriginalFileName = val);

            RuleFor(x => x.CreatedDate)
                .NormalizeDateIfInvalid((x, val) => x.CreatedDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: CreatedDate | Value: {x.CreatedDate}, Error when trying to convert value: {x.CreatedDate.ToString()} to DateTime");

            RuleFor(x => x.Image)
                .SetValidator(new ImageValidator());

            RuleFor(x => x.Video)
                .SetValidator(new VideoValidator());

            RuleForEach(x => x.MediaAssociations)
                .SetValidator(new MediaAssociationValidator());
        }
    }
}

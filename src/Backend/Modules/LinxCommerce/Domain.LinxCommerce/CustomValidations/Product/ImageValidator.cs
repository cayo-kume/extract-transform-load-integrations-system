using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Product;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Product
{
    public class ImageValidator : AbstractValidator<Image>
    {
        public ImageValidator()
        {
            RuleFor(x => x.RelativePath)
                .TruncateIfExceeds(800, (x, val) => x.RelativePath = val);

            RuleFor(x => x.AbsolutePath)
                .TruncateIfExceeds(800, (x, val) => x.AbsolutePath = val);
        }
    }
}

using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Product;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Product
{
    public class VideoValidator : AbstractValidator<Video>
    {
        public VideoValidator()
        {
            RuleFor(x => x.Title)
                .TruncateIfExceeds(60, (x, val) => x.Title = val);

            RuleFor(x => x.Url)
                .TruncateIfExceeds(800, (x, val) => x.Url = val);
        }
    }
}

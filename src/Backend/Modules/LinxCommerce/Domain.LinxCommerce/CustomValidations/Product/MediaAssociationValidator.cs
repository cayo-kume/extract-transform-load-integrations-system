using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Product;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Product
{
    public class MediaAssociationValidator : AbstractValidator<MediaAssociation>
    {
        public MediaAssociationValidator()
        {
            RuleFor(x => x.Path)
                .TruncateIfExceeds(50, (x, val) => x.Path = val);
        }
    }
}

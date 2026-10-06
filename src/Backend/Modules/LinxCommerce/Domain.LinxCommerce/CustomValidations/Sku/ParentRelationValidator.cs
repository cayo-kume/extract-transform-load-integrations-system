using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Sku;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Sku
{
    public class ParentRelationValidator : AbstractValidator<ParentRelation>
    {
        public ParentRelationValidator()
        {
            RuleFor(x => x.ParentSKU)
                .TruncateIfExceeds(50, (x, val) => x.ParentSKU = val);

            RuleFor(x => x.ParentIntegrationID)
                .TruncateIfExceeds(50, (x, val) => x.ParentIntegrationID = val);
        }
    }
}

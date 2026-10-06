using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Customer;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Customer
{
    public class GroupsValidator : AbstractValidator<Groups>
    {
        public GroupsValidator()
        {
            RuleFor(x => x.Title)
                .TruncateIfExceeds(60, (x, val) => x.Title = val);
        }
    }
}

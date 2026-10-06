using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderTagValidator : AbstractValidator<OrderTag>
    {
        public OrderTagValidator()
        {
            RuleFor(x => x.Alias)
                .TruncateIfExceeds(50, (x, val) => x.Alias = val);

            RuleFor(x => x.Name)
                .TruncateIfExceeds(50, (x, val) => x.Name = val);
        }
    }
}

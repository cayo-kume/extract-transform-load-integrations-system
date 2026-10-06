using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderTypeValidator : AbstractValidator<OrderType>
    {
        public OrderTypeValidator()
        {
            RuleFor(x => x.IntegrationID)
                .TruncateIfExceeds(50, (x, val) => x.IntegrationID = val);

            RuleFor(x => x.Name)
                .TruncateIfExceeds(50, (x, val) => x.Name = val);
        }
    }
}

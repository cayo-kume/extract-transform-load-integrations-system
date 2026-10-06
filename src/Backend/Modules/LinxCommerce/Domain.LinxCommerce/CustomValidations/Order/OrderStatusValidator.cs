using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Responses;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderStatusValidator : AbstractValidator<SearchOrderStatus.Result>
    {
        public OrderStatusValidator()
        {
            RuleFor(x => x.IntegrationID)
                .TruncateIfExceeds(50, (x, val) => x.IntegrationID = val);

            RuleFor(x => x.Status)
                .TruncateIfExceeds(50, (x, val) => x.Status = val);

            RuleFor(x => x.Color)
                .TruncateIfExceeds(10, (x, val) => x.Color = val);
        }
    }
}

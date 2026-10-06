using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderSellerValidator : AbstractValidator<OrderSeller>
    {
        public OrderSellerValidator()
        {
            RuleFor(x => x.EMail)
                .TruncateIfExceeds(60, (x, val) => x.EMail = val);

            RuleFor(x => x.IntegrationID)
                .TruncateIfExceeds(20, (x, val) => x.IntegrationID = val);

            RuleFor(x => x.Name)
                .TruncateIfExceeds(60, (x, val) => x.Name = val);

            RuleFor(x => x.Phone)
                .TruncateIfExceeds(20, (x, val) => x.Phone = val);
        }
    }
}

using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderDiscountValidator : AbstractValidator<OrderDiscount>
    {
        public OrderDiscountValidator()
        {
            //adicionar no repository as colunas em null
            RuleFor(x => x.Message)
                .TruncateIfExceeds(60, (x, val) => x.Message = val);

            RuleFor(x => x.Reference)
                .TruncateIfExceeds(60, (x, val) => x.Reference = val);

            RuleFor(x => x.Type)
                .TruncateIfExceeds(60, (x, val) => x.Type = val);
        }
    }
}

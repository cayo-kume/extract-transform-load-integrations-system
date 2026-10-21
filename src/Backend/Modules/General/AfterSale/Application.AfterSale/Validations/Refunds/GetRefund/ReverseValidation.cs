using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Reads.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class ReverseValidation : AbstractValidator<Reverse>
    {
        public ReverseValidation()
        {
            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));
            
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.reverse_type)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_type));

            RuleFor(x => x.created_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.created_at));

            RuleFor(x => x.refunds_count)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.refunds_count));

            RuleFor(x => x.status)
                .SetValidator(new StatusValidation());

            RuleFor(x => x.ecommerceOrder)
                .SetValidator(new EcommerceOrderValidation());

            RuleFor(x => x.ecommerce_order_seller)
                .SetValidator(new EcommerceOrderSellerValidation());
        }
    }
}

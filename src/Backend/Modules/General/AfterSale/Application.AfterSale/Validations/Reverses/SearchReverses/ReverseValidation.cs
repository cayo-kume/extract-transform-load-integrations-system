using Application.AfterSale.Validations.Me;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.SearchReverses
{
    public class ReverseValidation : AbstractValidator<Domain.AfterSale.Dtos.Reverses.SearchReverses.Reverse>
    {
        public ReverseValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.reverse_type)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_type));
            
            RuleFor(x => x.reverse_type_name)
                .MaximumLength(50)
                .WithMessage("O campo 'reverse_type_name' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reverse_type_name));

            RuleFor(x => x.created_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.created_at));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.order_id)
                .MaximumLength(50)
                .WithMessage("O campo 'order_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.order_id));

            RuleFor(x => x.ecommerce_order_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.ecommerce_order_id));

            RuleFor(x => x.total_amount)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.total_amount));

            RuleFor(x => x.refunds_count)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.refunds_count));

            RuleFor(x => x.customer)
                .SetValidator(new CustomerValidation());

            RuleFor(x => x.status)
                .SetValidator(new StatusValidation());

            RuleFor(x => x.tracking)
                .SetValidator(new TrackingValidation());
        }
    }
}

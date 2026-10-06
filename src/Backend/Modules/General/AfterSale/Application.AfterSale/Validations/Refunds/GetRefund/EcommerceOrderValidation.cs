using Domain.AfterSale.Dtos.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class EcommerceOrderValidation : AbstractValidator<EcommerceOrder>
    {
        public EcommerceOrderValidation()
        {
            RuleFor(x => x.id)
               .MustBeValidInt64()
               .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.order_id)
                .MaximumLength(50)
                .WithMessage("O campo 'order_id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.order_id));
        }
    }
}

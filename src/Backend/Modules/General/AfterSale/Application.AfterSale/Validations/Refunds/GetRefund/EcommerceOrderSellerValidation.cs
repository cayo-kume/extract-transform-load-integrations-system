using Domain.AfterSale.Dtos.Reads.Refunds.GetRefund;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.GetRefund
{
    public class EcommerceOrderSellerValidation : AbstractValidator<EcommerceOrderSeller>
    {
        public EcommerceOrderSellerValidation()
        {
            RuleFor(x => x.invoice_number)
               .MustBeValidInt64()
               .When(x => !string.IsNullOrEmpty(x.invoice_number));

            RuleFor(x => x.invoice_url)
                .MaximumLength(300)
                .WithMessage("O campo 'invoice_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.invoice_url));
        }
    }
}

using Domain.AfterSale.Dtos.Reads.Reverses.GetReverse;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class EcommerceOrderSellerValidation : AbstractValidator<EcommerceOrderSeller>
    {
        public EcommerceOrderSellerValidation()
        {
            RuleFor(x => x.invoice_number)
                .MaximumLength(20)
                .WithMessage("O campo 'invoice_number' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.invoice_number));

            RuleFor(x => x.invoice_url)
                .MaximumLength(300)
                .WithMessage("O campo 'invoice_url' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.invoice_url));
        }
    }
}

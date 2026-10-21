using Domain.AfterSale.Dtos.Reads.Refunds.SearchRefunds;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Refunds.SearchRefunds
{
    public class ReverseValidation : AbstractValidator<Reverse>
    {
        public ReverseValidation()
        {
            RuleFor(x => x.partner_store)
                .MaximumLength(50)
                .WithMessage("O campo 'partner_store' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.partner_store));
        }
    }
}

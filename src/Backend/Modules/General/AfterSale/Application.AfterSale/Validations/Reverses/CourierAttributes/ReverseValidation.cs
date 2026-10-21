using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.CourierAttributes
{
    public class ReverseValidation : AbstractValidator<Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes.Reverse>
    {
        public ReverseValidation()
        {
            RuleFor(x => x.id)
                .MaximumLength(50)
                .WithMessage("O campo 'id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id));
        }
    }
}

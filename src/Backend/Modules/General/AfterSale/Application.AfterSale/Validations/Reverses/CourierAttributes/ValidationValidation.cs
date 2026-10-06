using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.CourierAttributes
{
    public class ValidationValidation : AbstractValidator<Domain.AfterSale.Dtos.Reverses.CourierAttributes.Validation>
    {
        public ValidationValidation()
        {
            RuleFor(x => x.required)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.required));

            RuleFor(x => x.mime)
                .MaximumLength(50)
                .WithMessage("O campo 'mime' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.mime));
        }
    }
}

using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Reverses.CourierAttributes
{
    public class ValidationValidation : AbstractValidator<Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes.Validation>
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

            RuleFor(x => x.mask)
                .MaximumLength(250)
                .WithMessage("O campo 'mask' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.mask));
        }
    }
}

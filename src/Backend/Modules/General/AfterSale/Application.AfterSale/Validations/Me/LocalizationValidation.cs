using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class LocalizationValidation : AbstractValidator<Localization>
    {
        public LocalizationValidation()
        {
            RuleFor(x => x.language)
                .MaximumLength(50)
                .WithMessage("O campo 'language' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.language));

            RuleFor(x => x.timezone)
                .MaximumLength(50)
                .WithMessage("O campo 'timezone' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.timezone));

            RuleFor(x => x.currency)
                .MaximumLength(50)
                .WithMessage("O campo 'currency' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.currency));
        }
    }
}

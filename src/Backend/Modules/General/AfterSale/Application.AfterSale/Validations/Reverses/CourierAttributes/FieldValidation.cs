using Application.AfterSale.Validations.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.CourierAttributes
{
    public class FieldValidation : AbstractValidator<Domain.AfterSale.Dtos.Reverses.CourierAttributes.Field>
    {
        public FieldValidation()
        {
            RuleFor(x => x.id)
                .MaximumLength(50)
                .WithMessage("O campo 'id' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.type)
                .MaximumLength(50)
                .WithMessage("O campo 'type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.validation)
                .SetValidator(new ValidationValidation());
        }
    }
}

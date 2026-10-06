using Application.AfterSale.Validations.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.CourierAttributes
{
    public class CourierAttributeValidation : AbstractValidator<Domain.AfterSale.Dtos.Reverses.CourierAttributes.CourierAttribute>
    {
        public CourierAttributeValidation()
        {
            RuleFor(x => x.fill_form_from_file)
                .MaximumLength(50)
                .WithMessage("O campo 'fill_form_from_file' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fill_form_from_file));

            RuleForEach(x => x.fields)
                .SetValidator(new FieldValidation());

            RuleFor(x => x.reverse)
                .SetValidator(new ReverseValidation());
        }
    }
}

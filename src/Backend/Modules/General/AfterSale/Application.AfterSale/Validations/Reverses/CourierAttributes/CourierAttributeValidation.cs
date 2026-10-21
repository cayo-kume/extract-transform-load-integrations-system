using Application.AfterSale.Validations.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.CourierAttributes
{
    public class CourierAttributeValidation : AbstractValidator<Domain.AfterSale.Dtos.Reads.Reverses.CourierAttributes.CourierAttribute>
    {
        public CourierAttributeValidation()
        {
            RuleForEach(x => x.fields)
                .SetValidator(new FieldValidation());

            RuleFor(x => x.reverse)
                .SetValidator(new ReverseValidation());
        }
    }
}

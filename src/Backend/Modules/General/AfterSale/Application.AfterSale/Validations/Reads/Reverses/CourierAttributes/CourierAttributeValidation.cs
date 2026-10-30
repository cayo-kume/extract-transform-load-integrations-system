using Application.AfterSale.Validations.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Reverses.CourierAttributes
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

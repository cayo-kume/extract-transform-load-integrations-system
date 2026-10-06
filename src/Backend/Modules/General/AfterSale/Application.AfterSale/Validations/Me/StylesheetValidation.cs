using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class StylesheetValidation : AbstractValidator<Stylesheet>
    {
        public StylesheetValidation()
        {
            RuleFor(x => x.background)
                .SetValidator(new PropertyValidation());

            RuleFor(x => x.table)
                .SetValidator(new PropertyValidation());

            RuleFor(x => x.text)
                .SetValidator(new PropertyValidation());

            RuleFor(x => x.button)
                .SetValidator(new PropertyValidation());

            RuleFor(x => x.input)
                .SetValidator(new PropertyValidation());

            RuleFor(x => x.paragraph)
                .SetValidator(new PropertyValidation());

            RuleFor(x => x.label)
                .SetValidator(new PropertyValidation());

            RuleFor(x => x.whitebox)
                .SetValidator(new PropertyValidation());
        }
    }
}

using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class CreatedValidation : AbstractValidator<Created>
    {
        public CreatedValidation()
        {
            RuleFor(x => x.customer)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.agent)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.customer_by_agent)
                .SetValidator(new MailBodyContentValidation());
        }
    }
}

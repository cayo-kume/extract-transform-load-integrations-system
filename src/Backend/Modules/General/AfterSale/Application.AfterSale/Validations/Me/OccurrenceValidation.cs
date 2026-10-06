using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class OccurrenceValidation : AbstractValidator<Occurrence>
    {
        public OccurrenceValidation()
        {
            RuleFor(x => x.awaiting_customer)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.awaiting_customer_remind_deadline)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.created)
                .SetValidator(new MailBodyContentValidation());
        }
    }
}

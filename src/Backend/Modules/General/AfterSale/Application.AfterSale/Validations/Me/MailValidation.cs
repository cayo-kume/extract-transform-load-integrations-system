using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class MailValidation : AbstractValidator<Mail>
    {
        public MailValidation()
        {
            RuleFor(x => x.greeting)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.footer)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.reverse)
                .SetValidator(new ReverseValidation());

            RuleFor(x => x.refund)
                .SetValidator(new RefundValidation());

            RuleFor(x => x.retention)
                .SetValidator(new RetentionValidation());

            RuleFor(x => x.partials)
                .SetValidator(new MailPartialValidation());

            RuleFor(x => x.occurrence)
                .SetValidator(new OccurrenceValidation());

            RuleFor(x => x.cancellation)
                .SetValidator(new MailCancellationValidation());

            RuleFor(x => x.solicitation)
                .SetValidator(new SolicitationValidation());

            RuleFor(x => x.compensation)
                .SetValidator(new CompensationValidation());
        }
    }
}

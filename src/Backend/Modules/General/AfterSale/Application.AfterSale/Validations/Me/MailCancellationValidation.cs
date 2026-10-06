using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class MailCancellationValidation : AbstractValidator<MailCancellation>
    {
        public MailCancellationValidation()
        {
            RuleFor(x => x.failed)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.fully)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.partially)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.refused)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.created)
                .SetValidator(new CreatedValidation());
        }
    }
}

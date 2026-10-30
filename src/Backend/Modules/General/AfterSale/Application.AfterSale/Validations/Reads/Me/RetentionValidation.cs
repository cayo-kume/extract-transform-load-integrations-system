using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class RetentionValidation : AbstractValidator<Retention>
    {
        public RetentionValidation()
        {
            RuleFor(x => x.retention_disabled)
                .SetValidator(new MailBodyContentValidation());
        }
    }
}

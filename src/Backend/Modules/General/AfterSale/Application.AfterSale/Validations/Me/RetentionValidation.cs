using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
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

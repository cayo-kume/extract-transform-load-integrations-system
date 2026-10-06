using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class MailPartialValidation : AbstractValidator<MailPartial>
    {
        public MailPartialValidation()
        {
            RuleFor(x => x.partially_received_warning)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.new_products_include_info)
                .SetValidator(new MailBodyContentValidation());
        }
    }
}

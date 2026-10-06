using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class MessageValidation : AbstractValidator<Message>
    {
        public MessageValidation()
        {
            RuleFor(x => x.pages)
                .SetValidator(new PageValidation());

            RuleFor(x => x.mails)
                .SetValidator(new MailValidation());
        }
    }
}

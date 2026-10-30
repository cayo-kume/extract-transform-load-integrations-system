using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Me
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

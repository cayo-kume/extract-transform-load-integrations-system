using Domain.AfterSale.Dtos.Me;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class MailBodyContentValidation : AbstractValidator<MailBodyContent>
    {
        public MailBodyContentValidation()
        {
            RuleFor(x => x.subject)
                .MaximumLength(800)
                .WithMessage("O campo 'subject' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.subject));

            RuleFor(x => x.content)
                .MaximumLength(800)
                .WithMessage("O campo 'content' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.content));

            RuleFor(x => x.enabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.enabled));

            RuleFor(x => x.advancedEmailEnabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.advancedEmailEnabled));

            RuleFor(x => x.fromCustomerOriginEnabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.fromCustomerOriginEnabled));

            RuleFor(x => x.fromAgentOriginEnabled)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.fromAgentOriginEnabled));
        }
    }
}

using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class PaymentOccurrenceValidation : AbstractValidator<PaymentOccurrence>
    {
        public PaymentOccurrenceValidation()
        {
            RuleFor(x => x.gift_card)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.refund)
                .SetValidator(new MailBodyContentValidation());

            RuleFor(x => x.wire_transfer)
                .SetValidator(new MailBodyContentValidation());
        }
    }
}

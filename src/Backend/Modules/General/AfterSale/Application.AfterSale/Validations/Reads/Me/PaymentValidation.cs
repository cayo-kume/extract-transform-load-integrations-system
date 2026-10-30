using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class PaymentValidation : AbstractValidator<Payment>
    {
        public PaymentValidation()
        {
            RuleFor(x => x.cancellation)
                .SetValidator(new CompensationPaymentCancellationValidation());

            RuleFor(x => x.occurrence)
                .SetValidator(new PaymentOccurrenceValidation());
        }
    }
}

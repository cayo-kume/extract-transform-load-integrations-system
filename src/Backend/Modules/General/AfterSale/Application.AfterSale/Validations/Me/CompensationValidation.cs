using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class CompensationValidation : AbstractValidator<Compensation>
    {
        public CompensationValidation()
        {
            RuleFor(x => x.payment)
                .SetValidator(new PaymentValidation());

            RuleFor(x => x.wire_transfer)
                .SetValidator(new WireTransferValidation());
        }
    }
}

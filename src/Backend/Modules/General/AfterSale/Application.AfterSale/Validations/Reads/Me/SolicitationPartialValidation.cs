using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class SolicitationPartialValidation : AbstractValidator<SolicitationPartial>
    {
        public SolicitationPartialValidation()
        {
            RuleFor(x => x.requested_products)
                .SetValidator(new RequestedProductValidation());
        }
    }
}

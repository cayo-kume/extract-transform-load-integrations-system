using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class SolicitationValidation : AbstractValidator<Solicitation>
    {
        public SolicitationValidation()
        {
            RuleFor(x => x.partials)
                .SetValidator(new SolicitationPartialValidation());
        }
    }
}

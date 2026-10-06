using Domain.AfterSale.Dtos.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
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

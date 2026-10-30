using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class ReverseActionCustomizationLabelValidation : AbstractValidator<ReverseActionCustomizationLabel>
    {
        public ReverseActionCustomizationLabelValidation()
        {
            RuleFor(x => x.online)
                .SetValidator(new OnlineOfflineStringValidation());

            RuleFor(x => x.offline)
                .SetValidator(new OnlineOfflineStringValidation());
        }
    }
}

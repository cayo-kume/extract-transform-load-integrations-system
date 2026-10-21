using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class ReverseActionsCustomizationValidation : AbstractValidator<ReverseActionsCustomization>
    {
        public ReverseActionsCustomizationValidation()
        {
            RuleFor(x => x.online)
                .SetValidator(new OnlineOfflineBoolValidation());

            RuleFor(x => x.offline)
                .SetValidator(new OnlineOfflineBoolValidation());
        }
    }
}

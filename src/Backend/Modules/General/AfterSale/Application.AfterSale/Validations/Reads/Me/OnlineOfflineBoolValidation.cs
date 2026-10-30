using Domain.AfterSale.Dtos.Reads.Me;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class OnlineOfflineBoolValidation : AbstractValidator<OnlineOfflineBool>
    {
        public OnlineOfflineBoolValidation()
        {
            RuleFor(x => x.exchange)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.exchange));

            RuleFor(x => x.@return)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.@return));
        }
    }
}

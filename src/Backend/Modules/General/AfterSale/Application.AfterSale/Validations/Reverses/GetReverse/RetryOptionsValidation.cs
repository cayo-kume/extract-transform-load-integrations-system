using Domain.AfterSale.Dtos.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class RetryOptionsValidation : AbstractValidator<RetryOptions>
    {
        public RetryOptionsValidation()
        {
            RuleFor(x => x.can_retry)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.can_retry));
        }
    }
}

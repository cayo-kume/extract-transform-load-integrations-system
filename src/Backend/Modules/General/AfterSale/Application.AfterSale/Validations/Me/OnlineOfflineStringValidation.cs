using Domain.AfterSale.Dtos.Reads.Me;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Me
{
    public class OnlineOfflineStringValidation : AbstractValidator<OnlineOfflineString>
    {
        public OnlineOfflineStringValidation()
        {
            RuleFor(x => x.exchange)
                .MaximumLength(50)
                .WithMessage("O campo 'exchange' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.exchange));

            RuleFor(x => x.@return)
                .MaximumLength(50)
                .WithMessage("O campo '@return' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.@return));
        }
    }
}

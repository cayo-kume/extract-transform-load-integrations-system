using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class ReverseDestinationValidation : AbstractValidator<ReverseDestination>
    {
        public ReverseDestinationValidation()
        {
            RuleFor(x => x.is_omnichannel)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_omnichannel));

            RuleFor(x => x.ie_number)
                .MaximumLength(50)
                .WithMessage("O campo 'ie_number' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ie_number));

            RuleFor(x => x.display_name)
                .MaximumLength(250)
                .WithMessage("O campo 'display_name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.display_name));

            RuleFor(x => x.cnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj));

            RuleFor(x => x.address)
                .SetValidator(new AddressValidation());
        }
    }
}

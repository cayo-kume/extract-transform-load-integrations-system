using Application.AfterSale.Validations.Me;
using Domain.AfterSale.Dtos.Reads.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class DestinationDataValidation : AbstractValidator<DestinationData>
    {
        public DestinationDataValidation()
        {
            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.type)
                .MaximumLength(50)
                .WithMessage("O campo 'type' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.type_id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.type_id));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.seller_info)
                .MaximumLength(50)
                .WithMessage("O campo 'seller_info' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.seller_info));

            RuleFor(x => x.return_to_seller)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.return_to_seller));

            RuleFor(x => x.was_manually_changed)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.was_manually_changed));

            RuleFor(x => x.label)
                .MaximumLength(300)
                .WithMessage("O campo 'label' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.label));

            RuleFor(x => x.address)
                .SetValidator(new AddressValidation());
        }
    }
}

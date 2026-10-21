using Domain.AfterSale.Dtos.Reads.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class AddressValidation : AbstractValidator<Address>
    {
        public AddressValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.zip_code)
                .MaximumLength(9)
                .WithMessage("O campo 'zip_code' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.zip_code));

            RuleFor(x => x.address)
                .MaximumLength(250)
                .WithMessage("O campo 'address' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address));

            RuleFor(x => x.complement)
                .MaximumLength(50)
                .WithMessage("O campo 'complement' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complement));

            RuleFor(x => x.number)
                .MaximumLength(20)
                .WithMessage("O campo 'number' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.number));

            RuleFor(x => x.number_encrypted)
                .MaximumLength(20)
                .WithMessage("O campo 'number_encrypted' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.number_encrypted));

            RuleFor(x => x.neighborhood)
                .MaximumLength(50)
                .WithMessage("O campo 'neighborhood' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.neighborhood));

            RuleFor(x => x.city)
                .MaximumLength(40)
                .WithMessage("O campo 'city' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.city));

            RuleFor(x => x.state)
                .MaximumLength(20)
                .WithMessage("O campo 'state' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.state));

            RuleFor(x => x.country)
                .MaximumLength(80)
                .WithMessage("O campo 'country' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.country));

            RuleFor(x => x.lat)
                .MaximumLength(50)
                .WithMessage("O campo 'lat' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.lat));

            RuleFor(x => x._long)
                .MaximumLength(50)
                .WithMessage("O campo '_long' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x._long));

            RuleFor(x => x.is_restrict)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_restrict));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.deleted_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deleted_at));

            RuleFor(x => x.is_erased)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.is_erased));
        }
    }
}

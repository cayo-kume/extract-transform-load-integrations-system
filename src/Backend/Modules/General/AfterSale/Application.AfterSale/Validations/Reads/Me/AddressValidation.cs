using FluentValidation;
using Domain.AfterSale.Dtos.Reads.Me;
using Domain.Shared.Extensions;

namespace Application.AfterSale.Validations.Reads.Me
{
    public class AddressValidation : AbstractValidator<Address>
    {
        public AddressValidation()
        {
            RuleFor(x => x.deleted_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deleted_at));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.@long)
                .MaximumLength(50)
                .WithMessage("O campo 'long' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.@long));

            RuleFor(x => x.lat)
                .MaximumLength(50)
                .WithMessage("O campo 'lat' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.lat));

            RuleFor(x => x.country)
                .MaximumLength(50)
                .WithMessage("O campo 'country' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.country));

            RuleFor(x => x.state)
                .MaximumLength(50)
                .WithMessage("O campo 'state' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.state));

            RuleFor(x => x.city)
                .MaximumLength(50)
                .WithMessage("O campo 'city' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.city));

            RuleFor(x => x.number)
                .MaximumLength(20)
                .WithMessage("O campo 'number' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.number));

            RuleFor(x => x.neighborhood)
                .MaximumLength(50)
                .WithMessage("O campo 'neighborhood' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.neighborhood));

            RuleFor(x => x.complement)
                .MaximumLength(50)
                .WithMessage("O campo 'complement' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complement));

            RuleFor(x => x.address)
                .MaximumLength(250)
                .WithMessage("O campo 'address' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address));

            RuleFor(x => x.zip_code)
                .MaximumLength(9)
                .WithMessage("O campo 'zip_code' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.zip_code));

            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));
        }
    }
}

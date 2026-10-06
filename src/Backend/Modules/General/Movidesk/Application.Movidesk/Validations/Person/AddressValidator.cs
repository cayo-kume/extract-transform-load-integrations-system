using Domain.Core.Extensions;
using Domain.Movidesk.Dtos.Person;
using FluentValidation;

namespace Application.Movidesk.Validations.Person
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.addressType)
                .MaximumLength(128)
                .WithMessage("O campo 'addressType' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.addressType));

            RuleFor(x => x.country)
                .MaximumLength(128)
                .WithMessage("O campo 'country' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.country));

            RuleFor(x => x.postalCode)
                .MaximumLength(32)
                .WithMessage("O campo 'postalCode' deve ter no máximo 32 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.postalCode));

            RuleFor(x => x.state)
                .MaximumLength(128)
                .WithMessage("O campo 'state' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.state));

            RuleFor(x => x.city)
                .MaximumLength(128)
                .WithMessage("O campo 'city' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.city));

            RuleFor(x => x.district)
                .MaximumLength(128)
                .WithMessage("O campo 'district' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.district));

            RuleFor(x => x.street)
                .MaximumLength(128)
                .WithMessage("O campo 'street' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.street));

            RuleFor(x => x.number)
                .MaximumLength(32)
                .WithMessage("O campo 'number' deve ter no máximo 32 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.number));

            RuleFor(x => x.complement)
                .MaximumLength(128)
                .WithMessage("O campo 'complement' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complement));

            RuleFor(x => x.reference)
                .MaximumLength(128)
                .WithMessage("O campo 'reference' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reference));

            RuleFor(x => x.isDefault)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDefault));

            RuleFor(x => x.countryId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.countryId));
        }
    }
}

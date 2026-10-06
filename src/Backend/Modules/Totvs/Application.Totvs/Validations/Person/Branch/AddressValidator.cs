using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Person.Branch;
using FluentValidation;

namespace Application.Totvs.Validations.Person.Branch
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.sequenceCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequenceCode));

            RuleFor(x => x.addressTypeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.addressTypeCode));

            RuleFor(x => x.addressType)
                .MaximumLength(60)
                .WithMessage("O campo 'addressType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.addressType));

            RuleFor(x => x.publicPlace)
                .MaximumLength(1)
                .WithMessage("O campo 'publicPlace' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.publicPlace));

            RuleFor(x => x.address)
                .MaximumLength(250)
                .WithMessage("O campo 'address' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address));

            RuleFor(x => x.addressNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'addressNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.addressNumber));

            RuleFor(x => x.complement)
                .MaximumLength(60)
                .WithMessage("O campo 'complement' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complement));

            RuleFor(x => x.neighborhood)
                .MaximumLength(60)
                .WithMessage("O campo 'neighborhood' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.neighborhood));

            RuleFor(x => x.ibgeCityCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ibgeCityCode));

            RuleFor(x => x.cityName)
                .MaximumLength(40)
                .WithMessage("O campo 'cityName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cityName));

            RuleFor(x => x.stateAbbreviation)
                .MaximumLength(20)
                .WithMessage("O campo 'stateAbbreviation' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stateAbbreviation));

            RuleFor(x => x.cep)
                .MaximumLength(9)
                .WithMessage("O campo 'cep' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cep));

            RuleFor(x => x.bcbCountryCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.bcbCountryCode));

            RuleFor(x => x.countryName)
                .MaximumLength(80)
                .WithMessage("O campo 'countryName' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.countryName));

            RuleFor(x => x.postOfficeBox)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.postOfficeBox));

            RuleFor(x => x.reference)
                .MaximumLength(60)
                .WithMessage("O campo 'reference' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reference));
        }
    }
}

using Domain.Totvs.Dtos.SalesOrder;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrder
{
    public class ShippingAddressValidator : AbstractValidator<ShippingAddress>
    {
        public ShippingAddressValidator()
        {
            RuleFor(x => x.number)
                .MaximumLength(60)
                .WithMessage("O campo 'number' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.number));

            RuleFor(x => x.addressSequence)
                .MaximumLength(60)
                .WithMessage("O campo 'addressSequence' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.addressSequence));

            RuleFor(x => x.ibgeCityCode)
                .MaximumLength(60)
                .WithMessage("O campo 'ibgeCityCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ibgeCityCode));

            RuleFor(x => x.bcbCountryCode)
                .MaximumLength(60)
                .WithMessage("O campo 'bcbCountryCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bcbCountryCode));

            RuleFor(x => x.postOfficeBox)
                .MaximumLength(60)
                .WithMessage("O campo 'postOfficeBox' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.postOfficeBox));

            RuleFor(x => x.addressType)
                .MaximumLength(60)
                .WithMessage("O campo 'addressType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.addressType));

            RuleFor(x => x.publicPlace)
                .MaximumLength(60)
                .WithMessage("O campo 'publicPlace' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.publicPlace));

            RuleFor(x => x.address)
                .MaximumLength(250)
                .WithMessage("O campo 'address' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address));

            RuleFor(x => x.complement)
                .MaximumLength(60)
                .WithMessage("O campo 'complement' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complement));

            RuleFor(x => x.neighborhood)
                .MaximumLength(60)
                .WithMessage("O campo 'neighborhood' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.neighborhood));

            RuleFor(x => x.cityName)
                .MaximumLength(60)
                .WithMessage("O campo 'cityName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cityName));

            RuleFor(x => x.stateAbbreviation)
                .MaximumLength(60)
                .WithMessage("O campo 'stateAbbreviation' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stateAbbreviation));

            RuleFor(x => x.cep)
                .MaximumLength(9)
                .WithMessage("O campo 'cep' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cep));

            RuleFor(x => x.countryName)
                .MaximumLength(60)
                .WithMessage("O campo 'countryName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.countryName));

            RuleFor(x => x.reference)
                .MaximumLength(60)
                .WithMessage("O campo 'reference' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reference));
        }
    }
}

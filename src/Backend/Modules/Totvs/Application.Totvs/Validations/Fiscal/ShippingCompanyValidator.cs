using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class ShippingCompanyValidator : AbstractValidator<ShippingCompany>
    {
        public ShippingCompanyValidator()
        {
            RuleFor(x => x.shippingCompanyCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyCode));

            RuleFor(x => x.shippingCompanyName)
                .MaximumLength(250)
                .WithMessage("O campo 'shippingCompanyName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyName));

            RuleFor(x => x.freitghtType)
                .MaximumLength(60)
                .WithMessage("O campo 'freitghtType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.freitghtType));

            RuleFor(x => x.freitghtTypeRedispatch)
                .MaximumLength(60)
                .WithMessage("O campo 'freitghtTypeRedispatch' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.freitghtTypeRedispatch));

            RuleFor(x => x.redispatchCarrierCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.redispatchCarrierCode));

            RuleFor(x => x.redispatchCarrierName)
                .MaximumLength(250)
                .WithMessage("O campo 'redispatchCarrierName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.redispatchCarrierName));

            RuleFor(x => x.cityCodeRedispatch)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cityCodeRedispatch));

            RuleFor(x => x.cityNameRedispatch)
                .MaximumLength(250)
                .WithMessage("O campo 'cityNameRedispatch' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cityNameRedispatch));

            RuleFor(x => x.cityUfRedispatch)
                .MaximumLength(20)
                .WithMessage("O campo 'cityUfRedispatch' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cityUfRedispatch));

            RuleFor(x => x.freightValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.freightValue));

            RuleFor(x => x.shippingCompanyType)
                .MaximumLength(60)
                .WithMessage("O campo 'shippingCompanyType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyType));

            RuleFor(x => x.publicPlaceTypeName)
                .MaximumLength(60)
                .WithMessage("O campo 'publicPlaceTypeName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.publicPlaceTypeName));

            RuleFor(x => x.publicPlaceName)
                .MaximumLength(60)
                .WithMessage("O campo 'publicPlaceName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.publicPlaceName));

            RuleFor(x => x.publicPlaceCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.publicPlaceCode));

            RuleFor(x => x.neighborhood)
                .MaximumLength(60)
                .WithMessage("O campo 'neighborhood' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.neighborhood));

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

            RuleFor(x => x.postOfficeBox)
                .MaximumLength(60)
                .WithMessage("O campo 'postOfficeBox' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.postOfficeBox));

            RuleFor(x => x.numberStateRegistration)
                .MaximumLength(60)
                .WithMessage("O campo 'numberStateRegistration' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numberStateRegistration));

            RuleFor(x => x.cpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.plaqueCode)
                .MaximumLength(60)
                .WithMessage("O campo 'plaqueCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.plaqueCode));

            RuleFor(x => x.plaqueUf)
                .MaximumLength(20)
                .WithMessage("O campo 'plaqueUf' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.plaqueUf));

            RuleFor(x => x.packageNumber)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.packageNumber));

            RuleFor(x => x.grossWeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossWeight));

            RuleFor(x => x.netWeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netWeight));

            RuleFor(x => x.species)
                .MaximumLength(60)
                .WithMessage("O campo 'species' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.species));

            RuleFor(x => x.brandName)
                .MaximumLength(60)
                .WithMessage("O campo 'brandName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.brandName));

            RuleFor(x => x.trackingCode)
                .MaximumLength(60)
                .WithMessage("O campo 'trackingCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.trackingCode));
        }
    }
}

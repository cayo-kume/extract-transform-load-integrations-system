using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class PersonValidator : AbstractValidator<Domain.Totvs.Dtos.Fiscal.Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.personCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personCode));

            RuleFor(x => x.personType)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personType));

            RuleFor(x => x.personName)
                .MaximumLength(250)
                .WithMessage("O campo 'personName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personName));

            RuleFor(x => x.postOfficeBox)
                .MaximumLength(60)
                .WithMessage("O campo 'postOfficeBox' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.postOfficeBox));

            RuleFor(x => x.addressNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'addressNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.addressNumber));

            RuleFor(x => x.cep)
                .MaximumLength(9)
                .WithMessage("O campo 'cep' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cep));

            RuleFor(x => x.address)
                .MaximumLength(62500)
                .WithMessage("O campo 'address' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address));

            RuleFor(x => x.publicPlace)
                .MaximumLength(60)
                .WithMessage("O campo 'publicPlace' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.publicPlace));

            RuleFor(x => x.complement)
                .MaximumLength(60)
                .WithMessage("O campo 'complement' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complement));

            RuleFor(x => x.neighborhood)
                .MaximumLength(60)
                .WithMessage("O campo 'neighborhood' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.neighborhood));

            RuleFor(x => x.city)
                .MaximumLength(40)
                .WithMessage("O campo 'city' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.city));

            RuleFor(x => x.stateAbbreviation)
                .MaximumLength(20)
                .WithMessage("O campo 'stateAbbreviation' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stateAbbreviation));

            RuleFor(x => x.foneNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'foneNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.foneNumber));

            RuleFor(x => x.rgIe)
                .MaximumLength(60)
                .WithMessage("O campo 'rgIe' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rgIe));

            RuleFor(x => x.personCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'personCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personCpfCnpj));

            RuleFor(x => x.cityCode)
                .MaximumLength(40)
                .WithMessage("O campo 'cityCode' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cityCode));
        }
    }
}

using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.addressTypeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'addressTypeCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.sequenceCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequenceCode));

            RuleFor(x => x.addressType)
                .MaximumLength(20)
                .WithMessage("O campo 'addressType' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.addressType));

            RuleFor(x => x.publicPlace)
                .MaximumLength(10)
                .WithMessage("O campo 'publicPlace' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.publicPlace));

            RuleFor(x => x.address)
                .MaximumLength(60)
                .WithMessage("O campo 'address' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address));

            RuleFor(x => x.addressNumber)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.addressNumber));

            RuleFor(x => x.complement)
                .MaximumLength(65)
                .WithMessage("O campo 'complement' deve ter no máximo 65 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complement));

            RuleFor(x => x.neighborhood)
                .MaximumLength(60)
                .WithMessage("O campo 'neighborhood' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.neighborhood));

            RuleFor(x => x.ibgeCityCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ibgeCityCode));

            RuleFor(x => x.cityName)
                .MaximumLength(60)
                .WithMessage("O campo 'cityName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cityName));

            RuleFor(x => x.stateAbbreviation)
                .MaximumLength(2)
                .WithMessage("O campo 'stateAbbreviation' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stateAbbreviation));

            RuleFor(x => x.cep)
                .MaximumLength(8)
                .WithMessage("O campo 'cep' deve ter no máximo 8 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cep));

            RuleFor(x => x.bcbCountryCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.bcbCountryCode));

            RuleFor(x => x.countryName)
                .MaximumLength(60)
                .WithMessage("O campo 'countryName' deve ter no máximo 60 caracteres.")
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

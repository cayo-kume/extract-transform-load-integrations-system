using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.personCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personCode));

            RuleFor(x => x.personName)
                .MaximumLength(100)
                .WithMessage("O campo 'personName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personName));

            RuleFor(x => x.personType)
                .MaximumLength(5)
                .WithMessage("O campo 'personType' deve ter no máximo 5 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personType));

            RuleFor(x => x.postOfficeBox)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.postOfficeBox));

            RuleFor(x => x.addressNumber)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.addressNumber));

            RuleFor(x => x.cep)
                .MaximumLength(8)
                .WithMessage("O campo 'cep' deve ter no máximo 8 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cep));

            RuleFor(x => x.address)
                .MaximumLength(100)
                .WithMessage("O campo 'address' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address));

            RuleFor(x => x.publicPlace)
                .MaximumLength(10)
                .WithMessage("O campo 'publicPlace' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.publicPlace));

            RuleFor(x => x.complement)
                .MaximumLength(100)
                .WithMessage("O campo 'complement' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complement));

            RuleFor(x => x.neighborhood)
                .MaximumLength(60)
                .WithMessage("O campo 'neighborhood' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.neighborhood));

            RuleFor(x => x.city)
                .MaximumLength(60)
                .WithMessage("O campo 'city' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.city));

            RuleFor(x => x.stateAbbreviation)
                .MaximumLength(2)
                .WithMessage("O campo 'stateAbbreviation' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stateAbbreviation));

            RuleFor(x => x.foneNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'foneNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.foneNumber));

            RuleFor(x => x.rgIe)
                .MaximumLength(20)
                .WithMessage("O campo 'rgIe' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rgIe));

            RuleFor(x => x.personCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'personCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personCpfCnpj));

            RuleFor(x => x.cityCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cityCode));
        }
    }
}

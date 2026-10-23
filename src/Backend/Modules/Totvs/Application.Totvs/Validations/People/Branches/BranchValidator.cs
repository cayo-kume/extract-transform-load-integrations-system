using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Branches;
using FluentValidation;

namespace Application.Totvs.Validations.People.Branches
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os do swagger da API Person e das colunas do Script004_Create_Totvs_Branches_Tables
    /// </summary>
    public class BranchValidator : AbstractValidator<Branch>
    {
        public BranchValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(60)
                .WithMessage("O campo 'description' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.cnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj));

            RuleFor(x => x.personCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personCode));

            RuleFor(x => x.personName)
                .MaximumLength(60)
                .WithMessage("O campo 'personName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personName));

            RuleFor(x => x.fantasyName)
                .MaximumLength(60)
                .WithMessage("O campo 'fantasyName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fantasyName));

            RuleFor(x => x.branchGroupName)
                .MaximumLength(100)
                .WithMessage("O campo 'branchGroupName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.branchGroupName));

            RuleFor(x => x.lastchange)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastchange));

            // só vem preenchido com StartChangeDate/EndChangeDate na chamada; sem ele a filial é regravada em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleForEach(x => x.addresses)
                .SetValidator(new AddressValidator());
        }
    }
}

using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Person.Branch;
using FluentValidation;

namespace Application.Totvs.Validations.Person.Branch
{
    public class BranchValidator : AbstractValidator<Domain.Totvs.Dtos.Person.Branch.Branch>
    {
        public BranchValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.description)
                .MaximumLength(300)
                .WithMessage("O campo 'description' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.cnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj));

            RuleFor(x => x.personCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personCode));

            RuleFor(x => x.personName)
                .MaximumLength(300)
                .WithMessage("O campo 'personName' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personName));

            RuleFor(x => x.fantasyName)
                .MaximumLength(60)
                .WithMessage("O campo 'fantasyName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fantasyName));

            RuleFor(x => x.branchGroupName)
                .MaximumLength(60)
                .WithMessage("O campo 'branchGroupName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.branchGroupName));

            RuleFor(x => x.lastchange)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastchange));

            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleForEach(x => x.addresses)
                .SetValidator(new AddressValidator());
        }
    }
}

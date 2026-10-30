using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class FamiliarValidator : AbstractValidator<Familiar>
    {
        public FamiliarValidator()
        {
            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.birthDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.birthDate));

            RuleFor(x => x.gender)
                .MaximumLength(20)
                .WithMessage("O campo 'gender' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.gender));

            RuleFor(x => x.kinshipDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'kinshipDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.kinshipDescription));
        }
    }
}

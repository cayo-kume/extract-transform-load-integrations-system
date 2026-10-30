using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class ReferenceValidator : AbstractValidator<Reference>
    {
        public ReferenceValidator()
        {
            RuleFor(x => x.sequence)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'sequence' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.type)
                .MaximumLength(20)
                .WithMessage("O campo 'type' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.description)
                .MaximumLength(20)
                .WithMessage("O campo 'description' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.phoneNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'phoneNumber' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.phoneNumber));

            RuleFor(x => x.responsiblePersonName)
                .MaximumLength(60)
                .WithMessage("O campo 'responsiblePersonName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.responsiblePersonName));

            RuleFor(x => x.isInactive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isInactive));
        }
    }
}

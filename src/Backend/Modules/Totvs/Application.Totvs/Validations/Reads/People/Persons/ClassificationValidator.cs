using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class ClassificationValidator : AbstractValidator<Classification>
    {
        public ClassificationValidator()
        {
            RuleFor(x => x.typeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'typeCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.code)
                .MaximumLength(10)
                .WithMessage("O campo 'code' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.typeName)
                .MaximumLength(40)
                .WithMessage("O campo 'typeName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}

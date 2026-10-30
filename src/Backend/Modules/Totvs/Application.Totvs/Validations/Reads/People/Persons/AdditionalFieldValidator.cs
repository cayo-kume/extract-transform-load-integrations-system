using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class AdditionalFieldValidator : AbstractValidator<AdditionalField>
    {
        public AdditionalFieldValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.type)
                .MaximumLength(20)
                .WithMessage("O campo 'type' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.value)
                .MaximumLength(60)
                .WithMessage("O campo 'value' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.value));
        }
    }
}

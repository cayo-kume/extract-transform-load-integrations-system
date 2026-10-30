using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class PartnerValidator : AbstractValidator<Partner>
    {
        public PartnerValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.cpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.percentageParticipation)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentageParticipation));
        }
    }
}

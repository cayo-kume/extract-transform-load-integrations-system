using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class SocialNetworkValidator : AbstractValidator<SocialNetwork>
    {
        public SocialNetworkValidator()
        {
            RuleFor(x => x.sequence)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'sequence' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.typeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.typeCode));

            RuleFor(x => x.typeName)
                .MaximumLength(20)
                .WithMessage("O campo 'typeName' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            RuleFor(x => x.address)
                .MaximumLength(20)
                .WithMessage("O campo 'address' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.address));
        }
    }
}

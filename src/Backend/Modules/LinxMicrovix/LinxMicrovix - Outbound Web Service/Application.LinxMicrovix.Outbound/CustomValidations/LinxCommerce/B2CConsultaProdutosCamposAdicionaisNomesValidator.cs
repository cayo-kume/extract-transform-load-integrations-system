using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosCamposAdicionaisNomesValidator : AbstractValidator<B2CConsultaProdutosCamposAdicionaisNomes>
    {
        public B2CConsultaProdutosCamposAdicionaisNomesValidator()
        {
            RuleFor(x => x.id_campo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campo));

            RuleFor(x => x.ordem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem));

            RuleFor(x => x.legenda)
                .MaximumLength(30)
                .WithMessage("O campo 'legenda' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda));

            RuleFor(x => x.tipo)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.tipo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

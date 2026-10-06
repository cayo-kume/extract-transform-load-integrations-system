using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosCamposAdicionaisDetalhesValidator : AbstractValidator<B2CConsultaProdutosCamposAdicionaisDetalhes>
    {
        public B2CConsultaProdutosCamposAdicionaisDetalhesValidator()
        {
            RuleFor(x => x.id_campo_detalhe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campo_detalhe));

            RuleFor(x => x.ordem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem));

            RuleFor(x => x.descricao)
                .MaximumLength(30)
                .WithMessage("O campo 'descricao' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.id_campo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campo));

            RuleFor(x => x.ativo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaVendedoresValidator : AbstractValidator<B2CConsultaVendedores>
    {
        public B2CConsultaVendedoresValidator()
        {
            RuleFor(x => x.cod_vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_vendedor));
            RuleFor(x => x.nome_vendedor).MaximumLength(50).WithMessage("O campo 'nome_vendedor' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_vendedor));
            RuleFor(x => x.comissao_produtos).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.comissao_produtos));
            RuleFor(x => x.comissao_servicos).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.comissao_servicos));
            RuleFor(x => x.tipo).MaximumLength(1).WithMessage("O campo 'tipo' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.tipo));
            RuleFor(x => x.ativo).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.comissionado).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.comissionado));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

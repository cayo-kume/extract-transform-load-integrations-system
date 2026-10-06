using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxAcoesPromocionaisProdutosCortesiaValidator : AbstractValidator<LinxAcoesPromocionaisProdutosCortesia>
    {
        public LinxAcoesPromocionaisProdutosCortesiaValidator()
        {
            RuleFor(x => x.id_acoes_promocionais_produtos_cortesia).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_acoes_promocionais_produtos_cortesia));
            RuleFor(x => x.id_acoes_promocionais).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_acoes_promocionais));
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.id_combinacao_produto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_combinacao_produto));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

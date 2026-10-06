using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxAcoesPromocionaisCombinacaoProdutosItensValidator : AbstractValidator<LinxAcoesPromocionaisCombinacaoProdutosItens>
    {
        public LinxAcoesPromocionaisCombinacaoProdutosItensValidator()
        {
            RuleFor(x => x.id_acoes_promocionais_combinacao_produtos_itens).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_acoes_promocionais_combinacao_produtos_itens));
            RuleFor(x => x.id_combinacao_produto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_combinacao_produto));
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

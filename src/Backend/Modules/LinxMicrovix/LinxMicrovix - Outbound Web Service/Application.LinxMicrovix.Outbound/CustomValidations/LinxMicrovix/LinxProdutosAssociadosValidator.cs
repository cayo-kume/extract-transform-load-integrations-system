using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosAssociadosValidator : AbstractValidator<LinxProdutosAssociados>
    {
        public LinxProdutosAssociadosValidator()
        {
            RuleFor(x => x.codigoproduto_associado).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto_associado));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.codigoproduto_origem).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto_origem));
            RuleFor(x => x.coeficiente_desconto).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.coeficiente_desconto));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.qtde_item).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_item));
            RuleFor(x => x.item_obrigatorio).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.item_obrigatorio));
        }
    }
}

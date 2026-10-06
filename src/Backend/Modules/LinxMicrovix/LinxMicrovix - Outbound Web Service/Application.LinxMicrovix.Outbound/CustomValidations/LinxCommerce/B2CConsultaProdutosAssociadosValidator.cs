using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosAssociadosValidator : AbstractValidator<B2CConsultaProdutosAssociados>
    {
        public B2CConsultaProdutosAssociadosValidator()
        {
            RuleFor(x => x.id)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.codigoproduto_associado)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto_associado));

            RuleFor(x => x.coeficiente_desconto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.coeficiente_desconto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.qtde_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_item));

            RuleFor(x => x.item_obrigatorio)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.item_obrigatorio));
        }
    }
}

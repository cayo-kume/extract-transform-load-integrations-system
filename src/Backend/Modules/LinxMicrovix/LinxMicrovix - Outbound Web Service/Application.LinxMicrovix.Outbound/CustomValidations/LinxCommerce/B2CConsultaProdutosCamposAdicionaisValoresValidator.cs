using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosCamposAdicionaisValoresValidator : AbstractValidator<B2CConsultaProdutosCamposAdicionaisValores>
    {
        public B2CConsultaProdutosCamposAdicionaisValoresValidator()
        {
            RuleFor(x => x.id_campo_valor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campo_valor));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.id_campo_detalhe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campo_detalhe));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

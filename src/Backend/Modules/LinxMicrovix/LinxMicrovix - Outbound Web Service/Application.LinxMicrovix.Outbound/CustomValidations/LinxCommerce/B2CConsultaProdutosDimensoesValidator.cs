using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosDimensoesValidator : AbstractValidator<B2CConsultaProdutosDimensoes>
    {
        public B2CConsultaProdutosDimensoesValidator()
        {
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.altura).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.altura));
            RuleFor(x => x.comprimento).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.comprimento));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.largura).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.largura));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

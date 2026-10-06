using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosInformacoesValidator : AbstractValidator<B2CConsultaProdutosInformacoes>
    {
        public B2CConsultaProdutosInformacoesValidator()
        {
            RuleFor(x => x.id_produtos_informacoes)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_produtos_informacoes));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

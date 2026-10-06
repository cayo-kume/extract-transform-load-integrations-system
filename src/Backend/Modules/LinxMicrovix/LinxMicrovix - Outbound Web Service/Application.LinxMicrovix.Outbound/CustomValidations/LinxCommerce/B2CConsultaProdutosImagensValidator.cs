using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosImagensValidator : AbstractValidator<B2CConsultaProdutosImagens>
    {
        public B2CConsultaProdutosImagensValidator()
        {
            RuleFor(x => x.id_imagem_produto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_imagem_produto));

            RuleFor(x => x.id_imagem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_imagem));

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

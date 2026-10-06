using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaTiposCobrancaFreteValidator : AbstractValidator<B2CConsultaTiposCobrancaFrete>
    {
        public B2CConsultaTiposCobrancaFreteValidator()
        {
            RuleFor(x => x.codigo_tipo_cobranca_frete)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_tipo_cobranca_frete));

            RuleFor(x => x.nome_tipo_cobranca_frete)
                .MaximumLength(60)
                .WithMessage("O campo 'nome_tipo_cobranca_frete' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_tipo_cobranca_frete));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

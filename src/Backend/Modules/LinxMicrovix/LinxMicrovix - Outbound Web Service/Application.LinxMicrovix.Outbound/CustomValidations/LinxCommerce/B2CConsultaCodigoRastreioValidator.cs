using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaCodigoRastreioValidator : AbstractValidator<B2CConsultaCodigoRastreio>
    {
        public B2CConsultaCodigoRastreioValidator()
        {
            RuleFor(x => x.id_pedido)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_pedido));

            RuleFor(x => x.documento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.documento));

            RuleFor(x => x.serie)
                .MaximumLength(10)
                .WithMessage("O campo 'serie' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serie));

            RuleFor(x => x.codigo_rastreio)
                .MaximumLength(20)
                .WithMessage("O campo 'codigo_rastreio' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_rastreio));

            RuleFor(x => x.sequencia_volume)
                .MaximumLength(20)
                .WithMessage("O campo 'sequencia_volume' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sequencia_volume));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaPedidosTiposValidator : AbstractValidator<B2CConsultaPedidosTipos>
    {
        public B2CConsultaPedidosTiposValidator()
        {
            RuleFor(x => x.id_tipo_b2c)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_tipo_b2c));

            RuleFor(x => x.descricao)
                .MaximumLength(200)
                .WithMessage("O campo 'descricao' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.pos_timestamp_old)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.pos_timestamp_old));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

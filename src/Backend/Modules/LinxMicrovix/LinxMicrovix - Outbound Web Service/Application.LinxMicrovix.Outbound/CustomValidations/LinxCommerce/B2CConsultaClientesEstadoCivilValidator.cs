using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaClientesEstadoCivilValidator : AbstractValidator<B2CConsultaClientesEstadoCivil>
    {
        public B2CConsultaClientesEstadoCivilValidator()
        {
            RuleFor(x => x.id_estado_civil)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_estado_civil));

            RuleFor(x => x.estado_civil)
                .MaximumLength(20)
                .WithMessage("O campo 'estado_civil' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.estado_civil));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

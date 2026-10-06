using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaPlanosParcelasValidator : AbstractValidator<B2CConsultaPlanosParcelas>
    {
        public B2CConsultaPlanosParcelasValidator()
        {
            RuleFor(x => x.plano)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano));

            RuleFor(x => x.ordem_parcela)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem_parcela));

            RuleFor(x => x.prazo_parc)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.prazo_parc));

            RuleFor(x => x.id_planos_parcelas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_planos_parcelas));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

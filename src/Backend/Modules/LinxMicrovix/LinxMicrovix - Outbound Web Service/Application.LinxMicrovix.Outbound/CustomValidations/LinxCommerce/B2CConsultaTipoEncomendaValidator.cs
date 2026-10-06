using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaTipoEncomendaValidator : AbstractValidator<B2CConsultaTipoEncomenda>
    {
        public B2CConsultaTipoEncomendaValidator()
        {
            RuleFor(x => x.id_tipo_encomenda)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_tipo_encomenda));

            RuleFor(x => x.nm_tipo_encomenda)
                .MaximumLength(100)
                .WithMessage("O campo 'nm_tipo_encomenda' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nm_tipo_encomenda));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

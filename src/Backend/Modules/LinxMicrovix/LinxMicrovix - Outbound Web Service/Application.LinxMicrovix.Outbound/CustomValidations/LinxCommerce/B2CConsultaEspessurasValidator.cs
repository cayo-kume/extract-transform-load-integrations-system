using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaEspessurasValidator : AbstractValidator<B2CConsultaEspessuras>
    {
        public B2CConsultaEspessurasValidator()
        {
            RuleFor(x => x.codigo_espessura)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_espessura));

            RuleFor(x => x.nome_espessura)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_espessura' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_espessura));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaUnidadeValidator : AbstractValidator<B2CConsultaUnidade>
    {
        public B2CConsultaUnidadeValidator()
        {
            RuleFor(x => x.unidade)
                .MaximumLength(50)
                .WithMessage("O campo 'unidade' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.unidade));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

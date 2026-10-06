using Domain.Core.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.Validations.LinxCommerce
{
    public class B2CConsultaStatusValidator : AbstractValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaStatus>
    {
        public B2CConsultaStatusValidator()
        {
            RuleFor(x => x.id_status)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_status));

            RuleFor(x => x.descricao_status)
                .MaximumLength(30)
                .WithMessage("O campo 'descricao_status' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_status));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaNFeSituacaoValidator : AbstractValidator<B2CConsultaNFeSituacao>
    {
        public B2CConsultaNFeSituacaoValidator()
        {
            RuleFor(x => x.id_nfe_situacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_nfe_situacao));

            RuleFor(x => x.descricao)
                .MaximumLength(30)
                .WithMessage("O campo 'descricao' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

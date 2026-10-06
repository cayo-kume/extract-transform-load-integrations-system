using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaClientesContatosParentescoValidator : AbstractValidator<B2CConsultaClientesContatosParentesco>
    {
        public B2CConsultaClientesContatosParentescoValidator()
        {
            RuleFor(x => x.id_parentesco)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_parentesco));

            RuleFor(x => x.descricao_parentesco)
                .MaximumLength(50)
                .WithMessage("O campo 'descricao_parentesco' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_parentesco));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

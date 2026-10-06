using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosCodebarValidator : AbstractValidator<B2CConsultaProdutosCodebar>
    {
        public B2CConsultaProdutosCodebarValidator()
        {
            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.codebar)
                .MaximumLength(20)
                .WithMessage("O campo 'codebar' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codebar));

            RuleFor(x => x.id_produtos_codebar)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_produtos_codebar));

            RuleFor(x => x.principal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.principal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.tipo_codebar)
                .MaximumLength(20)
                .WithMessage("O campo 'tipo_codebar' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_codebar));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

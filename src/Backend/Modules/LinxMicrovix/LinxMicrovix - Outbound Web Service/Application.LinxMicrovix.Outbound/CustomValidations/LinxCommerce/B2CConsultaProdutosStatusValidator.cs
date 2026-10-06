using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosStatusValidator : AbstractValidator<B2CConsultaProdutosStatus>
    {
        public B2CConsultaProdutosStatusValidator()
        {
            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.referencia)
                .MaximumLength(20)
                .WithMessage("O campo 'referencia' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.referencia));

            RuleFor(x => x.ativo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.b2c)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.b2c));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

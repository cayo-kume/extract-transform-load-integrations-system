using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaClassificacaoValidator : AbstractValidator<B2CConsultaClassificacao>
    {
        public B2CConsultaClassificacaoValidator()
        {
            RuleFor(x => x.codigo_classificacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_classificacao));

            RuleFor(x => x.nome_classificacao)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_classificacao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_classificacao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

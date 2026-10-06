using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosCampanhasValidator : AbstractValidator<B2CConsultaProdutosCampanhas>
    {
        public B2CConsultaProdutosCampanhasValidator()
        {
            RuleFor(x => x.codigo_campanha)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_campanha));

            RuleFor(x => x.nome_campanha)
                .MaximumLength(60)
                .WithMessage("O campo 'nome_campanha' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_campanha));

            RuleFor(x => x.vigencia_inicio)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.vigencia_inicio));

            RuleFor(x => x.vigencia_fim)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.vigencia_fim));

            RuleFor(x => x.ativo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

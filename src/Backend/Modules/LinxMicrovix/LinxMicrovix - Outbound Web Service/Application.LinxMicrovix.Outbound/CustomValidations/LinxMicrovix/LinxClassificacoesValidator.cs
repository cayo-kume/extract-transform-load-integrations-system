using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxClassificacoesValidator : AbstractValidator<LinxClassificacoes>
    {
        public LinxClassificacoesValidator()
        {
            RuleFor(x => x.id_classificacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_classificacao));
            RuleFor(x => x.desc_classificacao).MaximumLength(50).WithMessage("O campo 'desc_classificacao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_classificacao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.codigo_integracao_ws).MaximumLength(50).WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
        }
    }
}

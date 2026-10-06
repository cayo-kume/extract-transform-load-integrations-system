using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxLinhasValidator : AbstractValidator<LinxLinhas>
    {
        public LinxLinhasValidator()
        {
            RuleFor(x => x.id_linha).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_linha));
            RuleFor(x => x.desc_linha).MaximumLength(30).WithMessage("O campo 'desc_linha' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_linha));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.codigo_integracao_ws).MaximumLength(50).WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.coeficiente_comissao).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.coeficiente_comissao));
        }
    }
}
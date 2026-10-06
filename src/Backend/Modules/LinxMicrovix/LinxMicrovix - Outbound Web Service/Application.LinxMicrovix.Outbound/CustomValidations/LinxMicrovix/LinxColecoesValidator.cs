using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxColecoesValidator : AbstractValidator<LinxColecoes>
    {
        public LinxColecoesValidator()
        {
            RuleFor(x => x.id_colecao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_colecao));
            RuleFor(x => x.desc_colecao).MaximumLength(50).WithMessage("O campo 'desc_colecao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_colecao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.codigo_integracao_ws).MaximumLength(50).WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));
        }
    }
}

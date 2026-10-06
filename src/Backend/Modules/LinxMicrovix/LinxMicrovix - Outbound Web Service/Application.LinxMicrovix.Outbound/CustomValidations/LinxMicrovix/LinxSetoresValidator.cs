using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxSetoresValidator : AbstractValidator<LinxSetores>
    {
        public LinxSetoresValidator()
        {
            RuleFor(x => x.id_setor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_setor));
            RuleFor(x => x.desc_setor).MaximumLength(30).WithMessage("O campo 'desc_setor' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_setor));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.ativo).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.codigo_integracao_ws).MaximumLength(50).WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));
        }
    }
}

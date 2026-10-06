using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxTamanhosValidator : AbstractValidator<LinxTamanhos>
    {
        public LinxTamanhosValidator()
        {
            RuleFor(x => x.id_tamanho).MaximumLength(5).WithMessage("O campo 'id_tamanho' deve ter no máximo 5 caracteres.").When(x => !string.IsNullOrEmpty(x.id_tamanho));
            RuleFor(x => x.desc_tamanho).MaximumLength(50).WithMessage("O campo 'desc_tamanho' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_tamanho));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.codigo_integracao_ws).MaximumLength(50).WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
        }
    }
}

using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMarcasValidator : AbstractValidator<LinxMarcas>
    {
        public LinxMarcasValidator()
        {
            RuleFor(x => x.id_marca).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_marca));
            RuleFor(x => x.desc_marca).MaximumLength(30).WithMessage("O campo 'desc_marca' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_marca));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.codigo_integracao_ws).MaximumLength(50).WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));
        }
    }
}

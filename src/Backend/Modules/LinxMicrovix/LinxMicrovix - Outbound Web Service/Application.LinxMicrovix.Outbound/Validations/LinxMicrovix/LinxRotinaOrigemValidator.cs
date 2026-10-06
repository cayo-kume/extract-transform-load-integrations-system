using Domain.Core.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.Validations.LinxMicrovix
{
    public class LinxRotinaOrigemValidator : AbstractValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxRotinaOrigem>
    {
        public LinxRotinaOrigemValidator()
        {
            RuleFor(x => x.codigo_rotina).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_rotina));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.descricao_rotina).MaximumLength(150).WithMessage("O campo 'descricao_rotina' deve ter no máximo 150 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_rotina));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
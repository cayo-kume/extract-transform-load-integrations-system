using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxB2CStatusValidator : AbstractValidator<LinxB2CStatus>
    {
        public LinxB2CStatusValidator()
        {
            RuleFor(x => x.id_status).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_status));
            RuleFor(x => x.descricao_status).MaximumLength(30).WithMessage("O campo 'descricao_status' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_status));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxClasseFiscalValidator : AbstractValidator<LinxClasseFiscal>
    {
        public LinxClasseFiscalValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.id_classe_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_classe_fiscal));
            RuleFor(x => x.descricao).MaximumLength(150).WithMessage("O campo 'descricao' deve ter no máximo 150 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.excluido).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.excluido));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

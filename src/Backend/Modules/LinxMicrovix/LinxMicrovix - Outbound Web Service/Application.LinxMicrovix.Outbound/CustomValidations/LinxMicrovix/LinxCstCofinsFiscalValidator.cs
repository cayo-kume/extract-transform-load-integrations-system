using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxCstCofinsFiscalValidator : AbstractValidator<LinxCstCofinsFiscal>
    {
        public LinxCstCofinsFiscalValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.id_csosn_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_csosn_fiscal));
            RuleFor(x => x.csosn_fiscal).MaximumLength(5).WithMessage("O campo 'csosn_fiscal' deve ter no máximo 5 caracteres.").When(x => !string.IsNullOrEmpty(x.csosn_fiscal));
            RuleFor(x => x.descricao).MaximumLength(200).WithMessage("O campo 'descricao' deve ter no máximo 200 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.id_csosn_fiscal_substitutiva).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_csosn_fiscal_substitutiva));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

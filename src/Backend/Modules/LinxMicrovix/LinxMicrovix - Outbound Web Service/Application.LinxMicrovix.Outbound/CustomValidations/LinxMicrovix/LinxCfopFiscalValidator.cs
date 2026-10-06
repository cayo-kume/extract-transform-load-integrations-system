using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxCfopFiscalValidator : AbstractValidator<LinxCfopFiscal>
    {
        public LinxCfopFiscalValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.id_cfop_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cfop_fiscal));
            RuleFor(x => x.cfop_fiscal).MaximumLength(5).WithMessage("O campo 'cfop_fiscal' deve ter no máximo 5 caracteres.").When(x => !string.IsNullOrEmpty(x.cfop_fiscal));
            RuleFor(x => x.descricao).MaximumLength(300).WithMessage("O campo 'descricao' deve ter no máximo 300 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.excluido).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.excluido));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxCstIcmsFiscalValidator : AbstractValidator<LinxCstIcmsFiscal>
    {
        public LinxCstIcmsFiscalValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.id_cst_icms_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cst_icms_fiscal));
            RuleFor(x => x.cst_icms_fiscal).MaximumLength(4).WithMessage("O campo 'cst_icms_fiscal' deve ter no máximo 4 caracteres.").When(x => !string.IsNullOrEmpty(x.cst_icms_fiscal));
            RuleFor(x => x.descricao).MaximumLength(150).WithMessage("O campo 'descricao' deve ter no máximo 150 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.substituicao_tributaria).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.substituicao_tributaria));
            RuleFor(x => x.excluido).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.excluido));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

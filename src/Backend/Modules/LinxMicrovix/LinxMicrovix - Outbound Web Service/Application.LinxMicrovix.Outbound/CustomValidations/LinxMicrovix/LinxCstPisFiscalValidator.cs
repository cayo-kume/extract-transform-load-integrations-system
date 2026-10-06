using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxCstPisFiscalValidator : AbstractValidator<LinxCstPisFiscal>
    {
        public LinxCstPisFiscalValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.id_cst_pis_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cst_pis_fiscal));
            RuleFor(x => x.cst_pis_fiscal).MaximumLength(4).WithMessage("O campo 'cst_pis_fiscal' deve ter no máximo 4 caracteres.").When(x => !string.IsNullOrEmpty(x.cst_pis_fiscal));
            RuleFor(x => x.descricao).MaximumLength(150).WithMessage("O campo 'descricao' deve ter no máximo 150 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.excluido).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.excluido));
            RuleFor(x => x.inicio_vigencia).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.inicio_vigencia));
            RuleFor(x => x.termino_vigencia).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.termino_vigencia));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

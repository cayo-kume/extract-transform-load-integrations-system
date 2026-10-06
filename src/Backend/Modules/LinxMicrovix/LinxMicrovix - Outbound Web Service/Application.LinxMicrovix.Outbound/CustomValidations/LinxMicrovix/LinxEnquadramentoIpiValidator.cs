using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxEnquadramentoIpiValidator : AbstractValidator<LinxEnquadramentoIpi>
    {
        public LinxEnquadramentoIpiValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.id_enquadramento_ipi).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_enquadramento_ipi));
            RuleFor(x => x.codigo).MaximumLength(3).WithMessage("O campo 'codigo' deve ter no máximo 3 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo));
            RuleFor(x => x.descricao).MaximumLength(600).WithMessage("O campo 'descricao' deve ter no máximo 600 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

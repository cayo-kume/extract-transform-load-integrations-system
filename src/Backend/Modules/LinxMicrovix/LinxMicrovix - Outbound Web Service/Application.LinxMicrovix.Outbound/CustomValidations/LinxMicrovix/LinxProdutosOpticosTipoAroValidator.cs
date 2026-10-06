using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosOpticosTipoAroValidator : AbstractValidator<LinxProdutosOpticosTipoAro>
    {
        public LinxProdutosOpticosTipoAroValidator()
        {
            RuleFor(x => x.id_tipo_aro).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_tipo_aro));
            RuleFor(x => x.tipo_aro).MaximumLength(100).WithMessage("O campo 'tipo_aro' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.tipo_aro));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

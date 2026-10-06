using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosOpticosTipoValidator : AbstractValidator<LinxProdutosOpticosTipo>
    {
        public LinxProdutosOpticosTipoValidator()
        {
            RuleFor(x => x.id_produtos_opticos_tipo).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_produtos_opticos_tipo));
            RuleFor(x => x.tipo).MaximumLength(100).WithMessage("O campo 'tipo' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.tipo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

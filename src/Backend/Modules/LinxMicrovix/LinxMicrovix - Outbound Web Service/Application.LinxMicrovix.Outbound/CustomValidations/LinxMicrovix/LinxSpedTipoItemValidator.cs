using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxSpedTipoItemValidator : AbstractValidator<LinxSpedTipoItem>
    {
        public LinxSpedTipoItemValidator()
        {
            RuleFor(x => x.id_sped_tipo_item).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_sped_tipo_item));
            RuleFor(x => x.descricao).MaximumLength(60).WithMessage("O campo 'descricao' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.codigo).MaximumLength(2).WithMessage("O campo 'codigo' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}

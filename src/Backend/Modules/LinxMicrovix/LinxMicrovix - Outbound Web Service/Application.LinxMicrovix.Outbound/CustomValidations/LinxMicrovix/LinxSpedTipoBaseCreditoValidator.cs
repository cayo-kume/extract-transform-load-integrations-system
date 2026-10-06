using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxSpedTipoBaseCreditoValidator : AbstractValidator<LinxSpedTipoBaseCredito>
    {
        public LinxSpedTipoBaseCreditoValidator()
        {
            RuleFor(x => x.id_sped_tipo_base_credito).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_sped_tipo_base_credito));
            RuleFor(x => x.codigo_sped_tipo_base_credito).MaximumLength(2).WithMessage("O campo 'codigo_sped_tipo_base_credito' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_sped_tipo_base_credito));
            RuleFor(x => x.descricao).MaximumLength(150).WithMessage("O campo 'descricao' deve ter no máximo 150 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

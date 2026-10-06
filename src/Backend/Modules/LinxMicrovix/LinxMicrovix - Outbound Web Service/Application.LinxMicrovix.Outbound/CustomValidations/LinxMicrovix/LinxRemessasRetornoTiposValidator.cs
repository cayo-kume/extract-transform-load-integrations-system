using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxRemessasRetornoTiposValidator : AbstractValidator<LinxRemessasRetornoTipos>
    {
        public LinxRemessasRetornoTiposValidator()
        {
            RuleFor(x => x.id_remessa_retorno_tipo).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessa_retorno_tipo));
            RuleFor(x => x.descricao).MaximumLength(50).WithMessage("O campo 'descricao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
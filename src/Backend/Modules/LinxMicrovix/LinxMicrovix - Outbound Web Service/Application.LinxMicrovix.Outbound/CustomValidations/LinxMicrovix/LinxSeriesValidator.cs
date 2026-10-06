using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxSeriesValidator : AbstractValidator<LinxSeries>
    {
        public LinxSeriesValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.serie).MaximumLength(10).WithMessage("O campo 'serie' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.serie));
            RuleFor(x => x.id_modelo_nf).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_modelo_nf));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

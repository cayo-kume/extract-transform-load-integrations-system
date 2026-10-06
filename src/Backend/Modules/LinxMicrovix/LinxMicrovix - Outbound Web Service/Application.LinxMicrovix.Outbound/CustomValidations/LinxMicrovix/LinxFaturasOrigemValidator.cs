using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxFaturasOrigemValidator : AbstractValidator<LinxFaturasOrigem>
    {
        public LinxFaturasOrigemValidator()
        {
            RuleFor(x => x.codigo_fatura).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigo_fatura));
            RuleFor(x => x.codigo_fatura_origem).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigo_fatura_origem));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}

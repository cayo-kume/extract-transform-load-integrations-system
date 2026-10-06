using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Stone.Validations
{
    public class StoneMetricsValidator : AbstractValidator<Domain.Stone.Dtos.Metrics>
    {
        public StoneMetricsValidator()
        {
            RuleFor(x => x.totalDepth).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.totalDepth));
            RuleFor(x => x.totalSizes).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.totalSizes));
            RuleFor(x => x.totalWidth).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.totalWidth));
            RuleFor(x => x.totalCubage).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.totalCubage));
            RuleFor(x => x.totalHeight).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.totalHeight));
            RuleFor(x => x.totalWeight).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.totalWeight));
        }
    }
}

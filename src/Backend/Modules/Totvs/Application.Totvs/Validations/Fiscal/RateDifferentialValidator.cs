using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class RateDifferentialValidator : AbstractValidator<RateDifferential>
    {
        public RateDifferentialValidator()
        {
            RuleFor(x => x.calculationBaseValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.calculationBaseValue));

            RuleFor(x => x.internalRate)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.internalRate));

            RuleFor(x => x.interstateRate)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.interstateRate));

            RuleFor(x => x.fcpValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.fcpValue));

            RuleFor(x => x.fcpRate)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.fcpRate));

            RuleFor(x => x.recipientRateDifferentialValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.recipientRateDifferentialValue));

            RuleFor(x => x.senderRateDifferentialValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.senderRateDifferentialValue));
        }
    }
}

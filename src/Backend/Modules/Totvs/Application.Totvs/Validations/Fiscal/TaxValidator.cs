using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class TaxValidator : AbstractValidator<Tax>
    {
        public TaxValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.cst)
                .MaximumLength(60)
                .WithMessage("O campo 'cst' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cst));

            RuleFor(x => x.taxPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.taxPercentage));

            RuleFor(x => x.calculationBasisPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.calculationBasisPercentage));

            RuleFor(x => x.calculationBasisDiscountPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.calculationBasisDiscountPercentage));

            RuleFor(x => x.calculationBasisValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.calculationBasisValue));

            RuleFor(x => x.freeValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.freeValue));

            RuleFor(x => x.otherValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.otherValue));

            RuleFor(x => x.taxValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.taxValue));

            RuleFor(x => x.benefitCode)
                .MaximumLength(60)
                .WithMessage("O campo 'benefitCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.benefitCode));

            RuleFor(x => x.unencumberedValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unencumberedValue));

            RuleFor(x => x.unencumberedReason)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unencumberedReason));

            RuleFor(x => x.deferredBaseValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.deferredBaseValue));
        }
    }
}

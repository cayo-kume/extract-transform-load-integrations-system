using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class ProductItemValidator : AbstractValidator<ProductItem>
    {
        public ProductItemValidator()
        {
            RuleFor(x => x.sequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sequence));

            RuleFor(x => x.code)
                .MaximumLength(44)
                .WithMessage("O campo 'code' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.ncm)
                .MaximumLength(60)
                .WithMessage("O campo 'ncm' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ncm));

            RuleFor(x => x.cfop)
                .MaximumLength(20)
                .WithMessage("O campo 'cfop' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cfop));

            RuleFor(x => x.measureUnit)
                .MaximumLength(60)
                .WithMessage("O campo 'measureUnit' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.measureUnit));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.grossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossValue));

            RuleFor(x => x.discountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountValue));

            RuleFor(x => x.netValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netValue));

            RuleFor(x => x.unitGrossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitGrossValue));

            RuleFor(x => x.unitDiscountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitDiscountValue));

            RuleFor(x => x.unitNetValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitNetValue));

            RuleFor(x => x.additionalValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.additionalValue));

            RuleFor(x => x.freightValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.freightValue));

            RuleFor(x => x.insuranceValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.insuranceValue));

            RuleFor(x => x.cest)
                .MaximumLength(60)
                .WithMessage("O campo 'cest' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cest));

            RuleFor(x => x.additionalItemInformation)
                .MaximumLength(60)
                .WithMessage("O campo 'additionalItemInformation' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.additionalItemInformation));

            RuleForEach(x => x.products)
                .SetValidator(new ProductValidator());

            RuleForEach(x => x.taxes)
                .SetValidator(new TaxValidator());

            RuleForEach(x => x.barCodes)
                .SetValidator(new BarcodeValidator());

            RuleFor(x => x.rateDifferential)
                .SetValidator(new RateDifferentialValidator());
        }
    }
}

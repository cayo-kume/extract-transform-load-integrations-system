using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class ProductValidator : AbstractValidator<Domain.Totvs.Dtos.Fiscal.Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.productCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.productCode));

            RuleFor(x => x.productName)
                .MaximumLength(250)
                .WithMessage("O campo 'productName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.productName));

            RuleFor(x => x.dealerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.dealerCode));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.unitGrossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitGrossValue));

            RuleFor(x => x.unitDiscountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitDiscountValue));

            RuleFor(x => x.unitNetValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitNetValue));

            RuleFor(x => x.grossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossValue));

            RuleFor(x => x.discountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountValue));

            RuleFor(x => x.netValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netValue));

            RuleForEach(x => x.referencedInvoices)
                .SetValidator(new ReferencedInvoiceValidator());
        }
    }
}

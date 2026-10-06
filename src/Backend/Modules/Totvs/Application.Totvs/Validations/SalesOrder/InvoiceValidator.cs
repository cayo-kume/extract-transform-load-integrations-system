using Application.Totvs.Validations.Fiscal;
using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.SalesOrder;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrder
{
    public class InvoiceValidator : AbstractValidator<Invoice>
    {
        public InvoiceValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.packageNumber)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.packageNumber));

            RuleFor(x => x.grossWeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossWeight));

            RuleFor(x => x.netWeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netWeight));

            RuleFor(x => x.discountPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountPercentage));

            RuleFor(x => x.quantity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.productValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.productValue));

            RuleFor(x => x.additionalValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.additionalValue));

            RuleFor(x => x.shippingValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.shippingValue));

            RuleFor(x => x.InsuranceValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.InsuranceValue));

            RuleFor(x => x.ipiValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.ipiValue));

            RuleFor(x => x.totalValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.totalValue));

            RuleFor(x => x.transactionBranchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transactionBranchCode));

            RuleFor(x => x.transactionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transactionCode));

            RuleFor(x => x.issueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.issueDate));

            RuleFor(x => x.transactionDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.transactionDate));

            RuleFor(x => x.accessKey)
                .MaximumLength(44)
                .WithMessage("O campo 'accessKey' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.accessKey));

            RuleFor(x => x.serial)
                .MaximumLength(60)
                .WithMessage("O campo 'serial' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serial));

            RuleFor(x => x.status)
                .MaximumLength(60)
                .WithMessage("O campo 'status' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.shippingCompanyName)
                .MaximumLength(250)
                .WithMessage("O campo 'shippingCompanyName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyName));

            RuleFor(x => x.trackingCode)
                .MaximumLength(60)
                .WithMessage("O campo 'trackingCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.trackingCode));

            RuleFor(x => x.electronic)
                .SetValidator(new ElectronicValidator());
        }
    }
}

using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class InvoiceValidator : AbstractValidator<Invoice>
    {
        public InvoiceValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.serial)
                .MaximumLength(3)
                .WithMessage("O campo 'serial' deve ter no máximo 3 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serial));

            RuleFor(x => x.accessKey)
                .MaximumLength(200)
                .WithMessage("O campo 'accessKey' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.accessKey));

            RuleFor(x => x.issueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.issueDate));

            RuleFor(x => x.status)
                .MaximumLength(20)
                .WithMessage("O campo 'status' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.shippingCompanyName)
                .MaximumLength(60)
                .WithMessage("O campo 'shippingCompanyName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyName));

            RuleFor(x => x.packageNumber)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.packageNumber));

            RuleFor(x => x.grossWeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossWeight));

            RuleFor(x => x.netWeight)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netWeight));

            RuleFor(x => x.trackingCode)
                .MaximumLength(40)
                .WithMessage("O campo 'trackingCode' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.trackingCode));

            RuleFor(x => x.discountPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountPercentage));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
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

            RuleFor(x => x.transactionDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.transactionDate));

            RuleFor(x => x.transactionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transactionCode));

            RuleFor(x => x.electronic!)
                .SetValidator(new ElectronicValidator())
                .When(x => x.electronic is not null);
        }
    }
}

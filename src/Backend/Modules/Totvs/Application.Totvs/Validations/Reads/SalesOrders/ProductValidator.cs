using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.productCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'productCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.referenceCode)
                .MaximumLength(60)
                .WithMessage("O campo 'referenceCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.referenceCode));

            RuleFor(x => x.referenceName)
                .MaximumLength(100)
                .WithMessage("O campo 'referenceName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.referenceName));

            RuleFor(x => x.productSku)
                .MaximumLength(40)
                .WithMessage("O campo 'productSku' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.productSku));

            RuleFor(x => x.colorCode)
                .MaximumLength(10)
                .WithMessage("O campo 'colorCode' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.colorCode));

            RuleFor(x => x.colorName)
                .MaximumLength(40)
                .WithMessage("O campo 'colorName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.colorName));

            RuleFor(x => x.sizeName)
                .MaximumLength(10)
                .WithMessage("O campo 'sizeName' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sizeName));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.toSettleQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.toSettleQuantity));

            RuleFor(x => x.settledQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.settledQuantity));

            RuleFor(x => x.canceledQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.canceledQuantity));

            RuleFor(x => x.extraQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.extraQuantity));

            RuleFor(x => x.pendingQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.pendingQuantity));

            RuleFor(x => x.originalPrice)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.originalPrice));

            RuleFor(x => x.price)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.price));

            RuleFor(x => x.discountPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountPercentage));

            RuleFor(x => x.lastChangeUserInsert)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.lastChangeUserInsert));

            RuleFor(x => x.lastChangeDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastChangeDate));

            RuleFor(x => x.billingForecastDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.billingForecastDate));

            RuleFor(x => x.insertDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.insertDate));

            RuleFor(x => x.cancellationLimitDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.cancellationLimitDate));

            RuleFor(x => x.billingLimitDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.billingLimitDate));

            RuleFor(x => x.lastBillingDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastBillingDate));

            RuleFor(x => x.rollMeasureCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.rollMeasureCode));

            RuleFor(x => x.rollMeasureDescription)
                .MaximumLength(80)
                .WithMessage("O campo 'rollMeasureDescription' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rollMeasureDescription));

            RuleFor(x => x.customerOrderCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.customerOrderCode));

            RuleFor(x => x.customerProductCode)
                .MaximumLength(20)
                .WithMessage("O campo 'customerProductCode' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerProductCode));

            RuleForEach(x => x.batchItems)
                .SetValidator(new BatchItemValidator());
        }
    }
}

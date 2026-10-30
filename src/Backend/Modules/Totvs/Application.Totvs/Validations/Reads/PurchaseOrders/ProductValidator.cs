using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.PurchaseOrders
{
    /// <summary>
    /// só registra mensagens no log; o swagger da API Purchase Order não traz tamanhos: os do código antigo, conferidos numa amostra real (colunas do script de tabelas de pedido de compras)
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
                .MaximumLength(100)
                .WithMessage("O campo 'name' deve ter no máximo 100 caracteres.")
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
                .MaximumLength(60)
                .WithMessage("O campo 'productSku' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.productSku));

            RuleFor(x => x.colorCode)
                .MaximumLength(20)
                .WithMessage("O campo 'colorCode' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.colorCode));

            RuleFor(x => x.colorName)
                .MaximumLength(60)
                .WithMessage("O campo 'colorName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.colorName));

            RuleFor(x => x.sizeName)
                .MaximumLength(20)
                .WithMessage("O campo 'sizeName' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sizeName));

            RuleFor(x => x.measuredUnit)
                .MaximumLength(10)
                .WithMessage("O campo 'measuredUnit' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.measuredUnit));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.lowQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.lowQuantity));

            RuleFor(x => x.settledQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.settledQuantity));

            RuleFor(x => x.canceledQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.canceledQuantity));

            RuleFor(x => x.extraQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.extraQuantity));

            RuleFor(x => x.deliveryForecastDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deliveryForecastDate));

            RuleFor(x => x.deliveryDeadlineDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deliveryDeadlineDate));

            RuleFor(x => x.value)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.value));

            RuleFor(x => x.discountPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountPercentage));

            RuleFor(x => x.conversionUnit)
                .MaximumLength(10)
                .WithMessage("O campo 'conversionUnit' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.conversionUnit));

            RuleFor(x => x.conversionQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.conversionQuantity));
        }
    }
}

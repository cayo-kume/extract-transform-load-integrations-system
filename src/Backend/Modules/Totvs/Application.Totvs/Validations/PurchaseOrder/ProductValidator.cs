using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.PurchaseOrder
{
    public class ProductValidator : AbstractValidator<Domain.Totvs.Dtos.PurchaseOrder.Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.productCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.productCode));

            RuleFor(x => x.quantity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.settledQuantity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.settledQuantity));

            RuleFor(x => x.lowQuantity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.lowQuantity));

            RuleFor(x => x.canceledQuantity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.canceledQuantity));

            RuleFor(x => x.extraQuantity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.extraQuantity));

            RuleFor(x => x.conversionQuantity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.conversionQuantity));

            RuleFor(x => x.discountPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountPercentage));

            RuleFor(x => x.value)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.value));

            RuleFor(x => x.deliveryForecastDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deliveryForecastDate));

            RuleFor(x => x.deliveryDeadlineDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deliveryDeadlineDate));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.referenceCode)
                .MaximumLength(60)
                .WithMessage("O campo 'referenceCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.referenceCode));

            RuleFor(x => x.referenceName)
                .MaximumLength(250)
                .WithMessage("O campo 'referenceName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.referenceName));

            RuleFor(x => x.productSku)
                .MaximumLength(60)
                .WithMessage("O campo 'productSku' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.productSku));

            RuleFor(x => x.colorCode)
                .MaximumLength(60)
                .WithMessage("O campo 'colorCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.colorCode));

            RuleFor(x => x.colorName)
                .MaximumLength(60)
                .WithMessage("O campo 'colorName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.colorName));

            RuleFor(x => x.sizeName)
                .MaximumLength(60)
                .WithMessage("O campo 'sizeName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sizeName));

            RuleFor(x => x.measuredUnit)
                .MaximumLength(60)
                .WithMessage("O campo 'measuredUnit' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.measuredUnit));

            RuleFor(x => x.conversionUnit)
                .MaximumLength(60)
                .WithMessage("O campo 'conversionUnit' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.conversionUnit));

        }
    }
}

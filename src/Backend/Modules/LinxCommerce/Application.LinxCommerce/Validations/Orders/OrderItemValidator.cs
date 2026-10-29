using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersItems (scripts de tabelas do módulo)
    /// </summary>
    public class OrderItemValidator : AbstractValidator<OrderItem>
    {
        public OrderItemValidator()
        {
            RuleFor(x => x.OrderItemID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderItemID' é obrigatório.")
                .MustBeValidInt64()
                .WithMessage("O campo 'OrderItemID' deve ser um número inteiro.");

            RuleFor(x => x.ParentItemID)
                .MustBeValidInt64()
                .WithMessage("O campo 'ParentItemID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.ParentItemID));

            RuleFor(x => x.ProductID)
                .MustBeValidInt64()
                .WithMessage("O campo 'ProductID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.ProductID));

            RuleFor(x => x.SkuID)
                .MustBeValidInt64()
                .WithMessage("O campo 'SkuID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.SkuID));

            RuleFor(x => x.SKU)
                .MaximumLength(100)
                .WithMessage("O campo 'SKU' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.SKU));

            RuleFor(x => x.SellerSKU)
                .MaximumLength(100)
                .WithMessage("O campo 'SellerSKU' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.SellerSKU));

            RuleFor(x => x.WebSiteID)
                .MustBeValidInt32()
                .WithMessage("O campo 'WebSiteID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.WebSiteID));

            RuleFor(x => x.CatalogID)
                .MustBeValidInt32()
                .WithMessage("O campo 'CatalogID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.CatalogID));

            RuleFor(x => x.PriceListID)
                .MustBeValidInt32()
                .WithMessage("O campo 'PriceListID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.PriceListID));

            RuleFor(x => x.WareHouseID)
                .MustBeValidInt32()
                .WithMessage("O campo 'WareHouseID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.WareHouseID));

            RuleFor(x => x.WarehouseIntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'WarehouseIntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.WarehouseIntegrationID));

            RuleFor(x => x.Qty)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Qty' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Qty));

            RuleFor(x => x.Price)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Price' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Price));

            RuleFor(x => x.DiscountAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'DiscountAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.DiscountAmount));

            RuleFor(x => x.TaxationAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'TaxationAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.TaxationAmount));

            RuleFor(x => x.SubtotalAdjustment)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'SubtotalAdjustment' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.SubtotalAdjustment));

            RuleFor(x => x.Subtotal)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Subtotal' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Subtotal));

            RuleFor(x => x.Total)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Total' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Total));

            RuleFor(x => x.IsFreeShipping)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsFreeShipping' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsFreeShipping));

            RuleFor(x => x.IsDeleted)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsDeleted' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsDeleted));

            RuleFor(x => x.Status)
                .MaximumLength(50)
                .WithMessage("O campo 'Status' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Status));

            RuleFor(x => x.ProductIntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'ProductIntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ProductIntegrationID));

            RuleFor(x => x.SKUIntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'SKUIntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.SKUIntegrationID));

            RuleFor(x => x.CatalogItemType)
                .MustBeValidInt32()
                .WithMessage("O campo 'CatalogItemType' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.CatalogItemType));

            RuleFor(x => x.IsFreeOffer)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsFreeOffer' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsFreeOffer));

            RuleFor(x => x.IsGiftWrapping)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsGiftWrapping' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsGiftWrapping));

            RuleFor(x => x.IsService)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsService' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsService));

            RuleFor(x => x.SpecialType)
                .MustBeValidInt32()
                .WithMessage("O campo 'SpecialType' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.SpecialType));

            RuleFor(x => x.BundlePriceType)
                .MaximumLength(50)
                .WithMessage("O campo 'BundlePriceType' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.BundlePriceType));

            RuleFor(x => x.BundleKitDiscount)
                .MaximumLength(50)
                .WithMessage("O campo 'BundleKitDiscount' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.BundleKitDiscount));

            RuleFor(x => x.BundleKitDiscountValue)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'BundleKitDiscountValue' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.BundleKitDiscountValue));

            RuleFor(x => x.InStockHandlingDays)
                .MustBeValidInt32()
                .WithMessage("O campo 'InStockHandlingDays' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.InStockHandlingDays));

            RuleFor(x => x.OutStockHandlingDays)
                .MustBeValidInt32()
                .WithMessage("O campo 'OutStockHandlingDays' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.OutStockHandlingDays));

            RuleFor(x => x.ProductName)
                .MaximumLength(250)
                .WithMessage("O campo 'ProductName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ProductName));

            RuleFor(x => x.SkuName)
                .MaximumLength(250)
                .WithMessage("O campo 'SkuName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.SkuName));

            RuleFor(x => x.IsDeliverable)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsDeliverable' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsDeliverable));

            RuleFor(x => x.Weight)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Weight' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Weight));

            RuleFor(x => x.Depth)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Depth' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Depth));

            RuleFor(x => x.Height)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Height' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Height));

            RuleFor(x => x.Width)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Width' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Width));

            RuleFor(x => x.FulfillmentID)
                .MustBeValidGuid()
                .WithMessage("O campo 'FulfillmentID' deve ser um Guid válido.")
                .When(x => !String.IsNullOrEmpty(x.FulfillmentID));

            RuleFor(x => x.UPC)
                .MaximumLength(50)
                .WithMessage("O campo 'UPC' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.UPC));
        }
    }
}

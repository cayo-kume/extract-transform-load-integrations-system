using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderItemValidator : AbstractValidator<OrderItem>
    {
        public OrderItemValidator()
        {
            RuleFor(x => x.SKU)
                .TruncateIfExceeds(20, (x, val) => x.SKU = val);

            RuleFor(x => x.SellerSKU)
                .TruncateIfExceeds(50, (x, val) => x.SellerSKU = val);

            RuleFor(x => x.WarehouseIntegrationID)
                .TruncateIfExceeds(30, (x, val) => x.WarehouseIntegrationID = val);

            RuleFor(x => x.ProductIntegrationID)
                .TruncateIfExceeds(30, (x, val) => x.ProductIntegrationID = val);

            RuleFor(x => x.SKUIntegrationID)
                .TruncateIfExceeds(20, (x, val) => x.SKUIntegrationID = val);

            RuleFor(x => x.BundlePriceType)
                .TruncateIfExceeds(50, (x, val) => x.BundlePriceType = val);

            RuleFor(x => x.BundleKitDiscount)
                .TruncateIfExceeds(50, (x, val) => x.BundleKitDiscount = val);

            RuleFor(x => x.ProductName)
                .TruncateIfExceeds(250, (x, val) => x.ProductName = val);

            RuleFor(x => x.SkuName)
                .TruncateIfExceeds(60, (x, val) => x.SkuName = val);

            RuleFor(x => x.UPC)
                .TruncateIfExceeds(50, (x, val) => x.UPC = val);

            RuleFor(x => x.Status)
                .TruncateIfExceeds(50, (x, val) => x.Status = val);
        }
    }
}

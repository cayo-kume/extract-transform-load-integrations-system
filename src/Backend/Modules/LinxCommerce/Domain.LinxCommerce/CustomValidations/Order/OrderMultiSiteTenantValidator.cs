using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderMultiSiteTenantValidator : AbstractValidator<OrderMultiSiteTenant>
    {
        public OrderMultiSiteTenantValidator()
        {
            RuleFor(x => x.BrandId)
                .TruncateIfExceeds(50, (x, val) => x.BrandId = val);

            RuleFor(x => x.BrandType)
                .TruncateIfExceeds(50, (x, val) => x.BrandType = val);

            RuleFor(x => x.CompanyId)
                .TruncateIfExceeds(50, (x, val) => x.CompanyId = val);

            RuleFor(x => x.DeviceType)
                .TruncateIfExceeds(50, (x, val) => x.DeviceType = val);
        }
    }
}

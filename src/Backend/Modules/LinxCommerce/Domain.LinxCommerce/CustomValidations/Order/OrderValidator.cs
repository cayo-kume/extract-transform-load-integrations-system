using Domain.Core.Extensions;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderValidator : AbstractValidator<Domain.LinxCommerce.Entities.Order.Order.Root>
    {
        public OrderValidator()
        {
            RuleFor(x => x.OrderNumber)
                .TruncateIfExceeds(12, (x, val) => x.OrderNumber = val);

            RuleFor(x => x.MarketPlaceBrand)
                .TruncateIfExceeds(50, (x, val) => x.MarketPlaceBrand = val);

            RuleFor(x => x.WebSiteIntegrationID)
                .TruncateIfExceeds(60, (x, val) => x.WebSiteIntegrationID = val);

            RuleFor(x => x.DeliveryPostalCode)
                .TruncateIfExceeds(9, (x, val) => x.DeliveryPostalCode = val);

            RuleFor(x => x.CreatedChannel)
                .TruncateIfExceeds(60, (x, val) => x.CreatedChannel = val);

            RuleFor(x => x.CreatedBy)
                .TruncateIfExceeds(60, (x, val) => x.CreatedBy = val);

            RuleFor(x => x.ModifiedBy)
                .TruncateIfExceeds(60, (x, val) => x.ModifiedBy = val);

            RuleFor(x => x.OrderGroupNumber)
                .TruncateIfExceeds(12, (x, val) => x.OrderGroupNumber = val);

            RuleFor(x => x.CustomerType)
                .TruncateIfExceeds(1, (x, val) => x.CustomerType = val);

            RuleFor(x => x.WebSiteName)
                .TruncateIfExceeds(60, (x, val) => x.WebSiteName = val);

            RuleFor(x => x.CustomerName)
                .TruncateIfExceeds(60, (x, val) => x.CustomerName = val);

            RuleFor(x => x.CustomerEmail)
                .TruncateIfExceeds(60, (x, val) => x.CustomerEmail = val);

            RuleFor(x => x.CustomerGender)
                .TruncateIfExceeds(1, (x, val) => x.CustomerGender = val);

            RuleFor(x => x.CustomerPhone)
                .TruncateIfExceeds(20, (x, val) => x.CustomerPhone = val);

            RuleFor(x => x.CustomerCPF)
                .TruncateIfExceeds(11, (x, val) => x.CustomerCPF = val);

            RuleFor(x => x.CustomerCNPJ)
                .TruncateIfExceeds(14, (x, val) => x.CustomerCNPJ = val);

            RuleFor(x => x.CustomerTradingName)
                .TruncateIfExceeds(60, (x, val) => x.CustomerTradingName = val);

            RuleFor(x => x.CustomerSiteTaxPayer)
                .TruncateIfExceeds(60, (x, val) => x.CustomerSiteTaxPayer = val);

            RuleFor(x => x.PaymentDate)
                .NormalizeDateIfInvalid((x, val) => x.PaymentDate = val);

            RuleFor(x => x.ShipmentDate)
                .NormalizeDateIfInvalid((x, val) => x.ShipmentDate = val);

            RuleFor(x => x.CreatedDate)
                .NormalizeDateIfInvalid((x, val) => x.CreatedDate = val);

            RuleFor(x => x.ModifiedDate)
                .NormalizeDateIfInvalid((x, val) => x.ModifiedDate = val);

            RuleFor(x => x.AcquiredDate)
                .NormalizeDateIfInvalid((x, val) => x.AcquiredDate = val);

            RuleFor(x => x.CancelledDate)
                .NormalizeDateIfInvalid((x, val) => x.CancelledDate = val);

            RuleFor(x => x.CustomerBirthDate)
                .NormalizeDateIfInvalid((x, val) => x.CustomerBirthDate = val);

            RuleForEach(x => x.Items)
                .SetValidator(new OrderItemValidator());

            RuleForEach(x => x.Tags)
                .SetValidator(new OrderTagValidator());

            RuleForEach(x => x.Addresses)
                .SetValidator(new OrderAddressValidator());

            RuleForEach(x => x.PaymentMethods)
                .SetValidator(new OrderPaymentMethodValidator());

            RuleForEach(x => x.DeliveryMethods)
                .SetValidator(new OrderDeliveryMethodValidator());

            RuleForEach(x => x.Discounts)
                .SetValidator(new OrderDiscountValidator());

            RuleForEach(x => x.Shipments)
                .SetValidator(new OrderShipmentValidator());

            RuleFor(x => x.OrderInvoice)
                .SetValidator(new OrderInvoiceValidator());
        }
    }
}

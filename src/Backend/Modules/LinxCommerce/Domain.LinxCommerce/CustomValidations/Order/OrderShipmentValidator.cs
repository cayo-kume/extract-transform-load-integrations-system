using Domain.Core.Extensions;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderShipmentValidator : AbstractValidator<OrderShipment>
    {
        public OrderShipmentValidator()
        {
            RuleFor(x => x.ShipmentNumber)
                .TruncateIfExceeds(10, (x, val) => x.ShipmentNumber = val);

            RuleFor(x => x.ShipmentStatus)
                .TruncateIfExceeds(1, (x, val) => x.ShipmentStatus = val);

            RuleFor(x => x.AssignUserName)
                .TruncateIfExceeds(50, (x, val) => x.AssignUserName = val);
        }
    }
}

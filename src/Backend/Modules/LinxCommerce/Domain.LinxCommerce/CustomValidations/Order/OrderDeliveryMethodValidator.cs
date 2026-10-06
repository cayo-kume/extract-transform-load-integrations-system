using Domain.Core.Extensions;
using Domain.LinxCommerce.CustomValidations.Validations;
using Domain.LinxCommerce.Entities.Order;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.Order
{
    public class OrderDeliveryMethodValidator : AbstractValidator<OrderDeliveryMethod>
    {
        public OrderDeliveryMethodValidator()
        {
            RuleFor(x => x.LogisticOptionId)
                .TruncateIfExceeds(60, (x, val) => x.LogisticOptionId = val);

            RuleFor(x => x.LogisticContractName)
                .TruncateIfExceeds(60, (x, val) => x.LogisticContractName = val);

            RuleFor(x => x.LogisticContractId)
                .TruncateIfExceeds(60, (x, val) => x.LogisticContractId = val);

            RuleFor(x => x.LogisticContractName)
                .TruncateIfExceeds(60, (x, val) => x.LogisticContractName = val);

            RuleFor(x => x.ScheduleDisplayName)
                .TruncateIfExceeds(50, (x, val) => x.ScheduleDisplayName = val);

            RuleFor(x => x.DeliveryMethodAlias)
                .TruncateIfExceeds(50, (x, val) => x.DeliveryMethodAlias = val);

            RuleFor(x => x.PointOfSaleIntegrationID)
                .TruncateIfExceeds(60, (x, val) => x.PointOfSaleIntegrationID = val);

            RuleFor(x => x.PointOfSaleName)
                .TruncateIfExceeds(60, (x, val) => x.PointOfSaleName = val);

            RuleFor(x => x.DeliveryMethodType)
                .TruncateIfExceeds(1, (x, val) => x.DeliveryMethodType = val);

            RuleFor(x => x.DeliveryLogisticType)
                .TruncateIfExceeds(60, (x, val) => x.DeliveryLogisticType = val);

            RuleFor(x => x.ExternalID)
                .TruncateIfExceeds(60, (x, val) => x.ExternalID = val);

            RuleFor(x => x.WarehouseIntegrationID)
                .TruncateIfExceeds(60, (x, val) => x.WarehouseIntegrationID = val);

            RuleFor(x => x.CarrierName)
                .TruncateIfExceeds(60, (x, val) => x.CarrierName = val);

            RuleFor(x => x.ScheduleDate)
                .NormalizeDateIfInvalid((x, val) => x.ScheduleDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: ScheduleDate | Value: {x.ScheduleDate}, Error when trying to convert value: {x.ScheduleDate.ToString()} to DateTime");


            RuleFor(x => x.DeliveryEstimatedDate)
                .NormalizeDateIfInvalid((x, val) => x.DeliveryEstimatedDate = val)
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: DeliveryEstimatedDate | Value: {x.DeliveryEstimatedDate}, Error when trying to convert value: {x.DeliveryEstimatedDate.ToString()} to DateTime");
        }
    }
}

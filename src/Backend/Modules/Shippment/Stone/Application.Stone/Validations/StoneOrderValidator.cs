using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Stone.Validations
{
    public class StoneOrderValidator : AbstractValidator<Domain.Stone.Dtos.Order>
    {
        public StoneOrderValidator()
        {
            RuleFor(x => x.id).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.id));
            RuleFor(x => x.quoteId).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.quoteId));
            RuleFor(x => x.senderAccountId).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.senderAccountId));
            RuleFor(x => x.ownerAccountId).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.ownerAccountId));
            RuleFor(x => x.pickupPointId).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.pickupPointId));
            RuleFor(x => x.updateId).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.updateId));
            RuleFor(x => x.trackingStreamCode).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.trackingStreamCode));

            RuleFor(x => x.createdAt).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.createdAt));
            RuleFor(x => x.updatedAt).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.updatedAt));
            RuleFor(x => x.customerDeadlineDate).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.customerDeadlineDate));
            RuleFor(x => x.deadlineDate).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.deadlineDate));
            RuleFor(x => x.deliveredAt).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.deliveredAt));
            RuleFor(x => x.collectedAt).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.collectedAt));

            RuleFor(x => x.isFromIntegration).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.isFromIntegration));
            RuleFor(x => x.isUsingContractPrice).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.isUsingContractPrice));
            RuleFor(x => x.shouldForceTracking).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.shouldForceTracking));
            RuleFor(x => x.acceptedAgreements).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.acceptedAgreements));
            RuleFor(x => x.userCanCancelDelivery).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.userCanCancelDelivery));
            RuleFor(x => x.wasTagGenerated).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.wasTagGenerated));
            RuleFor(x => x.insurance).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.insurance));

            RuleFor(x => x.contractPrice).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.contractPrice));
            RuleFor(x => x.totalCost).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.totalCost));
            RuleFor(x => x.discount).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.discount));

            RuleFor(x => x.retryTimes).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.retryTimes));
            RuleFor(x => x.slaWorkingDays).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.slaWorkingDays));

            RuleFor(x => x.totalETA).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.totalETA));

            RuleFor(x => x.pickupType).MaximumLength(60).WithMessage("O campo 'pickupType' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.pickupType));
            RuleFor(x => x.referenceKey).MaximumLength(60).WithMessage("O campo 'referenceKey' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.referenceKey));
            RuleFor(x => x.status).MaximumLength(60).WithMessage("O campo 'status' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.status));
            RuleFor(x => x.carrierReferenceKey).MaximumLength(60).WithMessage("O campo 'carrierReferenceKey' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.carrierReferenceKey));
            RuleFor(x => x.trackingCode).MaximumLength(60).WithMessage("O campo 'trackingCode' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.trackingCode));
            RuleFor(x => x.tagUrl).MaximumLength(300).WithMessage("O campo 'tagUrl' deve ter no máximo 300 caractere.").When(x => !string.IsNullOrEmpty(x.tagUrl));
            RuleFor(x => x.quoteExpirationDate).MaximumLength(60).WithMessage("O campo 'quoteExpirationDate' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.quoteExpirationDate));
            RuleFor(x => x.statusReason).MaximumLength(300).WithMessage("O campo 'statusReason' deve ter no máximo 300 caractere.").When(x => !string.IsNullOrEmpty(x.statusReason));
            RuleFor(x => x.modifiedBy).MaximumLength(60).WithMessage("O campo 'modifiedBy' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.modifiedBy));
            RuleFor(x => x.message).MaximumLength(300).WithMessage("O campo 'message' deve ter no máximo 300 caractere.").When(x => !string.IsNullOrEmpty(x.message));
            RuleFor(x => x.error).MaximumLength(300).WithMessage("O campo 'error' deve ter no máximo 300 caractere.").When(x => !string.IsNullOrEmpty(x.error));

            RuleFor(x => x.customer)
                .SetValidator(new StoneCustomerValidator());

            RuleFor(x => x.sender)
                .SetValidator(new StoneSenderValidator());

            RuleFor(x => x.carrier)
                .SetValidator(new StoneCarrierValidator());

            RuleFor(x => x.deliveryAddress)
                .SetValidator(new StoneDeliveryAddressValidator());

            RuleFor(x => x.pickupAddress)
                .SetValidator(new StonePickupAddressValidator());

            RuleFor(x => x.metrics)
                .SetValidator(new StoneMetricsValidator());

            RuleFor(x => x.owner)
                .SetValidator(new StoneOwnerValidator());

            RuleForEach(x => x.items)
                .SetValidator(new StoneItemValidator());

            RuleForEach(x => x.events)
                .SetValidator(new StoneEventValidator());
        }
    }
}

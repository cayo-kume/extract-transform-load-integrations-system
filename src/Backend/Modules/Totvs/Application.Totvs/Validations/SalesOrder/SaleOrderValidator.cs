using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.SalesOrder;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrder
{
    public class SaleOrderValidator : AbstractValidator<Item>
    {
        public SaleOrderValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.orderCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.orderCode));

            RuleFor(x => x.guideCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.guideCode));

            RuleFor(x => x.sellerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sellerCode));

            RuleFor(x => x.customerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.customerCode));

            RuleFor(x => x.representativeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.representativeCode));

            RuleFor(x => x.operationCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.operationCode));

            RuleFor(x => x.paymentConditionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.paymentConditionCode));

            RuleFor(x => x.quantity)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.shippingCompanyCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyCode));

            RuleFor(x => x.redispatchingShippingCompanyCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.redispatchingShippingCompanyCode));

            RuleFor(x => x.reasonBlockingCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.reasonBlockingCode));

            RuleFor(x => x.reasonCancellationCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.reasonCancellationCode));

            RuleFor(x => x.omniSiteId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.omniSiteId));

            RuleFor(x => x.priceTableCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.priceTableCode));

            RuleFor(x => x.rollMeasureCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.rollMeasureCode));

            RuleFor(x => x.totalAmountOrder)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.totalAmountOrder));

            RuleFor(x => x.netValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netValue));

            RuleFor(x => x.freightValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.freightValue));

            RuleFor(x => x.grossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossValue));

            RuleFor(x => x.discountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountValue));

            RuleFor(x => x.additionalValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.additionalValue));

            RuleFor(x => x.hasPdvTransaction)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.hasPdvTransaction));

            RuleFor(x => x.hasFinancialProcessed)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.hasFinancialProcessed));

            RuleFor(x => x.insertDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.insertDate));

            RuleFor(x => x.orderDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.orderDate));

            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleFor(x => x.arrivalDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.arrivalDate));

            RuleFor(x => x.cancellationLimitDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.cancellationLimitDate));

            RuleFor(x => x.billingForecastDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.billingForecastDate));

            RuleFor(x => x.billingLimitDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.billingLimitDate));

            RuleFor(x => x.lastBillingDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastBillingDate));

            RuleFor(x => x.paymentBaseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.paymentBaseDate));

            RuleFor(x => x.orderId)
                .MaximumLength(60)
                .WithMessage("O campo 'orderId' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.orderId));

            RuleFor(x => x.customerOrderCode)
                .MaximumLength(60)
                .WithMessage("O campo 'customerOrderCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerOrderCode));

            RuleFor(x => x.integrationCode)
                .MaximumLength(60)
                .WithMessage("O campo 'integrationCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.integrationCode));

            RuleFor(x => x.guideCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'guideCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.guideCpfCnpj));

            RuleFor(x => x.sellerCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'sellerCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sellerCpfCnpj));

            RuleFor(x => x.customerCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'customerCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerCpfCnpj));

            RuleFor(x => x.customerName)
                .MaximumLength(250)
                .WithMessage("O campo 'customerName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerName));

            RuleFor(x => x.representativeCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'representativeCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.representativeCpfCnpj));

            RuleFor(x => x.representativeName)
                .MaximumLength(250)
                .WithMessage("O campo 'representativeName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.representativeName));

            RuleFor(x => x.operationName)
                .MaximumLength(60)
                .WithMessage("O campo 'operationName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operationName));

            RuleFor(x => x.paymentConditionName)
                .MaximumLength(250)
                .WithMessage("O campo 'paymentConditionName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.paymentConditionName));

            RuleFor(x => x.freightType)
                .MaximumLength(60)
                .WithMessage("O campo 'freightType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.freightType));

            RuleFor(x => x.redispatchingFreightType)
                .MaximumLength(60)
                .WithMessage("O campo 'redispatchingFreightType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.redispatchingFreightType));

            RuleFor(x => x.redispatchingShippingCompanyCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'redispatchingShippingCompanyCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.redispatchingShippingCompanyCpfCnpj));

            RuleFor(x => x.redispatchingShippingCompanyName)
                .MaximumLength(250)
                .WithMessage("O campo 'redispatchingShippingCompanyName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.redispatchingShippingCompanyName));

            RuleFor(x => x.statusOrder)
                .MaximumLength(60)
                .WithMessage("O campo 'statusOrder' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.statusOrder));

            RuleFor(x => x.reasonBlockingDescription)
                .MaximumLength(250)
                .WithMessage("O campo 'reasonBlockingDescription' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reasonBlockingDescription));

            RuleFor(x => x.reasonCancellationDescription)
                .MaximumLength(250)
                .WithMessage("O campo 'reasonCancellationDescription' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reasonCancellationDescription));

            RuleFor(x => x.experienceType)
                .MaximumLength(60)
                .WithMessage("O campo 'experienceType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.experienceType));

            RuleFor(x => x.batchOrder)
                .MaximumLength(60)
                .WithMessage("O campo 'batchOrder' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.batchOrder));

            RuleFor(x => x.rollMeasureDescription)
                .MaximumLength(250)
                .WithMessage("O campo 'rollMeasureDescription' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rollMeasureDescription));

            RuleFor(x => x.ecommerceStage)
                .MaximumLength(60)
                .WithMessage("O campo 'ecommerceStage' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerceStage));

            RuleFor(x => x.shippingCompanyName)
                .MaximumLength(250)
                .WithMessage("O campo 'shippingCompanyName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyName));

            RuleFor(x => x.shippingCompanyCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'shippingCompanyCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyCpfCnpj));

            RuleFor(x => x.shippingService)
                .MaximumLength(60)
                .WithMessage("O campo 'shippingService' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingService));

            RuleFor(x => x.shippingServiceName)
                .MaximumLength(250)
                .WithMessage("O campo 'shippingServiceName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingServiceName));

            RuleForEach(x => x.items)
                .SetValidator(new ProductValidator());

            RuleForEach(x => x.invoices)
                .SetValidator(new InvoiceValidator());

            RuleFor(x => x.shippingAddress)
                .SetValidator(new ShippingAddressValidator());

            RuleForEach(x => x.observations)
                .SetValidator(new ObservationValidator());

            RuleForEach(x => x.invoiceObservations)
                .SetValidator(new InvoiceObservationValidator());

            RuleForEach(x => x.representativeObservations)
                .SetValidator(new RepresentativeObservationValidator());

            RuleForEach(x => x.classifications)
                .SetValidator(new ClassificationValidator());

            RuleForEach(x => x.discounts)
                .SetValidator(new DiscountValidator());

            RuleForEach(x => x.commissioneds)
                .SetValidator(new CommissionedValidator());

            RuleForEach(x => x.counts)
                .SetValidator(new CountValidator());
        }
    }
}

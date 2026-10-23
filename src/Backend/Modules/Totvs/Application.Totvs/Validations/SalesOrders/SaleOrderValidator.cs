using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class SaleOrderValidator : AbstractValidator<SaleOrder>
    {
        public SaleOrderValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.orderCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'orderCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.orderId)
                .MaximumLength(100)
                .WithMessage("O campo 'orderId' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.orderId));

            RuleFor(x => x.customerOrderCode)
                .MaximumLength(100)
                .WithMessage("O campo 'customerOrderCode' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerOrderCode));

            RuleFor(x => x.insertDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.insertDate));

            RuleFor(x => x.integrationCode)
                .MaximumLength(100)
                .WithMessage("O campo 'integrationCode' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.integrationCode));

            RuleFor(x => x.guideCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.guideCode));

            RuleFor(x => x.guideCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'guideCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.guideCpfCnpj));

            RuleFor(x => x.sellerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sellerCode));

            RuleFor(x => x.sellerCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'sellerCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sellerCpfCnpj));

            RuleFor(x => x.orderDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.orderDate));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleFor(x => x.arrivalDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.arrivalDate));

            RuleFor(x => x.customerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.customerCode));

            RuleFor(x => x.customerCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'customerCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerCpfCnpj));

            RuleFor(x => x.customerName)
                .MaximumLength(60)
                .WithMessage("O campo 'customerName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerName));

            RuleFor(x => x.representativeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.representativeCode));

            RuleFor(x => x.representativeCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'representativeCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.representativeCpfCnpj));

            RuleFor(x => x.representativeName)
                .MaximumLength(60)
                .WithMessage("O campo 'representativeName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.representativeName));

            RuleFor(x => x.operationCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.operationCode));

            RuleFor(x => x.operationName)
                .MaximumLength(80)
                .WithMessage("O campo 'operationName' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operationName));

            RuleFor(x => x.paymentConditionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.paymentConditionCode));

            RuleFor(x => x.paymentConditionName)
                .MaximumLength(40)
                .WithMessage("O campo 'paymentConditionName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.paymentConditionName));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.grossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossValue));

            RuleFor(x => x.discountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountValue));

            RuleFor(x => x.netValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netValue));

            RuleFor(x => x.freightType)
                .MaximumLength(30)
                .WithMessage("O campo 'freightType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.freightType));

            RuleFor(x => x.freightValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.freightValue));

            RuleFor(x => x.shippingCompanyCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyCode));

            RuleFor(x => x.shippingCompanyCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'shippingCompanyCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyCpfCnpj));

            RuleFor(x => x.shippingCompanyName)
                .MaximumLength(60)
                .WithMessage("O campo 'shippingCompanyName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyName));

            RuleFor(x => x.redispatchingFreightType)
                .MaximumLength(30)
                .WithMessage("O campo 'redispatchingFreightType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.redispatchingFreightType));

            RuleFor(x => x.redispatchingShippingCompanyCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.redispatchingShippingCompanyCode));

            RuleFor(x => x.redispatchingShippingCompanyCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'redispatchingShippingCompanyCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.redispatchingShippingCompanyCpfCnpj));

            RuleFor(x => x.redispatchingShippingCompanyName)
                .MaximumLength(60)
                .WithMessage("O campo 'redispatchingShippingCompanyName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.redispatchingShippingCompanyName));

            RuleFor(x => x.totalAmountOrder)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.totalAmountOrder));

            RuleFor(x => x.statusOrder)
                .MaximumLength(30)
                .WithMessage("O campo 'statusOrder' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.statusOrder));

            RuleFor(x => x.reasonBlockingCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.reasonBlockingCode));

            RuleFor(x => x.reasonBlockingDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'reasonBlockingDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reasonBlockingDescription));

            RuleFor(x => x.reasonCancellationCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.reasonCancellationCode));

            RuleFor(x => x.reasonCancellationDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'reasonCancellationDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.reasonCancellationDescription));

            RuleFor(x => x.omniSiteId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.omniSiteId));

            RuleFor(x => x.additionalValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.additionalValue));

            RuleFor(x => x.experienceType)
                .MaximumLength(30)
                .WithMessage("O campo 'experienceType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.experienceType));

            RuleFor(x => x.hasPdvTransaction)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.hasPdvTransaction));

            RuleFor(x => x.hasFinancialProcessed)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.hasFinancialProcessed));

            RuleFor(x => x.priceTableCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.priceTableCode));

            RuleFor(x => x.shippingService)
                .MaximumLength(10)
                .WithMessage("O campo 'shippingService' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingService));

            RuleFor(x => x.shippingServiceName)
                .MaximumLength(200)
                .WithMessage("O campo 'shippingServiceName' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingServiceName));

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

            RuleFor(x => x.batchOrder)
                .MaximumLength(10)
                .WithMessage("O campo 'batchOrder' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.batchOrder));

            RuleFor(x => x.rollMeasureCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.rollMeasureCode));

            RuleFor(x => x.rollMeasureDescription)
                .MaximumLength(80)
                .WithMessage("O campo 'rollMeasureDescription' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rollMeasureDescription));

            RuleFor(x => x.ecommerceStage)
                .MaximumLength(40)
                .WithMessage("O campo 'ecommerceStage' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ecommerceStage));

            RuleFor(x => x.shippingAddress!)
                .SetValidator(new ShippingAddressValidator())
                .When(x => x.shippingAddress is not null);

            RuleForEach(x => x.items)
                .SetValidator(new ProductValidator());

            RuleForEach(x => x.invoices)
                .SetValidator(new InvoiceValidator());

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

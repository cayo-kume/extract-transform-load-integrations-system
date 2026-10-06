using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.PurchaseOrder;
using FluentValidation;

namespace Application.Totvs.Validations.PurchaseOrder
{
    public class PurchaseOrderValidator : AbstractValidator<Item>
    {
        public PurchaseOrderValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.orderCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.orderCode));

            RuleFor(x => x.supplierCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.supplierCode));

            RuleFor(x => x.buyerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.buyerCode));

            RuleFor(x => x.operationCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.operationCode));

            RuleFor(x => x.paymentConditionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.paymentConditionCode));

            RuleFor(x => x.productValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.productValue));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.ipiValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.ipiValue));

            RuleFor(x => x.totalAmountOrder)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.totalAmountOrder));

            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleFor(x => x.registrationDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.registrationDate));

            RuleFor(x => x.deliveryForecastDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deliveryForecastDate));

            RuleFor(x => x.deliveryDeadlineDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deliveryDeadlineDate));

            RuleFor(x => x.basePaymentDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.basePaymentDate));

            RuleFor(x => x.status)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.supplierOrderCode)
                .MaximumLength(60)
                .WithMessage("O campo 'supplierOrderCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.supplierOrderCode));

            RuleFor(x => x.supplierCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'supplierCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.supplierCpfCnpj));

            RuleFor(x => x.supplierName)
                .MaximumLength(250)
                .WithMessage("O campo 'supplierName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.supplierName));

            RuleFor(x => x.buyerCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'buyerCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.buyerCpfCnpj));

            RuleFor(x => x.buyerName)
                .MaximumLength(250)
                .WithMessage("O campo 'buyerName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.buyerName));

            RuleFor(x => x.operationName)
                .MaximumLength(250)
                .WithMessage("O campo 'operationName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operationName));

            RuleFor(x => x.shippingCompanyName)
                .MaximumLength(250)
                .WithMessage("O campo 'shippingCompanyName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyName));

            RuleFor(x => x.paymentConditionName)
                .MaximumLength(60)
                .WithMessage("O campo 'paymentConditionName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.paymentConditionName));

            RuleFor(x => x.paymentType)
                .MaximumLength(60)
                .WithMessage("O campo 'paymentType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.paymentType));

            RuleFor(x => x.freightType)
                .MaximumLength(60)
                .WithMessage("O campo 'freightType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.freightType));

            RuleForEach(x => x.items)
                .SetValidator(new ProductValidator());
        }
    }
}

using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.PurchaseOrders
{
    /// <summary>
    /// só registra mensagens no log; o swagger da API Purchase Order não traz tamanhos: os do código antigo, conferidos numa amostra real (colunas do script de tabelas de pedido de compras)
    /// </summary>
    public class PurchaseOrderValidator : AbstractValidator<PurchaseOrder>
    {
        public PurchaseOrderValidator()
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

            RuleFor(x => x.supplierOrderCode)
                .MaximumLength(60)
                .WithMessage("O campo 'supplierOrderCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.supplierOrderCode));

            RuleFor(x => x.supplierCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.supplierCode));

            RuleFor(x => x.supplierCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'supplierCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.supplierCpfCnpj));

            RuleFor(x => x.supplierName)
                .MaximumLength(100)
                .WithMessage("O campo 'supplierName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.supplierName));

            RuleFor(x => x.buyerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.buyerCode));

            RuleFor(x => x.buyerCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'buyerCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.buyerCpfCnpj));

            RuleFor(x => x.buyerName)
                .MaximumLength(100)
                .WithMessage("O campo 'buyerName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.buyerName));

            RuleFor(x => x.operationCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.operationCode));

            RuleFor(x => x.operationName)
                .MaximumLength(100)
                .WithMessage("O campo 'operationName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operationName));

            RuleFor(x => x.shippingCompanyName)
                .MaximumLength(100)
                .WithMessage("O campo 'shippingCompanyName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyName));

            RuleFor(x => x.paymentConditionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.paymentConditionCode));

            RuleFor(x => x.paymentConditionName)
                .MaximumLength(100)
                .WithMessage("O campo 'paymentConditionName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.paymentConditionName));

            RuleFor(x => x.paymentType)
                .MaximumLength(30)
                .WithMessage("O campo 'paymentType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.paymentType));

            RuleFor(x => x.status)
                .MaximumLength(30)
                .WithMessage("O campo 'status' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.freightType)
                .MaximumLength(30)
                .WithMessage("O campo 'freightType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.freightType));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

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

            RuleForEach(x => x.items)
                .SetValidator(new ProductValidator());

            RuleForEach(x => x.requests)
                .SetValidator(new RequestValidator());
        }
    }
}

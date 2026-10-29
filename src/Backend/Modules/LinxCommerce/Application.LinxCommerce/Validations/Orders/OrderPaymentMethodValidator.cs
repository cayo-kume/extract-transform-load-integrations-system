using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersPaymentMethods (scripts de tabelas do módulo)
    /// </summary>
    public class OrderPaymentMethodValidator : AbstractValidator<OrderPaymentMethod>
    {
        public OrderPaymentMethodValidator()
        {
            RuleFor(x => x.OrderPaymentMethodID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderPaymentMethodID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'OrderPaymentMethodID' deve ser um número inteiro.");

            RuleFor(x => x.PaymentNumber)
                .MaximumLength(50)
                .WithMessage("O campo 'PaymentNumber' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PaymentNumber));

            RuleFor(x => x.PaymentMethodID)
                .MustBeValidInt32()
                .WithMessage("O campo 'PaymentMethodID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.PaymentMethodID));

            RuleFor(x => x.TransactionID)
                .MustBeValidGuid()
                .WithMessage("O campo 'TransactionID' deve ser um Guid válido.")
                .When(x => !String.IsNullOrEmpty(x.TransactionID));

            RuleFor(x => x.ReconciliationNumber)
                .MaximumLength(100)
                .WithMessage("O campo 'ReconciliationNumber' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ReconciliationNumber));

            RuleFor(x => x.Status)
                .MaximumLength(50)
                .WithMessage("O campo 'Status' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Status));

            RuleFor(x => x.IntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'IntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.IntegrationID));

            RuleFor(x => x.Amount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Amount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Amount));

            RuleFor(x => x.AmountNoInterest)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'AmountNoInterest' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.AmountNoInterest));

            RuleFor(x => x.InterestValue)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'InterestValue' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.InterestValue));

            RuleFor(x => x.PaidAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'PaidAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.PaidAmount));

            RuleFor(x => x.RefundAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'RefundAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.RefundAmount));

            RuleFor(x => x.Installments)
                .MustBeValidInt32()
                .WithMessage("O campo 'Installments' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.Installments));

            RuleFor(x => x.InstallmentAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'InstallmentAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.InstallmentAmount));

            RuleFor(x => x.TaxAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'TaxAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.TaxAmount));

            RuleFor(x => x.PaymentDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'PaymentDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.PaymentDate));

            RuleFor(x => x.CaptureDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'CaptureDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.CaptureDate));

            RuleFor(x => x.AcquiredDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'AcquiredDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.AcquiredDate));

            RuleFor(x => x.PaymentCancelledDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'PaymentCancelledDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.PaymentCancelledDate));

            RuleFor(x => x.PaymentInfo!.Identifier)
                .MaximumLength(100)
                .WithMessage("O campo 'PaymentInfo.Identifier' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.Identifier));

            RuleFor(x => x.PaymentInfo!.Alias)
                .MaximumLength(100)
                .WithMessage("O campo 'PaymentInfo.Alias' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.Alias));

            RuleFor(x => x.PaymentInfo!.PaymentDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'PaymentInfo.PaymentDate' deve ser uma data válida.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.PaymentDate));

            RuleFor(x => x.PaymentInfo!.ExpirationDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'PaymentInfo.ExpirationDate' deve ser uma data válida.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.ExpirationDate));

            RuleFor(x => x.PaymentInfo!.Month)
                .MustBeValidInt32()
                .WithMessage("O campo 'PaymentInfo.Month' deve ser um número inteiro.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.Month));

            RuleFor(x => x.PaymentInfo!.Year)
                .MustBeValidInt32()
                .WithMessage("O campo 'PaymentInfo.Year' deve ser um número inteiro.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.Year));

            RuleFor(x => x.PaymentInfo!.TransactionNumber)
                .MaximumLength(100)
                .WithMessage("O campo 'PaymentInfo.TransactionNumber' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.TransactionNumber));

            RuleFor(x => x.PaymentInfo!.AuthorizationCode)
                .MaximumLength(100)
                .WithMessage("O campo 'PaymentInfo.AuthorizationCode' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.AuthorizationCode));

            RuleFor(x => x.PaymentInfo!.ReceiptCode)
                .MaximumLength(100)
                .WithMessage("O campo 'PaymentInfo.ReceiptCode' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.ReceiptCode));

            RuleFor(x => x.PaymentInfo!.ReconciliationNumber)
                .MaximumLength(100)
                .WithMessage("O campo 'PaymentInfo.ReconciliationNumber' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.ReconciliationNumber));

            RuleFor(x => x.PaymentInfo!.ConfirmationNumber)
                .MaximumLength(100)
                .WithMessage("O campo 'PaymentInfo.ConfirmationNumber' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.ConfirmationNumber));

            RuleFor(x => x.PaymentInfo!.PaymentType)
                .MustBeValidInt32()
                .WithMessage("O campo 'PaymentInfo.PaymentType' deve ser um número inteiro.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.PaymentType));

            RuleFor(x => x.PaymentInfo!.Provider)
                .MaximumLength(100)
                .WithMessage("O campo 'PaymentInfo.Provider' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.Provider));

            RuleFor(x => x.PaymentInfo!.ProviderDocumentNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'PaymentInfo.ProviderDocumentNumber' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => x.PaymentInfo != null && !String.IsNullOrEmpty(x.PaymentInfo.ProviderDocumentNumber));
        }
    }
}

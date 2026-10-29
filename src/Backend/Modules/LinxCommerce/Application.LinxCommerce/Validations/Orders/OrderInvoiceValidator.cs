using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersInvoices (scripts de tabelas do módulo)
    /// </summary>
    public class OrderInvoiceValidator : AbstractValidator<OrderInvoice>
    {
        public OrderInvoiceValidator()
        {
            RuleFor(x => x.OrderInvoiceID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderInvoiceID' é obrigatório.")
                .MustBeValidGuid()
                .WithMessage("O campo 'OrderInvoiceID' deve ser um Guid válido.");

            RuleFor(x => x.Code)
                .MaximumLength(100)
                .WithMessage("O campo 'Code' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Code));

            RuleFor(x => x.Url)
                .MaximumLength(1000)
                .WithMessage("O campo 'Url' deve ter no máximo 1000 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Url));

            RuleFor(x => x.FulfillmentID)
                .MustBeValidGuid()
                .WithMessage("O campo 'FulfillmentID' deve ser um Guid válido.")
                .When(x => !String.IsNullOrEmpty(x.FulfillmentID));

            RuleFor(x => x.IsIssued)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsIssued' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsIssued));

            RuleFor(x => x.Series)
                .MaximumLength(20)
                .WithMessage("O campo 'Series' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Series));

            RuleFor(x => x.Number)
                .MaximumLength(50)
                .WithMessage("O campo 'Number' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Number));

            RuleFor(x => x.CFOP)
                .MaximumLength(20)
                .WithMessage("O campo 'CFOP' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CFOP));

            RuleFor(x => x.Operation)
                .MaximumLength(50)
                .WithMessage("O campo 'Operation' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Operation));

            RuleFor(x => x.ProcessedAt)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'ProcessedAt' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.ProcessedAt));

            RuleFor(x => x.UpdatedAt)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'UpdatedAt' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.UpdatedAt));

            RuleFor(x => x.IssuedAt)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'IssuedAt' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.IssuedAt));

            RuleFor(x => x.CreatedAt)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'CreatedAt' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.CreatedAt));
        }
    }
}

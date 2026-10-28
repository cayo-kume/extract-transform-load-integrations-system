using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo da transação PDV no controller (corpo inválido volta 400): a chave e o identificador da transação (100)
    /// </summary>
    public class OrderPdvTransactionValidator : AbstractValidator<OrderPdvTransaction>
    {
        public OrderPdvTransactionValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x.transactionId)
                .NotEmpty()
                .WithMessage("O campo 'transactionId' é obrigatório.")
                .MaximumLength(100)
                .WithMessage("O campo 'transactionId' aceita até 100 caracteres.");

            RuleFor(x => x.transactionBranchCode)
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'transactionBranchCode' deve ter de 1 a 4 dígitos.")
                .When(x => x.transactionBranchCode is not null);

            RuleFor(x => x.transactionCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'transactionCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.transactionCode is not null);
        }
    }
}

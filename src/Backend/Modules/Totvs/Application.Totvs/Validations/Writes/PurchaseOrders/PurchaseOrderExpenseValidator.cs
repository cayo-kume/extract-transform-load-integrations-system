using Domain.Totvs.Dtos.Writes.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.PurchaseOrders
{
    /// <summary>
    /// rateio de despesa do pedido ou do item: despesa e percentual (sem nullable no swagger: omitidos, iriam como 0) e o centro de custo
    /// </summary>
    public class PurchaseOrderExpenseValidator : AbstractValidator<PurchaseOrderExpense>
    {
        public PurchaseOrderExpenseValidator()
        {
            RuleFor(x => x.expenseCode)
                .NotNull()
                .WithMessage("O campo 'expenseCode' é obrigatório.")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'expenseCode' deve ter de 1 a 9 dígitos.");

            RuleFor(x => x.expensePercentage)
                .NotNull()
                .WithMessage("O campo 'expensePercentage' é obrigatório.")
                .InclusiveBetween(0, 100)
                .WithMessage("O campo 'expensePercentage' vai de 0 a 100.")
                .PrecisionScale(9, 6, true)
                .WithMessage("O campo 'expensePercentage' aceita até 6 casas decimais.");

            RuleFor(x => x.costCenter)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'costCenter' deve ter de 1 a 9 dígitos.")
                .When(x => x.costCenter is not null);
        }
    }
}

using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.AccountsPayable
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Accounts Payable (colunas do Script022_Create_Totvs_Accounts_Payable_Tables)
    /// </summary>
    public class ExpenseValidator : AbstractValidator<Expense>
    {
        public ExpenseValidator()
        {
            RuleFor(x => x.expenseCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'expenseCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.expenseName)
                .MaximumLength(100)
                .WithMessage("O campo 'expenseName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.expenseName));

            RuleFor(x => x.costCenterCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'costCenterCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.proratedPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.proratedPercentage));

            RuleFor(x => x.proratedValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.proratedValue));
        }
    }
}

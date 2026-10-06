using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.AccountsPayable
{
    public class ExpenseValidator : AbstractValidator<Expense>
    {
        public ExpenseValidator()
        {
            RuleFor(x => x.expenseCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.expenseCode));

            RuleFor(x => x.costCenterCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.costCenterCode));

            RuleFor(x => x.proratedPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.proratedPercentage));

            RuleFor(x => x.proratedValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.proratedValue));
        }
    }
}

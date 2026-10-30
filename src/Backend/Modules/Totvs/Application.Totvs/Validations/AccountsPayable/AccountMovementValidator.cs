using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.AccountsPayable
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Accounts Payable (colunas do Script020_Create_Totvs_Accounts_Payable_Tables)
    /// </summary>
    public class AccountMovementValidator : AbstractValidator<AccountMovement>
    {
        public AccountMovementValidator()
        {
            RuleFor(x => x.movementDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.movementDate));

            RuleFor(x => x.auxiliaryHistoryDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'auxiliaryHistoryDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.auxiliaryHistoryDescription));

            RuleFor(x => x.documentDescription)
                .MaximumLength(15)
                .WithMessage("O campo 'documentDescription' deve ter no máximo 15 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.documentDescription));

            RuleFor(x => x.historyCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.historyCode));

            RuleFor(x => x.historyDescription)
                .MaximumLength(40)
                .WithMessage("O campo 'historyDescription' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.historyDescription));

            RuleFor(x => x.launchValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.launchValue));
        }
    }
}

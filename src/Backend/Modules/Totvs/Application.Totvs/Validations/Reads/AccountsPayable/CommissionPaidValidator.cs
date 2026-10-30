using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.AccountsPayable
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Accounts Payable (colunas do Script020_Create_Totvs_Accounts_Payable_Tables)
    /// </summary>
    public class CommissionPaidValidator : AbstractValidator<CommissionPaid>
    {
        public CommissionPaidValidator()
        {
            RuleFor(x => x.closingBranchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'closingBranchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.closingDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.closingDate));

            RuleFor(x => x.closingCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'closingCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.commissionedCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'commissionedCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.commissionedCpfCnpj));

            RuleFor(x => x.commissionedName)
                .MaximumLength(60)
                .WithMessage("O campo 'commissionedName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.commissionedName));

            RuleFor(x => x.commissionStatus)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.commissionStatus));

            RuleFor(x => x.valueBilling)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valueBilling));

            RuleFor(x => x.valueBillingCommission)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valueBillingCommission));

            RuleFor(x => x.valueReceiving)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valueReceiving));

            RuleFor(x => x.valueReceivingCommission)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valueReceivingCommission));

            RuleFor(x => x.irrfValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.irrfValue));

            RuleFor(x => x.issqnValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.issqnValue));

            RuleFor(x => x.valueRebate)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valueRebate));

            RuleFor(x => x.valueRebateBilling)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valueRebateBilling));

            RuleFor(x => x.valueRebateReceiving)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valueRebateReceiving));

            RuleFor(x => x.discountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountValue));

            RuleFor(x => x.netValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netValue));

            RuleFor(x => x.accountBalanceValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.accountBalanceValue));

            RuleFor(x => x.grossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossValue));

            RuleFor(x => x.returnValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.returnValue));

            RuleForEach(x => x.accountMovements)
                .SetValidator(new AccountMovementValidator());
        }
    }
}

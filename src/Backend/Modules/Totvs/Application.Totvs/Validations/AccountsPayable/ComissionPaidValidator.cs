using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.AccountsPayable
{
    public class ComissionPaidValidator : AbstractValidator<ComissionPaid>
    {
        public ComissionPaidValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.commissionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.commissionCode));

            RuleFor(x => x.representativeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.representativeCode));

            RuleFor(x => x.duplicateCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.duplicateCode));

            RuleFor(x => x.installmentCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.installmentCode));

            RuleFor(x => x.baseValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.baseValue));

            RuleFor(x => x.percentageValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentageValue));

            RuleFor(x => x.commissionValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.commissionValue));

            RuleFor(x => x.paidValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.paidValue));

            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleFor(x => x.paymentDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.paymentDate));

            RuleFor(x => x.representativeCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'representativeCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.representativeCpfCnpj));

            RuleFor(x => x.representativeName)
                .MaximumLength(250)
                .WithMessage("O campo 'representativeName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.representativeName));

            RuleFor(x => x.status)
                .MaximumLength(60)
                .WithMessage("O campo 'status' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));
        }
    }
}

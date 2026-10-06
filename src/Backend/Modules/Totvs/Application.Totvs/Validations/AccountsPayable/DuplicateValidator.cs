using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.AccountsPayable
{
    public class DuplicateValidator : AbstractValidator<Duplicate>
    {
        public DuplicateValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));
            
            RuleFor(x => x.duplicateCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.duplicateCode));

            RuleFor(x => x.supplierCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.supplierCode));

            RuleFor(x => x.installmentCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.installmentCode));

            RuleFor(x => x.bearerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.bearerCode));

            RuleFor(x => x.userInclusionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.userInclusionCode));

            RuleFor(x => x.duplicateValue)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.duplicateValue));

            RuleFor(x => x.feesValue)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.feesValue));

            RuleFor(x => x.discountValue)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.discountValue));

            RuleFor(x => x.paidValue)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.paidValue));

            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleFor(x => x.entryDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.entryDate));

            RuleFor(x => x.issueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.issueDate));

            RuleFor(x => x.dueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dueDate));

            RuleFor(x => x.settlementDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.settlementDate));

            RuleFor(x => x.arrivalDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.arrivalDate));

            RuleFor(x => x.supplierCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'supplierCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.supplierCpfCnpj));

            RuleFor(x => x.status)
                .MaximumLength(60)
                .WithMessage("O campo 'status' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.inclusionType)
                .MaximumLength(60)
                .WithMessage("O campo 'inclusionType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.inclusionType));

            RuleFor(x => x.userInclusionName)
                .MaximumLength(60)
                .WithMessage("O campo 'userInclusionName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.userInclusionName));

            RuleForEach(x => x.expense)
                .SetValidator(new Application.Totvs.Validations.AccountsPayable.ExpenseValidator());
        }
    }
}

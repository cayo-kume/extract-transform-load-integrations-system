using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.AccountsPayable
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Accounts Payable (colunas do Script022_Create_Totvs_Accounts_Payable_Tables)
    /// </summary>
    public class DuplicateValidator : AbstractValidator<Duplicate>
    {
        public DuplicateValidator()
        {
            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.duplicateCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'duplicateCode' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.supplierCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'supplierCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.supplierCpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'supplierCpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.supplierCpfCnpj));

            RuleFor(x => x.installmentCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'installmentCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.bearerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.bearerCode));

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

            RuleFor(x => x.status)
                .MaximumLength(20)
                .WithMessage("O campo 'status' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.duplicateValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.duplicateValue));

            RuleFor(x => x.feesValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.feesValue));

            RuleFor(x => x.discountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountValue));

            RuleFor(x => x.paidValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.paidValue));

            RuleFor(x => x.inclusionType)
                .MaximumLength(40)
                .WithMessage("O campo 'inclusionType' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.inclusionType));

            RuleFor(x => x.userInclusionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.userInclusionCode));

            RuleFor(x => x.userInclusionName)
                .MaximumLength(60)
                .WithMessage("O campo 'userInclusionName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.userInclusionName));

            RuleForEach(x => x.expense)
                .SetValidator(new ExpenseValidator());

            RuleForEach(x => x.classifications)
                .SetValidator(new DuplicateClassificationValidator());
        }
    }
}

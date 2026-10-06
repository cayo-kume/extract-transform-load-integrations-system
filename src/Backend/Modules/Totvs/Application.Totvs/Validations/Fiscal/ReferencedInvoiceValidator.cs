using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class ReferencedInvoiceValidator : AbstractValidator<ReferencedInvoice>
    {
        public ReferencedInvoiceValidator()
        {
            RuleFor(x => x.accessKey)
                .MaximumLength(44)
                .WithMessage("O campo 'accessKey' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.accessKey));

            RuleFor(x => x.satSerialCode)
                .MaximumLength(60)
                .WithMessage("O campo 'satSerialCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.satSerialCode));

            RuleFor(x => x.issueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.issueDate));

            RuleFor(x => x.invoiceSequenceItem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.invoiceSequenceItem));

            RuleFor(x => x.invoiceCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.invoiceCode));

            RuleFor(x => x.invoiceSerialCode)
                .MaximumLength(10)
                .WithMessage("O campo 'invoiceSerialCode' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.invoiceSerialCode));

            RuleFor(x => x.unitNetValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitNetValue));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.cfop)
                .MaximumLength(20)
                .WithMessage("O campo 'cfop' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cfop));

            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.branchCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'branchCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.branchCnpj));

            RuleFor(x => x.terminalCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.terminalCode));
        }
    }
}

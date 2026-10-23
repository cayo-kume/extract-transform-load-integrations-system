using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script025_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class ItemProductReferencedInvoiceValidator : AbstractValidator<ItemProductReferencedInvoice>
    {
        public ItemProductReferencedInvoiceValidator()
        {
            RuleFor(x => x.invoiceSequenceItem)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'invoiceSequenceItem' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.accessKey)
                .MaximumLength(100)
                .WithMessage("O campo 'accessKey' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.accessKey));

            RuleFor(x => x.satSerialCode)
                .MaximumLength(20)
                .WithMessage("O campo 'satSerialCode' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.satSerialCode));

            RuleFor(x => x.issueDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.issueDate));

            RuleFor(x => x.invoiceCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.invoiceCode));

            RuleFor(x => x.invoiceSerialCode)
                .MaximumLength(3)
                .WithMessage("O campo 'invoiceSerialCode' deve ter no máximo 3 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.invoiceSerialCode));

            RuleFor(x => x.unitNetValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitNetValue));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.cfop)
                .MustBeValidInt32()
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

using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class ItemProductValidator : AbstractValidator<ItemProduct>
    {
        public ItemProductValidator()
        {
            RuleFor(x => x.productCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'productCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.productName)
                .MaximumLength(100)
                .WithMessage("O campo 'productName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.productName));

            RuleFor(x => x.dealerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.dealerCode));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.unitGrossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitGrossValue));

            RuleFor(x => x.unitDiscountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitDiscountValue));

            RuleFor(x => x.unitNetValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitNetValue));

            RuleFor(x => x.grossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossValue));

            RuleFor(x => x.discountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountValue));

            RuleFor(x => x.netValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netValue));

            RuleForEach(x => x.referencedInvoices)
                .SetValidator(new ItemProductReferencedInvoiceValidator());
        }
    }
}

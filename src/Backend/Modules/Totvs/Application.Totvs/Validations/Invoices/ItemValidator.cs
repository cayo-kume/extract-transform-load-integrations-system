using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script025_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator()
        {
            RuleFor(x => x.sequence)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'sequence' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.code)
                .MaximumLength(60)
                .WithMessage("O campo 'code' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.name)
                .MaximumLength(100)
                .WithMessage("O campo 'name' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.ncm)
                .MaximumLength(10)
                .WithMessage("O campo 'ncm' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ncm));

            RuleFor(x => x.cfop)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cfop));

            RuleFor(x => x.measureUnit)
                .MaximumLength(10)
                .WithMessage("O campo 'measureUnit' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.measureUnit));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.grossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.grossValue));

            RuleFor(x => x.discountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.discountValue));

            RuleFor(x => x.netValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.netValue));

            RuleFor(x => x.unitGrossValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitGrossValue));

            RuleFor(x => x.unitDiscountValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitDiscountValue));

            RuleFor(x => x.unitNetValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.unitNetValue));

            RuleFor(x => x.additionalValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.additionalValue));

            RuleFor(x => x.freightValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.freightValue));

            RuleFor(x => x.insuranceValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.insuranceValue));

            RuleFor(x => x.cest)
                .MaximumLength(10)
                .WithMessage("O campo 'cest' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cest));

            RuleFor(x => x.rateDifferential!)
                .SetValidator(new RateDifferentialValidator())
                .When(x => x.rateDifferential is not null);

            RuleForEach(x => x.products)
                .SetValidator(new ItemProductValidator());

            RuleForEach(x => x.taxes)
                .SetValidator(new ItemTaxValidator());

            RuleForEach(x => x.barCodes)
                .SetValidator(new ItemBarcodeValidator());
        }
    }
}

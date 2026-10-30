using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Values;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Values
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script011_Create_Totvs_Products_Tables (o swagger não dá tamanho aos nomes)
    /// </summary>
    public class ProductBalanceItemValidator : AbstractValidator<ProductBalanceItem>
    {
        public ProductBalanceItemValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.stockCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'stockCode' é obrigatório.")
                .MustBeValidDecimal();

            RuleFor(x => x.stockDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'stockDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.stockDescription));

            RuleFor(x => x.stock)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.stock));

            RuleFor(x => x.salesOrder)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.salesOrder));

            RuleFor(x => x.inputTransaction)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.inputTransaction));

            RuleFor(x => x.outputTransaction)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.outputTransaction));

            RuleFor(x => x.productionPlanning)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.productionPlanning));

            RuleFor(x => x.purchaseOrder)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.purchaseOrder));

            RuleFor(x => x.productionOrderProgress)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.productionOrderProgress));

            RuleFor(x => x.productionOrderWaitLib)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.productionOrderWaitLib));

            RuleFor(x => x.stockTemp)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.stockTemp));
        }
    }
}

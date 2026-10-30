using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class PriceTableScaleItemValidator : AbstractValidator<PriceTableScaleItem>
    {
        public PriceTableScaleItemValidator()
        {
            RuleFor(x => x.quantity)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'quantity' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.variationValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.variationValue));

            RuleFor(x => x.variationPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.variationPercentage));
        }
    }
}

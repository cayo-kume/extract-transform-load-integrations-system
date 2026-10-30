using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class PriceTableAveragePeriodQuantityValidator : AbstractValidator<PriceTableAveragePeriodQuantity>
    {
        public PriceTableAveragePeriodQuantityValidator()
        {
            RuleFor(x => x.averagePeriod)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.averagePeriod));

            RuleFor(x => x.variationQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.variationQuantity));

            RuleFor(x => x.variationPercentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.variationPercentage));
        }
    }
}

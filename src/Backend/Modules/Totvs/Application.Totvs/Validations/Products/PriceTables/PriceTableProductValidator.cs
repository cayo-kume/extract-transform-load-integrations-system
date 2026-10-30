using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.PriceTables;
using FluentValidation;

namespace Application.Totvs.Validations.Products.PriceTables
{
    /// <summary>
    /// só registra mensagens no log (colunas do Script035_Create_Totvs_Product_Prices_Tables)
    /// </summary>
    public class PriceTableProductValidator : AbstractValidator<PriceTableProduct>
    {
        public PriceTableProductValidator()
        {
            RuleFor(x => x.productCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'productCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.referenceId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.referenceId));

            RuleForEach(x => x.prices)
                .Must(x => !string.IsNullOrEmpty(x.branchCode))
                .WithMessage("Preço sem 'branchCode': descartado.");
        }
    }
}

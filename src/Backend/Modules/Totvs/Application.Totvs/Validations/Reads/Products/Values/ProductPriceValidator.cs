using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Values;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Products.Values
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script035_Create_Totvs_Product_Prices_Tables (o swagger não dá tamanho aos nomes)
    /// </summary>
    public class ProductPriceValidator : AbstractValidator<ProductPrice>
    {
        public ProductPriceValidator()
        {
            RuleFor(x => x.productCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'productCode' é obrigatório.")
                .MustBeValidInt32();

            // sem ele o produto é regravado em toda execução da janela
            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleForEach(x => x.prices)
                .SetValidator(new ProductPriceItemValidator());
        }
    }
}

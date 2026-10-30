using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Values;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Products.Values
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script035_Create_Totvs_Product_Prices_Tables (o swagger não dá tamanho aos nomes)
    /// </summary>
    public class ProductPriceItemValidator : AbstractValidator<ProductPriceItem>
    {
        public ProductPriceItemValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.priceCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'priceCode' é obrigatório.")
                .MustBeValidDecimal();

            RuleFor(x => x.priceName)
                .MaximumLength(60)
                .WithMessage("O campo 'priceName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.priceName));

            RuleFor(x => x.price)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.price));

            RuleFor(x => x.promotionalPrice)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.promotionalPrice));

            RuleFor(x => x.promotionalInformation!.description)
                .MaximumLength(40)
                .WithMessage("O campo 'promotionalInformation.description' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.promotionalInformation?.description));

            RuleForEach(x => x.informationOtherPromotions)
                .Must(x => !string.IsNullOrEmpty(x.code))
                .WithMessage("Promoção sem 'code' em 'informationOtherPromotions': descartada.");
        }
    }
}

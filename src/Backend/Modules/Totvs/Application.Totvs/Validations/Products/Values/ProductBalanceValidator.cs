using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Values;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Values
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script011_Create_Totvs_Products_Tables (o swagger não dá tamanho aos nomes)
    /// </summary>
    public class ProductBalanceValidator : AbstractValidator<ProductBalance>
    {
        public ProductBalanceValidator()
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

            RuleForEach(x => x.balances)
                .SetValidator(new ProductBalanceItemValidator());

            RuleForEach(x => x.locations)
                .Must(x => !string.IsNullOrEmpty(x.locationCode) && x.locationCode.Length <= 20)
                .WithMessage("Local sem 'locationCode' ou com mais de 20 caracteres: descartado ou cortado.");
        }
    }
}

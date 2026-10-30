using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script011_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
    public class BarcodeValidator : AbstractValidator<Barcode>
    {
        public BarcodeValidator()
        {
            RuleFor(x => x.code)
                .MaximumLength(60)
                .WithMessage("O campo 'code' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.quantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantity));

            RuleFor(x => x.isMainCode)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isMainCode));
        }
    }
}

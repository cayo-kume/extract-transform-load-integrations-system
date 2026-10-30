using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script011_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
    public class CompositionValidator : AbstractValidator<Composition>
    {
        public CompositionValidator()
        {
            RuleFor(x => x.material)
                .MaximumLength(60)
                .WithMessage("O campo 'material' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.material));

            RuleFor(x => x.percentage)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentage));
        }
    }
}

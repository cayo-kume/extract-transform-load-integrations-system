using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script011_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
    public class ReferenceCategoryValidator : AbstractValidator<ReferenceCategory>
    {
        public ReferenceCategoryValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.type)
                .MaximumLength(20)
                .WithMessage("O campo 'type' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));
        }
    }
}

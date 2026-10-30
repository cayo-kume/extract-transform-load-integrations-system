using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script011_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
    public class DetailValidator : AbstractValidator<Detail>
    {
        public DetailValidator()
        {
            RuleFor(x => x.typeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.typeCode));

            RuleFor(x => x.type)
                .MaximumLength(60)
                .WithMessage("O campo 'type' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.auxiliaryType)
                .MaximumLength(60)
                .WithMessage("O campo 'auxiliaryType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.auxiliaryType));

            RuleFor(x => x.title)
                .MaximumLength(250)
                .WithMessage("O campo 'title' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.title));
        }
    }
}

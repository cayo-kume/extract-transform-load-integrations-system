using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script010_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
    public class MedidaValidator : AbstractValidator<Medida>
    {
        public MedidaValidator()
        {
            RuleFor(x => x.chave)
                .MaximumLength(60)
                .WithMessage("O campo 'chave' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chave));

            RuleFor(x => x.valor)
                .MaximumLength(60)
                .WithMessage("O campo 'valor' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.valor));
        }
    }
}

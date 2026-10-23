using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script010_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
    public class ConservationInstructionItemValidator : AbstractValidator<ConservationInstructionItem>
    {
        public ConservationInstructionItemValidator()
        {
            RuleFor(x => x.order)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'order' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.item)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'item' é obrigatório.")
                .MustBeValidInt32();
        }
    }
}

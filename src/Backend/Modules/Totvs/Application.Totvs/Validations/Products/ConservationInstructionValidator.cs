using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script011_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
    public class ConservationInstructionValidator : AbstractValidator<ConservationInstruction>
    {
        public ConservationInstructionValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(80)
                .WithMessage("O campo 'description' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.@default)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.@default));

            RuleFor(x => x.grouperCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.grouperCode));

            RuleForEach(x => x.items)
                .SetValidator(new ConservationInstructionItemValidator());
        }
    }
}

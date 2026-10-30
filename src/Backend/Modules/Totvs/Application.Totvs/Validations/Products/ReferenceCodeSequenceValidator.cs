using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products;
using FluentValidation;

namespace Application.Totvs.Validations.Products
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script011_Create_Totvs_Products_Tables (swagger da API
    /// Product onde ele informa; senão, os do código antigo, conferidos numa amostra real)
    /// </summary>
    public class ReferenceCodeSequenceValidator : AbstractValidator<ReferenceCodeSequence>
    {
        public ReferenceCodeSequenceValidator()
        {
            RuleFor(x => x.referenceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'referenceId' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.parentCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'parentCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.code)
                .MaximumLength(60)
                .WithMessage("O campo 'code' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}

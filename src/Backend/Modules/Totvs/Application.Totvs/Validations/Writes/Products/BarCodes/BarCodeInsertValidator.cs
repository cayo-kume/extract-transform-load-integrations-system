using Domain.Totvs.Dtos.Writes.Products.BarCodes;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.BarCodes
{
    /// <summary>
    /// código de barras incluído: o código (40) e a quantidade da embalagem, inteira no schema
    /// </summary>
    public class BarCodeInsertValidator : AbstractValidator<BarCodeInsert>
    {
        public BarCodeInsertValidator()
        {
            RuleFor(x => x.barCode)
                .NotEmpty()
                .WithMessage("O campo 'barCode' é obrigatório.")
                .MaximumLength(40)
                .WithMessage("O campo 'barCode' aceita até 40 caracteres.");

            RuleFor(x => x.quantity)
                .NotNull()
                .WithMessage("O campo 'quantity' é obrigatório (quantidade da embalagem).")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'quantity' deve ser maior que zero, com até 9 dígitos.");
        }
    }
}

using Application.Totvs.Services;
using Domain.Totvs.Dtos.Writes.Products.Classifications;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Classifications
{
    /// <summary>
    /// tipo e código de uma classificação de produto; o tipo 6000 (marca) só entra na inclusão de referência
    /// </summary>
    public class ProductClassificationItemValidator : AbstractValidator<ProductClassificationItem>
    {
        public ProductClassificationItemValidator() : this(allowBrand: false) { }

        public ProductClassificationItemValidator(bool allowBrand)
        {
            RuleFor(x => x.typeCode)
                .NotNull()
                .WithMessage("O campo 'typeCode' da classificação é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'typeCode' da classificação deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.typeCode)
                .NotEqual(TotvsProductBrands.ClassificationType)
                .WithMessage($"A classificação {TotvsProductBrands.ClassificationType} (marca) só muda pelo ERP.")
                .When(_ => !allowBrand);

            RuleFor(x => x.code)
                .NotEmpty()
                .WithMessage("O campo 'code' da classificação é obrigatório.")
                .MaximumLength(10)
                .WithMessage("O campo 'code' da classificação aceita até 10 caracteres.");
        }
    }
}

using Application.Totvs.Services;
using Application.Totvs.Validations.Writes.People.Customers;
using Application.Totvs.Validations.Writes.Products.BarCodes;
using Application.Totvs.Validations.Writes.Products.Classifications;
using Domain.Totvs.Dtos.Writes.Products.References;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.References
{
    /// <summary>
    /// imagem da referência ou do SKU: código (6) e tipo (2)
    /// </summary>
    public class ProductImageInsertValidator : AbstractValidator<ProductImageInsert>
    {
        public ProductImageInsertValidator()
        {
            RuleFor(x => x.code)
                .NotNull()
                .WithMessage("O campo 'code' da imagem é obrigatório.")
                .InclusiveBetween(1, 999_999)
                .WithMessage("O campo 'code' da imagem deve ter de 1 a 6 dígitos.");

            RuleFor(x => x.imageTypeCode)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'imageTypeCode' deve ter até 2 dígitos.")
                .When(x => x.imageTypeCode is not null);
        }
    }
}

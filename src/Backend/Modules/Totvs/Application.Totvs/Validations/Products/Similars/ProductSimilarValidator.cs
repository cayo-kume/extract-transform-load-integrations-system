using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Similars;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Similars
{
    /// <summary>
    /// só registra mensagens no log
    /// </summary>
    public class ProductSimilarValidator : AbstractValidator<ProductSimilar>
    {
        public ProductSimilarValidator()
        {
            RuleFor(x => x.productCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'productCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.groupSequenceCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.groupSequenceCode));
        }
    }
}

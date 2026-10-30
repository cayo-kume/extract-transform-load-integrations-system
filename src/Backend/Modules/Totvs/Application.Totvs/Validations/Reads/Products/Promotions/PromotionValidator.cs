using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Promotions;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Products.Promotions
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product (ProductPromotionModel)
    /// </summary>
    public class PromotionValidator : AbstractValidator<Promotion>
    {
        public PromotionValidator()
        {
            RuleFor(x => x.promotionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'promotionCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.productCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'productCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.promotionDescription)
                .MaximumLength(40)
                .WithMessage("O campo 'promotionDescription' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.promotionDescription));

            RuleFor(x => x.promotionValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.promotionValue));

            RuleFor(x => x.status)
                .Must(x => x is "A" or "B" or "E")
                .WithMessage("O campo 'status' só aceita A, B ou E.")
                .When(x => !string.IsNullOrEmpty(x.status));
        }
    }
}

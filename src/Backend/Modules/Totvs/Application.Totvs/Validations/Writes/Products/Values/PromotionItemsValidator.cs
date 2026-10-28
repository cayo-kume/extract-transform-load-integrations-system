using Domain.Totvs.Dtos.Writes.Products.Values;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Values
{
    /// <summary>
    /// valida o corpo dos itens e valores de promoção no controller (corpo inválido volta 400): loja, promoção (6 dígitos) e até 500 produtos sem
    /// repetição, com valor positivo (12 inteiros e 6 decimais)
    /// </summary>
    public class PromotionItemsValidator : AbstractValidator<PromotionItems>
    {
        public PromotionItemsValidator()
        {
            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.promotionCode)
                .NotNull()
                .WithMessage("O campo 'promotionCode' é obrigatório.")
                .InclusiveBetween(1, 999_999)
                .WithMessage("O campo 'promotionCode' deve ter de 1 a 6 dígitos.");

            RuleFor(x => x.items)
                .NotEmpty()
                .WithMessage("Informe pelo menos um produto em 'items'.");

            RuleFor(x => x.items)
                .Must(l => l!.Count <= 500)
                .WithMessage("Até 500 produtos por chamada.")
                .Must(l => l!.Where(i => i?.productCode is not null).GroupBy(i => i.productCode).All(g => g.Count() == 1))
                .WithMessage("O mesmo produto aparece mais de uma vez em 'items'.")
                .When(x => x.items is { Count: > 0 });

            RuleForEach(x => x.items)
                .ChildRules(i =>
                {
                    i.RuleFor(x => x.productCode)
                        .NotNull()
                        .WithMessage("O campo 'productCode' é obrigatório.")
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos.");

                    i.RuleFor(x => x.promotionValue)
                        .NotNull()
                        .WithMessage("O campo 'promotionValue' é obrigatório.")
                        .GreaterThan(0)
                        .WithMessage("O campo 'promotionValue' deve ser maior que zero.")
                        .LessThanOrEqualTo(999_999_999_999.999999m)
                        .WithMessage("O campo 'promotionValue' aceita até 12 inteiros.")
                        .PrecisionScale(18, 6, true)
                        .WithMessage("O campo 'promotionValue' aceita até 6 casas decimais.");
                })
                .When(x => x.items is not null);
        }
    }
}

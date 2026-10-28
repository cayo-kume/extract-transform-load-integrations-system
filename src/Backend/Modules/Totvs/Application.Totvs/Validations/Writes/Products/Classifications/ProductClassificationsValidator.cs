using Application.Totvs.Services;
using Domain.Totvs.Dtos.Writes.Products.Classifications;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Classifications
{
    /// <summary>
    /// valida o corpo da inclusão e da alteração de classificações de produto no controller (corpo inválido volta 400): produto ou grupo, um ou
    /// outro, sem repetição, cada um com pelo menos uma classificação, um código por tipo e nunca o tipo 6000 (marca: PR10)
    /// </summary>
    public class ProductClassificationsValidator : AbstractValidator<ProductClassifications>
    {
        public ProductClassificationsValidator()
        {
            RuleFor(x => x.products)
                .NotEmpty()
                .WithMessage("Informe pelo menos um produto ou grupo em 'products'.")
                .Must(l => l!.Where(p => p is not null).GroupBy(p => (p.productCode, p.referenceId)).All(g => g.Count() == 1))
                .WithMessage("O mesmo produto ou grupo aparece mais de uma vez em 'products'.")
                .When(x => x.products is { Count: > 0 }, ApplyConditionTo.CurrentValidator);

            RuleForEach(x => x.products)
                .ChildRules(p =>
                {
                    p.RuleFor(x => x)
                        .Must(x => (x.productCode is null) != (x.referenceId is null))
                        .WithName("productCode")
                        .WithMessage("Informe exatamente um entre 'productCode' e 'referenceId'.");

                    p.RuleFor(x => x.productCode)
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos.")
                        .When(x => x.productCode is not null);

                    p.RuleFor(x => x.referenceId)
                        .GreaterThan(0)
                        .WithMessage("O campo 'referenceId' deve ser maior que zero.")
                        .When(x => x.referenceId is not null);

                    p.RuleFor(x => x.classifications)
                        .NotEmpty()
                        .WithMessage("Informe pelo menos uma classificação em 'classifications'.")
                        .Must(l => l!.Where(c => c?.typeCode is not null).GroupBy(c => c.typeCode).All(g => g.Count() == 1))
                        .WithMessage("O mesmo tipo de classificação aparece mais de uma vez no produto.")
                        .When(x => x.classifications is { Count: > 0 }, ApplyConditionTo.CurrentValidator);

                    p.RuleForEach(x => x.classifications)
                        .SetValidator(new ProductClassificationItemValidator(allowBrand: false))
                        .When(x => x.classifications is not null);
                })
                .When(x => x.products is not null);
        }
    }
}

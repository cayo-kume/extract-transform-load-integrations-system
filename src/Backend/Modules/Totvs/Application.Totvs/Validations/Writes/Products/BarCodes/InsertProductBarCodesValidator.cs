using Domain.Totvs.Dtos.Writes.Products.BarCodes;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.BarCodes
{
    /// <summary>
    /// valida o corpo da inclusão de códigos de barras no controller (corpo inválido volta 400): produtos sem repetição, cada um com pelo menos
    /// um código, nenhum código repetido no corpo e no máximo um padrão por produto (o padrão é o SKU das integrações)
    /// </summary>
    public class InsertProductBarCodesValidator : AbstractValidator<InsertProductBarCodes>
    {
        public InsertProductBarCodesValidator()
        {
            RuleFor(x => x.products)
                .NotEmpty()
                .WithMessage("Informe pelo menos um produto em 'products'.");

            RuleFor(x => x.products)
                .Must(l => l!.Where(p => p?.productCode is not null).GroupBy(p => p.productCode).All(g => g.Count() == 1))
                .WithMessage("O mesmo produto aparece mais de uma vez em 'products'.")
                .Must(l => BarCodeRules.Unique(l!.SelectMany(p => p?.barCodes ?? []).Select(b => b?.barCode)))
                .WithMessage("O mesmo código de barras aparece mais de uma vez no corpo.")
                .When(x => x.products is { Count: > 0 });

            RuleForEach(x => x.products)
                .ChildRules(p =>
                {
                    p.RuleFor(x => x.productCode)
                        .NotNull()
                        .WithMessage("O campo 'productCode' é obrigatório.")
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos.");

                    p.RuleFor(x => x.barCodes)
                        .NotEmpty()
                        .WithMessage("Informe pelo menos um código em 'barCodes'.")
                        .Must(l => l!.Count(b => b?.isMainCode == true) <= 1)
                        .WithMessage("No máximo um código padrão ('isMainCode') por produto.")
                        .When(x => x.barCodes is { Count: > 0 }, ApplyConditionTo.CurrentValidator);

                    p.RuleForEach(x => x.barCodes)
                        .SetValidator(new BarCodeInsertValidator())
                        .When(x => x.barCodes is not null);
                })
                .When(x => x.products is not null);
        }
    }
}

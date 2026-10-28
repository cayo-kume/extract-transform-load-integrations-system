using Domain.Totvs.Dtos.Writes.Products.BarCodes;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.BarCodes
{
    /// <summary>
    /// valida o corpo da alteração de códigos de barras no controller (corpo inválido volta 400): o barCode é a chave; a quantidade é decimal
    /// e obrigatória (sem nullable no swagger: omitida, iria 0)
    /// </summary>
    public class UpdateProductBarCodesValidator : AbstractValidator<UpdateProductBarCodes>
    {
        public UpdateProductBarCodesValidator()
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
                        .ChildRules(b =>
                        {
                            b.RuleFor(x => x.barCode)
                                .NotEmpty()
                                .WithMessage("O campo 'barCode' é obrigatório.")
                                .MaximumLength(40)
                                .WithMessage("O campo 'barCode' aceita até 40 caracteres.");

                            b.RuleFor(x => x.quantity)
                                .NotNull()
                                .WithMessage("O campo 'quantity' é obrigatório (quantidade da embalagem).")
                                .GreaterThan(0)
                                .WithMessage("O campo 'quantity' deve ser maior que zero.")
                                .LessThanOrEqualTo(999_999_999.999m)
                                .WithMessage("O campo 'quantity' aceita até 9 inteiros.")
                                .PrecisionScale(12, 3, true)
                                .WithMessage("O campo 'quantity' aceita até 3 casas decimais.");
                        })
                        .When(x => x.barCodes is not null);
                })
                .When(x => x.products is not null);
        }
    }
}

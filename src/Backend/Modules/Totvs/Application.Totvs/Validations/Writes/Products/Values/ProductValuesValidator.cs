using Domain.Totvs.Dtos.Writes.Products.Values;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.Values
{
    /// <summary>
    /// valida o corpo de preço e custo no controller (corpo inválido volta 400): até 500 produtos sem repetição, cada valor com loja, tipo C ou P,
    /// código e valor positivo, sem repetir loja + tipo + código no produto. Códigos e lojas permitidos e a variação ficam com o service (PR7)
    /// </summary>
    public class ProductValuesValidator : AbstractValidator<ProductValues>
    {
        public ProductValuesValidator()
        {
            RuleFor(x => x.products)
                .NotEmpty()
                .WithMessage("Informe pelo menos um produto em 'products'.");

            RuleFor(x => x.products)
                .Must(l => l!.Count <= 500)
                .WithMessage("Até 500 produtos por chamada.")
                .Must(l => l!.Where(p => p?.productCode is not null).GroupBy(p => p.productCode).All(g => g.Count() == 1))
                .WithMessage("O mesmo produto aparece mais de uma vez em 'products'.")
                .When(x => x.products is { Count: > 0 });

            RuleForEach(x => x.products)
                .ChildRules(p =>
                {
                    p.RuleFor(x => x.productCode)
                        .NotNull()
                        .WithMessage("O campo 'productCode' é obrigatório.")
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos.");

                    p.RuleFor(x => x.values)
                        .NotEmpty()
                        .WithMessage("Informe pelo menos um valor em 'values'.");

                    p.RuleFor(x => x.values)
                        .Must(l => l!.Where(v => v is not null).GroupBy(v => (v.branchCode, v.valueType, v.valueCode)).All(g => g.Count() == 1))
                        .WithMessage("A mesma loja, tipo e código de valor aparecem mais de uma vez no produto.")
                        .When(x => x.values is { Count: > 0 });

                    p.RuleForEach(x => x.values)
                        .ChildRules(v =>
                        {
                            v.RuleFor(x => x.branchCode)
                                .NotNull()
                                .WithMessage("O campo 'branchCode' do valor é obrigatório.")
                                .InclusiveBetween(1, 9_999)
                                .WithMessage("O campo 'branchCode' do valor deve ter de 1 a 4 dígitos.");

                            v.RuleFor(x => x.valueType)
                                .NotEmpty()
                                .WithMessage("O campo 'valueType' é obrigatório (C custo, P preço).")
                                .Must(t => t is "C" or "P")
                                .WithMessage("O campo 'valueType' só aceita C (custo) ou P (preço).");

                            v.RuleFor(x => x.valueCode)
                                .NotNull()
                                .WithMessage("O campo 'valueCode' é obrigatório.")
                                .GreaterThan(0)
                                .WithMessage("O campo 'valueCode' deve ser maior que zero.");

                            v.RuleFor(x => x.value)
                                .NotNull()
                                .WithMessage("O campo 'value' é obrigatório.")
                                .GreaterThan(0)
                                .WithMessage("O campo 'value' deve ser maior que zero.")
                                .PrecisionScale(18, 6, true)
                                .WithMessage("O campo 'value' aceita até 6 casas decimais.");
                        })
                        .When(x => x.values is not null);
                })
                .When(x => x.products is not null);
        }
    }
}

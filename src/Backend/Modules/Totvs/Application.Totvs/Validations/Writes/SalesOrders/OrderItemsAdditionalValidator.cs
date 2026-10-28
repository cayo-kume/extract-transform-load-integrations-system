using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo dos dados adicionais dos itens no controller (corpo inválido volta 400): a chave, pelo menos um item, o produto por
    /// código ou SKU (um ou outro, sem repetir) e pelo menos um campo por item (UpdateOrderItemsCommand)
    /// </summary>
    public class OrderItemsAdditionalValidator : AbstractValidator<OrderItemsAdditional>
    {
        public OrderItemsAdditionalValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x.items)
                .NotEmpty()
                .WithMessage("Informe pelo menos um item em 'items'.");

            RuleFor(x => x.items)
                .Must(items => items!.Select(i => i.productCode?.ToString() ?? $"sku:{i.productSku}").Distinct().Count() == items!.Count)
                .WithMessage("O mesmo produto aparece mais de uma vez em 'items'.")
                .When(x => x.items is { Count: > 0 });

            RuleForEach(x => x.items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i)
                        .Must(i => (i.productCode is null) != String.IsNullOrEmpty(i.productSku))
                        .WithName("productCode")
                        .WithMessage("Informe o produto por 'productCode' ou por 'productSku', um ou outro.");

                    item.RuleFor(i => i.productCode)
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos.")
                        .When(i => i.productCode is not null);

                    item.RuleFor(i => i.productSku)
                        .MaximumLength(40)
                        .WithMessage("O campo 'productSku' aceita até 40 caracteres.");

                    item.RuleFor(i => i)
                        .Must(i => i.rollMeasureCode is not null || i.billingForecastDate is not null)
                        .WithName("billingForecastDate")
                        .WithMessage("Informe 'rollMeasureCode' ou 'billingForecastDate' no item.");

                    item.RuleFor(i => i.rollMeasureCode)
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'rollMeasureCode' deve ter de 1 a 9 dígitos.")
                        .When(i => i.rollMeasureCode is not null);
                })
                .When(x => x.items is not null);
        }
    }
}

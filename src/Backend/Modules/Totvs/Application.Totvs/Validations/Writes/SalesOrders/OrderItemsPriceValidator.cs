using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo do preço dos itens no controller (corpo inválido volta 400): a chave e os itens, sem repetição
    /// </summary>
    public class OrderItemsPriceValidator : AbstractValidator<OrderItemsPrice>
    {
        public OrderItemsPriceValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x.items)
                .NotEmpty()
                .WithMessage("Informe pelo menos um item em 'items'.");

            RuleFor(x => x.items)
                .Must(l => l!.Where(i => i is not null).GroupBy(i => i.productCode?.ToString() ?? i.productSku).All(g => g.Count() == 1))
                .WithMessage("O mesmo produto aparece mais de uma vez em 'items'.")
                .When(x => x.items is { Count: > 0 });

            RuleForEach(x => x.items)
                .SetValidator(new OrderItemPriceValidator())
                .When(x => x.items is not null);
        }
    }
}

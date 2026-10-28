using Domain.Totvs.Dtos.Writes.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.PurchaseOrders
{
    /// <summary>
    /// valida o corpo da troca de quantidade dos itens no controller (corpo inválido volta 400): o pedido e pelo menos um item, sem repetição,
    /// com a quantidade nova (o swagger não marca nada como obrigatório)
    /// </summary>
    public class RequestedQuantityValidator : AbstractValidator<RequestedQuantity>
    {
        public RequestedQuantityValidator()
        {
            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.orderCode)
                .NotNull()
                .WithMessage("O campo 'orderCode' é obrigatório.")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'orderCode' deve ter de 1 a 9 dígitos.");

            RuleFor(x => x.items)
                .NotEmpty()
                .WithMessage("Informe pelo menos um item em 'items'.");

            RuleFor(x => x.items)
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

                    i.RuleFor(x => x.quantity)
                        .NotNull()
                        .WithMessage("O campo 'quantity' é obrigatório (sem ele iria 0).")
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("O campo 'quantity' não pode ser negativo.")
                        .PrecisionScale(12, 3, true)
                        .WithMessage("O campo 'quantity' aceita até 9 inteiros e 3 casas decimais.");
                })
                .When(x => x.items is not null);
        }
    }
}

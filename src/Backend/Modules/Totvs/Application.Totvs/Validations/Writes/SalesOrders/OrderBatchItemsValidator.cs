using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo do vínculo de itens de lote no controller (corpo inválido volta 400): loja e número do pedido, produtos sem repetição,
    /// cada item de lote por loja + lote + sequência ou pela barra, com a quantidade
    /// </summary>
    public class OrderBatchItemsValidator : AbstractValidator<OrderBatchItems>
    {
        public OrderBatchItemsValidator()
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
                .WithMessage("Informe pelo menos um produto em 'items'.");

            RuleFor(x => x.items)
                .Must(l => l!.Where(i => i?.code is not null).GroupBy(i => i.code).All(g => g.Count() == 1))
                .WithMessage("O mesmo produto aparece mais de uma vez em 'items'.")
                .When(x => x.items is { Count: > 0 });

            RuleForEach(x => x.items)
                .ChildRules(p =>
                {
                    p.RuleFor(x => x.code)
                        .NotNull()
                        .WithMessage("O campo 'code' (produto) é obrigatório.")
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'code' (produto) deve ter de 1 a 9 dígitos.");

                    p.RuleFor(x => x.batchItems)
                        .NotEmpty()
                        .WithMessage("Informe pelo menos um item de lote em 'batchItems'.");

                    p.RuleForEach(x => x.batchItems)
                        .ChildRules(b =>
                        {
                            b.RuleFor(x => x)
                                .Must(x => (x.branchCode is not null && x.code is not null && x.sequence is not null && String.IsNullOrEmpty(x.barCode))
                                    || (x.branchCode is null && x.code is null && x.sequence is null && !String.IsNullOrEmpty(x.barCode)))
                                .WithName("batchItems.branchCode")
                                .WithMessage("Informe o item de lote por 'branchCode' + 'code' + 'sequence' ou por 'barCode', um ou outro.");

                            b.RuleFor(x => x.barCode).MaximumLength(40).WithMessage("O campo 'barCode' aceita até 40 caracteres.");

                            b.RuleFor(x => x.quantity)
                                .NotNull()
                                .WithMessage("O campo 'quantity' do item de lote é obrigatório.")
                                .GreaterThan(0)
                                .WithMessage("O campo 'quantity' do item de lote deve ser maior que zero.");
                        })
                        .When(x => x.batchItems is not null);
                })
                .When(x => x.items is not null);
        }
    }
}

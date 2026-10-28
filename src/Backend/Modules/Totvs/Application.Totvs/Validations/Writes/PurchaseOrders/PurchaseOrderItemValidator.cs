using Domain.Totvs.Dtos.Writes.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.PurchaseOrders
{
    /// <summary>
    /// item do pedido de compra: produto pelo código ou pelo SKU (o schema marca o código, a descrição aceita só o SKU), valor, quantidade e
    /// pelo menos um valor por tipo (C custo ou P preço)
    /// </summary>
    public class PurchaseOrderItemValidator : AbstractValidator<PurchaseOrderItem>
    {
        public PurchaseOrderItemValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.productCode is null) != String.IsNullOrEmpty(x.productSku))
                .WithName("productCode")
                .WithMessage("Informe exatamente um entre 'productCode' e 'productSku' no item.");

            RuleFor(x => x.productCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.productCode is not null);

            RuleFor(x => x.productSku).MaximumLength(40).WithMessage("O campo 'productSku' aceita até 40 caracteres.");

            RuleFor(x => x.value)
                .NotNull()
                .WithMessage("O campo 'value' do item é obrigatório (valor unitário).")
                .GreaterThanOrEqualTo(0)
                .WithMessage("O campo 'value' do item não pode ser negativo.")
                .PrecisionScale(15, 6, true)
                .WithMessage("O campo 'value' do item aceita até 9 inteiros e 6 casas decimais.");

            RuleFor(x => x.quantity)
                .NotNull()
                .WithMessage("O campo 'quantity' do item é obrigatório.")
                .GreaterThan(0)
                .WithMessage("O campo 'quantity' do item deve ser maior que zero.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'quantity' do item aceita até 9 inteiros e 3 casas decimais.");

            foreach (var (field, get) in new (string, Func<PurchaseOrderItem, decimal?>)[] { ("discountPercentage", x => x.discountPercentage), ("ipiPercentage", x => x.ipiPercentage) })
                RuleFor(x => get(x))
                    .InclusiveBetween(0, 999.999999m)
                    .WithName(field)
                    .WithMessage($"O campo '{field}' do item deve ficar entre 0 e 999,999999.")
                    .PrecisionScale(9, 6, true)
                    .WithMessage($"O campo '{field}' do item aceita até 6 casas decimais.")
                    .When(x => get(x) is not null);

            RuleFor(x => x.itemsValues)
                .NotEmpty()
                .WithMessage("Informe pelo menos um valor em 'itemsValues' (obrigatório no schema).");

            RuleForEach(x => x.itemsValues)
                .ChildRules(v =>
                {
                    v.RuleFor(x => x.valueType)
                        .NotEmpty()
                        .WithMessage("O campo 'valueType' é obrigatório (C custo, P preço).")
                        .Must(t => t is "C" or "P")
                        .WithMessage("O campo 'valueType' só aceita C (custo) ou P (preço).");

                    v.RuleFor(x => x.valueCode)
                        .NotNull()
                        .WithMessage("O campo 'valueCode' é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'valueCode' deve ter de 1 a 4 dígitos.");

                    v.RuleFor(x => x.value)
                        .NotNull()
                        .WithMessage("O campo 'value' do tipo de valor é obrigatório.")
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("O campo 'value' do tipo de valor não pode ser negativo.")
                        .PrecisionScale(15, 6, true)
                        .WithMessage("O campo 'value' do tipo de valor aceita até 6 casas decimais.");
                })
                .When(x => x.itemsValues is not null);

            RuleForEach(x => x.itemsExpenses)
                .SetValidator(new PurchaseOrderExpenseValidator())
                .When(x => x.itemsExpenses is not null);
        }
    }
}

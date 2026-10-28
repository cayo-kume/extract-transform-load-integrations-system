using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida os parâmetros da remoção de item do pedido de venda (400 se inválidos): a chave e o produto por código ou SKU
    /// </summary>
    public class OrderItemDeleteValidator : AbstractValidator<OrderItemDelete>
    {
        public OrderItemDeleteValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x)
                .Must(x => (x.productCode is null) != String.IsNullOrEmpty(x.productSku))
                .WithName("productCode")
                .WithMessage("Informe exatamente um entre 'productCode' e 'productSku'.");

            RuleFor(x => x.productCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.productCode is not null);

            RuleFor(x => x.productSku)
                .MaximumLength(40)
                .WithMessage("O campo 'productSku' aceita até 40 caracteres.");
        }
    }
}

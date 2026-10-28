using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida a chave do pedido de venda das escritas (§7.4): orderId (até 100) ou loja (até 4 dígitos) + número (até 9), um ou outro, e
    /// loja e número sempre juntos. Incluído pelos validadores de cada escrita
    /// </summary>
    public class OrderKeyValidator : AbstractValidator<OrderKey>
    {
        public OrderKeyValidator()
        {
            RuleFor(x => x)
                .Must(x => String.IsNullOrEmpty(x.orderId) != (x.branchCode is null && x.orderCode is null))
                .WithName("orderId")
                .WithMessage("Informe o pedido por 'orderId' ou por 'branchCode' + 'orderCode', um ou outro.");

            RuleFor(x => x)
                .Must(x => (x.branchCode is null) == (x.orderCode is null))
                .WithName("branchCode")
                .WithMessage("'branchCode' e 'orderCode' vão juntos.")
                .When(x => String.IsNullOrEmpty(x.orderId));

            RuleFor(x => x.branchCode)
                .InclusiveBetween(1, 9999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.")
                .When(x => x.branchCode is not null);

            RuleFor(x => x.orderCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'orderCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.orderCode is not null);

            RuleFor(x => x.orderId)
                .MaximumLength(100)
                .WithMessage("O campo 'orderId' aceita até 100 caracteres.");
        }
    }
}

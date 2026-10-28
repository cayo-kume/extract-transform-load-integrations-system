using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo do cancelamento do pedido de venda no controller (corpo inválido volta 400): a chave e o motivo
    /// </summary>
    public class OrderCancelValidator : AbstractValidator<OrderCancel>
    {
        public OrderCancelValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x.reasonCancellationCode)
                .NotNull()
                .WithMessage("O campo 'reasonCancellationCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'reasonCancellationCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.ReasonCancellationDescription)
                .MaximumLength(80)
                .WithName("ReasonCancellationDescription")
                .WithMessage("O campo 'ReasonCancellationDescription' aceita até 80 caracteres.");
        }
    }
}

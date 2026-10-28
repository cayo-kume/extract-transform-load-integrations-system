using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo da troca de situação do pedido de venda no controller (corpo inválido volta 400): a chave e a situação pelo nome
    /// </summary>
    public class OrderStatusChangeValidator : AbstractValidator<OrderStatusChange>
    {
        public OrderStatusChangeValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x.statusOrder)
                .NotEmpty()
                .WithMessage($"O campo 'statusOrder' é obrigatório ({String.Join(", ", OrderStatusNames.Values)}).")
                .Must(s => OrderStatusNames.Values.Contains(s!))
                .WithMessage($"O campo 'statusOrder' só aceita {String.Join(", ", OrderStatusNames.Values)} (pelo nome).")
                .When(x => !String.IsNullOrEmpty(x.statusOrder), ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.reasonBlockingCode)
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'reasonBlockingCode' deve ter de 1 a 4 dígitos.")
                .When(x => x.reasonBlockingCode is not null);

            RuleFor(x => x.reasonBlockingDescription)
                .MaximumLength(80)
                .WithMessage("O campo 'reasonBlockingDescription' aceita até 80 caracteres.");
        }
    }
}

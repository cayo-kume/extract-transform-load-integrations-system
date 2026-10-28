using Domain.Totvs.Dtos.Writes.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.PurchaseOrders
{
    /// <summary>
    /// valida o corpo da troca de situação do pedido de compra no controller, antes de chamar a Totvs (corpo inválido volta 400). O swagger
    /// não marca nada como obrigatório; sem a loja, o pedido ou a situação a rota não faz sentido
    /// </summary>
    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatus>
    {
        public UpdateOrderStatusValidator()
        {
            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.orderCode)
                .NotNull()
                .WithMessage("O campo 'orderCode' é obrigatório.")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'orderCode' deve ter de 1 a 9 dígitos.");

            RuleFor(x => x.status)
                .NotNull()
                .WithMessage("O campo 'status' é obrigatório.")
                .IsInEnum()
                .WithMessage("O campo 'status' só aceita 1 (em andamento) ou 5 (bloqueado).");
        }
    }
}

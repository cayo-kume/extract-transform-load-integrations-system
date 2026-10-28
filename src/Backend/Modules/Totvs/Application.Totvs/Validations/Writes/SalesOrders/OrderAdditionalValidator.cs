using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo dos dados adicionais do pedido no controller (corpo inválido volta 400): a chave, pelo menos um campo e os tamanhos do
    /// swagger (OrderAdditionalInDto)
    /// </summary>
    public class OrderAdditionalValidator : AbstractValidator<OrderAdditional>
    {
        public OrderAdditionalValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x)
                .Must(x => x.shippingService is not null || x.trackingId is not null || x.ecommerceStage is not null || x.omniSiteId is not null
                    || x.compensationDays is not null || x.anticipationDay is not null || x.entryType is not null || x.experienceType is not null)
                .WithName("trackingId")
                .WithMessage("Informe pelo menos um campo além da chave do pedido.");

            RuleFor(x => x.shippingService).MaximumLength(10).WithMessage("O campo 'shippingService' aceita até 10 caracteres.");
            RuleFor(x => x.trackingId).MaximumLength(40).WithMessage("O campo 'trackingId' aceita até 40 caracteres.");
            RuleFor(x => x.ecommerceStage).MaximumLength(40).WithMessage("O campo 'ecommerceStage' aceita até 40 caracteres.");

            RuleFor(x => x.omniSiteId)
                .GreaterThan(0)
                .WithMessage("O campo 'omniSiteId' deve ser maior que zero.")
                .When(x => x.omniSiteId is not null);

            RuleFor(x => x.compensationDays)
                .InclusiveBetween(0, 999)
                .WithMessage("O campo 'compensationDays' deve ter até 3 dígitos.")
                .When(x => x.compensationDays is not null);

            RuleFor(x => x.anticipationDay)
                .InclusiveBetween(0, 999)
                .WithMessage("O campo 'anticipationDay' deve ter até 3 dígitos.")
                .When(x => x.anticipationDay is not null);

            RuleFor(x => x.entryType)
                .Must(x => x is "N" or "S")
                .WithMessage("O campo 'entryType' só aceita 'N' (não aceita entrega antecipada) ou 'S' (aceita).")
                .When(x => x.entryType is not null);

            RuleFor(x => x.experienceType)
                .IsInEnum()
                .WithMessage("O campo 'experienceType' só aceita de 1 a 5 (e-commerce, a loja envia, clique e colete, coleta na loja, prateleira infinita).");
        }
    }
}

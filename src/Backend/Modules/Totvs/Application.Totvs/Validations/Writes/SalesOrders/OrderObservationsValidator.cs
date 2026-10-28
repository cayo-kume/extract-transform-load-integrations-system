using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo das observações do pedido no controller (corpo inválido volta 400): a chave e pelo menos uma linha, cada uma com até
    /// 80 caracteres (UpdateOrderObservationsCommand)
    /// </summary>
    public class OrderObservationsValidator : AbstractValidator<OrderObservations>
    {
        public OrderObservationsValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x)
                .Must(x => x.observations is { Count: > 0 } || x.invoiceObservations is { Count: > 0 })
                .WithName("observations")
                .WithMessage("Informe pelo menos uma linha em 'observations' ou em 'invoiceObservations'.");

            RuleForEach(x => x.observations)
                .ChildRules(line =>
                {
                    line.RuleFor(l => l.observation)
                        .NotEmpty()
                        .WithMessage("O campo 'observation' é obrigatório.")
                        .MaximumLength(80)
                        .WithMessage("O campo 'observation' aceita até 80 caracteres.");

                    // enum não anulável no swagger: omitido, a Totvs receberia 0
                    line.RuleFor(l => l.visualizationType)
                        .NotNull()
                        .WithMessage("O campo 'visualizationType' é obrigatório (1 sugestão, 2 expedição).")
                        .IsInEnum()
                        .WithMessage("O campo 'visualizationType' só aceita 1 (sugestão) ou 2 (expedição).");
                })
                .When(x => x.observations is not null);

            RuleForEach(x => x.invoiceObservations)
                .ChildRules(line =>
                {
                    line.RuleFor(l => l.observation)
                        .NotEmpty()
                        .WithMessage("O campo 'observation' é obrigatório.")
                        .MaximumLength(80)
                        .WithMessage("O campo 'observation' aceita até 80 caracteres.");
                })
                .When(x => x.invoiceObservations is not null);
        }
    }
}

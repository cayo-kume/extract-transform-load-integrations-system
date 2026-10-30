using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class InvoiceObservationValidator : AbstractValidator<InvoiceObservation>
    {
        public InvoiceObservationValidator()
        {
            RuleFor(x => x.observation)
                .MaximumLength(80)
                .WithMessage("O campo 'observation' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));
        }
    }
}

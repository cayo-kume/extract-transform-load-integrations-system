using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class RepresentativeObservationValidator : AbstractValidator<RepresentativeObservation>
    {
        public RepresentativeObservationValidator()
        {
            RuleFor(x => x.observation)
                .MaximumLength(80)
                .WithMessage("O campo 'observation' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));

            RuleFor(x => x.visualizationType)
                .MaximumLength(30)
                .WithMessage("O campo 'visualizationType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.visualizationType));
        }
    }
}

using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Orders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script052_Create_LinxMicrovix_Orders_Tables
    /// </summary>
    public class LinxPedidosVendaChecklistEntregaLocalValidator : AbstractValidator<LinxPedidosVendaChecklistEntregaLocal>
    {
        public LinxPedidosVendaChecklistEntregaLocalValidator()
        {
            RuleFor(x => x.id_checklist_entrega_local)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_checklist_entrega_local' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(100)
                .WithMessage("O campo 'descricao' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
